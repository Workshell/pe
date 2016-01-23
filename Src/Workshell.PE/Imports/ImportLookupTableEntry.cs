using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class ImportLookupTableEntry : ILocationSupport, IRawDataSupport
    {

        private ImportLookupTable table;
        private StreamLocation location;
        private ulong address;
        private int ordinal;

        internal ImportLookupTableEntry(ImportLookupTable lookupTable, StreamLocation streamLoc, ulong entryAddress, int entryOrdinal)
        {
            table = lookupTable;
            location = streamLoc;
            address = entryAddress;
            ordinal = entryOrdinal;
        }

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = table.Tables.Content.Section.Sections.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        public override string ToString()
        {
            if (ordinal == -1)
            {
                return String.Format("0x{0:X8}",address);
            }
            else
            {
                return String.Format("{0}",ordinal);
            }
        }

        #endregion

        #region Properties

        public ImportLookupTable Table
        {
            get
            {
                return table;
            }
        }

        public StreamLocation Location
        {
            get
            {
                return location;
            }
        }

        public ulong Address
        {
            get
            {
                return address;
            }
        }

        public int Ordinal
        {
            get
            {
                return ordinal;
            }
        }

        public bool IsOrdinal
        {
            get
            {
                return (ordinal != -1);
            }
        }

        #endregion

    }

}
