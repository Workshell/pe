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
    public abstract class ImportAddressTablesBase<TTable, TEntry, TDirectoryEntry> : DataContent, IEnumerable<TTable> 
        where TTable : ImportAddressTableBase<TEntry, TDirectoryEntry>
        where TEntry : ImportAddressTableEntryBase
        where TDirectoryEntry : ImportDirectoryEntryBase
    {
        private readonly TTable[] _tables;

        protected internal ImportAddressTablesBase(PortableExecutableImage image, DataDirectory directory, Location location, Tuple<uint, TDirectoryEntry, ulong[]>[] tables) : base(image, directory, location)
        {
            _tables = new TTable[tables.Length];

            var tableType = typeof(TTable);
            var ctors = tableType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var ctor = ctors.First(); // TODO: Should probably match it better somehow

            for (var i = 0; i < tables.Length; i++)
                _tables[i] = (TTable)ctor.Invoke(new object[] {image, tables[i].Item2, tables[i].Item1, tables[i].Item3});

            Count = _tables.Length;
        }

        #region Static Methods

        internal static async Task<TTables> LoadAsync<TStaticTable, TStaticEntry, TStaticDirectoryEntry, TTables>(PortableExecutableImage image, ImportDirectoryBase<TStaticDirectoryEntry> directory, Func<TStaticDirectoryEntry, uint> thunkRVA)
            where TStaticTable : ImportAddressTableBase<TStaticEntry, TStaticDirectoryEntry>
            where TStaticEntry : ImportAddressTableEntryBase
            where TStaticDirectoryEntry : ImportDirectoryEntryBase
            where TTables : ImportAddressTablesBase<TStaticTable, TStaticEntry, TStaticDirectoryEntry>
        {
            if (directory == null)
                return null;

            var calc = image.GetCalculator();
            var stream = image.GetStream();
            var tuples = new List<Tuple<uint, TStaticDirectoryEntry, ulong[]>>();

            foreach (var directoryEntry in directory)
            {
                var thunk = thunkRVA(directoryEntry);

                if (thunk == 0)
                    continue;

                var entries = new List<ulong>();
                var offset = calc.RVAToOffset(thunk);

                stream.Seek(offset.ToInt64(), SeekOrigin.Begin);

                while (true)
                {
                    ulong entry;

                    if (!image.Is64Bit)
                    {
                        entry = await stream.ReadUInt32Async().ConfigureAwait(false);
                    }
                    else
                    {
                        entry = await stream.ReadUInt64Async().ConfigureAwait(false);
                    }

                    entries.Add(entry);

                    if (entry == 0)
                        break;
                }

                var table = new Tuple<uint, TStaticDirectoryEntry, ulong[]>(thunk, directoryEntry, entries.ToArray());

                tuples.Add(table);
            }

            var rva = 0u;

            if (tuples.Count > 0)
                rva = tuples.MinBy(table => table.Item1).Item1;

            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
            var va = imageBase + rva;
            var fileOffset = calc.RVAToOffset(rva);
            ulong fileSize = 0;

            foreach (var table in tuples)
            {
                var size = (table.Item3.Length * (!image.Is64Bit ? sizeof(uint) : sizeof(ulong))).ToUInt32();

                fileSize += size;
            }

            var section = calc.RVAToSection(rva);
            var location = new Location(fileOffset, rva, va, fileSize, fileSize, section);
            var tableType = typeof(TTables);
            var ctors = tableType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var ctor = ctors.First(); // TODO: Should probably match it better somehow
            var result = (TTables)ctor.Invoke(new object[] {image, directory.Directory, location, tuples.ToArray()});

            return result;
        }

        #endregion

        #region Methods

        public IEnumerator<TTable> GetEnumerator()
        {
            foreach (var table in _tables)
                yield return table;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public int Count { get; }
        public TTable this[int index] => _tables[index];

        #endregion
    }
}
