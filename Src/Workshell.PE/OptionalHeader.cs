using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Annotations;
using Workshell.PE.Extensions;
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

    public abstract class OptionalHeader : ISupportsLocation, ISupportsBytes
    {
        private readonly PortableExecutableImage _image;

        internal OptionalHeader(PortableExecutableImage image, ulong headerOffset, uint headerSize, ulong imageBase, ushort magic)
        {
            _image = image;

            Location = new Location(image.GetCalculator(), headerOffset, headerOffset.ToUInt32(), imageBase + headerOffset, headerSize, headerSize);
            Magic = magic;
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

        #region Static Properties

        public static int Size32 { get; } = Marshal.SizeOf<IMAGE_OPTIONAL_HEADER32>();
        public static int Size64 { get; } = Marshal.SizeOf<IMAGE_OPTIONAL_HEADER64>();

        #endregion

        #region Properties

        public Location Location { get; }

        [FieldAnnotation("Magic")]
        public ushort Magic { get; }

        [FieldAnnotation("Major Linker Version")]
        public abstract byte MajorLinkerVersion { get; }

        [FieldAnnotation("Minor Linker Version")]
        public abstract byte MinorLinkerVersion { get; }

        [FieldAnnotation("Size of Code")]
        public abstract uint SizeOfCode { get; }

        [FieldAnnotation("Size of Initialized Data")]
        public abstract uint SizeOfInitializedData { get; }

        [FieldAnnotation("Size of Uninitialized Data")]
        public abstract uint SizeOfUninitializedData { get; }

        [FieldAnnotation("Address of Entry Point")]
        public abstract uint AddressOfEntryPoint { get; }

        [FieldAnnotation("Base of Code")]
        public abstract uint BaseOfCode { get; }

        [FieldAnnotation("Base of Data")]
        public abstract uint BaseOfData { get; }

        [FieldAnnotation("Image Base")]
        public abstract ulong ImageBase { get; }

        [FieldAnnotation("Section Alignment")]
        public abstract uint SectionAlignment { get; }

        [FieldAnnotation("File Alignment")]
        public abstract uint FileAlignment { get; }

        [FieldAnnotation("Major Operating System Version")]
        public abstract ushort MajorOperatingSystemVersion { get; }

        [FieldAnnotation("Minor Operating System Version")]
        public abstract ushort MinorOperatingSystemVersion { get; }

        [FieldAnnotation("Major Image Version")]
        public abstract ushort MajorImageVersion { get; }

        [FieldAnnotation("Minor Image Version")]
        public abstract ushort MinorImageVersion { get; }

        [FieldAnnotation("Major Sub-System Version")]
        public abstract ushort MajorSubsystemVersion { get; }

        [FieldAnnotation("Minor Sub-System Version")]
        public abstract ushort MinorSubsystemVersion { get; }

        [FieldAnnotation("Win32 Version Value")]
        public abstract uint Win32VersionValue { get; }

        [FieldAnnotation("Size of Image")]
        public abstract uint SizeOfImage { get; }

        [FieldAnnotation("Size of Headers")]
        public abstract uint SizeOfHeaders { get; }

        [FieldAnnotation("Checksum")]
        public abstract uint CheckSum { get; }

        [FieldAnnotation("Sub-System",Flags = true,FlagType = typeof(SubSystemType))]
        public abstract ushort Subsystem { get; }

        [FieldAnnotation("DLL Characteristics",Flags = true,FlagType = typeof(DllCharacteristicsType))]
        public abstract ushort DllCharacteristics { get; }

        [FieldAnnotation("Size of Stack Reserve")]
        public abstract ulong SizeOfStackReserve { get; }

        [FieldAnnotation("Size of Stack Commit")]
        public abstract ulong SizeOfStackCommit { get; }

        [FieldAnnotation("Size of Heap Reserve")]
        public abstract ulong SizeOfHeapReserve { get; }

        [FieldAnnotation("Size of Heap Commit")]
        public abstract ulong SizeOfHeapCommit { get; }

        [FieldAnnotation("Loader Flags")]
        public abstract uint LoaderFlags { get; }

        [FieldAnnotation("Number of RVA and Sizes")]
        public abstract uint NumberOfRvaAndSizes { get; }

        #endregion
    }

    public sealed class OptionalHeader32 : OptionalHeader
    {
        private readonly IMAGE_OPTIONAL_HEADER32 _header;

        internal OptionalHeader32(PortableExecutableImage image, IMAGE_OPTIONAL_HEADER32 optHeader, ulong headerOffset, ulong imageBase, ushort magic) : base(image, headerOffset, OptionalHeader.Size32.ToUInt32(), imageBase,magic)
        {
            _header = optHeader;
        }

        #region Properties

        public override byte MajorLinkerVersion => _header.MajorLinkerVersion;
        public override byte MinorLinkerVersion => _header.MinorLinkerVersion;
        public override uint SizeOfCode => _header.SizeOfCode;
        public override uint SizeOfInitializedData => _header.SizeOfInitializedData;
        public override uint SizeOfUninitializedData => _header.SizeOfUninitializedData;
        public override uint AddressOfEntryPoint => _header.AddressOfEntryPoint;
        public override uint BaseOfCode => _header.BaseOfCode;
        public override uint BaseOfData => _header.BaseOfData;
        public override ulong ImageBase => _header.ImageBase;
        public override uint SectionAlignment => _header.SectionAlignment;
        public override uint FileAlignment => _header.FileAlignment;
        public override ushort MajorOperatingSystemVersion => _header.MajorOperatingSystemVersion;
        public override ushort MinorOperatingSystemVersion => _header.MinorOperatingSystemVersion;
        public override ushort MajorImageVersion => _header.MajorImageVersion;
        public override ushort MinorImageVersion => _header.MinorImageVersion;
        public override ushort MajorSubsystemVersion => _header.MajorSubsystemVersion;
        public override ushort MinorSubsystemVersion => _header.MinorSubsystemVersion;
        public override uint Win32VersionValue => _header.Win32VersionValue;
        public override uint SizeOfImage => _header.SizeOfImage;
        public override uint SizeOfHeaders => _header.SizeOfHeaders;
        public override uint CheckSum => _header.CheckSum;
        public override ushort Subsystem => _header.Subsystem;
        public override ushort DllCharacteristics => _header.DllCharacteristics;
        public override ulong SizeOfStackReserve => _header.SizeOfStackReserve;
        public override ulong SizeOfStackCommit => _header.SizeOfStackCommit;
        public override ulong SizeOfHeapReserve => _header.SizeOfHeapReserve;
        public override ulong SizeOfHeapCommit => _header.SizeOfHeapCommit;
        public override uint LoaderFlags => _header.LoaderFlags;
        public override uint NumberOfRvaAndSizes => _header.NumberOfRvaAndSizes;

        #endregion
    }

    public sealed class OptionalHeader64 : OptionalHeader
    {
        private readonly IMAGE_OPTIONAL_HEADER64 _header;

        internal OptionalHeader64(PortableExecutableImage image, IMAGE_OPTIONAL_HEADER64 optHeader, ulong headerOffset, ulong imageBase, ushort magic) : base(image, headerOffset, OptionalHeader.Size64.ToUInt32(), imageBase, magic)
        {
            _header = optHeader;
        }

        #region Properties

        public override byte MajorLinkerVersion => _header.MajorLinkerVersion;
        public override byte MinorLinkerVersion => _header.MinorLinkerVersion;
        public override uint SizeOfCode => _header.SizeOfCode;
        public override uint SizeOfInitializedData => _header.SizeOfInitializedData;
        public override uint SizeOfUninitializedData => _header.SizeOfUninitializedData;
        public override uint AddressOfEntryPoint => _header.AddressOfEntryPoint;
        public override uint BaseOfCode => _header.BaseOfCode;
        public override uint BaseOfData => 0;
        public override ulong ImageBase => _header.ImageBase;
        public override uint SectionAlignment => _header.SectionAlignment;
        public override uint FileAlignment => _header.FileAlignment;
        public override ushort MajorOperatingSystemVersion => _header.MajorOperatingSystemVersion;
        public override ushort MinorOperatingSystemVersion => _header.MinorOperatingSystemVersion;
        public override ushort MajorImageVersion => _header.MajorImageVersion;
        public override ushort MinorImageVersion => _header.MinorImageVersion;
        public override ushort MajorSubsystemVersion => _header.MajorSubsystemVersion;
        public override ushort MinorSubsystemVersion => _header.MinorSubsystemVersion;
        public override uint Win32VersionValue => _header.Win32VersionValue;
        public override uint SizeOfImage => _header.SizeOfImage;
        public override uint SizeOfHeaders => _header.SizeOfHeaders;
        public override uint CheckSum => _header.CheckSum;
        public override ushort Subsystem => _header.Subsystem;
        public override ushort DllCharacteristics => _header.DllCharacteristics;
        public override ulong SizeOfStackReserve => _header.SizeOfStackReserve;
        public override ulong SizeOfStackCommit => _header.SizeOfStackCommit;
        public override ulong SizeOfHeapReserve => _header.SizeOfHeapReserve;
        public override ulong SizeOfHeapCommit => _header.SizeOfHeapCommit;
        public override uint LoaderFlags => _header.LoaderFlags;
        public override uint NumberOfRvaAndSizes => _header.NumberOfRvaAndSizes;

        #endregion   
    }
}
