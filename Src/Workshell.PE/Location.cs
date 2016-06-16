using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public sealed class Location : IEquatable<Location>
    {

        private ulong _file_offset;
        private uint _rva;
        private ulong _va;
        private ulong _file_size;
        private ulong _virtual_size;
        private Section _section;

        public Location(ulong fileOffset, uint rva, ulong va, ulong fileSize, ulong virtualSize) : this(fileOffset,rva,va,fileSize,virtualSize,null)
        {
        }

        public Location(ulong fileOffset, uint rva, ulong va, ulong fileSize, ulong virtualSize, Section section)
        {
            _file_offset = fileOffset;
            _file_size = fileSize;
            _rva = rva;
            _va = va;
            _virtual_size = virtualSize;
            _section = section;
        }

        #region Methods

        public override string ToString()
        {
            return String.Format("File Offset: 0x{0:X16}, File Size: 0x{1:X8}, RVA: 0x{2:X8}, Virtual Address: 0x{3:X16}, Virtual Size: 0x{4:X8}",FileOffset,FileSize,RelativeVirtualAddress,VirtualAddress,VirtualSize);
        }

        public override bool Equals(object other)
        {
            return Equals(other as Location);
        }

        public bool Equals(Location other)
        {
            if (other == null)
                return false;

            if (_file_offset != other.FileOffset)
                return false;

            if (_file_size != other.FileSize)
                return false;

            if (_rva != other.RelativeVirtualAddress)
                return false;

            if (_va != other.VirtualAddress)
                return false;

            if (_virtual_size != other.VirtualSize)
                return false;

            if (_section != other.Section)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 13;

            hash = (hash * 7) + _file_offset.GetHashCode();
            hash = (hash * 7) + _file_size.GetHashCode();
            hash = (hash * 7) + _rva.GetHashCode();
            hash = (hash * 7) + _va.GetHashCode();
            hash = (hash * 7) + _virtual_size.GetHashCode();

            if (_section != null)
                hash = (hash * 7) + _section.GetHashCode();

            return hash;
        }

        #endregion

        #region Properties

        public ulong FileOffset
        {
            get
            {
                return _file_offset;
            }
        }

        public ulong FileSize
        {
            get
            {
                return _file_size;
            }
        }

        public uint RelativeVirtualAddress
        {
            get
            {
                return _rva;
            }
        }

        public ulong VirtualAddress
        {
            get
            {
                return _va;
            }
        }

        public ulong VirtualSize
        {
            get
            {
                return _virtual_size;
            }
        }

        public Section Section
        {
            get
            {
                return _section;
            }
        }

        #endregion

    }

}
