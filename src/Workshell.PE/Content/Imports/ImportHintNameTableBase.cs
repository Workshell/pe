using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public abstract class ImportHintNameTableBase<TEntry> : DataContent, IEnumerable<TEntry>
        where TEntry : ImportHintNameEntryBase
    {
        private readonly TEntry[] _entries;

        protected internal ImportHintNameTableBase(PortableExecutableImage image, DataDirectory directory, Location location, IEnumerable<Tuple<ulong,uint,ushort,string,bool>> entries, bool isDelayed) : base(image, directory, location)
        {
            _entries = BuildEntries(image, entries);

            Count = _entries.Length;
            IsDelayed = isDelayed;
        }

        #region Static Methods

        internal static async Task<TStaticTable> LoadAsync<TAddressTable, TStaticTable, TStaticEntry>(PortableExecutableImage image, TAddressTable tables)
            where TStaticTable : ImportHintNameTableBase<TStaticEntry>
            where TStaticEntry : ImportHintNameEntryBase
            where TAddressTable : ImportAddressTablesBase<ImportAddressTableBase<ImportAddressTableEntryBase, ImportDirectoryEntryBase>, ImportAddressTableEntryBase, ImportDirectoryEntryBase>
        {          
            var entries = new Dictionary<uint, Tuple<ulong, uint, ushort, string, bool>>();
            //var ilt = await ImportAddressTables.GetLookupTableAsync(image).ConfigureAwait(false);
            var calc = image.GetCalculator();
            var stream = image.GetStream();

            foreach (var table in tables)
            {
                foreach (var entry in table)
                {
                    if (entry.Address == 0)
                        continue;

                    if (entries.ContainsKey(entry.Address))
                        continue;

                    if (!entry.IsOrdinal)
                    {
                        var offset = calc.RVAToOffset(entry.Address);
                        var size = 0u;
                        var isPadded = false;
                        ushort hint = 0;
                        var name = new StringBuilder(256);

                        stream.Seek(offset.ToInt64(), SeekOrigin.Begin);

                        hint = await stream.ReadUInt16Async().ConfigureAwait(false);
                        size += sizeof(ushort);

                        while (true)
                        {
                            var b = await stream.ReadByteAsync();

                            size++;

                            if (b <= 0)
                                break;

                            name.Append((char) b);
                        }

                        if ((size % 2) != 0)
                        {
                            isPadded = true;
                            size++;
                        }

                        var tuple = new Tuple<ulong, uint, ushort, string, bool>(offset, size, hint, name.ToString(), isPadded);

                        entries.Add(entry.Address, tuple);
                    }
                }
            }

            Location location;

            if (entries.Count > 0)
            {
                var firstEntry = entries.Values.MinBy(tuple => tuple.Item1);
                var lastEntry = entries.Values.MaxBy(tuple => tuple.Item1);
                var tableOffset = firstEntry.Item1;
                var tableSize = ((lastEntry.Item1 + lastEntry.Item2) - tableOffset).ToUInt32();
                var rva = calc.OffsetToRVA(tableOffset);
                var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
                var va = imageBase + rva;

                location = new Location(calc, tableOffset, rva, va, tableSize, tableSize);
            }
            else
            {
                location = new Location(0, 0, 0, 0, 0, null);
            }

            var dataDirectory = tables.Directory;
            var tableType = typeof(TStaticTable);
            var ctors = tableType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var ctor = ctors.First(); // TODO: Should probably match it better somehow
            var result = (TStaticTable)ctor.Invoke(new object[] { image, dataDirectory, location, entries.Values });

            return result;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"File Offset: 0x{Location.FileOffset:X8}, Name Count: {_entries.Length}";
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

        private TEntry[] BuildEntries(PortableExecutableImage image, IEnumerable<Tuple<ulong,uint,ushort,string,bool>> entries)
        {
            var results = new List<TEntry>();
            var entryType = typeof(TEntry);
            var ctors = entryType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var ctor = ctors.First(); // TODO: Should probably match it better somehow

            foreach (var tuple in entries)
            {
                var entry = (TEntry)ctor.Invoke(new object[] { image, tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5 });

                results.Add(entry);
            }

            return results.OrderBy(entry => entry.Location.FileOffset).ToArray();
        }

        #endregion

        #region Properties

        public int Count { get; }
        public TEntry this[int index] => _entries[index];
        public bool IsDelayed { get; }

        #endregion
    }
}
