using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Resources.Graphics
{
    public enum IconGroupSaveFormat
    {
        Raw,
        Icon
    }

    public sealed class IconGroup : IEnumerable<IconGroupEntry>
    {
        private readonly IconGroupEntry[] _entries;

        internal IconGroup(ResourceLanguage language, IconGroupEntry[] entries)
        {
            _entries = entries;

            Language = language;
            Count = entries.Length;
        }

        #region Methods

        public IEnumerator<IconGroupEntry> GetEnumerator()
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

        public ResourceLanguage Language { get; }
        public int Count { get; }
        public IconGroupEntry this[int index] => _entries[index];

        #endregion
    }
}
