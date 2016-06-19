using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE
{

    [Flags]
    public enum COMImageFlags : int
    {
        [EnumAnnotationAttribute("COMIMAGE_FLAGS_ILONLY")]
        ILOnly = 0x00001,
        [EnumAnnotationAttribute("COMIMAGE_FLAGS_32BITREQUIRED")]
        Requires32Bit = 0x00002,
        [EnumAnnotationAttribute("COMIMAGE_FLAGS_IL_LIBRARY")]
        ILLibrary = 0x00004,
        [EnumAnnotationAttribute("COMIMAGE_FLAGS_STRONGNAMESIGNED")]
        StrongNameSigned = 0x00008,
        [EnumAnnotationAttribute("COMIMAGE_FLAGS_NATIVE_ENTRYPOINT")]
        NativeEntryPoint = 0x00010,
        [EnumAnnotationAttribute("COMIMAGE_FLAGS_TRACKDEBUGDATA")]
        TrackDebugData = 0x10000,
        [EnumAnnotationAttribute("COMIMAGE_FLAGS_32BITPREFERRED")]
        Prefer32Bit = 0x20000
    }

    public sealed class CLRHeader : ISupportsLocation, ISupportsBytes
    {

        private CLR clr;
        private Location location;
        private IMAGE_COR20_HEADER header;
        private Version runtime_version;
        private CLRDataDirectory meta_data;
        private CLRDataDirectory resources;
        private CLRDataDirectory sn_sig;
        private CLRDataDirectory code_man_table;
        private CLRDataDirectory vtable_fixups;
        private CLRDataDirectory export_address_table;
        private CLRDataDirectory native_header;

        internal CLRHeader(CLR clrContent, Location clrLocation, IMAGE_COR20_HEADER clrHeader)
        {
            clr = clrContent;           
            location = clrLocation;
            header = clrHeader;
            runtime_version = null;
            meta_data = null;
            resources = null;
            sn_sig = null;
            code_man_table = null;
            vtable_fixups = null;
            export_address_table = null;
            native_header = null;
        }

        #region Static Methods

        public static CLRHeader Get(CLR clr)
        {
            LocationCalculator calc = clr.DataDirectory.Directories.Image.GetCalculator();
            ulong offset = calc.RVAToOffset(clr.DataDirectory.VirtualAddress);
            uint size = Utils.SizeOf<IMAGE_COR20_HEADER>().ToUInt32();
            ulong image_base = clr.DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            Section section = calc.RVAToSection(clr.DataDirectory.VirtualAddress);
            Location location = new Location(offset, clr.DataDirectory.VirtualAddress, image_base + clr.DataDirectory.VirtualAddress, size, size, section);
            Stream stream = clr.DataDirectory.Directories.Image.GetStream();

            stream.Seek(offset.ToInt64(), SeekOrigin.Begin);

            IMAGE_COR20_HEADER clr_header = Utils.Read<IMAGE_COR20_HEADER>(stream, Convert.ToInt32(size));
            CLRHeader result = new CLRHeader(clr, location, clr_header);

            return result;
        }

        #endregion

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = clr.DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        public Version GetRuntimeVersion()
        {
            if (runtime_version == null)
                runtime_version = new Version(header.MajorRuntimeVersion, header.MinorRuntimeVersion);

            return runtime_version;
        }

        public CLRDataDirectory GetMetaData()
        {
            if (meta_data == null)
                meta_data = new CLRDataDirectory(header.MetaData);

            return meta_data;
        }

        public COMImageFlags GetFlags()
        {
            return (COMImageFlags)header.Flags;
        }

        public CLRDataDirectory GetResources()
        {
            if (resources == null)
                resources = new CLRDataDirectory(header.Resources);

            return resources;
        }

        public CLRDataDirectory GetStrongNameSignature()
        {
            if (sn_sig == null)
                sn_sig = new CLRDataDirectory(header.StrongNameSignature);

            return sn_sig;
        }

        public CLRDataDirectory GetCodeManagerTable()
        {
            if (code_man_table == null)
                code_man_table = new CLRDataDirectory(header.CodeManagerTable);

            return code_man_table;
        }

        public CLRDataDirectory GetVTableFixups()
        {
            if (vtable_fixups == null)
                vtable_fixups = new CLRDataDirectory(header.VTableFixups);

            return vtable_fixups;
        }

        public CLRDataDirectory GetExportAddressTableJumps()
        {
            if (export_address_table == null)
                export_address_table = new CLRDataDirectory(header.ExportAddressTableJumps);

            return export_address_table;
        }

        public CLRDataDirectory GetManagedNativeHeader()
        {
            if (native_header == null)
                native_header = new CLRDataDirectory(header.ManagedNativeHeader);

            return native_header;
        }

        #endregion

        #region Properties

        public CLR CLR
        {
            get
            {
                return clr;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        [FieldAnnotation("Header Size")]
        public uint HeaderSize
        {
            get
            {
                return header.cb;
            }
        }

        [FieldAnnotation("Major Runtime Version")]
        public ushort MajorRuntimeVersion
        {
            get
            {
                return header.MajorRuntimeVersion;
            }
        }

        [FieldAnnotation("Minor Runtime Version")]
        public ushort MinorRuntimeVersion
        {
            get
            {
                return header.MinorRuntimeVersion;
            }
        }

        [FieldAnnotation("MetaData Virtual Address")]
        public uint MetaDataAddress
        {
            get
            {
                return header.MetaData.VirtualAddress;
            }
        }

        [FieldAnnotation("MetaData Size")]
        public uint MetaDataSize
        {
            get
            {
                return header.MetaData.Size;
            }
        }

        [FieldAnnotation("Flags")]
        public uint Flags
        {
            get
            {
                return header.Flags;
            }
        }

        [FieldAnnotation("EntryPoint Token/Virtual Address")]
        public uint EntryPointTokenOrVirtualAddress
        {
            get
            {
                return header.EntryPointTokenOrRVA;
            }
        }

        [FieldAnnotation("Resources Virtual Address")]
        public uint ResourcesAddress
        {
            get
            {
                return header.Resources.VirtualAddress;
            }
        }

        [FieldAnnotation("Resources Size")]
        public uint ResourcesSize
        {
            get
            {
                return header.Resources.Size;
            }
        }

        [FieldAnnotation("Strongname Signature Virtual Address")]
        public uint StrongNameSignatureAddress
        {
            get
            {
                return header.StrongNameSignature.VirtualAddress;
            }
        }

        [FieldAnnotation("Strongname Signature Size")]
        public uint StrongNameSignatureSize
        {
            get
            {
                return header.StrongNameSignature.Size;
            }
        }

        [FieldAnnotation("Code Manager Table Virtual Address")]
        public uint CodeManagerTableAddress
        {
            get
            {
                return header.CodeManagerTable.VirtualAddress;
            }
        }

        [FieldAnnotation("Code Manager Table Size")]
        public uint CodeManagerTableSize
        {
            get
            {
                return header.CodeManagerTable.Size;
            }
        }

        [FieldAnnotation("VTable Fixups Virtual Address")]
        public uint VTableFixupsAddress
        {
            get
            {
                return header.VTableFixups.VirtualAddress;
            }
        }

        [FieldAnnotation("VTable Fixups Size")]
        public uint VTableFixupsSize
        {
            get
            {
                return header.VTableFixups.Size;
            }
        }

        [FieldAnnotation("Export Address Table Jumps Virtual Address")]
        public uint ExportAddressTableJumpsAddress
        {
            get
            {
                return header.ExportAddressTableJumps.VirtualAddress;
            }
        }

        [FieldAnnotation("Export Address Table Jumps Size")]
        public uint ExportAddressTableJumpsSize
        {
            get
            {
                return header.CodeManagerTable.Size;
            }
        }

        [FieldAnnotation("Managed Native Header Virtual Address")]
        public uint ManagedNativeHeaderAddress
        {
            get
            {
                return header.ManagedNativeHeader.VirtualAddress;
            }
        }

        [FieldAnnotation("Managed Native Header Size")]
        public uint ManagedNativeHeaderSize
        {
            get
            {
                return header.ManagedNativeHeader.Size;
            }
        }

        #endregion

    }

}
