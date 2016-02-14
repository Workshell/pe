using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.MoreLinq;

namespace Workshell.PE
{

    public sealed class ImportAddressTableEntry : ISupportsLocation, ISupportsBytes
    {

        private ImportAddressTable table;
        private Location location;
        private uint address;
        private ushort ordinal;
        private bool is_ordinal;

        internal ImportAddressTableEntry(ImportAddressTable addressTable, ulong entryOffset, uint entryAddress, ushort entryOrdinal, bool isOrdinal)
        {
            bool is_64bit = addressTable.Tables.Content.DataDirectory.Directories.Reader.Is64Bit;
            LocationCalculator calc = addressTable.Tables.Content.DataDirectory.Directories.Reader.GetCalculator();
            uint rva = calc.OffsetToRVA(addressTable.Tables.Content.Section,entryOffset);
            ulong va = calc.OffsetToVA(addressTable.Tables.Content.Section,entryOffset);
            uint size = Convert.ToUInt32(is_64bit ? sizeof(ulong) : sizeof(uint));

            table = addressTable;
            location = new Location(entryOffset,rva,va,size,size);
            address = entryAddress;
            ordinal = entryOrdinal;
            is_ordinal = isOrdinal;
        }

        #region Methods

        public override string ToString()
        {
            string result = String.Format("File Offset: 0x{0:X8}, ",location.FileOffset);

            if (!is_ordinal)
            {
                if (location.FileSize == sizeof(ulong))
                {
                    result += String.Format("Address: 0x{0:X16}",address);
                }
                else
                {
                    result = String.Format("Address: 0x{0:X8}",address);
                }
            }
            else
            {
                result = String.Format("Ordinal: 0x{0:D4}",ordinal);
            }

            return result;
        }

        public byte[] GetBytes()
        {
            Stream stream = table.Tables.Content.DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        #endregion

        #region Properties

        public ImportAddressTable Table
        {
            get
            {
                return table;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        public uint Address
        {
            get
            {
                return address;
            }
        }

        public ushort Ordinal
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
                return is_ordinal;
            }
        }

        #endregion

    }

    public sealed class ImportAddressTable : IEnumerable<ImportAddressTableEntry>, IReadOnlyCollection<ImportAddressTableEntry>, ISupportsLocation, ISupportsBytes
    {

        private ImportAddressTableCollection tables;
        private ImportDirectoryEntry dir_entry;
        private Location location;
        private ImportAddressTableEntry[] entries;

        internal ImportAddressTable(ImportAddressTableCollection addressTables,  ImportDirectoryEntry directoryEntry, ulong tableOffset, IEnumerable<ulong> tableEntries)
        {
            bool is_64bit = addressTables.Content.DataDirectory.Directories.Reader.Is64Bit;
            LocationCalculator calc = addressTables.Content.DataDirectory.Directories.Reader.GetCalculator();
            uint rva = calc.OffsetToRVA(addressTables.Content.Section,tableOffset);
            ulong va = calc.OffsetToVA(addressTables.Content.Section,tableOffset);
            uint size = Convert.ToUInt32(tableEntries.Count() * (is_64bit ? sizeof(ulong) : sizeof(uint)));

            tables = addressTables;
            dir_entry = directoryEntry;
            location = new Location(tableOffset,rva,va,size,size);
            entries = new ImportAddressTableEntry[0];

            LoadEntries(is_64bit,tableOffset,tableEntries);
        }

        #region Methods

        public IEnumerator<ImportAddressTableEntry> GetEnumerator()
        {
            return entries.Cast<ImportAddressTableEntry>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("File Offset: 0x{0:X8}, Table Entry Count: {1}",location.FileOffset,entries.Length);
        }

