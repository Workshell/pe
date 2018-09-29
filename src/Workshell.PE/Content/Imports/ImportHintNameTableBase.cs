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

namespace Workshell.PE.Content
{
    public abstract class ImportHintNameTableBase<TEntry> : DataContent, IEnumerable<TEntry>
        where TEntry : ImportHintNameEntryBase
    {
        private readonly TEntry[] _entries;

        protected internal ImportHintNameTableBase(PortableExecutableImage image, DataDirectory dataDirectory, Location location, Tuple<ulong,uint,ushort,string,bool>[] entries, bool isDelayed) : base(image, dataDirectory, location)
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
