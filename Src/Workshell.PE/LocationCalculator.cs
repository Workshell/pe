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

    public class LocationCalculator
    {

        private ExecutableImage reader;

        internal LocationCalculator(ExecutableImage imageReader)
        {
            reader = imageReader;
        }

        #region Methods

        /* VA */

        public Section VAToSection(ulong va)
        {
            ulong image_base = reader.NTHeaders.OptionalHeader.ImageBase;
            uint rva = Convert.ToUInt32(va - image_base);

            return RVAToSection(rva);
        }

        public SectionTableEntry VAToSectionTableEntry(ulong va)
        {
            ulong image_base = reader.NTHeaders.OptionalHeader.ImageBase;
            uint rva = Convert.ToUInt32(va - image_base);

            return RVAToSectionTableEntry(rva);
        }

        public ulong VAToOffset(ulong va)
        {
            ulong image_base = reader.NTHeaders.OptionalHeader.ImageBase;
            uint rva = Convert.ToUInt32(va - image_base);

            return RVAToOffset(rva);
        }

        public ulong VAToOffset(Section section, ulong va)
        {
            return VAToOffset(section.TableEntry,va);
        }

        public ulong VAToOffset(SectionTableEntry section, ulong va)
        {
            ulong image_base = reader.NTHeaders.OptionalHeader.ImageBase;
            uint rva = Convert.ToUInt32(va - image_base);

            return RVAToOffset(section,rva);
        }

        public ulong OffsetToVA(ulong offset)
        {
            SectionTableEntry[] entries = reader.SectionTable.OrderBy(e => e.PointerToRawData).ToArray();
            SectionTableEntry entry = null;

            for(var i = 0; i < entries.Length; i++)
            {
                if (offset >= entries[i].PointerToRawData && offset < (entries[i].PointerToRawData + entries[i].SizeOfRawData))
                    entry = entries[i];
            }

            if (entry != null)
            {
                return OffsetToVA(entry, offset);
            }
            else
            {
                return 0;
            }
        }

        public ulong OffsetToVA(Section section, ulong offset)
        {
            return OffsetToVA(section.TableEntry,offset);
        }

        public ulong OffsetToVA(SectionTableEntry section, ulong offset)
        {
            ulong image_base = reader.NTHeaders.OptionalHeader.ImageBase;
            uint rva = Convert.ToUInt32((offset + section.VirtualAddress) - section.PointerToRawData);

            return image_base + rva;
        }

        /* RVA */

        public Section RVAToSection(uint rva)
        {
            SectionTableEntry entry = RVAToSectionTableEntry(rva);

            if (entry == null)
                return null;

            return reader.Sections[entry];
        }

        public SectionTableEntry RVAToSectionTableEntry(uint rva)
        {
            SectionTableEntry[] entries = reader.SectionTable.OrderBy(e => e.VirtualAddress).ToArray();
            SectionTableEntry entry = null;

            for (var i = 0; i < entries.Length; i++)
            {
                uint max_rva = entries[i].VirtualAddress + entries[i].SizeOfRawData;

                if (i != (entries.Length - 1))
                    max_rva = entries[i + 1].VirtualAddress;

                if (rva >= entries[i].VirtualAddress && rva < max_rva)
                    entry = entries[i];
            }

            return entry;
        }

        public ulong RVAToOffset(uint rva)
        {
            SectionTableEntry[] entries = reader.SectionTable.OrderBy(e => e.VirtualAddress).ToArray();
            SectionTableEntry entry = null;

            for (var i = 0; i < entries.Length; i++)
            {
                if (rva >= entries[i].VirtualAddress && rva < (entries[i].VirtualAddress + entries[i].SizeOfRawData))
                    entry = entries[i];
            }

            if (entry != null)
            {
                return RVAToOffset(entry, rva);
            }
            else
            {
                return 0;
            }
        }

        public ulong RVAToOffset(Section section, uint rva)
        {
            return RVAToOffset(section.TableEntry,rva);
        }

        public ulong RVAToOffset(SectionTableEntry section, uint rva)
        {
            ulong offset = (rva - section.VirtualAddress) + section.PointerToRawData;

            return offset;
        }

        public uint OffsetToRVA(ulong offset)
        {
            SectionTableEntry[] entries = reader.SectionTable.OrderBy(e => e.PointerToRawData).ToArray();
            SectionTableEntry entry = null;

            for (var i = 0; i < entries.Length; i++)
            {
                if (offset >= entries[i].PointerToRawData && offset < (entries[i].PointerToRawData + entries[i].SizeOfRawData))
                    entry = entries[i];
            }

            if (entry != null)
            {
                return OffsetToRVA(entry, offset);
            }
            else
            {
                return 0;
            }
        }

        public uint OffsetToRVA(Section section, ulong offset)
        {
            return OffsetToRVA(section.TableEntry,offset);
        }

        public uint OffsetToRVA(SectionTableEntry section, ulong offset)
        {
            uint rva = Convert.ToUInt32((offset + section.VirtualAddress) - section.PointerToRawData);

            return rva;
        }

        #endregion

    }

}
