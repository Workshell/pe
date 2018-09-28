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

namespace Workshell.PE.Content
{
    public abstract class ImportHintNameEntryBase : ISupportsLocation, ISupportsBytes
    {
        private readonly PortableExecutableImage _image;

        protected internal ImportHintNameEntryBase(PortableExecutableImage image, ulong offset, uint size, ushort entryHint, string entryName, bool isPadded, bool isDelayed)
        {
            _image = image;

            var calc = image.GetCalculator();
            var rva = calc.OffsetToRVA(offset);
            var va = image.NTHeaders.OptionalHeader.ImageBase + rva;

            Location = new Location(calc, offset, rva, va, size, size);
            Hint = entryHint;
            Name = entryName;
            IsPadded = isPadded;
            IsDelayed = isDelayed;
        }

        #region Methods

        public override string ToString()
        {
            return $"0x{Hint:X4} {Name}";
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
        public ushort Hint { get; }
        public string Name { get; }
        public bool IsPadded { get; }
        public bool IsDelayed { get; }

        #endregion
    }
}
