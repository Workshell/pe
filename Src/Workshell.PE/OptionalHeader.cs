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

    public enum MagicType : int
    {
        [EnumAnnotation("IMAGE_NT_OPTIONAL_HDR32_MAGIC")]
        PE32 = 0x10b,
        [EnumAnnotation("IMAGE_NT_OPTIONAL_HDR64_MAGIC")]
        PE32plus = 0x20b,
        [EnumAnnotation("IMAGE_ROM_OPTIONAL_HDR_MAGIC")]
        ROM = 0x107
    }

    public enum SubSystemType : int
    {
        [EnumAnnotation("IMAGE_SUBSYSTEM_UNKNOWN")]
        Unknown = 0,
        [EnumAnnotation("IMAGE_SUBSYSTEM_NATIVE")]
        Native = 1,
        [EnumAnnotation("IMAGE_SUBSYSTEM_WINDOWS_GUI")]
        WindowsGUI = 2,
        [EnumAnnotation("IMAGE_SUBSYSTEM_WINDOWS_CUI")]
        WindowsCUI = 3,
        [EnumAnnotation("IMAGE_SUBSYSTEM_OS2_CUI")]
        OS2 = 5,
        [EnumAnnotation("IMAGE_SUBSYSTEM_POSIX_CUI")]
        Posix = 7,
        [EnumAnnotation("IMAGE_SUBSYSTEM_WINDOWS_CE_GUI")]
        WindowsCE = 9,
        [EnumAnnotation("IMAGE_SUBSYSTEM_EFI_APPLICATION")]
        EFIApplication = 10,
        [EnumAnnotation("IMAGE_SUBSYSTEM_EFI_BOOT_SERVICE_DRIVER")]
        EFIBootServiceDriver = 11,
        [EnumAnnotation("IMAGE_SUBSYSTEM_EFI_RUNTIME_DRIVER")]
        EFIRuntimeDriver = 12,
        [EnumAnnotation("IMAGE_SUBSYSTEM_EFI_ROM")]
        EFIRom = 13,
        [EnumAnnotation("IMAGE_SUBSYSTEM_XBOX")]
        Xbox = 14,
        [EnumAnnotation("IMAGE_SUBSYSTEM_WINDOWS_BOOT_APPLICATION")]
        BootApplication = 16
    }

    [Flags]
    public enum DllCharacteristicsType : int
    {
        [EnumAnnotation("IMAGE_DLLCHARACTERISTICS_HIGH_ENTROPY_VA")]
        HighEntropyVA = 0x0020,
        [EnumAnnotation("IMAGE_DLLCHARACTERISTICS_DYNAMIC_BASE")]
        DynamicBase = 0x0040,
        [EnumAnnotation("IMAGE_DLLCHARACTERISTICS_FORCE_INTEGRITY")]
        ForceIntegrity = 0x0080,
        [EnumAnnotation("IMAGE_DLLCHARACTERISTICS_NX_COMPAT")]
        NXCompat = 0x0100,
        [EnumAnnotation("IMAGE_DLLCHARACTERISTICS_NO_ISOLATION")]
        NoIsolation = 0x0200,
        [EnumAnnotation("IMAGE_DLLCHARACTERISTICS_NO_SEH")]
        NoSEH = 0x0400,
        [EnumAnnotation("IMAGE_DLLCHARACTERISTICS_NO_BIND")]
        NoBind = 0x0800,
        [EnumAnnotation("IMAGE_DLLCHARACTERISTICS_APPCONTAINER")]
        AppContainer = 0x1000,
        [EnumAnnotation("IMAGE_DLLCHARACTERISTICS_WDM_DRIVER")]
        WDMDriver = 0x2000,
        [EnumAnnotation("IMAGE_DLLCHARACTERISTICS_GUARD_CF")]
        GuardCf = 0x4000,
        [EnumAnnotation("IMAGE_DLLCHARACTERISTICS_TERMINAL_SERVER_AWARE")]
        TerminalServerAware = 0x8000
    }

    public abstract class OptionalHeader : ISupportsLocation
    {

        public static readonly int Size32 = Utils.SizeOf<IMAGE_OPTIONAL_HEADER32>();
        public static readonly int Size64 = Utils.SizeOf<IMAGE_OPTIONAL_HEADER64>();

        private ImageReader reader;
        private Location location;

        internal OptionalHeader(ImageReader exeReader, ulong headerOffset, uint headerSize, ulong imageBase)
        {
            reader = exeReader;
            location = new Location(headerOffset,Convert.ToUInt32(headerOffset),imageBase + headerOffset,headerSize,headerSize);
        }

        #region Methods

        public abstract byte[] GetBytes();

        public MagicType GetMagic()
        {
            return (MagicType)Magic;
        }

        public Version GetLinkerVersion()
        {
            return new Version(MajorLinkerVersion,MinorLinkerVersion);
        }

        public Version GetOperatingSystemVersion()
        {
            return new Version(MajorOperatingSystemVersion,MinorOperatingSystemVersion);
        }

        public Version GetImageVersion()
        {
            return new Version(MajorImageVersion,MinorImageVersion);
        }

        public Version GetSubsystemVersion()
        {
            return new Version(MajorSubsystemVersion,MinorSubsystemVersion);
        }

        public SubSystemType GetSubsystem()
        {
            return (SubSystemType)Subsystem;
        }

        public DllCharacteristicsType GetDllCharacteristics()
        {
            return (DllCharacteristicsType)DllCharacteristics;
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

        [FieldAnnotation("Magic")]
        public abstract ushort Magic
        {
            get;
        }

        [FieldAnnotation("Major Linker Version")]
        public abstract byte MajorLinkerVersion
        {
            get;
        }

        [FieldAnnotation("Minor Linker Version")]
        public abstract byte MinorLinkerVersion
        {
            get;
        }

        [FieldAnnotation("Size of Code")]
        public abstract uint SizeOfCode
        {
            get;
        }

        [FieldAnnotation("Size of Initialized Data")]
        public abstract uint SizeOfInitializedData
        {
            get;
        }

        [FieldAnnotation("Size of Uninitialized Data")]
        public abstract uint SizeOfUninitializedData
        {
            get;
        }

        [FieldAnnotation("Address of Entry Point")]
        public abstract uint AddressOfEntryPoint
        {
            get;
        }

        [FieldAnnotation("Base of Code")]
        public abstract uint BaseOfCode
        {
            get;
        }

        [FieldAnnotation("Image Base")]
        public abstract ulong ImageBase
        {
            get;
        }

        [FieldAnnotation("Section Alignment")]
        public abstract uint SectionAlignment
        {
            get;
        }

        [FieldAnnotation("File Alignment")]
        public abstract uint FileAlignment
        {
            get;
        }

        [FieldAnnotation("Major Operating System Version")]
        public abstract ushort MajorOperatingSystemVersion
        {
            get;
        }

        [FieldAnnotation("Minor Operating System Version")]
        public abstract ushort MinorOperatingSystemVersion
        {
            get;
        }

        [FieldAnnotation("Major Image Version")]
        public abstract ushort MajorImageVersion
        {
            get;
        }

        [FieldAnnotation("Minor Image Version")]
        public abstract ushort MinorImageVersion
        {
            get;
        }

        [FieldAnnotation("Major Sub-System Version")]
        public abstract ushort MajorSubsystemVersion
        {
            get;
        }

        [FieldAnnotation("Minor Sub-System Version")]
        public abstract ushort MinorSubsystemVersion
        {
            get;
        }

        [FieldAnnotation("Win32 Version Value")]
        public abstract uint Win32VersionValue
        {
            get;
        }

        [FieldAnnotation("Size of Image")]
        public abstract uint SizeOfImage
        {
            get;
        }

        [FieldAnnotation("Size of Headers")]
        public abstract uint SizeOfHeaders
        {
            get;
        }

        [FieldAnnotation("Checksum")]
        public abstract uint CheckSum
        {
            get;
        }

        [FieldAnnotation("Sub-System",Flags = true,FlagType = typeof(SubSystemType))]
        public abstract ushort Subsystem
        {
            get;
        }

        [FieldAnnotation("DLL Characteristics",Flags = true,FlagType = typeof(DllCharacteristicsType))]
        public abstract ushort DllCharacteristics
        {
            get;
        }

        [FieldAnnotation("Size of Stack Reserve")]
        public abstract ulong SizeOfStackReserve
        {
            get;
        }

        [FieldAnnotation("Size of Stack Commit")]
        public abstract ulong SizeOfStackCommit
        {
            get;
        }

        [FieldAnnotation("Size of Heap Reserve")]
        public abstract ulong SizeOfHeapReserve
        {
            get;
        }

        [FieldAnnotation("Size of Heap Commit")]
        public abstract ulong SizeOfHeapCommit
        {
            get;
        }

        [FieldAnnotation("Loader Flags")]
        public abstract uint LoaderFlags
        {
            get;
        }

        [FieldAnnotation("Number of RVA and Sizes")]
        public abstract uint NumberOfRvaAndSizes
        {
            get;
        }

        public abstract DataDirectories DataDirectories
        {
            get;
        }

        #endregion

    }

    public sealed class OptionalHeader32 : OptionalHeader
    {

        private IMAGE_OPTIONAL_HEADER32 header;
        private DataDirectories data_dirs;

        internal OptionalHeader32(ImageReader exeReader, IMAGE_OPTIONAL_HEADER32 optHeader, ulong headerOffset, ulong imageBase) : base(exeReader,headerOffset,Convert.ToUInt32(OptionalHeader.Size32),imageBase)
        {
            header = optHeader;

            /*
            List<DataDirectory> dirs = new List<DataDirectory>();

            dirs.AddRange(new DataDirectory[] {
                new DataDirectory(DataDirectoryType.ExportTable,header.ExportTable),
                new DataDirectory(DataDirectoryType.ImportTable,header.ImportTable),
                new DataDirectory(DataDirectoryType.ResourceTable,header.ResourceTable),
                new DataDirectory(DataDirectoryType.ExceptionTable,header.ExceptionTable),
                new DataDirectory(DataDirectoryType.CertificateTable,header.CertificateTable),
                new DataDirectory(DataDirectoryType.BaseRelocationTable,header.BaseRelocationTable),
                new DataDirectory(DataDirectoryType.Debug,header.Debug),
                new DataDirectory(DataDirectoryType.Architecture,header.Architecture),
                new DataDirectory(DataDirectoryType.GlobalPtr,header.GlobalPtr),
                new DataDirectory(DataDirectoryType.TLSTable,header.TLSTable),
                new DataDirectory(DataDirectoryType.LoadConfigTable,header.LoadConfigTable),
                new DataDirectory(DataDirectoryType.BoundImport,header.BoundImport),
                new DataDirectory(DataDirectoryType.ImportAddressTable,header.IAT),
                new DataDirectory(DataDirectoryType.DelayImportDescriptor,header.DelayImportDescriptor),
                new DataDirectory(DataDirectoryType.CLRRuntimeHeader,header.CLRRuntimeHeader)
            });

            long dir_size = 16 * DataDirectories.EntrySize;
            StreamLocation location = new StreamLocation((streamLoc.Offset + streamLoc.Size) - dir_size,dir_size);

            data_dirs = new DataDirectories(this,location,dirs.Where(dir => dir.DirectoryType != DataDirectoryType.None).ToDictionary(dir => dir.DirectoryType));
            */
        }

        #region Methods

        public override byte[] GetBytes()
        {
            byte[] buffer = new byte[OptionalHeader.Size32];

            Utils.Write<IMAGE_OPTIONAL_HEADER32>(header,buffer,0,buffer.Length);

            return buffer;
        }

        #endregion

        #region Properties

        public override ushort Magic
        {
            get
            {
                return header.Magic;
            }
        }

        public override byte MajorLinkerVersion
        {
            get
            {
                return header.MajorLinkerVersion;
            }
        }

        public override byte MinorLinkerVersion
        {
            get
            {
                return header.MinorLinkerVersion;
            }
        }

        public override uint SizeOfCode
        {
            get
            {
                return header.SizeOfCode;
            }
        }

        public override uint SizeOfInitializedData
        {
            get
            {
                return header.SizeOfInitializedData;
            }
        }

        public override uint SizeOfUninitializedData
        {
            get
            {
                return header.SizeOfUninitializedData;
            }
        }

        public override uint AddressOfEntryPoint
        {
            get
            {
                return header.AddressOfEntryPoint;
            }
        }

        public override uint BaseOfCode
        {
            get
            {
                return header.BaseOfCode;
            }
        }

        public override ulong ImageBase
        {
            get
            {
                return header.ImageBase;
            }
        }

        public override uint SectionAlignment
        {
            get
            {
                return header.SectionAlignment;
            }
        }

        public override uint FileAlignment
        {
            get
            {
                return header.FileAlignment;
            }
        }

        public override ushort MajorOperatingSystemVersion
        {
            get
            {
                return header.MajorOperatingSystemVersion;
            }
        }

        public override ushort MinorOperatingSystemVersion
        {
            get
            {
                return header.MinorOperatingSystemVersion;
            }
        }

        public override ushort MajorImageVersion
        {
            get
            {
                return header.MajorImageVersion;
            }
        }

        public override ushort MinorImageVersion
        {
            get
            {
                return header.MinorImageVersion;
            }
        }

        public override ushort MajorSubsystemVersion
        {
            get
            {
                return header.MajorSubsystemVersion;
            }
        }

        public override ushort MinorSubsystemVersion
        {
            get
            {
                return header.MinorSubsystemVersion;
            }
        }

        public override uint Win32VersionValue
        {
            get
            {
                return header.Win32VersionValue;
            }
        }

        public override uint SizeOfImage
        {
            get
            {
                return header.SizeOfImage;
            }
        }

        public override uint SizeOfHeaders
        {
            get
            {
                return header.SizeOfHeaders;
            }
        }

        public override uint CheckSum
        {
            get
            {
                return header.CheckSum;
            }
        }

        public override ushort Subsystem
        {
            get
            {
                return header.Subsystem;
            }
        }

        public override ushort DllCharacteristics
        {
            get
            {
                return header.DllCharacteristics;
            }
        }

        public override ulong SizeOfStackReserve
        {
            get
            {
                return header.SizeOfStackReserve;
            }
        }

        public override ulong SizeOfStackCommit
        {
            get
            {
                return header.SizeOfStackCommit;
            }
        }

        public override ulong SizeOfHeapReserve
        {
            get
            {
                return header.SizeOfHeapReserve;
            }
        }

        public override ulong SizeOfHeapCommit
        {
            get
            {
                return header.SizeOfHeapCommit;
            }
        }

        public override uint LoaderFlags
        {
            get
            {
                return header.LoaderFlags;
            }
        }

        public override uint NumberOfRvaAndSizes
        {
            get
            {
                return header.NumberOfRvaAndSizes;
            }
        }

        public override DataDirectories DataDirectories
        {
            get
            {
                return data_dirs;
            }
        }

        #endregion

    }

    public sealed class OptionalHeader64 : OptionalHeader
    {

        private IMAGE_OPTIONAL_HEADER64 header;
        private DataDirectories data_dirs;

        internal OptionalHeader64(ImageReader exeReader, IMAGE_OPTIONAL_HEADER64 optHeader, ulong headerOffset, ulong imageBase) : base(exeReader,headerOffset,Convert.ToUInt32(OptionalHeader.Size64),imageBase)
        {
            header = optHeader;

            /*
            List<DataDirectory> dirs = new List<DataDirectory>();

            dirs.AddRange(new DataDirectory[] {
                new DataDirectory(DataDirectoryType.ExportTable,header.ExportTable),
                new DataDirectory(DataDirectoryType.ImportTable,header.ImportTable),
                new DataDirectory(DataDirectoryType.ResourceTable,header.ResourceTable),
                new DataDirectory(DataDirectoryType.ExceptionTable,header.ExceptionTable),
                new DataDirectory(DataDirectoryType.CertificateTable,header.CertificateTable),
                new DataDirectory(DataDirectoryType.BaseRelocationTable,header.BaseRelocationTable),
                new DataDirectory(DataDirectoryType.Debug,header.Debug),
                new DataDirectory(DataDirectoryType.Architecture,header.Architecture),
                new DataDirectory(DataDirectoryType.GlobalPtr,header.GlobalPtr),
                new DataDirectory(DataDirectoryType.TLSTable,header.TLSTable),
                new DataDirectory(DataDirectoryType.LoadConfigTable,header.LoadConfigTable),
                new DataDirectory(DataDirectoryType.BoundImport,header.BoundImport),
                new DataDirectory(DataDirectoryType.ImportAddressTable,header.IAT),
                new DataDirectory(DataDirectoryType.DelayImportDescriptor,header.DelayImportDescriptor),
                new DataDirectory(DataDirectoryType.CLRRuntimeHeader,header.CLRRuntimeHeader)
            });

            long dir_size = 16 * DataDirectories.EntrySize;
            StreamLocation location = new StreamLocation((streamLoc.Offset + streamLoc.Size) - dir_size,dir_size);

            data_dirs = new DataDirectories(this,location,dirs.Where(dir => dir.DirectoryType != DataDirectoryType.None).ToDictionary(dir => dir.DirectoryType));
             */
        }

        #region Methods

        public override byte[] GetBytes()
        {
            byte[] buffer = new byte[OptionalHeader.Size64];

            Utils.Write<IMAGE_OPTIONAL_HEADER64>(header,buffer,0,buffer.Length);

            return buffer;
        }

        #endregion

        #region Properties

        public override ushort Magic
        {
            get
            {
                return header.Magic;
            }
        }

        public override byte MajorLinkerVersion
        {
            get
            {
                return header.MajorLinkerVersion;
            }
        }

        public override byte MinorLinkerVersion
        {
            get
            {
                return header.MinorLinkerVersion;
            }
        }

        public override uint SizeOfCode
        {
            get
            {
                return header.SizeOfCode;
            }
        }

        public override uint SizeOfInitializedData
        {
            get
            {
                return header.SizeOfInitializedData;
            }
        }

        public override uint SizeOfUninitializedData
        {
            get
            {
                return header.SizeOfUninitializedData;
            }
        }

        public override uint AddressOfEntryPoint
        {
            get
            {
                return header.AddressOfEntryPoint;
            }
        }

        public override uint BaseOfCode
        {
            get
            {
                return header.BaseOfCode;
            }
        }

        public override ulong ImageBase
        {
            get
            {
                return header.ImageBase;
            }
        }

        public override uint SectionAlignment
        {
            get
            {
                return header.SectionAlignment;
            }
        }

        public override uint FileAlignment
        {
            get
            {
                return header.FileAlignment;
            }
        }

        public override ushort MajorOperatingSystemVersion
        {
            get
            {
                return header.MajorOperatingSystemVersion;
            }
        }

        public override ushort MinorOperatingSystemVersion
        {
            get
            {
                return header.MinorOperatingSystemVersion;
            }
        }

        public override ushort MajorImageVersion
        {
            get
            {
                return header.MajorImageVersion;
            }
        }

        public override ushort MinorImageVersion
        {
            get
            {
                return header.MinorImageVersion;
            }
        }

        public override ushort MajorSubsystemVersion
        {
            get
            {
                return header.MajorSubsystemVersion;
            }
        }

        public override ushort MinorSubsystemVersion
        {
            get
            {
                return header.MinorSubsystemVersion;
            }
        }

        public override uint Win32VersionValue
        {
            get
            {
                return header.Win32VersionValue;
            }
        }

        public override uint SizeOfImage
        {
            get
            {
                return header.SizeOfImage;
            }
        }

        public override uint SizeOfHeaders
        {
            get
            {
                return header.SizeOfHeaders;
            }
        }

        public override uint CheckSum
        {
            get
            {
                return header.CheckSum;
            }
        }

        public override ushort Subsystem
        {
            get
            {
                return header.Subsystem;
            }
        }

        public override ushort DllCharacteristics
        {
            get
            {
                return header.DllCharacteristics;
            }
        }

        public override ulong SizeOfStackReserve
        {
            get
            {
                return header.SizeOfStackReserve;
            }
        }

        public override ulong SizeOfStackCommit
        {
            get
            {
                return header.SizeOfStackCommit;
            }
        }

        public override ulong SizeOfHeapReserve
        {
            get
            {
                return header.SizeOfHeapReserve;
            }
        }

        public override ulong SizeOfHeapCommit
        {
            get
            {
                return header.SizeOfHeapCommit;
            }
        }

        public override uint LoaderFlags
        {
            get
            {
                return header.LoaderFlags;
            }
        }

        public override uint NumberOfRvaAndSizes
        {
            get
            {
                return header.NumberOfRvaAndSizes;
            }
        }

        public override DataDirectories DataDirectories
        {
            get
            {
                return data_dirs;
            }
        }

        #endregion
    
    }

}
