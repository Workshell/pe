#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Annotations;
using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public sealed class CLRMetaDataHeader : ISupportsLocation, ISupportsBytes
    {
        public const uint CLR_METADATA_SIGNATURE = 0x424A5342;

        private readonly PortableExecutableImage _image;

        internal CLRMetaDataHeader(PortableExecutableImage image, Location location)
        {
            _image = image;

            Location = location;
        }

        #region Static Methods

        public static async Task<CLRMetaDataHeader> LoadAsync(PortableExecutableImage image, Location mdLocation)
        {
            try
            {
                var stream = image.GetStream();
                var rva = mdLocation.RelativeVirtualAddress;
                var va = mdLocation.VirtualAddress;  
                var offset = mdLocation.FileOffset;
                var section = mdLocation.Section;

                stream.Seek(offset, SeekOrigin.Begin);

                var signature = await stream.ReadUInt32Async().ConfigureAwait(false);

                if (signature != CLR_METADATA_SIGNATURE)
                { 
                    throw new PortableExecutableImageException(image, "Incorrect signature found in CLR meta-data header.");
                }

                var majorVersion = await stream.ReadUInt16Async().ConfigureAwait(false);
                var minorVersion = await stream.ReadUInt16Async().ConfigureAwait(false);
                var reserved = await stream.ReadUInt32Async().ConfigureAwait(false);
                var versionLength = await stream.ReadInt32Async().ConfigureAwait(false);
                var version = await stream.ReadStringAsync(versionLength).ConfigureAwait(false);
                var flags = await stream.ReadUInt16Async().ConfigureAwait(false);
                var streams = await stream.ReadUInt16Async().ConfigureAwait(false);
                var size = sizeof(uint) + sizeof(ushort) + sizeof(ushort) + sizeof(uint) + sizeof(uint) + versionLength + sizeof(ushort) + sizeof(ushort);
                var location = new Location(image, offset, rva, va, size.ToUInt32(), size.ToUInt32(), section);
                var header = new CLRMetaDataHeader(image, location)
                {
                    Signature = signature,
                    MajorVersion = majorVersion,
                    MinorVersion = minorVersion,
                    Reserved = reserved,
                    VersionLength = versionLength,
                    Version = version,
                    Flags = flags,
                    Streams = streams
                };

                return header;
            }
            catch (Exception ex)
            {
                if (ex is PortableExecutableImageException)
                    throw;

                throw new PortableExecutableImageException(image, "Could not read CLR meta-data header from stream.", ex);
            }
        }

        #endregion

        #region Methods

        public byte[] GetBytes()
        {
            return GetBytesAsync().GetAwaiter().GetResult();
        }

        public async Task<byte[]> GetBytesAsync()
        {
            var stream = _image.GetStream();
            var buffer = await stream.ReadBytesAsync(Location).ConfigureAwait(false);

            return buffer;
        }

        #endregion

        #region Properties

        public Location Location { get; }

        [FieldAnnotation("Signature", Order = 1)]
        public uint Signature { get; private set; }

        [FieldAnnotation("Major Version", Order = 2)]
        public ushort MajorVersion { get; private set; }

        [FieldAnnotation("Minor Version", Order = 3)]
        public ushort MinorVersion { get; private set; }

        [FieldAnnotation("Reserved", Order = 4)]
        public uint Reserved { get; private set; }

        [FieldAnnotation("Version String Length", Order = 5)]
        public int VersionLength { get; private set; }

        [FieldAnnotation("Version String", Order = 6)]
        public string Version { get; private set; }

        [FieldAnnotation("Flags", Order = 7)]
        public ushort Flags { get; private set; }

        [FieldAnnotation("Number of Streams", Order = 8)]
        public ushort Streams { get; private set; }

        #endregion
    }
}
