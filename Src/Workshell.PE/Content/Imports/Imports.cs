using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class Imports : IEnumerable<ImportLibrary>, IReadOnlyCollection<ImportLibrary>
    {

        private ImportTableContent content;
        private ImportLibrary[] libraries;

        internal Imports(ImportTableContent tableContent, IEnumerable<Tuple<string,ImportAddressTable,ImportHintNameTable>> importLibraries)
        {
            content = tableContent;
            libraries = new ImportLibrary[0];

            LoadLibraries(importLibraries);
        }

        #region Methods

        public IEnumerator<ImportLibrary> GetEnumerator()
        {
            return libraries.Cast<ImportLibrary>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("Library Count: {0}",libraries.Length);
        }

        private void LoadLibraries(IEnumerable<Tuple<string,ImportAddressTable,ImportHintNameTable>> importLibraries)
        {
            List<ImportLibrary> list = new List<ImportLibrary>();

            foreach(Tuple<string,ImportAddressTable,ImportHintNameTable> tuple in importLibraries)
            {
                ImportLibrary library = new ImportLibrary(this,tuple.Item2,tuple.Item3,tuple.Item1);

                list.Add(library);
            }

            libraries = list.ToArray();
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
