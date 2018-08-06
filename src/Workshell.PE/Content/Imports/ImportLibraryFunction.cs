using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Content
{
    public enum ImportLibraryBindingType
    {
        Name,
        Ordinal
    }

    public abstract class ImportLibraryFunction
    {
        internal ImportLibraryFunction(ImportAddressTableEntryBase tableEntry)
        {
            TableEntry = tableEntry;
        }

        #region Properties

        public ImportAddressTableEntryBase TableEntry { get; }
        public abstract ImportLibraryBindingType BindingType { get; }

        #endregion
    }

    public sealed class ImportLibraryOrdinalFunction : ImportLibraryFunction
    {
        internal ImportLibraryOrdinalFunction(ImportAddressTableEntryBase tableEntry, int ordinal) : base(tableEntry)
        {
            Ordinal = ordinal;
        }

        #region Methods

        public override string ToString()
        {
            return $"{Ordinal:D4} (0x{Ordinal:X4})";
        }

        #endregion

        #region Properties

        public override ImportLibraryBindingType BindingType => ImportLibraryBindingType.Ordinal;
        public int Ordinal { get; }

        #endregion
    }

    public sealed class ImportLibraryNamedFunction : ImportLibraryFunction
    {
        internal ImportLibraryNamedFunction(ImportAddressTableEntryBase tableEntry, ImportHintNameEntryBase hintEntry) : base(tableEntry)
        {
            HintEntry = hintEntry;
        }

        #region Methods

        public override string ToString()
        {
            return HintEntry.ToString();
        }

        #endregion

        #region Properties

        public override ImportLibraryBindingType BindingType => ImportLibraryBindingType.Name;
        public ImportHintNameEntryBase HintEntry { get; }
        public string Name => HintEntry.Name;

        #endregion
    }
}
