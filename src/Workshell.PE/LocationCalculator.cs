﻿#region License
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
using System.Linq;
using System.Text;

namespace Workshell.PE
{
    public sealed class LocationCalculator
    {
        private PortableExecutableImage _image;

        internal LocationCalculator(PortableExecutableImage image)
        {
            _image = image;
        }

        #region Methods

        public Location OffsetToLocation(ulong offset, ulong size)
        {
            var rva = OffsetToRVA(offset);
            var va = OffsetToVA(offset);

            return new Location(_image, offset, rva, va, size, size);
        }

        /* VA */

        public Section VAToSection(ulong va)
        {
            var imageBase = _image.NTHeaders.OptionalHeader.ImageBase;
            var rva = Convert.ToUInt32(va - imageBase);

            return RVAToSection(rva);
        }

        public SectionTableEntry VAToSectionTableEntry(ulong va)
        {
            var imageBase = _image.NTHeaders.OptionalHeader.ImageBase;
            var rva = Convert.ToUInt32(va - imageBase);

            return RVAToSectionTableEntry(rva);
        }

        public ulong VAToOffset(ulong va)
        {
            var imageBase = _image.NTHeaders.OptionalHeader.ImageBase;
            var rva = Convert.ToUInt32(va - imageBase);

            return RVAToOffset(rva);
        }

        public ulong VAToOffset(Section section, ulong va)
        {
            return VAToOffset(section.TableEntry,va);
        }

        public ulong VAToOffset(SectionTableEntry section, ulong va)
        {
            var imageBase = _image.NTHeaders.OptionalHeader.ImageBase;
            var rva = Convert.ToUInt32(va - imageBase);

            return RVAToOffset(section,rva);
        }

        public ulong OffsetToVA(ulong offset)
        {
            var entries = _image.SectionTable.OrderBy(e => e.PointerToRawData).ToArray();
            SectionTableEntry entry = null;

            for(var i = 0; i < entries.Length; i++)
            {
                if (offset >= entries[i].PointerToRawData && offset < (entries[i].PointerToRawData + entries[i].SizeOfRawData))
                    entry = entries[i];
            }

            if (entry != null)
                return OffsetToVA(entry, offset);
            
            return 0;
        }

        public ulong OffsetToVA(Section section, ulong offset)
        {
            return OffsetToVA(section.TableEntry,offset);
        }

        public ulong OffsetToVA(SectionTableEntry section, ulong offset)
        {
            var imageBase = _image.NTHeaders.OptionalHeader.ImageBase;
            var rva = Convert.ToUInt32((offset + section.VirtualAddress) - section.PointerToRawData);

            return imageBase + rva;
        }

        /* RVA */

        public Section RVAToSection(uint rva)
        {
            var entry = RVAToSectionTableEntry(rva);

            if (entry == null)
            {
                return null;
            }

            return _image.Sections[entry];
        }

        public SectionTableEntry RVAToSectionTableEntry(uint rva)
        {
            if (_image.SectionTable == null)
            {
                return null;
            }

            var entries = _image.SectionTable.OrderBy(e => e.VirtualAddress).ToArray();
            SectionTableEntry entry = null;

            for (var i = 0; i < entries.Length; i++)
            {
                var maxRVA = entries[i].VirtualAddress + entries[i].SizeOfRawData;

                if (i != (entries.Length - 1))
                {
                    maxRVA = entries[i + 1].VirtualAddress;
                }

                if (rva >= entries[i].VirtualAddress && rva < maxRVA)
                {
                    entry = entries[i];
                }
            }

            return entry;
        }

        public ulong RVAToOffset(uint rva)
        {
            var entries = _image.SectionTable.OrderBy(e => e.VirtualAddress).ToArray();
            SectionTableEntry entry = null;

            for (var i = 0; i < entries.Length; i++)
            {
                if (rva >= entries[i].VirtualAddress && rva < (entries[i].VirtualAddress + entries[i].SizeOfRawData))
                    entry = entries[i];
            }

            if (entry != null)
                return RVAToOffset(entry, rva);

            return 0;
        }

        public ulong RVAToOffset(Section section, uint rva)
        {
            return RVAToOffset(section.TableEntry,rva);
        }

        public ulong RVAToOffset(SectionTableEntry section, uint rva)
        {
            var offset = (rva - section.VirtualAddress) + section.PointerToRawData;

            return offset;
        }

        public uint OffsetToRVA(ulong offset)
        {
            var entries = _image.SectionTable.OrderBy(e => e.PointerToRawData).ToArray();
            SectionTableEntry entry = null;

            for (var i = 0; i < entries.Length; i++)
            {
                if (offset >= entries[i].PointerToRawData && offset < (entries[i].PointerToRawData + entries[i].SizeOfRawData))
                    entry = entries[i];
            }

            if (entry != null)
                return OffsetToRVA(entry, offset);

            return 0;
        }

        public uint OffsetToRVA(Section section, ulong offset)
        {
            return OffsetToRVA(section.TableEntry,offset);
        }

        public uint OffsetToRVA(SectionTableEntry section, ulong offset)
        {
            var rva = Convert.ToUInt32((offset + section.VirtualAddress) - section.PointerToRawData);

            return rva;
        }

        #endregion
    }
}
