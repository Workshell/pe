using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public sealed class DelayImportLibrary : IEnumerable<DelayImportLibraryFunction>
    {

        private DelayImports imports;
        private DelayImportAddressTable address_table;
        private DelayImportHintNameTable name_table;
        private string name;
        private DelayImportLibraryFunction[] functions;

        internal DelayImportLibrary(DelayImports owningImports, DelayImportAddressTable addressTable, DelayImportHintNameTable nameTable, string libraryName)
        {
            imports = owningImports;
            address_table = addressTable;
            name_table = nameTable;
            name = libraryName;
            functions = LoadFunctions();
        }

        #region Methods

        public IEnumerator<DelayImportLibraryFunction> GetEnumerator()
        {
            for(var i = 0; i < functions.Length; i++)
            {
                yield return functions[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("Name: {0}, Imported Function Count: {1}",name,functions.Length);
        }

        public IEnumerable<DelayImportLibraryNamedFunction> GetNamedFunctions()
        {
            for(var i = 0; i < functions.Length; i++)
            {
                if (functions[i].BindingType == DelayImportLibraryBindingType.Name)
                    yield return (DelayImportLibraryNamedFunction)functions[i];
            }
        }

        public IEnumerable<DelayImportLibraryOrdinalFunction> GetOrdinalFunctions()
        {
            for (var i = 0; i < functions.Length; i++)
            {
                if (functions[i].BindingType == DelayImportLibraryBindingType.Ordinal)
                    yield return (DelayImportLibraryOrdinalFunction)functions[i];
            }
        }

        private DelayImportLibraryFunction[] LoadFunctions()
        {
            List<DelayImportLibraryFunction> list = new List<DelayImportLibraryFunction>();

            foreach (DelayImportAddressTableEntry entry in address_table)
            {
                DelayImportLibraryFunction func = null;

                if (entry.IsOrdinal)
                {
                    func = new DelayImportLibraryOrdinalFunction(this, entry, entry.Ordinal);
                }
                else
                {
                    DelayImportHintNameEntry hint_entry = name_table.FirstOrDefault(hne => hne.Location.RelativeVirtualAddress == entry.Address);

                    if (hint_entry != null)
                        func = new DelayImportLibraryNamedFunction(this, entry, hint_entry);
                }

                if (func != null)
                    list.Add(func);
            }

            DelayImportLibraryFunction[] functions = list.ToArray();

            return functions;
        }

        #endregion

        #region Properties

        public DelayImports Imports
        {
            get
            {
                return imports;
            }
        }

        public DelayImportAddressTable Table
        {
            get
            {
                return address_table;
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
                return functions.Length;
            }
        }

        public DelayImportLibraryFunction this[int index]
        {
            get
            {
                return functions[index];
            }
        }

        #endregion

    }

}
