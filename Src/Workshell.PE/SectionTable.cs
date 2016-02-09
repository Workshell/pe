using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Native;

namespace Workshell.PE
{

    [Flags]
    public enum SectionCharacteristicsType : uint 
    {
        [EnumAnnotation("IMAGE_SCN_TYPE_REG")]
        TypeReg = 0x00000000,
        [EnumAnnotation("IMAGE_SCN_TYPE_DSECT")]
        TypeDsect = 0x00000001,
        [EnumAnnotation("IMAGE_SCN_TYPE_NOLOAD")]
        TypeNoLoad = 0x00000002,
        [EnumAnnotation("IMAGE_SCN_TYPE_GROUP")]
        TypeGroup = 0x00000004,
        [EnumAnnotation("IMAGE_SCN_TYPE_NO_PAD")]
        TypeNoPadded = 0x00000008,
        [EnumAnnotation("IMAGE_SCN_TYPE_COPY")]
        TypeCopy = 0x00000010,
        [EnumAnnotation("IMAGE_SCN_CNT_CODE")]
        ContentCode = 0x00000020,
        [EnumAnnotation("IMAGE_SCN_CNT_INITIALIZED_DATA")]
        ContentInitializedData = 0x00000040,
        [EnumAnnotation("IMAGE_SCN_CNT_UNINITIALIZED_DATA")]
        ContentUninitializedData = 0x00000080,
        [EnumAnnotation("IMAGE_SCN_LNK_OTHER")]
        LinkOther = 0x00000100,
        [EnumAnnotation("IMAGE_SCN_LNK_INFO")]
        LinkInfo = 0x00000200,
        [EnumAnnotation("IMAGE_SCN_TYPE_OVER")]
        TypeOver = 0x00000400,
        [EnumAnnotation("IMAGE_SCN_LNK_REMOVE")]
        LinkRemove = 0x00000800,
        [EnumAnnotation("IMAGE_SCN_LNK_COMDAT")]
        LinkComDat = 0x00001000,
        [EnumAnnotation("IMAGE_SCN_NO_DEFER_SPEC_EXC")]
        NoDeferSpecExceptions = 0x00004000,
        [EnumAnnotation("IMAGE_SCN_GPREL")]
        RelativeGP = 0x00008000,
        [EnumAnnotation("IMAGE_SCN_MEM_PURGEABLE")]
        MemPurgeable = 0x00020000,
        //[EnumAnnotation("IMAGE_SCN_MEM_16BIT")]
        //Memory16Bit = 0x00020000,
        [EnumAnnotation("IMAGE_SCN_MEM_LOCKED")]
        MemoryLocked = 0x00040000,
        [EnumAnnotation("IMAGE_SCN_MEM_PRELOAD")]
        MemoryPreload = 0x00080000,
        [EnumAnnotation("IMAGE_SCN_ALIGN_1BYTES")]
        Align1Bytes = 0x00100000,
        [EnumAnnotation("IMAGE_SCN_ALIGN_1BYTES")]
        Align2Bytes = 0x00200000,
        [EnumAnnotation("IMAGE_SCN_ALIGN_1BYTES")]
        Align4Bytes = 0x00300000,
        [EnumAnnotation("IMAGE_SCN_ALIGN_8BYTES")]
        Align8Bytes = 0x00400000,
        [EnumAnnotation("IMAGE_SCN_ALIGN_16BYTES")]
        Align16Bytes = 0x00500000,
        [EnumAnnotation("IMAGE_SCN_ALIGN_32BYTES")]
        Align32Bytes = 0x00600000,
        [EnumAnnotation("IMAGE_SCN_ALIGN_64BYTES")]
        Align64Bytes = 0x00700000,
        [EnumAnnotation("IMAGE_SCN_ALIGN_128BYTES")]
        Align128Bytes = 0x00800000,
        [EnumAnnotation("IMAGE_SCN_ALIGN_256BYTES")]
        Align256Bytes = 0x00900000,
        [EnumAnnotation("IMAGE_SCN_ALIGN_512BYTES")]
        Align512Bytes = 0x00A00000,
        [EnumAnnotation("IMAGE_SCN_ALIGN_1024BYTES")]
        Align1024Bytes = 0x00B00000,
        [EnumAnnotation("IMAGE_SCN_ALIGN_2048BYTES")]
        Align2048Bytes = 0x00C00000,
        [EnumAnnotation("IMAGE_SCN_ALIGN_4096BYTES")]
        Align4096Bytes = 0x00D00000,
        [EnumAnnotation("IMAGE_SCN_ALIGN_8192BYTES")]
        Align8192Bytes = 0x00E00000,
        [EnumAnnotation("IMAGE_SCN_LNK_NRELOC_OVFL")]
        LinkExtendedRelocationOverflow = 0x01000000,
        [EnumAnnotation("IMAGE_SCN_MEM_DISCARDABLE")]
        MemoryDiscardable = 0x02000000,
        [EnumAnnotation("IMAGE_SCN_MEM_NOT_CACHED")]
        MemoryNotCached = 0x04000000,
        [EnumAnnotation("IMAGE_SCN_MEM_NOT_PAGED")]
        MemoryNotPaged = 0x08000000,
        [EnumAnnotation("IMAGE_SCN_MEM_SHARED")]
        MemoryShared = 0x10000000,
        [EnumAnnotation("IMAGE_SCN_MEM_EXECUTE")]
        MemoryExecute = 0x20000000,
        [EnumAnnotation("IMAGE_SCN_MEM_READ")]
        MemoryRead = 0x40000000,
        [EnumAnnotation("IMAGE_SCN_MEM_WRITE")]
        MemoryWrite = 0x80000000
    }

