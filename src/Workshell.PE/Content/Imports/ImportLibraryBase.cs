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
