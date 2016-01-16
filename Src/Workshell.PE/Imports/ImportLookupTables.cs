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

        internal ImportLookupTables(ImportContent importContent, Dictionary<int,ILTTable> iltTables)
        {
            content = importContent;
            tables = new List<ImportLookupTable>();

            foreach(KeyValuePair<int,ILTTable> kvp in iltTables)
            {
                ImportDirectoryEntry directory_entry = importContent.Directory[kvp.Key];
                StreamLocation table_location = new StreamLocation(kvp.Value.Offset,kvp.Value.Size);

                ImportLookupTable table = new ImportLookupTable(this,directory_entry,kvp.Value.Entries,table_location);

                tables.Add(table);
            }

            ImportLookupTable first_table = tables.MinBy(table => table.Location.Offset);
            long size = tables.Sum(table => table.Location.Size);
            
            location = new StreamLocation(first_table.Location.Offset,size);
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

        public byte[] GetBytes()
        {
            byte[] buffer = Utils.ReadBytes(content.Section.Sections.Reader.Stream,location);

            return buffer;
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
