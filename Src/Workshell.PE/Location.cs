using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public sealed class Location : IEquatable<Location>
    {

        internal Location(ulong fileOffset, uint rva, ulong va, uint fileSize, uint virtualSize)
        {
            FileOffset = fileOffset;
            FileSize = fileSize;
            RelativeVirtualAddress = rva;
            VirtualAddress = va;
            VirtualSize = virtualSize;
        }

        #region Methods

        public override bool Equals(object other)
        {
            return Equals(other as Location);
        }

        public bool Equals(Location other)
        {
            if (other == null)
                return false;

            if (FileOffset != other.FileOffset)
                return false;

            if (FileSize != other.FileSize)
                return false;

            if (RelativeVirtualAddress != other.RelativeVirtualAddress)
                return false;

            if (VirtualAddress != other.VirtualAddress)
                return false;

            if (VirtualSize != other.VirtualSize)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 13;

            hash = (hash * 7) + FileOffset.GetHashCode();
            hash = (hash * 7) + FileSize.GetHashCode();
            hash = (hash * 7) + RelativeVirtualAddress.GetHashCode();
            hash = (hash * 7) + VirtualAddress.GetHashCode();
            hash = (hash * 7) + VirtualSize.GetHashCode();

            return hash;
        }

        #endregion

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
