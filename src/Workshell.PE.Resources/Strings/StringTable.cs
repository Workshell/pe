using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Resources.Strings
{
    public sealed class StringTable : IEnumerable<StringTableEntry>
    {
        private readonly StringTableEntry[] _entries;

        internal StringTable(StringTableResource resource, uint languageId, StringTableEntry[] entries)
        {
            _entries = entries;

            Count = _entries.Length;
            Resource = resource;
            Language = languageId;
        }

        #region Methods

        public IEnumerator<StringTableEntry> GetEnumerator()
        {
            foreach (var entry in _entries)
                yield return entry;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public StringTableResource Resource { get; }
        public ResourceLanguage Language { get; }

        public int Count { get; }
        public StringTableEntry this[int index] => _entries[index];

        #endregion
    }
}
