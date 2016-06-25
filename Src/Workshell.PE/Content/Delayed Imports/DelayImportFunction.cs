using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public enum DelayImportLibraryBindingType
    {
        Name,
        Ordinal
    }

    public abstract class DelayImportLibraryFunction
    {

        private DelayImportLibrary library;
        private DelayImportAddressTableEntry table_entry;

        internal DelayImportLibraryFunction(DelayImportLibrary importLibrary, DelayImportAddressTableEntry tableEntry)
        {
            library = importLibrary;
            table_entry = tableEntry;
        }

        #region Properties

        public DelayImportLibrary Library
        {
            get
            {
                return library;
            }
        }

        public DelayImportAddressTableEntry TableEntry
        {
            get
            {
                return table_entry;
            }
        }

        public abstract DelayImportLibraryBindingType BindingType
        {
            get;
        }

        #endregion

    }

    public sealed class DelayImportLibraryOrdinalFunction : DelayImportLibraryFunction
    {

        private int ordinal;

        internal DelayImportLibraryOrdinalFunction(DelayImportLibrary importLibrary, DelayImportAddressTableEntry tableEntry, int ordinalNo) : base(importLibrary, tableEntry)
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

        public override DelayImportLibraryBindingType BindingType
        {
            get
            {
                return DelayImportLibraryBindingType.Ordinal;
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

    public sealed class DelayImportLibraryNamedFunction : DelayImportLibraryFunction
    {

        private DelayImportHintNameEntry hint_entry;

        internal DelayImportLibraryNamedFunction(DelayImportLibrary importLibrary, DelayImportAddressTableEntry tableEntry, DelayImportHintNameEntry hintEntry) : base(importLibrary, tableEntry)
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

        public override DelayImportLibraryBindingType BindingType
        {
            get
            {
                return DelayImportLibraryBindingType.Name;
            }
        }

        public DelayImportHintNameEntry HintEntry
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
