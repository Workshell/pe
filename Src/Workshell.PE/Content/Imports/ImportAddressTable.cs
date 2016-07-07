#region License
//  Copyright(c) 2016, Workshell Ltd
//  All rights reserved.
//  
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Workshell Ltd nor the names of its contributors
//  may be used to endorse or promote products
//  derived from this software without specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED.IN NO EVENT SHALL WORKSHELL BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

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

    public sealed class ImportAddressTableEntry : ISupportsLocation, ISupportsBytes
    {

        private ImportAddressTable table;
        private Location location;
        private ulong value;
        private uint address;
        private ushort ordinal;
        private bool is_ordinal;

        internal ImportAddressTableEntry(ImportAddressTable addressTable, ulong entryOffset, ulong entryValue, uint entryAddress, ushort entryOrdinal, bool isOrdinal)
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

    public sealed class ImportAddressTable : IEnumerable<ImportAddressTableEntry>, ISupportsLocation, ISupportsBytes
    {

        private ImportAddressTables tables;
        private ImportDirectoryEntry dir_entry;
        private Location location;
        private ImportAddressTableEntry[] entries;

        internal ImportAddressTable(ImportAddressTables addressTables,  ImportDirectoryEntry directoryEntry, uint tableRVA/*, ulong tableOffset*/, ulong[] tableEntries)
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

        public IEnumerator<ImportAddressTableEntry> GetEnumerator()
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

        private ImportAddressTableEntry[] LoadEntries(bool is64Bit, ulong tableOffset, ulong[] tableEntries)
        {
            ImportAddressTableEntry[] results = new ImportAddressTableEntry[tableEntries.Length];
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

                ImportAddressTableEntry entry = new ImportAddressTableEntry(this, offset, addr_or_ord, address, ordinal, is_ordinal);

                results[i] = entry;
                offset += Convert.ToUInt32(is64Bit ? sizeof(ulong) : sizeof(uint));
            }

            return results;
        }

        #endregion

        #region Properties

        public ImportAddressTables Tables
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

    public sealed class ImportAddressTables : ExecutableImageContent, IEnumerable<ImportAddressTable>, ISupportsBytes
    {

        private ImportAddressTable[] tables;

        internal ImportAddressTables(DataDirectory dataDirectory, Location dataLocation, Tuple<uint, ImportDirectoryEntry, ulong[]>[] addressTables) : base(dataDirectory,dataLocation)
        {
            tables = new ImportAddressTable[addressTables.Length];

            LocationCalculator calc = DataDirectory.Directories.Image.GetCalculator();

            for (var i = 0; i < addressTables.Length; i++)
            {
                Tuple<uint, ImportDirectoryEntry, ulong[]> tuple = addressTables[i];
                ulong offset = calc.RVAToOffset(tuple.Item1);
                ImportAddressTable table = new ImportAddressTable(this, tuple.Item2, tuple.Item1, tuple.Item3);

                tables[i] = table;
            }
        }

        #region Static Methods

        public static ImportAddressTables GetLookupTable(ImportDirectory directory)
        {
            LocationCalculator calc = directory.DataDirectory.Directories.Image.GetCalculator();          
            bool is_64bit = directory.DataDirectory.Directories.Image.Is64Bit;
            Stream stream = directory.DataDirectory.Directories.Image.GetStream();
            List<Tuple<uint, ImportDirectoryEntry, ulong[]>> tables = new List<Tuple<uint, ImportDirectoryEntry, ulong[]>>();

            foreach (ImportDirectoryEntry dir_entry in directory)
            {
                if (dir_entry.OriginalFirstThunk == 0)
                    continue;

                List<ulong> entries = new List<ulong>();
                ulong offset = calc.RVAToOffset(dir_entry.OriginalFirstThunk);

                stream.Seek(offset.ToInt64(), SeekOrigin.Begin);

                while (true)
                {
                    ulong entry = (!is_64bit ? Utils.ReadUInt32(stream) : Utils.ReadUInt64(stream));

                    entries.Add(entry);

                    if (entry == 0)
                        break;
                }

                Tuple<uint, ImportDirectoryEntry, ulong[]> table = new Tuple<uint, ImportDirectoryEntry, ulong[]>(dir_entry.OriginalFirstThunk, dir_entry, entries.ToArray());

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
                int size = table.Item3.Length * (!is_64bit ? sizeof(uint) : sizeof(ulong));

                file_size += size.ToUInt32();
            }

            Section section = calc.RVAToSection(rva);
            Location location = new Location(file_offset, rva, va, file_size, file_size, section);
            ImportAddressTables result = new ImportAddressTables(directory.DataDirectory, location, tables.ToArray());

            return result;
        }

        public static ImportAddressTables GetAddressTable(ImportDirectory directory)
        {
            LocationCalculator calc = directory.DataDirectory.Directories.Image.GetCalculator();
            bool is_64bit = directory.DataDirectory.Directories.Image.Is64Bit;
            Stream stream = directory.DataDirectory.Directories.Image.GetStream();
            List<Tuple<uint, ImportDirectoryEntry, ulong[]>> tables = new List<Tuple<uint, ImportDirectoryEntry, ulong[]>>();

            foreach (ImportDirectoryEntry dir_entry in directory)
            {
                if (dir_entry.FirstThunk == 0)
                    continue;

                List<ulong> entries = new List<ulong>();
                ulong offset = calc.RVAToOffset(dir_entry.FirstThunk);

                stream.Seek(offset.ToInt64(), SeekOrigin.Begin);

                while (true)
                {
                    ulong entry = (!is_64bit ? Utils.ReadUInt32(stream) : Utils.ReadUInt64(stream));

                    entries.Add(entry);

                    if (entry == 0)
                        break;
                }

                Tuple<uint, ImportDirectoryEntry, ulong[]> table = new Tuple<uint, ImportDirectoryEntry, ulong[]>(dir_entry.FirstThunk, dir_entry, entries.ToArray());

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
                int size = table.Item3.Length * (!is_64bit ? sizeof(uint) : sizeof(ulong));

                file_size += size.ToUInt32();
            }

            Section section = calc.RVAToSection(rva);
            Location location = new Location(file_offset, rva, va, file_size, file_size, section);
            ImportAddressTables result = new ImportAddressTables(directory.DataDirectory, location, tables.ToArray());

            return result;
        }

        #endregion

        #region Methods

        public IEnumerator<ImportAddressTable> GetEnumerator()
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
