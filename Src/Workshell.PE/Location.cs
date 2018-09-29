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

namespace Workshell.PE
{
    public sealed class Location : IEquatable<Location>
    {
        private readonly LocationCalculator _calc;
        private Section _section;

        internal Location(LocationCalculator calc, ulong fileOffset, uint rva, ulong va, ulong fileSize, ulong virtualSize) : this(calc, fileOffset, rva, va, fileSize, virtualSize, null)
        {
        }

        internal Location(ulong fileOffset, uint rva, ulong va, ulong fileSize, ulong virtualSize, Section section) : this(null, fileOffset, rva, va, fileSize, virtualSize, section)
        {
        }

        private Location(LocationCalculator calc, ulong fileOffset, uint rva, ulong va, ulong fileSize, ulong virtualSize, Section section)
        {
            _calc = calc;
            _section = section;

            FileOffset = fileOffset;
            FileSize = fileSize;
            RelativeVirtualAddress = rva;
            VirtualAddress = va;
            VirtualSize = virtualSize;
        }


        #region Methods

        public override string ToString()
        {
            var result = $"File Offset: 0x{FileOffset:X16}, File Size: 0x{FileSize:X8} ({FileSize}), RVA: 0x{RelativeVirtualAddress:X8}, Virtual Address: 0x{VirtualAddress:X16}, Virtual Size: 0x{VirtualSize:X8} ({VirtualSize})";

            if (Section != null)
                result += $", Section: {Section.Name}";

            return result;
        }

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
            var hash = 13;

            hash = (hash * 7) + FileOffset.GetHashCode();
            hash = (hash * 7) + FileSize.GetHashCode();
            hash = (hash * 7) + RelativeVirtualAddress.GetHashCode();
            hash = (hash * 7) + VirtualAddress.GetHashCode();
            hash = (hash * 7) + VirtualSize.GetHashCode();

            return hash;
        }

        #endregion

        #region Properties

        public ulong FileOffset { get; }

        public ulong FileSize { get; }

        public uint RelativeVirtualAddress { get; }

        public ulong VirtualAddress { get; }

        public ulong VirtualSize { get; }

        public Section Section
        {
            get
            {
                if (_section == null)
                    _section = _calc.VAToSection(VirtualAddress);

                return _section;
            }
        }

        #endregion
    }
}
