using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Workshell.PE.Content
{
    public abstract class ImportAddressTablesBase<TTable, TTableEntry> : DataContent, IEnumerable<TTable>
        where TTable : ImportAddressTableBase<TTableEntry>
        where TTableEntry : ImportAddressTableEntryBase
    {
        private readonly TTable[] _tables;

        protected internal ImportAddressTablesBase(PortableExecutableImage image, DataDirectory directory, Location location, Tuple<uint, ulong[]>[] tables, bool isDelayed) : base(image, directory, location)
        {
            _tables = new TTable[tables.Length];

            var type = typeof(TTable);
            var ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var ctor = ctors.First();

            for (var i = 0; i < tables.Length; i++)
            {
                var tuple = tables[i];
                var table = (TTable)ctor.Invoke(new object[] { image, tuple.Item1, tuple.Item2 });

                _tables[i] = table;
            }

            Count = _tables.Length;
            IsDelayed = isDelayed;
        }

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
        public bool IsDelayed { get; }

        #endregion
    }
}
