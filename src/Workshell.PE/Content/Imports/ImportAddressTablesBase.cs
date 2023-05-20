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
using System.Reflection;
using System.Text;

namespace Workshell.PE.Content
{
    public abstract class ImportAddressTablesBase<TTable, TTableEntry> : DataContent, IEnumerable<TTable>
        where TTable : ImportAddressTableBase<TTableEntry>
        where TTableEntry : ImportAddressTableEntryBase
    {
        private readonly TTable[] _tables;

        protected internal ImportAddressTablesBase(PortableExecutableImage image, DataDirectory dataDirectory, Location location, IEnumerable<Tuple<uint, ulong[], ImportDirectoryEntryBase>> tables, bool isDelayed) : base(image, dataDirectory, location)
        {
            _tables = new TTable[tables.Count()];

            var type = typeof(TTable);
            var ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var ctor = ctors.First();
            var idx = 0;

            foreach (var t in tables)
            {
                var table = (TTable)ctor.Invoke(new object[] { image, t.Item1, t.Item2, t.Item3 });

                _tables[idx] = table;
                idx++;
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