    public sealed class SectionTableEntry : IEquatable<SectionTableEntry>, ISupportsLocation
    {

        private static readonly uint size = Convert.ToUInt32(Utils.SizeOf<IMAGE_SECTION_HEADER>());

        private SectionTable table;
        private IMAGE_SECTION_HEADER header;
        private Location location;
        private string name;

        internal SectionTableEntry(SectionTable sectionTable, IMAGE_SECTION_HEADER entryHeader, ulong entryOffset, ulong imageBase)
        {
            table = sectionTable;
            header = entryHeader;
            location = new Location(entryOffset,Convert.ToUInt32(entryOffset),imageBase + entryOffset,size,size);
            name = GetName();
        }

        #region Methods

        public override string ToString()
        {
            if (!String.IsNullOrWhiteSpace(name))
            {
                return name;
            }
            else
            {
                return base.ToString();
            }
        }

        public override bool Equals(object other)
        {
            return Equals(other as SectionTableEntry);
        }

        public bool Equals(SectionTableEntry other)
        {
            if (other == null)
                return false;

            if (other == this)
                return true;

            if (Location != other.Location)
                return false;

            if (Name != other.Name)
                return false;

            if (VirtualSizeOrPhysicalAddress != other.VirtualSizeOrPhysicalAddress)
                return false;

            if (VirtualAddress != other.VirtualAddress)
                return false;

            if (SizeOfRawData != other.SizeOfRawData)
                return false;

            if (PointerToRawData != other.PointerToRawData)
                return false;

            if (PointerToRelocations != other.PointerToRelocations)
                return false;

            if (PointerToLineNumbers != other.PointerToLineNumbers)
                return false;

            if (NumberOfRelocations != other.NumberOfRelocations)
                return false;

            if (NumberOfLineNumbers != other.NumberOfLineNumbers)
                return false;

            if (Characteristics != other.Characteristics)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int prime = 397;
            int result = 0;

            result = (result * prime) ^ location.GetHashCode();
            result = (result * prime) ^ name.GetHashCode();
            result = (result * prime) ^ header.VirtualSize.GetHashCode();
            result = (result * prime) ^ header.SizeOfRawData.GetHashCode();
            result = (result * prime) ^ header.PointerToRawData.GetHashCode();
            result = (result * prime) ^ header.PointerToRelocations.GetHashCode();
            result = (result * prime) ^ header.PointerToLineNumbers.GetHashCode();
            result = (result * prime) ^ header.NumberOfRelocations.GetHashCode();
            result = (result * prime) ^ header.NumberOfLineNumbers.GetHashCode();
            result = (result * prime) ^ header.Characteristics.GetHashCode();

            return result;
        }

