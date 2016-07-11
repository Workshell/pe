#region License
//  Copyright(c) 2016, Workshell Ltd
//  All rights reserved.
//  
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Workshell Ltd nor the names of its contributors
//  may be used to endorse or promote products
//  derived from this software without specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED.IN NO EVENT SHALL WORKSHELL BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Imports
{

    public sealed class ImportLibrary : IEnumerable<ImportLibraryFunction>
    {

        private ImportCollection imports;
        private ImportAddressTable address_table;
        private ImportHintNameTable name_table;
        private string name;
        private ImportLibraryFunction[] functions;

        internal ImportLibrary(ImportCollection owningImports, ImportAddressTable addressTable, ImportHintNameTable nameTable, string libraryName)
        {
            imports = owningImports;
            address_table = addressTable;
            name_table = nameTable;
            name = libraryName;
            functions = LoadFunctions();
        }

        #region Methods

        public IEnumerator<ImportLibraryFunction> GetEnumerator()
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

        public IEnumerable<ImportLibraryNamedFunction> GetNamedFunctions()
        {
            for(var i = 0; i < functions.Length; i++)
            {
                if (functions[i].BindingType == ImportLibraryBindingType.Name)
                    yield return (ImportLibraryNamedFunction)functions[i];
            }
        }

        public IEnumerable<ImportLibraryOrdinalFunction> GetOrdinalFunctions()
        {
            for (var i = 0; i < functions.Length; i++)
            {
                if (functions[i].BindingType == ImportLibraryBindingType.Ordinal)
                    yield return (ImportLibraryOrdinalFunction)functions[i];
            }
        }

        private ImportLibraryFunction[] LoadFunctions()
        {
            List<ImportLibraryFunction> list = new List<ImportLibraryFunction>();

            foreach (ImportAddressTableEntry entry in address_table)
            {
                ImportLibraryFunction func = null;

                if (entry.IsOrdinal)
                {
                    func = new ImportLibraryOrdinalFunction(this, entry, entry.Ordinal);
                }
                else
                {
                    ImportHintNameEntry hint_entry = name_table.FirstOrDefault(hne => hne.Location.RelativeVirtualAddress == entry.Address);

                    if (hint_entry != null)
                        func = new ImportLibraryNamedFunction(this, entry, hint_entry);
                }

                if (func != null)
                    list.Add(func);
            }

            ImportLibraryFunction[] functions = list.ToArray();

            return functions;
        }

        #endregion

        #region Properties

        public ImportCollection Imports
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
