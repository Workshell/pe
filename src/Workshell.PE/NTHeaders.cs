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
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;

namespace Workshell.PE
{
    public sealed class NTHeaders : ISupportsLocation, ISupportsBytes
    {
        public const uint PE_MAGIC_MZ = 17744;

        private readonly PortableExecutableImage _image;

        internal NTHeaders(PortableExecutableImage image, uint headerOffset, ulong imageBase, FileHeader fileHeader, OptionalHeader optHeader, DataDirectories dataDirs)
        {
            _image = image;

            var size = (4 + fileHeader.Location.FileSize + optHeader.Location.FileSize + dataDirs.Location.FileSize).ToUInt32();

            Location = new Location(image, headerOffset, headerOffset, imageBase + headerOffset, size, size);
            FileHeader = fileHeader;
            OptionalHeader = optHeader;
            DataDirectories = dataDirs;
        }

        #region Methods

        public override string ToString()
        {
            return "NT Headers";
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
        public FileHeader FileHeader { get; }
        public OptionalHeader OptionalHeader { get; }
        public DataDirectories DataDirectories { get; }

        #endregion
    }
}