        public byte[] GetBytes()
        {
            return null;
        }

        public SectionCharacteristicsType GetCharacteristics()
        {
            return (SectionCharacteristicsType)header.Characteristics;
        }

        private string GetName()
        {
            StringBuilder builder = new StringBuilder();

            for(var i = 0; i < header.Name.Length; i++)
            {
                if (header.Name[i] == '\0')
                    break;

                builder.Append(header.Name[i]);
            }

            return builder.ToString();
        }

        #endregion

        #region Properties

        public SectionTable Table
        {
            get
            {
                return table;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public uint VirtualSizeOrPhysicalAddress
        {
            get
            {
                return header.VirtualSize;
            }
        }

        public uint VirtualAddress
        {
            get
            {
                return header.VirtualAddress;
            }
        }

        public uint SizeOfRawData
        {
            get
            {
                return header.SizeOfRawData;
            }
        }

        public uint PointerToRawData
        {
            get
            {
                return header.PointerToRawData;
            }
        }

        public uint PointerToRelocations
        {
            get
            {
                return header.PointerToRelocations;
            }
        }

        public uint PointerToLineNumbers
        {
            get
            {
                return header.PointerToLineNumbers;
            }
        }

        public ushort NumberOfRelocations
        {
            get
            {
                return header.NumberOfRelocations;
            }
        }

        public ushort NumberOfLineNumbers
        {
            get
            {
                return header.NumberOfLineNumbers;
            }
        }

        public uint Characteristics
        {
            get
            {
                return header.Characteristics;
            }
        }

        #endregion

    }

    public sealed class SectionTable : IEnumerable<SectionTableEntry>, IReadOnlyCollection<SectionTableEntry>, ISupportsLocation
    {

        private ImageReader reader;
        private Location location;
        private List<SectionTableEntry> table;

        internal SectionTable(ImageReader exeReader, IMAGE_SECTION_HEADER[] sectionHeaders, ulong tableOffset, ulong imageBase)
        {
            uint size = Convert.ToUInt32(Utils.SizeOf<IMAGE_SECTION_HEADER>() * sectionHeaders.Length);

            reader = exeReader;
            location = new Location(tableOffset,Convert.ToUInt32(tableOffset),imageBase + tableOffset,size,size);
            table = new List<SectionTableEntry>();

            ulong offset = tableOffset;

            foreach(IMAGE_SECTION_HEADER header in sectionHeaders)
            {
                table.Add(new SectionTableEntry(this,header,offset,imageBase));

                offset += Convert.ToUInt32(Utils.SizeOf<IMAGE_SECTION_HEADER>());
            }
        }

        #region Methods

        public byte[] GetBytes()
        {
            return null;
        }

        public bool Has(string name)
        {
            return table.Any(section => String.Compare(name,section.Name,true) == 0);
        }

        public IEnumerator<SectionTableEntry> GetEnumerator()
        {
            return table.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public ImageReader Reader
        {
            get
            {
                return reader;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        public int Count
        {
            get
            {
                return table.Count;
            }
        }

        public SectionTableEntry this[int index]
        {
            get
            {
                return table[index];
            }
        }

        public SectionTableEntry this[string name]
        {
            get
            {
                return table.FirstOrDefault(entry => String.Compare(name,entry.Name,StringComparison.OrdinalIgnoreCase) == 0);
            }
        }

        #endregion

    }

}
