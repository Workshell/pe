using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public class Imports : IEnumerable<ImportLibrary>
    {

        private ImportTableContent content;
        private List<ImportLibrary> libraries;

        internal Imports(ImportTableContent tableContent, IEnumerable<Tuple<string,ImportAddressTable,ImportHintNameTable>> importLibraries)
        {
            content = tableContent;
            libraries = new List<ImportLibrary>();

            LoadLibraries(importLibraries);
        }

        #region Methods

        public IEnumerator<ImportLibrary> GetEnumerator()
        {
            return libraries.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void LoadLibraries(IEnumerable<Tuple<string,ImportAddressTable,ImportHintNameTable>> importLibraries)
        {
            foreach(Tuple<string,ImportAddressTable,ImportHintNameTable> tuple in importLibraries)
            {
                ImportLibrary library = new ImportLibrary(this,tuple.Item2,tuple.Item3,tuple.Item1);

                libraries.Add(library);
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

        public int Count
        {
            get
            {
                return libraries.Count;
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
