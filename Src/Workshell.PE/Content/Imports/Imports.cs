using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class Imports : IEnumerable<ImportLibrary>
    {

        private ImportLibrary[] libraries;

        internal Imports(Tuple<string,ImportAddressTable,ImportHintNameTable>[] importLibraries)
        {
            libraries = LoadLibraries(importLibraries);
        }

        #region Static Methods

        public static Imports Get(ImportAddressTables ilt, ImportHintNameTable hnTable)
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

            Imports imports = new Imports(libraries.ToArray());

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
