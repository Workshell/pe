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

namespace Workshell.PE.Content
{
    public abstract class ImportsBase<TLibrary> : IEnumerable<TLibrary>
        where TLibrary : ImportLibraryBase
    {
        private readonly TLibrary[] _libraries;

        protected internal ImportsBase(TLibrary[] libraries)
        {
            _libraries = libraries;

            Count = _libraries.Length;
        }

        #region Methods

        public IEnumerator<TLibrary> GetEnumerator()
        {
            foreach (var library in _libraries)
                yield return library;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public int Count { get; }
        public TLibrary this[int index] => _libraries[index];
        public TLibrary this[string name] => _libraries.FirstOrDefault(lib => string.Compare(name, lib.Name, StringComparison.OrdinalIgnoreCase) == 0);

        #endregion
    }
}
