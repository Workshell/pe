#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

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
