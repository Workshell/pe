using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class ExportTable<T> : DataDirectoryContent, IEnumerable<T>, ISupportsBytes
    {

        private T[] table;

        internal ExportTable(DataDirectory dataDirectory, Location tableLocation, T[] tableContent) : base(dataDirectory,tableLocation)
        {
            table = tableContent;
        }

        #region Methods

        public IEnumerator<T> GetEnumerator()
        {
            for(var i = 0; i < table.Length; i++)
            {
                yield return table[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public byte[] GetBytes()
        {
            Stream stream = DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream, Location);

            return buffer;
        }

        #endregion

        #region Properties

        public int Count
        {
            get
            {
                return table.Length;
            }
        }

        public T this[int index]
        {
            get
            {
                return table[index];
            }
        }

        #endregion

    }

}
