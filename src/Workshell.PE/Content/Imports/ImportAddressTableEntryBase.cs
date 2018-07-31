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

        protected internal ImportAddressTableEntryBase(PortableExecutableImage image, ulong entryOffset, ulong entryValue, uint entryAddress, ushort entryOrdinal, bool isOrdinal, bool isDelayed)
        {
            _image = image;

            var calc = image.GetCalculator();
            var rva = calc.OffsetToRVA(entryOffset);
            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
            var va = imageBase + rva;
            var size = (image.Is64Bit ? sizeof(ulong) : sizeof(uint)).ToUInt64();

            Location = new Location(calc, entryOffset, rva, va, size, size);
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
