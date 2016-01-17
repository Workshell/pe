using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class ImportLibrary : IEnumerable<ImportLibraryFunction>
    {

        private ImportContent content;
        private ImportLookupTable table;
        private string name;
        private List<ImportLibraryFunction> functions;

        internal ImportLibrary(ImportContent importContent, ImportLookupTable lookupTable, string libraryName)
        {
            content = importContent;
            table = lookupTable;
            name = libraryName;
            functions = new List<ImportLibraryFunction>();
        }

        #region Methods

        public IEnumerator<ImportLibraryFunction> GetEnumerator()
        {
            return functions.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return name;
        }

        internal void Add(ImportLookupTableEntry tableEntry, int ordinalNo)
        {
            ImportLibraryOrdinalFunction function = new ImportLibraryOrdinalFunction(this,tableEntry,ordinalNo);

            functions.Add(function);
        }

        internal void Add(ImportLookupTableEntry tableEntry, ImportHintNameEntry hintEntry)
        {
            ImportLibraryNamedFunction function = new ImportLibraryNamedFunction(this,tableEntry,hintEntry);

            functions.Add(function);
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

        public ImportLookupTable Table
        {
            get
            {
                return table;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public int Count
        {
            get
            {
                return functions.Count;
            }
        }

        public ImportLibraryFunction this[int index]
        {
            get
            {
                return functions[index];
            }
        }

        #endregion

    }

}
