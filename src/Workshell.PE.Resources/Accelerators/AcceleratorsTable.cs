using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Resources.Accelerators
{
    public sealed class AcceleratorsTable : IEnumerable<AcceleratorEntry>
    {
        private readonly AcceleratorEntry[] _entries;

        internal AcceleratorsTable(AcceleratorsResource resource, uint language, AcceleratorEntry[] entries)
        {
            _entries = entries;

            Resource = resource;
            Language = language;
            Count = entries.Length;
        }

        #region Methods

        public IEnumerator<AcceleratorEntry> GetEnumerator()
        {
            foreach(var entry in _entries)
                yield return entry;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public AcceleratorsResource Resource { get; }
        public uint Language { get; }
        public int Count { get; }
        public AcceleratorEntry this[int index] => _entries[index];

        #endregion
    }
}
