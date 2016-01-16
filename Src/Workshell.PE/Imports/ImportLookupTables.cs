using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.MoreLinq;

namespace Workshell.PE
{

    public class ImportLookupTables : ILocationSupport, IRawDataSupport, IEnumerable<ImportLookupTable>
    {

        private ImportContent content;
        private List<ImportLookupTable> tables;
        private StreamLocation location;

        internal ImportLookupTables(ImportContent importContent)
        {
            content = importContent;
            tables = new List<ImportLookupTable>();
            location = null;

            /*
            foreach(KeyValuePair<int,ILTTable> kvp in iltTables)
            {
                ImportDirectoryEntry directory_entry = importContent.Directory[kvp.Key];
                StreamLocation table_location = new StreamLocation(kvp.Value.Offset,kvp.Value.Size);

                ImportLookupTable table = new ImportLookupTable(this,directory_entry,kvp.Value.Entries,table_location);

                tables.Add(table);
            }
             */


        }

        #region Methods

        public IEnumerator<ImportLookupTable> GetEnumerator()
        {
            return tables.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("Tables: {0:n0}",tables.Count);
        }

        public byte[] GetBytes()
        {
            byte[] buffer = Utils.ReadBytes(content.Section.Sections.Reader.Stream,location);

            return buffer;
        }

        private void UpdateLocation()
        {
            if (tables.Count == 0)
            {
                location = new StreamLocation(0,0);
            }
            else
            {
                ImportLookupTable first_table = tables.MinBy(table => table.Location.Offset);
                long size = tables.Sum(table => table.Location.Size);
            
                location = new StreamLocation(first_table.Location.Offset,size);
            }
        }

        internal ImportLookupTable Create(ImportDirectoryEntry directoryEntry, long offset, long size, List<ulong> entries)
        {
            ImportLookupTable table = tables.FirstOrDefault(tbl => tbl.Location.Offset == offset);

            if (table != null)
                return table;

            StreamLocation table_location = new StreamLocation(offset,size);

            table = new ImportLookupTable(this,directoryEntry,entries,table_location);
            location = null;

            tables.Add(table);

            return table;
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
                return tables.Count;
            }
        }

        public ImportLookupTable this[int index]
        {
            get
            {
                return tables[index];
            }
        }

        #endregion

    }

}
