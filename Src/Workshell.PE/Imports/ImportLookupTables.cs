using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class ImportLookupTables : IEnumerable<ImportLookupTable>
    {

        private ImportContent content;
        private List<ImportLookupTable> tables;
        private StreamLocation location;

        internal ImportLookupTables(ImportContent importContent, Stream stream)
        {
            content = importContent;
            tables = new List<ImportLookupTable>();

            foreach(ImportDirectoryEntry entry in importContent.Directory)
            {
                //ImportLookupTable table = new ImportLookupTable(stream,this,entry);
                //
                //tables.Add(table);
            }

            //ImportLookupTable first_table = tables.MinBy(table => table.Location.Offset);
            //long size = tables.Sum(table => table.Location.Size);
            //
            //location = new DataLocation(first_table.Location.Offset,size);
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
