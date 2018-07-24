using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Annotations;
using Workshell.PE.Extensions;
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
        [EnumAnnotation("IMAGE_SCN_ALIGN_2BYTES")]
        Align2Bytes = 0x00200000,
        [EnumAnnotation("IMAGE_SCN_ALIGN_4BYTES")]
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

    public sealed class SectionTableEntry : IEquatable<SectionTableEntry>, ISupportsLocation, ISupportsBytes
    {
        private static readonly uint _headerSize = Marshal.SizeOf<IMAGE_SECTION_HEADER>().ToUInt32();

        private readonly PortableExecutableImage _image;
        private readonly IMAGE_SECTION_HEADER _header;

        internal SectionTableEntry(PortableExecutableImage image, SectionTable sectionTable, IMAGE_SECTION_HEADER entryHeader, ulong entryOffset, ulong imageBase)
        {
            _image = image;
            _header = entryHeader;

            Table = sectionTable;
            Location = new Location(image.GetCalculator(), entryOffset, entryOffset.ToUInt32(), imageBase + entryOffset, _headerSize, _headerSize);
            Name = GetName();
        }

        #region Methods

        public override string ToString()
        {
            return !string.IsNullOrWhiteSpace(Name) ? Name : base.ToString();
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
            var prime = 397;
            var result = 0;

            result = (result * prime) ^ Location.GetHashCode();
            result = (result * prime) ^ Name.GetHashCode();
            result = (result * prime) ^ _header.VirtualSize.GetHashCode();
            result = (result * prime) ^ _header.SizeOfRawData.GetHashCode();
            result = (result * prime) ^ _header.PointerToRawData.GetHashCode();
            result = (result * prime) ^ _header.PointerToRelocations.GetHashCode();
            result = (result * prime) ^ _header.PointerToLineNumbers.GetHashCode();
            result = (result * prime) ^ _header.NumberOfRelocations.GetHashCode();
            result = (result * prime) ^ _header.NumberOfLineNumbers.GetHashCode();
            result = (result * prime) ^ _header.Characteristics.GetHashCode();

            return result;
        }

        public byte[] GetBytes()
        {
            return GetBytesAsync().GetAwaiter().GetResult();
        }

        public async Task<byte[]> GetBytesAsync()
        {
            var stream = _image.GetStream();
            var buffer = await stream.ReadBytesAsync(Location).ConfigureAwait(false);

            return buffer;
        }

        public SectionCharacteristicsType GetCharacteristics()
        {
            return (SectionCharacteristicsType)_header.Characteristics;
        }

        private string GetName()
        {
            var builder = new StringBuilder(16);

            foreach (var c in _header.Name)
            {
                if (c == '\0')
                    break;

                builder.Append(c);
            }

            return builder.ToString();
        }

        #endregion

        #region Properties

        public SectionTable Table { get; }

        public Location Location { get; }

        public string Name { get; }

        public uint VirtualSizeOrPhysicalAddress => _header.VirtualSize;
        public uint VirtualAddress => _header.VirtualAddress;
        public uint SizeOfRawData => _header.SizeOfRawData;
        public uint PointerToRawData => _header.PointerToRawData;
        public uint PointerToRelocations => _header.PointerToRelocations;
        public uint PointerToLineNumbers => _header.PointerToLineNumbers;
        public ushort NumberOfRelocations => _header.NumberOfRelocations;
        public ushort NumberOfLineNumbers => _header.NumberOfLineNumbers;
        public uint Characteristics => _header.Characteristics;

        #endregion
    }

    public sealed class SectionTable : IEnumerable<SectionTableEntry>, ISupportsLocation, ISupportsBytes
    {
        private readonly PortableExecutableImage _image;
        private readonly SectionTableEntry[] _table;

        internal SectionTable(PortableExecutableImage image, IMAGE_SECTION_HEADER[] sectionHeaders, ulong tableOffset, ulong imageBase)
        {
            _image = image;
            _table = new SectionTableEntry[sectionHeaders.Length];

            var size = (Marshal.SizeOf<IMAGE_SECTION_HEADER>() * sectionHeaders.Length).ToUInt32();

            Location = new Location(image.GetCalculator(), tableOffset, tableOffset.ToUInt32(), imageBase + tableOffset, size, size);

            var offset = tableOffset;

            for(var i = 0; i < sectionHeaders.Length; i++)
            {
                var entry = new SectionTableEntry(_image,this,sectionHeaders[i],offset,imageBase);

                _table[i] = entry;
                offset += Marshal.SizeOf<IMAGE_SECTION_HEADER>().ToUInt32();
            }
        }

        #region Methods

        public byte[] GetBytes()
        {
            return GetBytesAsync().GetAwaiter().GetResult();
        }

        public async Task<byte[]> GetBytesAsync()
        {
            var stream = _image.GetStream();
            var buffer = await stream.ReadBytesAsync(Location).ConfigureAwait(false);

            return buffer;
        }

        public IEnumerator<SectionTableEntry> GetEnumerator()
        {
            foreach (var entry in _table)
            {
                yield return entry;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public Location Location { get; }
        public int Count => _table.Length;
        public SectionTableEntry this[int index] => _table[index];

        public SectionTableEntry this[string name]
        {
            get
            {
                return _table.FirstOrDefault(entry => string.Compare(name,entry.Name,StringComparison.OrdinalIgnoreCase) == 0);
            }
        }

        #endregion
    }
}
