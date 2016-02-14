﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public sealed class ImportLibrary : IEnumerable<ImportLibraryFunction>, IReadOnlyCollection<ImportLibraryFunction>
    {

        private Imports imports;
        private ImportAddressTable address_table;
        private ImportHintNameTable name_table;
        private string name;
        private ImportLibraryFunction[] functions;

        internal ImportLibrary(Imports owningImports, ImportAddressTable addressTable, ImportHintNameTable nameTable, string libraryName)
        {
            imports = owningImports;
            address_table = addressTable;
            name_table = nameTable;
            name = libraryName;
            functions = new ImportLibraryFunction[0];

            LoadFunctions();
        }

        #region Methods

        public IEnumerator<ImportLibraryFunction> GetEnumerator()
        {
            return functions.Cast<ImportLibraryFunction>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("Name: {0}, Imported Function Count: {1}",name,functions.Length);
        }

        private void LoadFunctions()
        {
            List<ImportLibraryFunction> list = new List<ImportLibraryFunction>();
            LocationCalculator calc = imports.Content.DataDirectory.Directories.Reader.GetCalculator();

            foreach (ImportAddressTableEntry entry in address_table)
            {
                ImportLibraryFunction func = null;

                if (entry.IsOrdinal)
                {
                    func = new ImportLibraryOrdinalFunction(this, entry, entry.Ordinal);
                }
                else
                {
                    ulong offset = calc.RVAToOffset(entry.Address);
                    ImportHintNameEntry hint_entry = name_table.FirstOrDefault(hne => hne.Location.FileOffset == offset);

                    if (hint_entry != null)
                        func = new ImportLibraryNamedFunction(this, entry, hint_entry);
                }

                if (func != null)
                    list.Add(func);
            }

            functions = list.ToArray();
        }

        #endregion

        #region Properties

        public Imports Imports
        {
            get
            {
                return imports;
            }
        }

        public ImportAddressTable Table
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
