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
                StreamLocation entry_location = new StreamLocation(offset,size);
                ImportLookupTableEntry entry = new ImportLookupTableEntry(this,entry_location,table_entry);

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
