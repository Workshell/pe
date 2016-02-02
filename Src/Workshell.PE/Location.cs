using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public sealed class Location
    {

        internal Location(ulong fileOffset, uint rva, ulong va, uint fileSize, uint virtualSize)
        {
            FileOffset = fileOffset;
            FileSize = fileSize;
            RelativeVirtualAddress = rva;
            VirtualAddress = va;
            VirtualSize = virtualSize;
        }

        #region Properties

        public ulong FileOffset
        {
            get;
            private set;
        }

        public uint FileSize
        {
            get;
            private set;
        }

        public uint RelativeVirtualAddress
        {
            get;
            private set;
        }

        public ulong VirtualAddress
        {
            get;
            private set;
        }

        public uint VirtualSize
        {
            get;
            private set;
        }

        #endregion

    }

}
