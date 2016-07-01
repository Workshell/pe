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
