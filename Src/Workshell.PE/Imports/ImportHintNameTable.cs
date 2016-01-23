using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.MoreLinq;

namespace Workshell.PE
{

    public class ImportHintNameEntry : ILocationSupport
    {

        private ImportHintNameTable table;
        private StreamLocation location;
        private ushort hint;
        private string name;
        private bool is_padded;

        internal ImportHintNameEntry(ImportHintNameTable parentTable, long offset, long size, ushort entryHint, string entryName, bool isPadded)
        {
            table = parentTable;
            location = new StreamLocation(offset,size);
            hint = entryHint;
            name = entryName;
            is_padded = isPadded;
        }

        #region Methods

        public override string ToString()
        {
            return String.Format("0x{0:X4} {1}",hint,name);
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

        public StreamLocation Location
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

    public class ImportHintNameTable : ILocationSupport, IRawDataSupport, IEnumerable<ImportHintNameEntry>
    {

        private ImportContent content;
        private List<ImportHintNameEntry> table;
        private StreamLocation location;

        internal ImportHintNameTable(ImportContent importContent)
        {
            content = importContent;
            table = new List<ImportHintNameEntry>();
            location = null;
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

        public byte[] GetBytes()
        {
            if (location == null)
                UpdateLocation();

            Stream stream = content.Section.Sections.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        private void UpdateLocation()
        {
            if (table.Count == 0)
            {
                location = new StreamLocation(0,0);
                
                return;
            }

            ImportHintNameEntry first_entry = table.MinBy(entry => entry.Location.Offset);
            ImportHintNameEntry last_entry = table.MaxBy(entry => entry.Location.Offset);

            long offset = first_entry.Location.Offset;
            long size = (last_entry.Location.Offset + last_entry.Location.Size) - offset;

            location = new StreamLocation(offset,size);
        }

        internal ImportHintNameEntry Create(long offset, long size, ushort hint, string name, bool isPadded)
        {
            ImportHintNameEntry entry = table.FirstOrDefault(e => e.Location.Offset == offset);

            if (entry != null)
                return entry;

            entry = new ImportHintNameEntry(this,offset,size,hint,name,isPadded);
            location = null;

            table.Add(entry);

            return entry;
        }

        #endregion

        #region Properties

        public ImportContent Content
        {
            get
            {
                return content;
            }
        }

        public StreamLocation Location
        {
            get
            {
                if (location == null)
                    UpdateLocation();

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
