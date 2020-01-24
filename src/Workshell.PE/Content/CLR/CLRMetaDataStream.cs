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
using System.Threading;
using System.Threading.Tasks;

using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public sealed class CLRMetaDataStream : ISupportsLocation, ISupportsBytes
    {
        private readonly PortableExecutableImage _image;

        internal CLRMetaDataStream(PortableExecutableImage image, Location mdLocation, ulong imageBase, CLRMetaDataStreamTableEntry tableEntry)
        {
            _image = image;

            var calc = image.GetCalculator();
            var offset = mdLocation.FileOffset + tableEntry.Offset;
            var rva = calc.OffsetToRVA(offset);
            var va = imageBase + rva;

            Location = new Location(image, offset, rva, va, tableEntry.Size, tableEntry.Size);
            TableEntry = tableEntry;
            Name = tableEntry.Name;
        }

        #region Methods

        public override string ToString()
        {
            return Name;
        }

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

        public Stream GetStream()
        {
            var buffer = GetBytes();

            return new MemoryStream(buffer);
        }

        public async Task<Stream> GetStreamAsync()
        {
            var buffer = await GetBytesAsync().ConfigureAwait(false);

            return new MemoryStream(buffer);
        }

        public long CopyTo(Stream stream, int bufferSize = 4096)
        {
            return CopyToAsync(stream, bufferSize).GetAwaiter().GetResult();
        }

        public async Task<long> CopyToAsync(Stream stream, int bufferSize = 4096)
        {
            using (var mem = await GetStreamAsync().ConfigureAwait(false))
            {
                return await Utils.CopyStreamAsync(mem, stream, bufferSize).ConfigureAwait(false);
            }
        }

        #endregion

        #region Properties

        public Location Location { get; }
        public CLRMetaDataStreamTableEntry TableEntry { get; }
        public string Name { get; }

        #endregion
    }
}
