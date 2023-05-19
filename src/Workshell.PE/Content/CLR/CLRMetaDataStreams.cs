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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public sealed class CLRMetaDataStreams : IEnumerable<CLRMetaDataStream>, ISupportsLocation, ISupportsBytes
    {
        private readonly PortableExecutableImage _image;
        private readonly CLRMetaDataStream[] _streams;

        internal CLRMetaDataStreams(PortableExecutableImage image, Location location, CLRMetaDataStream[] streams)
        {
            _image = image;
            _streams = streams;

            Location = location;
            Count = _streams.Length;
        }
        
        #region Static Methods

        internal static Task<CLRMetaDataStreams> LoadAsync(PortableExecutableImage image, Location mdLocation, CLRMetaDataStreamTable streamTable)
        {
            try
            {
                var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
                var streams = new CLRMetaDataStream[streamTable.Count];

                for(var i = 0; i < streams.Length; i++)
                {
                    var entry = streamTable[i];
                    var stream = new CLRMetaDataStream(image, mdLocation, imageBase, entry);

                    streams[i] = stream;
                }

                var rva = 0U;
                var va = 0UL;
                var offset = 0L;
                var size = 0L;

                if (streams.Length > 0)
                {
                    var stream = streams.MinBy(s => s.Location.FileOffset);

                    rva = stream.Location.RelativeVirtualAddress;
                    va = stream.Location.VirtualAddress;
                    offset = stream.Location.FileOffset;
                }

                foreach (var stream in streams)
                {
                    size += stream.Location.FileSize;
                }

                var location = new Location(image, offset, rva, va, size, size);
                var result = new CLRMetaDataStreams(image, location, streams);

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new PortableExecutableImageException(image, "Could not load CLR meta-data streams from stream.", ex);
            }
        }

        #endregion

        #region Methods

        public IEnumerator<CLRMetaDataStream> GetEnumerator()
        {
            foreach (var strm in _streams)
                yield return strm;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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

        #endregion

        #region Properties

        public Location Location { get; }
        public int Count { get; }
        public CLRMetaDataStream this[int index] => _streams[index];
        public CLRMetaDataStream this[string name] => _streams.SingleOrDefault(strm => string.Compare(name, strm.Name, StringComparison.OrdinalIgnoreCase) == 0);

        #endregion
    }
}
