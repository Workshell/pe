using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public enum ImportLibraryBindingType
    {
        Name,
        Ordinal
    }

    public abstract class ImportLibraryFunction
    {

        private ImportLibrary library;
        private ImportAddressTableEntry table_entry;

        internal ImportLibraryFunction(ImportLibrary importLibrary, ImportAddressTableEntry tableEntry)
        {
            library = importLibrary;
            table_entry = tableEntry;
        }

        #region Properties

        public ImportLibrary Library
        {
            get
            {
                return library;
            }
        }

        public ImportAddressTableEntry TableEntry
        {
            get
            {
                return table_entry;
            }
        }

        public abstract ImportLibraryBindingType BindingType
        {
            get;
        }

        #endregion

    }

    public sealed class ImportLibraryOrdinalFunction : ImportLibraryFunction
    {

        private int ordinal;

        internal ImportLibraryOrdinalFunction(ImportLibrary importLibrary, ImportAddressTableEntry tableEntry, int ordinalNo) : base(importLibrary, tableEntry)
        {
            ordinal = ordinalNo;
        }

        #region Methods

        public override string ToString()
        {
            return String.Format("{0:D4} (0x{1})", ordinal, ordinal.ToString("X4"));
        }

        #endregion

        #region Properties

        public override ImportLibraryBindingType BindingType
        {
            get
            {
                return ImportLibraryBindingType.Ordinal;
            }
        }

        public int Ordinal
        {
            get
            {
                return ordinal;
            }
        }

        #endregion

    }

    public sealed class ImportLibraryNamedFunction : ImportLibraryFunction
    {

        private ImportHintNameEntry hint_entry;

        internal ImportLibraryNamedFunction(ImportLibrary importLibrary, ImportAddressTableEntry tableEntry, ImportHintNameEntry hintEntry) : base(importLibrary, tableEntry)
        {
            hint_entry = hintEntry;
        }

        #region Methods

        public override string ToString()
        {
            return hint_entry.ToString();
        }

        #endregion

        #region Properties

        public override ImportLibraryBindingType BindingType
        {
            get
            {
                return ImportLibraryBindingType.Name;
            }
        }

        public ImportHintNameEntry HintEntry
        {
            get
            {
                return hint_entry;
            }
        }

        public string Name
        {
            get
            {
                return hint_entry.Name;
            }
        }

        #endregion

    }

}
