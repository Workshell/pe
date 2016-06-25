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

    public sealed class DelayImportHintNameEntry : ISupportsLocation, ISupportsBytes
    {

        private DelayImportHintNameTable table;
        private Location location;
        private ushort hint;
        private string name;
        private bool is_padded;

        internal DelayImportHintNameEntry(DelayImportHintNameTable parentTable, ulong offset, uint size, ushort entryHint, string entryName, bool isPadded)
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

        public DelayImportHintNameTable Table
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

    public sealed class DelayImportHintNameTable : ExecutableImageContent, IEnumerable<DelayImportHintNameEntry>, ISupportsBytes
    {

        private DelayImportHintNameEntry[] table;

        internal DelayImportHintNameTable(DataDirectory dataDirectory, Location dataLocation, Tuple<ulong,uint,ushort,string,bool>[] tableEntries) : base(dataDirectory,dataLocation)
        {
            table = LoadTable(tableEntries);
        }

        #region Static Methods

        public static DelayImportHintNameTable Get(DelayImportDirectory directory)
        {
            Dictionary<uint, Tuple<ulong, uint, ushort, string, bool>> entries = new Dictionary<uint, Tuple<ulong, uint, ushort, string, bool>>();
            DelayImportAddressTables tables = DelayImportAddressTables.GetLookupTable(directory);
            LocationCalculator calc = directory.DataDirectory.Directories.Image.GetCalculator();
            Stream stream = directory.DataDirectory.Directories.Image.GetStream();

            foreach (DelayImportAddressTable table in tables)
            {
                foreach (DelayImportAddressTableEntry entry in table)
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

            var first_entry = entries.Values.MinBy(tuple => tuple.Item1);
            var last_entry = entries.Values.MaxBy(tuple => tuple.Item1);

            ulong table_offset = first_entry.Item1;
            uint table_size = ((last_entry.Item1 + last_entry.Item2) - table_offset).ToUInt32();

            uint table_rva = calc.OffsetToRVA(table_offset);
            ulong image_base = directory.DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            ulong table_va = image_base + table_rva;
            Section table_section = calc.RVAToSection(table_rva);
            Location location = new Location(table_offset, table_rva, table_va, table_size, table_size, table_section);

            DelayImportHintNameTable hint_name_table = new DelayImportHintNameTable(directory.DataDirectory, location, entries.Values.ToArray());

            return hint_name_table;
        }

        #endregion

        #region Methods

        public IEnumerator<DelayImportHintNameEntry> GetEnumerator()
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

        private DelayImportHintNameEntry[] LoadTable(Tuple<ulong, uint, ushort, string, bool>[] tableEntries)
        {
            DelayImportHintNameEntry[] results = new DelayImportHintNameEntry[tableEntries.Length];

            for(var i = 0; i < tableEntries.Length; i++)
            {
                DelayImportHintNameEntry entry = new DelayImportHintNameEntry(this, tableEntries[i].Item1, tableEntries[i].Item2, tableEntries[i].Item3, tableEntries[i].Item4, tableEntries[i].Item5);

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

        public DelayImportHintNameEntry this[int index]
        {
            get
            {
                return table[index];
            }
        }

        #endregion

    }

}
