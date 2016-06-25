using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.MoreLinq;

using Workshell.PE.Extensions;

namespace Workshell.PE
{

    public sealed class DelayImportAddressTableEntry : ISupportsLocation, ISupportsBytes
    {

        private DelayImportAddressTable table;
        private Location location;
        private ulong value;
        private uint address;
        private ushort ordinal;
        private bool is_ordinal;

        internal DelayImportAddressTableEntry(DelayImportAddressTable addressTable, ulong entryOffset, ulong entryValue, uint entryAddress, ushort entryOrdinal, bool isOrdinal)
        {
            bool is_64bit = addressTable.Tables.DataDirectory.Directories.Image.Is64Bit;
            LocationCalculator calc = addressTable.Tables.DataDirectory.Directories.Image.GetCalculator();
            uint rva = calc.OffsetToRVA(entryOffset);
            ulong image_base = addressTable.Tables.DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            ulong va = image_base + rva;
            ulong size = (is_64bit ? sizeof(ulong) : sizeof(uint)).ToUInt64();

            table = addressTable;
            location = new Location(entryOffset,rva,va,size,size);
            value = entryValue;
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
            Stream stream = table.Tables.DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        #endregion

        #region Properties

        public DelayImportAddressTable Table
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

        public ulong Value
        {
            get
            {
                return value;
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

    public sealed class DelayImportAddressTable : IEnumerable<DelayImportAddressTableEntry>, ISupportsLocation, ISupportsBytes
    {

        private DelayImportAddressTables tables;
        private DelayImportDirectoryEntry dir_entry;
        private Location location;
        private DelayImportAddressTableEntry[] entries;

        internal DelayImportAddressTable(DelayImportAddressTables addressTables, DelayImportDirectoryEntry directoryEntry, uint tableRVA, ulong[] tableEntries)
        {
            bool is_64bit = addressTables.DataDirectory.Directories.Image.Is64Bit;
            LocationCalculator calc = addressTables.DataDirectory.Directories.Image.GetCalculator();
            uint rva = tableRVA;
            ulong image_base = addressTables.DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            ulong va = image_base + rva;
            ulong offset = calc.RVAToOffset(rva);
            ulong size = (tableEntries.Count() * (is_64bit ? sizeof(ulong) : sizeof(uint))).ToUInt64();
            Section section = calc.RVAToSection(rva);

            tables = addressTables;
            dir_entry = directoryEntry;
            location = new Location(offset, rva, va, size, size, section);
            entries = LoadEntries(is_64bit, offset, tableEntries);
        }

        #region Methods

        public IEnumerator<DelayImportAddressTableEntry> GetEnumerator()
        {
            for(var i = 0; i < entries.Length; i++)
            {
                yield return entries[i];
            }
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
            Stream stream = tables.DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        private DelayImportAddressTableEntry[] LoadEntries(bool is64Bit, ulong tableOffset, ulong[] tableEntries)
        {
            DelayImportAddressTableEntry[] results = new DelayImportAddressTableEntry[tableEntries.Length];
            ulong offset = tableOffset;

            for(var i = 0; i < tableEntries.Length; i++)
            {
                ulong addr_or_ord = tableEntries[i];
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

                DelayImportAddressTableEntry entry = new DelayImportAddressTableEntry(this, offset, addr_or_ord, address, ordinal, is_ordinal);

                results[i] = entry;
                offset += Convert.ToUInt32(is64Bit ? sizeof(ulong) : sizeof(uint));
            }

            return results;
        }

        #endregion

        #region Properties

        public DelayImportAddressTables Tables
        {
            get
            {
                return tables;
            }
        }

        public DelayImportDirectoryEntry DirectoryEntry
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

        public DelayImportAddressTableEntry this[int index]
        {
            get
            {
                return entries[index];
            }
        }

        #endregion

    }

    public sealed class DelayImportAddressTables : ExecutableImageContent, IEnumerable<DelayImportAddressTable>, ISupportsBytes
    {

        private DelayImportAddressTable[] tables;

        internal DelayImportAddressTables(DataDirectory dataDirectory, Location dataLocation, Tuple<uint, DelayImportDirectoryEntry, ulong[]>[] addressTables) : base(dataDirectory,dataLocation)
        {
            tables = new DelayImportAddressTable[addressTables.Length];

            LocationCalculator calc = DataDirectory.Directories.Image.GetCalculator();

            for (var i = 0; i < addressTables.Length; i++)
            {
                Tuple<uint, DelayImportDirectoryEntry, ulong[]> tuple = addressTables[i];
                ulong offset = calc.RVAToOffset(tuple.Item1);
                DelayImportAddressTable table = new DelayImportAddressTable(this, tuple.Item2, tuple.Item1, tuple.Item3);

                tables[i] = table;
            }
        }

        #region Static Methods

        public static DelayImportAddressTables GetLookupTable(DelayImportDirectory directory)
        {
            LocationCalculator calc = directory.DataDirectory.Directories.Image.GetCalculator();
            bool is_64bit = directory.DataDirectory.Directories.Image.Is64Bit;
            Stream stream = directory.DataDirectory.Directories.Image.GetStream();
            List<Tuple<uint, DelayImportDirectoryEntry, ulong[]>> tables = new List<Tuple<uint, DelayImportDirectoryEntry, ulong[]>>();

            foreach (DelayImportDirectoryEntry dir_entry in directory)
            {
                if (dir_entry.DelayNameTable == 0)
                    continue;

                List<ulong> entries = new List<ulong>();
                ulong offset = calc.RVAToOffset(dir_entry.DelayNameTable);

                stream.Seek(offset.ToInt64(), SeekOrigin.Begin);

                while (true)
                {
                    ulong entry = (!is_64bit ? Utils.ReadUInt32(stream) : Utils.ReadUInt64(stream));

                    if (entry == 0)
                        break;

                    entries.Add(entry);
                }

                Tuple<uint, DelayImportDirectoryEntry, ulong[]> table = new Tuple<uint, DelayImportDirectoryEntry, ulong[]>(dir_entry.DelayNameTable, dir_entry, entries.ToArray());

                tables.Add(table);
            }

            uint rva = 0;

            if (tables.Count > 0)
                rva = tables.MinBy(table => table.Item1).Item1;

            ulong image_base = directory.DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            ulong va = image_base + rva;
            ulong file_offset = calc.RVAToOffset(rva);
            ulong file_size = 0;

            foreach (var table in tables)
            {
                int size = (table.Item3.Length + 1) * (!is_64bit ? sizeof(uint) : sizeof(ulong));

                file_size += size.ToUInt32();
            }

            Section section = calc.RVAToSection(rva);
            Location location = new Location(file_offset, rva, va, file_size, file_size, section);
            DelayImportAddressTables result = new DelayImportAddressTables(directory.DataDirectory, location, tables.ToArray());

            return result;
        }

        public static DelayImportAddressTables GetAddressTable(DelayImportDirectory directory)
        {
            LocationCalculator calc = directory.DataDirectory.Directories.Image.GetCalculator();          
            bool is_64bit = directory.DataDirectory.Directories.Image.Is64Bit;
            Stream stream = directory.DataDirectory.Directories.Image.GetStream();
            List<Tuple<uint, DelayImportDirectoryEntry, ulong[]>> tables = new List<Tuple<uint, DelayImportDirectoryEntry, ulong[]>>();

            foreach (DelayImportDirectoryEntry dir_entry in directory)
            {
                if (dir_entry.DelayAddressTable == 0)
                    continue;

                List<ulong> entries = new List<ulong>();
                ulong offset = calc.RVAToOffset(dir_entry.DelayAddressTable);

                stream.Seek(offset.ToInt64(), SeekOrigin.Begin);

                while (true)
                {
                    ulong entry = (!is_64bit ? Utils.ReadUInt32(stream) : Utils.ReadUInt64(stream));

                    if (entry == 0)
                        break;

                    entries.Add(entry);
                }

                Tuple<uint, DelayImportDirectoryEntry, ulong[]> table = new Tuple<uint, DelayImportDirectoryEntry, ulong[]>(dir_entry.DelayAddressTable, dir_entry, entries.ToArray());

                tables.Add(table);
            }

            uint rva = 0;

            if (tables.Count > 0)
                rva = tables.MinBy(table => table.Item1).Item1;

            ulong image_base = directory.DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            ulong va = image_base + rva;
            ulong file_offset = calc.RVAToOffset(rva);
            ulong file_size = 0;

            foreach (var table in tables)
            {
                int size = (table.Item3.Length + 1) * (!is_64bit ? sizeof(uint) : sizeof(ulong));

                file_size += size.ToUInt32();
            }

            Section section = calc.RVAToSection(rva);
            Location location = new Location(file_offset, rva, va, file_size, file_size, section);
            DelayImportAddressTables result = new DelayImportAddressTables(directory.DataDirectory, location, tables.ToArray());

            return result;
        }

        #endregion

        #region Methods

        public IEnumerator<DelayImportAddressTable> GetEnumerator()
        {
            for(var i = 0; i < tables.Length; i++)
            {
                yield return tables[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("File Offset: 0x{0:X8}, Table Count: {1}",Location.FileOffset,tables.Length);
        }

        public byte[] GetBytes()
        {
            Stream stream = DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,Location);

            return buffer;
        }

        #endregion

        #region Properties

        public int Count
        {
            get
            {
                return tables.Length;
            }
        }

        public DelayImportAddressTable this[int index]
        {
            get
            {
                return tables[index];
            }
        }

        #endregion

    }

}
