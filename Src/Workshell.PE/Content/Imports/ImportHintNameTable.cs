using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.MoreLinq;

namespace Workshell.PE
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
            LocationCalculator calc = parentTable.Content.DataDirectory.Directories.Reader.GetCalculator();

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
            Stream stream = table.Content.DataDirectory.Directories.Reader.GetStream();
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

    public sealed class ImportHintNameTable : IEnumerable<ImportHintNameEntry>, IReadOnlyCollection<ImportHintNameEntry>, ISupportsLocation, ISupportsBytes
    {

        private ImportTableContent content;
        private List<ImportHintNameEntry> table;
        private Location location;
        private Section section;

        internal ImportHintNameTable(ImportTableContent tableContent, IEnumerable<Tuple<ulong,uint,ushort,string,bool>> tableEntries)
        {
            content = tableContent;
            table = new List<ImportHintNameEntry>();
            location = null;

            LoadTable(tableEntries);
        }

        #region Methods

        public IEnumerator<ImportHintNameEntry> GetEnumerator()
        {
            return table.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("File Offset: 0x{0:X8}, Name Count: {1}", location.FileOffset, table.Count);
        }

        public byte[] GetBytes()
        {
            if (location == null)
                UpdateLocation();

            Stream stream = content.DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream, location);

            return buffer;
        }

        private void LoadTable(IEnumerable<Tuple<ulong, uint, ushort, string, bool>> tableEntries)
        {
            foreach(var tuple in tableEntries)
            {
                ImportHintNameEntry entry = new ImportHintNameEntry(this, tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5);

                table.Add(entry);
            }

            table = table.OrderBy(entry => entry.Location.VirtualAddress).ToList();
        }

        private void UpdateLocation()
        {
            if (table.Count == 0)
            {
                location = new Location(0,0,0,0,0);

                return;
            }

            ImportHintNameEntry first_entry = table.MinBy(entry => entry.Location.VirtualAddress);
            ImportHintNameEntry last_entry = table.MaxBy(entry => entry.Location.VirtualAddress);

            ulong offset = first_entry.Location.FileOffset;
            uint size = Convert.ToUInt32((last_entry.Location.FileOffset + last_entry.Location.FileSize) - offset);

            LocationCalculator calc = content.DataDirectory.Directories.Reader.GetCalculator();

            uint rva = calc.OffsetToRVA(offset);
            ulong va = calc.OffsetToVA(offset);

            location = new Location(offset, rva, va, size, size);
            section = calc.RVAToSection(rva);
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
                if (location == null)
                    UpdateLocation();

                return location;
            }
        }

        public Section Section
        {
            get
            {
                if (section == null)
                    UpdateLocation();

                return section;
            }
        }

        public int Count
        {
            get
            {
                return table.Count;
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
