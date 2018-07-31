using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public abstract class ImportAddressTableBase<TEntry> : IEnumerable<TEntry>, ISupportsLocation, ISupportsBytes
        where TEntry : ImportAddressTableEntryBase
    {
        private readonly PortableExecutableImage _image;
        private readonly TEntry[] _entries;

        protected internal ImportAddressTableBase(PortableExecutableImage image, uint rva, ulong[] entries, bool isDelayed)
        {
            var calc = image.GetCalculator();
            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
            var va = imageBase + rva;
            var offset = calc.RVAToOffset(rva);
            var size = (entries.Length * (image.Is64Bit ? sizeof(ulong) : sizeof(uint))).ToUInt64();
            var section = calc.RVAToSection(rva);

            _image = image;
            _entries = BuildEntries(image, offset, entries);

            Location = new Location(offset, rva, va, size, size, section);
            Count = _entries.Length;
            IsDelayed = isDelayed;
        }

        #region Methods

        public IEnumerator<TEntry> GetEnumerator()
        {
            foreach (var entry in _entries)
                yield return entry;
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

        private TEntry[] BuildEntries(PortableExecutableImage image, ulong tableOffset, ulong[] entries)
        {
            var results = new TEntry[entries.Length];
            var ctors = typeof(TEntry).GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var ctor = ctors.First();
            var offset = tableOffset;

            for(var i = 0; i < entries.Length; i++)
            {
                var addrOrOrd = entries[i];
                ushort ordinal = 0;
                var isOrdinal = false;

                if (!image.Is64Bit)
                {
                    var value = addrOrOrd.ToUInt32();

                    if ((value & 0x80000000) == 0x80000000)
                    {
                        value &= 0x7fffffff;

                        ordinal = value.ToUInt16();
                        isOrdinal = true;
                    }
                }
                else
                {
                    var value = addrOrOrd;

                    if ((value & 0x8000000000000000) == 0x8000000000000000)
                    {
                        value &= 0x7fffffffffffffff;

                        ordinal = value.ToUInt16();
                        isOrdinal = true;
                    }
                }

                uint address;

                if (isOrdinal)
                {
                    address = 0;
                }
                else
                {
                    address = Utils.LoDWord(addrOrOrd);
                }

                results[i] = (TEntry)ctor.Invoke(new object[] { image, offset, addrOrOrd, address, ordinal, isOrdinal });
                offset += Convert.ToUInt32(image.Is64Bit ? sizeof(ulong) : sizeof(uint));
            }

            return results;
        }

        #endregion

        #region Properties

        public Location Location { get; }
        public int Count { get; }
        public TEntry this[int index] => _entries[index];
        public bool IsDelayed { get; }

        #endregion
    }
}
