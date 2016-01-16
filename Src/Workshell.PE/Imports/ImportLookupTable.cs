using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class ImportLookupTable : ILocationSupport, IRawDataSupport, IEnumerable<ImportLookupTableEntry>
    {

        private ImportLookupTables tables;
        private ImportDirectoryEntry directory_entry;
        private StreamLocation location;
        private List<ImportLookupTableEntry> entries;

        internal ImportLookupTable(ImportLookupTables lookupTables, ImportDirectoryEntry directoryEntry, List<ulong> tableEntries, StreamLocation streamLoc)
        {
            tables = lookupTables;
            directory_entry = directoryEntry;
            location = streamLoc;
            entries = new List<ImportLookupTableEntry>();

            long offset = location.Offset;
            long size = (tables.Content.Section.Sections.Reader.Is64Bit ? sizeof(ulong) : sizeof(uint));

            foreach(ulong table_entry in tableEntries)
            {
                int ordinal = -1;

                if (size == sizeof(uint))
                {
                    uint value = Convert.ToUInt32(table_entry);

                    if ((value & 0x80000000) == 0x80000000)
                    {
                        value &= 0x7fffffff;

                        ordinal = Convert.ToInt32(value);
                    }
                }
                else
                {
                    ulong value = table_entry;

                    if ((value & 0x8000000000000000) == 0x8000000000000000)
                    {
                        value &= 0x7fffffffffffffff;

                        ordinal = Convert.ToInt32(value);
                    }
                }

                uint address = (ordinal != -1 ? 0 : Convert.ToUInt32(table_entry));
                StreamLocation entry_location = new StreamLocation(offset,size);
                ImportLookupTableEntry entry = new ImportLookupTableEntry(this,entry_location,address,ordinal);

                entries.Add(entry);

                offset += size;
            }
        }

        #region Methods

        public IEnumerator<ImportLookupTableEntry> GetEnumerator()
        {
            return entries.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("Entries: {0:n0}",entries.Count);
        }

        public byte[] GetBytes()
        {
            byte[] buffer = Utils.ReadBytes(tables.Content.Section.Sections.Reader.Stream,location);

            return buffer;
        }

        #endregion

        #region Properties

        public ImportLookupTables Tables
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
                return directory_entry;
            }
        }

        public StreamLocation Location
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
                return entries.Count;
            }
        }

        public ImportLookupTableEntry this[int index]
        {
            get
            {
                return entries[index];
            }
        }

        #endregion

    }

}
