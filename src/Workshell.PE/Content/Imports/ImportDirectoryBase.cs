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

        protected internal ImportDirectoryBase(PortableExecutableImage image, DataDirectory directory, Location location, T[] entries) : base(image, directory, location)
        {
            _entries = entries;

            Count = _entries.Length;
        }

        #region Static Methods

        protected internal static async Task<string> GetNameAsync(LocationCalculator calc, Stream stream, uint nameRVA)
        {
            var fileOffset = calc.RVAToOffset(nameRVA);

            stream.Seek(fileOffset.ToInt64(), SeekOrigin.Begin);

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
