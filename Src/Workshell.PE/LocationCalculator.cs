using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class LocationCalculator
    {

        private ImageReader reader;

        internal LocationCalculator(ImageReader imageReader)
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
            foreach(SectionTableEntry entry in reader.SectionTable)
            {
                if (offset >= entry.PointerToRawData && offset < (entry.PointerToRawData + entry.SizeOfRawData))
                    return OffsetToVA(entry,offset);
            }

            return 0;
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
            foreach(SectionTableEntry entry in reader.SectionTable)
            {
                if (rva >= entry.VirtualAddress && rva <= (entry.VirtualAddress + entry.SizeOfRawData))
                    return entry;
            }

            return null;
        }

        public ulong RVAToOffset(uint rva)
        {
            foreach(SectionTableEntry entry in reader.SectionTable)
            {
                if (rva >= entry.VirtualAddress && rva < (entry.VirtualAddress + entry.SizeOfRawData))
                    return RVAToOffset(entry,rva);
            }

            return 0;
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
            foreach(SectionTableEntry entry in reader.SectionTable)
            {
                if (offset >= entry.PointerToRawData && offset < (entry.PointerToRawData + entry.SizeOfRawData))
                    return OffsetToRVA(entry,offset);
            }

            return 0;
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
