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

namespace Workshell.PE
{

    public sealed class DelayImports : IEnumerable<DelayImportLibrary>
    {

        private DelayImportLibrary[] libraries;

        internal DelayImports(Tuple<string,DelayImportAddressTable,DelayImportHintNameTable>[] importLibraries)
        {
            libraries = LoadLibraries(importLibraries);
        }

        #region Static Methods

        public static DelayImports Get(ExecutableImage image)
        {
            DelayImportDirectory directory = DelayImportDirectory.Get(image);

            if (directory == null)
                return null;

            DelayImportAddressTables ilt = DelayImportAddressTables.GetLookupTable(directory);

            if (ilt == null)
                return null;

            DelayImportHintNameTable hnt = DelayImportHintNameTable.Get(directory);

            if (hnt == null)
                return null;

            return Get(ilt, hnt);
        }


        public static DelayImports Get(DelayImportAddressTables ilt, DelayImportHintNameTable hnTable)
        {
            List<Tuple<string, DelayImportAddressTable, DelayImportHintNameTable>> libraries = new List<Tuple<string, DelayImportAddressTable, DelayImportHintNameTable>>();
            LocationCalculator calc = ilt.DataDirectory.Directories.Image.GetCalculator();
            Stream stream = ilt.DataDirectory.Directories.Image.GetStream();

            foreach (DelayImportAddressTable table in ilt)
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

                Tuple<string, DelayImportAddressTable, DelayImportHintNameTable> tuple = new Tuple<string, DelayImportAddressTable, DelayImportHintNameTable>(builder.ToString(), table, hnTable);

                libraries.Add(tuple);
            }

            DelayImports imports = new DelayImports(libraries.ToArray());

            return imports;
        }

        #endregion

        #region Methods

        public IEnumerator<DelayImportLibrary> GetEnumerator()
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

        private DelayImportLibrary[] LoadLibraries(Tuple<string, DelayImportAddressTable, DelayImportHintNameTable>[] importLibraries)
        {
            DelayImportLibrary[] results = new DelayImportLibrary[importLibraries.Length];

            for(var i = 0; i < importLibraries.Length; i++)
            {
                Tuple<string, DelayImportAddressTable, DelayImportHintNameTable> tuple = importLibraries[i];
                DelayImportLibrary library = new DelayImportLibrary(this, tuple.Item2, tuple.Item3, tuple.Item1);

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

        public DelayImportLibrary this[int index]
        {
            get
            {
                return libraries[index];
            }
        }

        public DelayImportLibrary this[string libraryName]
        {
            get
            {
                DelayImportLibrary library = libraries.FirstOrDefault(lib => String.Compare(libraryName,lib.Name,StringComparison.OrdinalIgnoreCase) == 0);

                return library;
            }
        }

        #endregion

    }

}
