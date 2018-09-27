using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Workshell.PE.Resources.Version
{
    public sealed class VersionStringTable : IEnumerable<VersionString>
    {
        private readonly VersionString[] _strings;

        internal VersionStringTable(string key, VersionString[] strings)
        {
            _strings = strings;

            Key = key;
            Count = _strings.Length;
        }

        #region Methods

        public override string ToString()
        {
            return $"Key: {Key}, Strings: {Count:n0}";
        }

        public IEnumerator<VersionString> GetEnumerator()
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
        public VersionString this[int index] => _strings[index];
        public VersionString this[string key] => _strings.FirstOrDefault(s => string.Compare(key, s.Key, StringComparison.OrdinalIgnoreCase) == 0);

        #endregion
    }
}
