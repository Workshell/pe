using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Workshell.PE.Content
{
    public abstract class ImportHintNameTableBase<TEntry> : DataContent, IEnumerable<TEntry>
        where TEntry : ImportHintNameEntryBase
    {
        private readonly TEntry[] _entries;

        protected internal ImportHintNameTableBase(PortableExecutableImage image, DataDirectory directory, Location location, Tuple<ulong,uint,ushort,string,bool>[] entries, bool isDelayed) : base(image, directory, location)
        {
            _entries = BuildTable(entries);

            Count = _entries.Length;
            IsDelayed = isDelayed;
        }

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

        private TEntry[] BuildTable(Tuple<ulong, uint, ushort, string, bool>[] entries)
        {
            TEntry[] results = new TEntry[entries.Length];

            var type = typeof(TEntry);
            var ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var ctor = ctors.First();

            for(var i = 0; i < entries.Length; i++)
            {
                var entry = (TEntry)ctor.Invoke(new object[] { Image, entries[i].Item1, entries[i].Item2, entries[i].Item3, entries[i].Item4, entries[i].Item5 });

                results[i] = entry;
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
