using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class ExportTable<T> : ILocationSupport, IEnumerable<T>
    {

        private ExportContent content;
        private StreamLocation location;
        private List<T> table;

        internal ExportTable(ExportContent exportContent, long offset, long size, IEnumerable<T> tableList)
        {
            content = exportContent;
            location = new StreamLocation(offset,size);
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
            return Utils.ReadBytes(content.Section.Sections.Reader.Stream,location.Offset,location.Size);
        }

        #endregion

        #region Properties

        public ExportContent Content
        {
            get
            {
                return content;
            }
        }

        public StreamLocation Location
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
