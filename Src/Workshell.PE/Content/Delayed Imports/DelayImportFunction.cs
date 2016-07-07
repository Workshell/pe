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
