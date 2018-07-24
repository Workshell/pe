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
                return null;

            return _image.Sections[entry];
        }

        public SectionTableEntry RVAToSectionTableEntry(uint rva)
        {
            var entries = _image.SectionTable.OrderBy(e => e.VirtualAddress).ToArray();
            SectionTableEntry entry = null;

            for (var i = 0; i < entries.Length; i++)
            {
                var maxRVA = entries[i].VirtualAddress + entries[i].SizeOfRawData;

                if (i != (entries.Length - 1))
                    maxRVA = entries[i + 1].VirtualAddress;

                if (rva >= entries[i].VirtualAddress && rva < maxRVA)
                    entry = entries[i];
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
