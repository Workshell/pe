using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Workshell.PE.Resources.Version
{
    public sealed class VerStringTable : IEnumerable<VerString>
    {
        private readonly VerString[] _strings;

        internal VerStringTable(string key, VerString[] strings)
        {
            _strings = strings;

            Key = key;
            Count = _strings.Length;
        }

        #region Methods

        public IEnumerator<VerString> GetEnumerator()
        {
            foreach (var str in _strings)
                yield return str;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public string Key { get; }
        public int Count { get; }
        public VerString this[int index] => _strings[index];
        public VerString this[string key] => _strings.FirstOrDefault(s => string.Compare(key, s.Key, StringComparison.OrdinalIgnoreCase) == 0);

        #endregion
    }
}
