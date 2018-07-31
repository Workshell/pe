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
    public abstract class ImportAddressTableBase<TEntry, TDirectoryEntry> : ISupportsLocation, ISupportsBytes, IEnumerable<TEntry> 
        where TEntry : ImportAddressTableEntryBase
        where TDirectoryEntry : ImportDirectoryEntryBase
    {
        private readonly PortableExecutableImage _image;
        private readonly TEntry[] _entries;

        protected internal ImportAddressTableBase(PortableExecutableImage image, TDirectoryEntry directoryEntry, uint rva, ulong[] entries, bool isDelayed)
        {
            _image = image;
            _entries = BuildEntries(image, rva, entries);

            var calc = image.GetCalculator();
            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
            var va = imageBase + rva;
            var offset = calc.RVAToOffset(rva);
            var size = (entries.Length * (image.Is64Bit ? sizeof(ulong) : sizeof(uint))).ToUInt64();

            DirectoryEntry = directoryEntry;
            Location = new Location(calc, offset, rva, va, size, size);
            Count = _entries.Length;
            IsDelayed = isDelayed;
        }

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

        public IEnumerator<TEntry> GetEnumerator()
        {
            foreach (var entry in _entries)
                yield return entry;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private TEntry[] BuildEntries(PortableExecutableImage image, uint rva, ulong[] entries)
        {
            var results = new TEntry[entries.Length];
            var calc = image.GetCalculator();
            var offset = calc.RVAToOffset(rva);

            var entryType = typeof(TEntry);
            var ctors = entryType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var ctor = ctors.First(); // TODO: Should probably match it better somehow

            for (var i = 0; i < entries.Length; i++)
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

                        ordinal = Convert.ToUInt16(value);
                        isOrdinal = true;
                    }
                }
                else
                {
                    var value = addrOrOrd;

                    if ((value & 0x8000000000000000) == 0x8000000000000000)
                    {
                        value &= 0x7fffffffffffffff;

                        ordinal = Convert.ToUInt16(value);
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

        public TDirectoryEntry DirectoryEntry { get; }
        public Location Location { get; }
        public int Count { get; }
        public TEntry this[int index] => _entries[index];
        public bool IsDelayed { get; }

        #endregion
    }
}
