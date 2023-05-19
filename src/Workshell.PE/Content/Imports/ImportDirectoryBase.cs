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
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public abstract class ImportDirectoryBase<T> : DataContent, IEnumerable<T> where T : ImportDirectoryEntryBase
    {
        private readonly T[] _entries;

        protected internal ImportDirectoryBase(PortableExecutableImage image, DataDirectory dataDirectory, Location location, T[] entries) : base(image, dataDirectory, location)
        {
            _entries = entries;

            Count = _entries.Length;
        }

        #region Static Methods

        protected internal static async Task<string> GetNameAsync(LocationCalculator calc, Stream stream, uint nameRVA)
        {
            var fileOffset = calc.RVAToOffset(nameRVA);

            stream.Seek(fileOffset, SeekOrigin.Begin);

            var result = await stream.ReadStringAsync().ConfigureAwait(false);

            return result;
        }

        #endregion

        #region Methods

        public IEnumerator<T> GetEnumerator()
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

        public int Count { get; }
        public T this[int index] => _entries[index];

        #endregion
    }
}
