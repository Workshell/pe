using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class ExportTable<T> : IEnumerable<T>, IReadOnlyCollection<T>, ISupportsLocation
    {

        private ExportTableContent content;
        private Location location;
        private T[] table;

        internal ExportTable(ExportTableContent exportContent, Location tableLocation, IEnumerable<T> tableList)
        {
            content = exportContent;
            location = tableLocation;
            table = tableList.ToArray();
        }

        #region Methods

        public IEnumerator<T> GetEnumerator()
        {
            return table.Cast<T>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public byte[] GetBytes()
        {
            return null;
        }

        #endregion

        #region Properties

        public ExportTableContent Content
        {
            get
            {
                return content;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

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
