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

namespace Workshell.PE.Imports
{

    public sealed class ImportHintNameEntry : ISupportsLocation, ISupportsBytes
    {

        private ImportHintNameTable table;
        private Location location;
        private ushort hint;
        private string name;
        private bool is_padded;

        internal ImportHintNameEntry(ImportHintNameTable parentTable, ulong offset, uint size, ushort entryHint, string entryName, bool isPadded)
        {
            LocationCalculator calc = parentTable.DataDirectory.Directories.Image.GetCalculator();

            uint rva = calc.OffsetToRVA(offset);
            ulong va = calc.OffsetToVA(offset);

            table = parentTable;
            location = new Location(offset, rva, va, size, size);
            hint = entryHint;
            name = entryName;
            is_padded = isPadded;
        }

        #region Methods

        public override string ToString()
        {
            return String.Format("0x{0:X4} {1}", hint, name);
        }

        public byte[] GetBytes()
        {
            Stream stream = table.DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream, location);

            return buffer;
        }

        #endregion

        #region Properties

        public ImportHintNameTable Table
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

        public ushort Hint
        {
            get
            {
                return hint;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public bool IsPadded
        {
            get
            {
                return is_padded;
            }
        }

        #endregion

    }

    public sealed class ImportHintNameTable : ExecutableImageContent, IEnumerable<ImportHintNameEntry>, ISupportsBytes
    {

        private ImportHintNameEntry[] table;

        internal ImportHintNameTable(DataDirectory dataDirectory, Location dataLocation, Tuple<ulong,uint,ushort,string,bool>[] tableEntries) : base(dataDirectory,dataLocation)
        {
            table = LoadTable(tableEntries);
        }

        #region Static Methods

        public static ImportHintNameTable Get(ImportDirectory directory)
        {
            if (directory == null)
                return null;

            Dictionary<uint, Tuple<ulong, uint, ushort, string, bool>> entries = new Dictionary<uint, Tuple<ulong, uint, ushort, string, bool>>();
            ImportAddressTables ilt = ImportAddressTables.GetLookupTable(directory);
            LocationCalculator calc = directory.DataDirectory.Directories.Image.GetCalculator();
            Stream stream = directory.DataDirectory.Directories.Image.GetStream();

            foreach (ImportAddressTable table in ilt)
            {
                foreach (ImportAddressTableEntry entry in table)
                {
                    if (entry.Address == 0)
                        continue;

                    if (entries.ContainsKey(entry.Address))
                        continue;

                    if (!entry.IsOrdinal)
                    {
                        ulong offset = calc.RVAToOffset(entry.Address);
                        uint size = 0;
                        bool is_padded = false;
                        ushort hint = 0;
                        StringBuilder name = new StringBuilder(256);

                        stream.Seek(offset.ToInt64(), SeekOrigin.Begin);

                        hint = Utils.ReadUInt16(stream);
                        size += sizeof(ushort);

                        while (true)
                        {
                            int b = stream.ReadByte();

                            size++;

                            if (b <= 0)
                                break;

                            name.Append((char)b);
                        }

                        if ((size % 2) != 0)
                        {
                            is_padded = true;
                            size++;
                        }

                        Tuple<ulong, uint, ushort, string, bool> tuple = new Tuple<ulong, uint, ushort, string, bool>(offset, size, hint, name.ToString(), is_padded);

                        entries.Add(entry.Address, tuple);
                    }
                }
            }

            Location location;

            if (entries.Count > 0)
            {
                var first_entry = entries.Values.MinBy(tuple => tuple.Item1);
                var last_entry = entries.Values.MaxBy(tuple => tuple.Item1);

                ulong table_offset = first_entry.Item1;
                uint table_size = ((last_entry.Item1 + last_entry.Item2) - table_offset).ToUInt32();

                uint table_rva = calc.OffsetToRVA(table_offset);
                ulong image_base = directory.DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
                ulong table_va = image_base + table_rva;
                Section table_section = calc.RVAToSection(table_rva);

                location = new Location(table_offset, table_rva, table_va, table_size, table_size, table_section);
            }
            else
            {
                location = new Location(0, 0, 0, 0, 0, null);
            }

            ImportHintNameTable hint_name_table = new ImportHintNameTable(directory.DataDirectory, location, entries.Values.ToArray());

            return hint_name_table;
        }

        #endregion

        #region Methods

        public IEnumerator<ImportHintNameEntry> GetEnumerator()
        {
            for(var i = 0; i < table.Length; i++)
            {
                yield return table[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("File Offset: 0x{0:X8}, Name Count: {1}",Location.FileOffset,table.Length);
        }

        public byte[] GetBytes()
        {
            Stream stream = DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream, Location);

            return buffer;
        }

        private ImportHintNameEntry[] LoadTable(Tuple<ulong, uint, ushort, string, bool>[] tableEntries)
        {
            ImportHintNameEntry[] results = new ImportHintNameEntry[tableEntries.Length];

            for(var i = 0; i < tableEntries.Length; i++)
            {
                ImportHintNameEntry entry = new ImportHintNameEntry(this, tableEntries[i].Item1, tableEntries[i].Item2, tableEntries[i].Item3, tableEntries[i].Item4, tableEntries[i].Item5);

                results[i] = entry;
            }

            return results.OrderBy(entry => entry.Location.FileOffset).ToArray();
        }

        #endregion

        #region Properties

        public int Count
        {
            get
            {
                return table.Length;
            }
        }

        public ImportHintNameEntry this[int index]
        {
            get
            {
                return table[index];
            }
        }

        #endregion

    }

}
