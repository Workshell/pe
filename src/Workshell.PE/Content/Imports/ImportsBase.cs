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
