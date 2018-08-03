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
            return $"0x{hint:X4} {name}";
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