        public byte[] GetBytes()
        {
            Stream stream = tables.Content.DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        private void LoadEntries(bool is64Bit, ulong tableOffset, IEnumerable<ulong> tableEntries)
        {
            List<ImportAddressTableEntry> list = new List<ImportAddressTableEntry>();
            ulong offset = tableOffset;

            foreach(ulong addr_or_ord in tableEntries)
            {
                ushort ordinal = 0;
                bool is_ordinal = false;

                if (!is64Bit)
                {
                    uint value = Convert.ToUInt32(addr_or_ord);

                    if ((value & 0x80000000) == 0x80000000)
                    {
                        value &= 0x7fffffff;

                        ordinal = Convert.ToUInt16(value);
                        is_ordinal = true;
                    }
                }
                else
                {
                    ulong value = addr_or_ord;

                    if ((value & 0x8000000000000000) == 0x8000000000000000)
                    {
                        value &= 0x7fffffffffffffff;

                        ordinal = Convert.ToUInt16(value);
                        is_ordinal = true;
                    }
                }

                uint address;

                if (is_ordinal)
                {
                    address = 0;
                }
                else
                {
                    address = Utils.LoDWord(addr_or_ord);
                }

                ImportAddressTableEntry entry = new ImportAddressTableEntry(this,offset,address,ordinal,is_ordinal);

                list.Add(entry);
                offset += Convert.ToUInt32(is64Bit ? sizeof(ulong) : sizeof(uint));
            }

            entries = list.ToArray();
        }

        #endregion

        #region Properties

        public ImportAddressTableCollection Tables
        {
            get
            {
                return tables;
            }
        }

        public ImportDirectoryEntry DirectoryEntry
        {
            get
            {
                return dir_entry;
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
                return entries.Length;
            }
        }

        public ImportAddressTableEntry this[int index]
        {
            get
            {
                return entries[index];
            }
        }

        #endregion

    }

    public sealed class ImportAddressTableCollection : IEnumerable<ImportAddressTable>, ISupportsLocation, ISupportsBytes
    {

        private ImportTableContent content;
        private ImportAddressTable[] tables;
        private Location location;
        
        internal ImportAddressTableCollection(ImportTableContent tableContent, IEnumerable<Tuple<ulong,ImportDirectoryEntry>> tablesInfo)
        {
            content = tableContent;
            location = null;
            tables = new ImportAddressTable[0];

            LoadTables(tablesInfo);

            ulong lowest_offset = 0;

            if (tablesInfo.Count() > 0)
                lowest_offset = tablesInfo.MinBy(table => table.Item1).Item1;

            LocationCalculator calc = content.DataDirectory.Directories.Reader.GetCalculator();
            uint rva = calc.OffsetToRVA(content.Section,lowest_offset);
            ulong va = calc.OffsetToVA(content.Section,lowest_offset);
            uint size = Convert.ToUInt32(tables.Sum(table => table.Location.FileSize));

            location = new Location(lowest_offset,rva,va,size,size);
        }

        #region Methods

        public IEnumerator<ImportAddressTable> GetEnumerator()
        {
            return tables.Cast<ImportAddressTable>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("File Offset: 0x{0:X8}, Table Count: {1}",location.FileOffset,tables.Length);
        }

        public byte[] GetBytes()
        {
            Stream stream = content.DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        private void LoadTables(IEnumerable<Tuple<ulong,ImportDirectoryEntry>> tablesInfo)
        {
            List<ImportAddressTable> list = new List<ImportAddressTable>();
            bool is_64bit = content.DataDirectory.Directories.Reader.Is64Bit;
            Stream stream = content.DataDirectory.Directories.Reader.GetStream();

            foreach(Tuple<ulong,ImportDirectoryEntry> table_info in tablesInfo)
            {
                stream.Seek(Convert.ToInt64(table_info.Item1),SeekOrigin.Begin);

                List<ulong> entries = new List<ulong>();

                while (true)
                {
                    ulong entry;

                    if (!is_64bit)
                    {
                        entry = Utils.ReadUInt32(stream);
                    }
                    else
                    {
                        entry = Utils.ReadUInt64(stream);
                    }

                    entries.Add(entry);

                    if (entry == 0)
                        break;
                }

                ImportAddressTable table = new ImportAddressTable(this,table_info.Item2,table_info.Item1,entries);

                list.Add(table);
            }

            tables = list.ToArray();
        }

        #endregion

        #region Properties

        public ImportTableContent Content
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
                return tables.Length;
            }
        }

        public ImportAddressTable this[int index]
        {
            get
            {
                return tables[index];
            }
        }

        #endregion

    }

}
