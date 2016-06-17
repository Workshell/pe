using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public enum MachineType : int
    {
        Native = 0,
        [EnumAnnotation("IMAGE_FILE_MACHINE_I386")]
        x86 = 0x014c,
        [EnumAnnotation("IMAGE_FILE_MACHINE_IA64")]
        Itanium = 0x0200,
        [EnumAnnotation("IMAGE_FILE_MACHINE_AMD64")]
        x64 = 0x8664
    }

    [Flags]
    public enum CharacteristicsType : int
    {
        [EnumAnnotation("IMAGE_FILE_RELOCS_STRIPPED")]
        RelocationInformationStrippedFromFile = 0x1,
        [EnumAnnotation("IMAGE_FILE_EXECUTABLE_IMAGE")]
        Executable = 0x2,
        [EnumAnnotation("IMAGE_FILE_LINE_NUMS_STRIPPED")]
        LineNumbersStripped = 0x4,
        [EnumAnnotation("IMAGE_FILE_LOCAL_SYMS_STRIPPED")]
        SymbolTableStripped = 0x8,
        [EnumAnnotation("IMAGE_FILE_AGGRESIVE_WS_TRIM")]
        AggresiveTrimWorkingSet = 0x10,
        [EnumAnnotation("IMAGE_FILE_LARGE_ADDRESS_AWARE")]
        LargeAddressAware = 0x20,
        [EnumAnnotation("IMAGE_FILE_BYTES_REVERSED_LO")]
        Supports16Bit = 0x40,
        [EnumAnnotation("IMAGE_FILE_16BIT_MACHINE")]
        ReservedBytesWo = 0x80,
        [EnumAnnotation("IMAGE_FILE_32BIT_MACHINE")]
        Supports32Bit = 0x100,
        [EnumAnnotation("IMAGE_FILE_DEBUG_STRIPPED")]
        DebugInfoStripped = 0x200,
        [EnumAnnotation("IMAGE_FILE_REMOVABLE_RUN_FROM_SWAP")]
        RunFromSwapIfInRemovableMedia = 0x400,
        [EnumAnnotation("IMAGE_FILE_NET_RUN_FROM_SWAP")]
        RunFromSwapIfInNetworkMedia = 0x800,
        [EnumAnnotation("IMAGE_FILE_SYSTEM")]
        IsSytemFile = 0x1000,
        [EnumAnnotation("IMAGE_FILE_DLL")]
        IsDLL = 0x2000,
        [EnumAnnotation("IMAGE_FILE_UP_SYSTEM_ONLY")]
        IsOnlyForSingleCoreProcessor = 0x4000,
        [EnumAnnotation("IMAGE_FILE_BYTES_REVERSED_HI")]
        BytesOfWordReserved = 0x8000
    }

    public sealed class FileHeader : ISupportsLocation, ISupportsBytes
    {

        internal static readonly int Size = Utils.SizeOf<IMAGE_FILE_HEADER>();

        private ExecutableImage reader;
        private IMAGE_FILE_HEADER header;
        private Location location;

        internal FileHeader(ExecutableImage exeReader, IMAGE_FILE_HEADER fileHeader, ulong headerOffset, ulong imageBase)
        {
            reader = exeReader;
            header = fileHeader;

            uint size = Convert.ToUInt32(Utils.SizeOf<IMAGE_FILE_HEADER>());

            location = new Location(headerOffset,Convert.ToUInt32(headerOffset),imageBase + headerOffset,size,size);
        }

        #region Methods

        public override string ToString()
        {
            return "File Header";
        }

        public byte[] GetBytes()
        {
            Stream stream = reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        public MachineType GetMachineType()
        {
            return (MachineType)header.Machine;
        }

        public DateTime GetTimeDateStamp()
        {
            return Utils.ConvertTimeDateStamp(header.TimeDateStamp);
        }

        public CharacteristicsType GetCharacteristics()
        {
            return (CharacteristicsType)header.Characteristics;
        }

        #endregion

        #region Properties

        public Location Location
        {
            get
            {
                return location;
            }
        }

        [FieldAnnotation("Machine Type",Flags = true,FlagType = typeof(MachineType))]
        public ushort Machine
        {
            get
            {
                return header.Machine;
            }
        }

        [FieldAnnotation("Number of Sections")]
        public ushort NumberOfSections
        {
            get
            {
                return header.NumberOfSections;
            }
        }

        [FieldAnnotation("Date/Time Stamp")]
        public uint TimeDateStamp
        {
            get
            {
                return header.TimeDateStamp;
            }
        }

        [FieldAnnotation("Pointer to Symbol Table")]
        public uint PointerToSymbolTable
        {
            get
            {
                return header.PointerToSymbolTable;
            }
        }

        [FieldAnnotation("Number of Symbols")]
        public uint NumberOfSymbols
        {
            get
            {
                return header.NumberOfSymbols;
            }
        }

        [FieldAnnotation("Size of Optional Header")]
        public ushort SizeOfOptionalHeader
        {
            get
            {
                return header.SizeOfOptionalHeader;
            }
        }

        [FieldAnnotation("Characteristics",Flags = true,FlagType = typeof(CharacteristicsType))]
        public ushort Characteristics
        {
            get
            {
                return header.Characteristics;
            }
        }

        #endregion

    }

}
