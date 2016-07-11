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

using Workshell.PE.Native;

namespace Workshell.PE.Imports
{

    public sealed class ImportCollection : IEnumerable<ImportLibrary>
    {

        private ImportLibrary[] libraries;

        internal ImportCollection(Tuple<string,ImportAddressTable,ImportHintNameTable>[] importLibraries)
        {
            libraries = LoadLibraries(importLibraries);
        }

        #region Static Methods

        public static ImportCollection Get(ExecutableImage image)
        {
            ImportDirectory directory = ImportDirectory.Get(image);

            if (directory == null)
                return null;

            ImportAddressTables ilt = ImportAddressTables.GetLookupTable(directory);

            if (ilt == null)
                return null;

            ImportHintNameTable hnt = ImportHintNameTable.Get(directory);

            if (hnt == null)
                return null;

            return Get(ilt, hnt);
        }

        public static ImportCollection Get(ImportAddressTables ilt, ImportHintNameTable hnTable)
        {
            List<Tuple<string, ImportAddressTable, ImportHintNameTable>> libraries = new List<Tuple<string, ImportAddressTable, ImportHintNameTable>>();
            LocationCalculator calc = ilt.DataDirectory.Directories.Image.GetCalculator();
            Stream stream = ilt.DataDirectory.Directories.Image.GetStream();

            foreach (ImportAddressTable table in ilt)
            {
                StringBuilder builder = new StringBuilder(256);
                ulong offset = calc.RVAToOffset(table.DirectoryEntry.Name);

                stream.Seek(Convert.ToInt64(offset), SeekOrigin.Begin);

                while (true)
                {
                    int b = stream.ReadByte();

                    if (b <= 0)
                        break;

                    builder.Append((char)b);
                }

                Tuple<string, ImportAddressTable, ImportHintNameTable> tuple = new Tuple<string, ImportAddressTable, ImportHintNameTable>(builder.ToString(), table, hnTable);

                libraries.Add(tuple);
            }

            ImportCollection imports = new ImportCollection(libraries.ToArray());

            return imports;
        }

        #endregion

        #region Methods

        public IEnumerator<ImportLibrary> GetEnumerator()
        {
            for(var i = 0; i < libraries.Length; i++)
            {
                yield return libraries[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("Library Count: {0}",libraries.Length);
        }

        private ImportLibrary[] LoadLibraries(Tuple<string,ImportAddressTable,ImportHintNameTable>[] importLibraries)
        {
            ImportLibrary[] results = new ImportLibrary[importLibraries.Length];

            for(var i = 0; i < importLibraries.Length; i++)
            {
                Tuple<string, ImportAddressTable, ImportHintNameTable> tuple = importLibraries[i];
                ImportLibrary library = new ImportLibrary(this, tuple.Item2, tuple.Item3, tuple.Item1);

                results[i] = library;
            }

            return results;
        }

        #endregion

        #region Properties

        public int Count
        {
            get
            {
                return libraries.Length;
            }
        }

        public ImportLibrary this[int index]
        {
            get
            {
                return libraries[index];
            }
        }

        public ImportLibrary this[string libraryName]
        {
            get
            {
                ImportLibrary library = libraries.FirstOrDefault(lib => String.Compare(libraryName,lib.Name,StringComparison.OrdinalIgnoreCase) == 0);

                return library;
            }
        }

        #endregion

    }

}
