using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class ExportAddressTable : IEnumerable<uint>, ILocatable
    {

        private ExportContent content;
        private StreamLocation location;
        private List<uint> table;

        internal ExportAddressTable(ExportContent exportContent, long offset, List<uint> addresses)
        {
            content = exportContent;
            location = new StreamLocation(offset,addresses.Count * sizeof(uint));
            table = addresses;
        }

        #region Methods

        public IEnumerator<uint> GetEnumerator()
        {
            return table.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

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

        public uint this[int index]
        {
            get
            {
                return table[index];
            }
        }

        #endregion

    }

}
