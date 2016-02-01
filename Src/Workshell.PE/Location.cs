using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class Location
    {

        internal Location(ulong fileOffset, uint rva, ulong va, uint fileSize, uint virtualSize)
        {
            FileOffset = fileOffset;
            RVA = rva;
            VA = va;
            FileSize = fileSize;
            VirtualSize = virtualSize;
        }

        #region Properties

        public ulong FileOffset
        {
            get;
            private set;
        }

        public uint RVA
        {
            get;
            private set;
        }

        public ulong VA
        {
            get;
            private set;
        }

        public uint FileSize
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
