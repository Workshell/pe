#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

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
