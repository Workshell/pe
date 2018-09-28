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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Workshell.PE.Content
{
    public abstract class ImportLibraryBase : IEnumerable<ImportLibraryFunction>
    {
        private readonly ImportLibraryFunction[] _functions;

        protected internal ImportLibraryBase(ImportLibraryFunction[] functions, string name, bool isDelayed)
        {
            _functions = functions;

            Name = name;
            Count = _functions.Length;
            IsDelayed = isDelayed;
        }

        #region Methods

        public IEnumerator<ImportLibraryFunction> GetEnumerator()
        {
            foreach (var function in _functions)
                yield return function;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return $"Name: {Name}, Imported Function Count: {_functions.Length}";
        }

        public IEnumerable<ImportLibraryNamedFunction> GetNamedFunctions()
        {
            foreach (var function in _functions)
            {
                if (function.BindingType == ImportLibraryBindingType.Name)
                    yield return (ImportLibraryNamedFunction)function;
            }
        }

        public IEnumerable<ImportLibraryOrdinalFunction> GetOrdinalFunctions()
        {
            foreach (var function in _functions)
            {
                if (function.BindingType == ImportLibraryBindingType.Ordinal)
                    yield return (ImportLibraryOrdinalFunction)function;
            }
        }

        #endregion

        #region Properties

        public ImportLibraryFunction this[int index] => _functions[index];
        public string Name { get; }
        public int Count { get; }
        public bool IsDelayed { get; }

        #endregion
    }
}
