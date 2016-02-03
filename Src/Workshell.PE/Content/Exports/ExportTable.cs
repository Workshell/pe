using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class ExportTable<T> : IEnumerable<T>, ISupportsLocation
    {

        private ExportTableContent content;
        private Location location;
        private Section section;
        private List<T> table;

        internal ExportTable(ExportTableContent exportContent, Location tableLocation, Section tableSection, IEnumerable<T> tableList)
        {
            content = exportContent;
            location = tableLocation;
            section = tableSection;
            table = new List<T>(tableList);
        }

        #region Methods

        public IEnumerator<T> GetEnumerator()
        {
            return table.GetEnumerator();
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

        public Section Section
        {
            get
            {
                return section;
            }
        }

        public int Count
        {
            get
            {
                return table.Count;
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
