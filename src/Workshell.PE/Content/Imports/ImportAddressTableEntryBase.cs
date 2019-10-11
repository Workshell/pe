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
    public abstract class ImportAddressTableEntryBase : ISupportsLocation, ISupportsBytes
    {
        private readonly PortableExecutableImage _image;

        protected internal ImportAddressTableEntryBase(PortableExecutableImage image, long entryOffset, ulong entryValue, uint entryAddress, ushort entryOrdinal, bool isOrdinal, bool isDelayed)
        {
            _image = image;

            var calc = image.GetCalculator();
            var rva = calc.OffsetToRVA(entryOffset);
            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
            var va = imageBase + rva;
            var size = (image.Is64Bit ? sizeof(ulong) : sizeof(uint));

            Location = new Location(image, entryOffset, rva, va, size, size);
            Value = entryValue;
            Address = entryAddress;
            Ordinal = entryOrdinal;
            IsOrdinal = isOrdinal;
            IsDelayed = isDelayed;
        }

        #region Methods

        public override string ToString()
        {
            var result = $"File Offset: 0x{Location.FileOffset:X8}, ";

            if (!IsOrdinal)
            {
                if (Location.FileSize == sizeof(ulong))
                {
                    result += $"Address: 0x{Address:X16}";
                }
                else
                {
                    result = $"Address: 0x{Address:X8}";
                }
            }
            else
            {
                result = $"Ordinal: 0x{Ordinal:D4}";
            }

            return result;
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
        public ulong Value { get; }
        public uint Address { get; }
        public ushort Ordinal { get; }
        public bool IsOrdinal { get; }
        public bool IsDelayed { get; }

        #endregion
    }
}
