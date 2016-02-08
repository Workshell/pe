using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class ImportLibrary : IEnumerable<ImportLibraryFunction>
    {

        private ImportTableContent content;
        private ImportAddressTable table;
        private string name;
        private List<ImportLibraryFunction> functions;

        internal ImportLibrary(ImportTableContent tableContent, ImportAddressTable addressTable, string libraryName)
        {
            content = tableContent;
            table = addressTable;
            name = libraryName;
            functions = new List<ImportLibraryFunction>();

            LoadFunctions();
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
            return String.Format("Name: {0}, Imported Function Count: {1}", name, functions.Count);
        }

        private void LoadFunctions()
        {
            foreach (ImportAddressTableEntry entry in table)
            {
                ImportLibraryFunction func;

                if (entry.IsOrdinal)
                {
                    func = new ImportLibraryOrdinalFunction(this, entry, entry.Ordinal);
                }
                else
                {
                    ImportHintNameEntry hint_entry = content.HintNameTable.FirstOrDefault(hne => hne.Location.VirtualAddress == entry.Address);

                    if (hint_entry != null)
                        func = new ImportLibraryNamedFunction(this, entry, hint_entry);
                }

                functions.Add(func);
            }
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

        public ImportAddressTable Table
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
