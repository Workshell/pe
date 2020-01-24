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
using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public sealed class DebugData : ISupportsLocation, ISupportsBytes
    {
        private readonly PortableExecutableImage _image;

        internal DebugData(PortableExecutableImage image, Location location, DebugDirectoryEntry entry)
        {
            _image = image;

            Location = location;
            Entry = entry;
        }

        #region Methods

        public override string ToString()
        {
            var type = Entry.GetEntryType().ToString();

            return $"Debug Type: {type}, File Offset: 0x{Location.FileOffset:X8}, Size: 0x{Location.FileSize:X8}";
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

        public DebugDirectoryEntry Entry { get; }
        public Location Location { get; }

        #endregion
    }
}
