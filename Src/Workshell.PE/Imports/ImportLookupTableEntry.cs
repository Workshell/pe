using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{


    public class ImportLookupTableEntry : ILocationSupport, IRawDataSupport, IEquatable<ImportLookupTableEntry>
    {

        private ImportLookupTable table;
        private StreamLocation location;
        private ulong address;
        private int ordinal;

        internal ImportLookupTableEntry(ImportLookupTable lookupTable, StreamLocation streamLoc, ulong entryAddress)
        {
            table = lookupTable;
            location = streamLoc;
            address = entryAddress;
            ordinal = -1;

            if (location.Size == sizeof(uint))
            {
                uint value = GetAddress();

                if ((value & 0x80000000) == 0x80000000)
                {
                    value &= 0x7fffffff;

                    ordinal = Convert.ToInt32(value);
                }
            }
            else
            {
                ulong value = GetLongAddress();

                if ((value & 0x8000000000000000) == 0x8000000000000000)
                {
                    value &= 0x7fffffffffffffff;

                    ordinal = Convert.ToInt32(value);
                }
            }
        }

        #region Methods

        public uint GetAddress()
        {
            if (location.Size == sizeof(uint))
            {
                return Convert.ToUInt32(address);
            }
            else
            {
                return 0;
            }
        }

        public ulong GetLongAddress()
        {
            if (location.Size == sizeof(ulong))
            {
                return address;
            }
            else
            {
                return 0;
            }
        }

        public override int GetHashCode()
        {
            int prime = 397;
            int result = 0;

            result = (result * prime) ^ location.GetHashCode();

            uint address = GetAddress();
            ulong long_address = GetLongAddress();

            result = (result * prime) ^ address.GetHashCode();
            result = (result * prime) ^ long_address.GetHashCode();

            return result;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ImportLookupTableEntry);
        }

        public bool Equals(ImportLookupTableEntry other)
        {
            if (other == null)
                return false;

            if (other == this)
                return true;

            if (Location != other.Location)
                return false;

            if (other.GetAddress() != GetAddress())
                return false;

            if (other.GetLongAddress() != GetLongAddress())
                return false;

            return true;
        }

        public byte[] GetBytes()
        {
            byte[] buffer = Utils.ReadBytes(table.Tables.Content.Section.Sections.Reader.Stream,location);

            return buffer;
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
