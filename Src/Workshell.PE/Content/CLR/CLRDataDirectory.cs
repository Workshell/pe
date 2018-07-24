using System;
using System.Collections.Generic;
using System.Text;
using Workshell.PE.Native;

namespace Workshell.PE.Content
{
    public sealed class CLRDataDirectory
    {
        internal CLRDataDirectory(uint virtualAddress, uint size)
        {
            VirtualAddress = virtualAddress;
            Size = size;
        }

        #region Static Methods

        public static bool IsNullOrEmpty(DataDirectory dataDirectory)
        {
            if (dataDirectory == null)
                return true;

            if (dataDirectory.VirtualAddress == 0)
                return true;

            if (dataDirectory.Size == 0)
                return true;

            return false;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"0x{VirtualAddress:X8}+{Size}";
        }

        #endregion

        #region Properties

        public uint VirtualAddress { get; }
        public uint Size { get; }

        #endregion
    }
}
