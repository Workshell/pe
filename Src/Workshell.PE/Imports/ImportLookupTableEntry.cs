using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{


    public abstract class ImportLookupTableEntry : ILocationSupport, IRawDataSupport, IEquatable<ImportLookupTableEntry>
    {

        private ImportLookupTable table;
        private StreamLocation location;

        internal ImportLookupTableEntry(ImportLookupTable lookupTable, long offset, int size)
        {
            table = lookupTable;
            location = new StreamLocation(offset,size);
        }

        #region Methods

        public abstract uint GetAddress();
        public abstract ulong GetLongAddress();

        public int GetOrdinal()
        {
            if (location.Size == 4)
            {
                uint value = GetAddress();

                if ((value & 0x80000000) == 0x80000000)
                {
                    value &= 0x7fffffff;

                    return Convert.ToInt32(value);
                }
            }
            else
            {
                ulong value = GetLongAddress();

                if ((value & 0x8000000000000000) == 0x8000000000000000)
                {
                    value &= 0x7fffffffffffffff;

                    return Convert.ToInt32(value);
                }
            }

            return 0;
        }

#if DEBUG

        public override string ToString()
        {
            ulong value;

            if (location.Size == 4)
            {
                value = GetAddress();

                if (IsOrdinal)
                {
                    return String.Format("{0} (Ordinal)",value);
                }
                else
                {
                    return String.Format("0x{0} (Address)",value.ToString("X8"));
                }
            }
            else
            {
                value = GetLongAddress();
            }

            if (IsOrdinal)
            {
                return String.Format("{0} (Ordinal)",value);
            }
            else
            {
                return String.Format("0x{0} (Address)",value.ToString("X16"));
            }
        }

#endif

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
                if (location.Size == 4)
                {
                    uint value = GetAddress();

                    return ((value & 0x80000000) == 0x80000000);
                }
                else
                {
                    ulong value = GetLongAddress();

                    return ((value & 0x8000000000000000) == 0x8000000000000000);
                }
            }
        }

        #endregion

    }

    public class ImportLookupTableEntry32 : ImportLookupTableEntry
    {

        private uint address;

        internal ImportLookupTableEntry32(ImportLookupTable lookupTable, long entryOffset, uint entryAddress) : base(lookupTable,entryOffset,4)
        {
            address = entryAddress;
        }

        #region Methods

        public override uint GetAddress()
        {
            return address;
        }

        public override ulong GetLongAddress()
        {
            return 0;
        }

        #endregion

    }

    public class ImportLookupTableEntry64 : ImportLookupTableEntry
    {

        private ulong address;

        internal ImportLookupTableEntry64(ImportLookupTable lookupTable, long entryOffset, ulong entryAddress) : base(lookupTable,entryOffset,8)
        {
            address = entryAddress;
        }

        #region Methods

        public override uint GetAddress()
        {
            return 0;
        }

        public override ulong GetLongAddress()
        {
            return address;
        }

        #endregion

    }


}
