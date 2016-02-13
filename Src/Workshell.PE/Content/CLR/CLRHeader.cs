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

        private CLRContent content;
        private IMAGE_COR20_HEADER header;
        private Location location;
        private Section section;

        internal CLRHeader(CLRContent clrContent, IMAGE_COR20_HEADER clrHeader, Location clrLocation, Section clrSection)
        {
            content = clrContent;
            header = clrHeader;
            location = clrLocation;
            section = clrSection;
        }

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = content.DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        public Version GetRuntimeVersion()
        {
            return new Version(header.MajorRuntimeVersion,header.MinorRuntimeVersion);
        }

        public CLRDataDirectory GetMetaData()
        {
            return new CLRDataDirectory(header.MetaData);
        }

        public COMImageFlags GetFlags()
        {
            return (COMImageFlags)header.Flags;
        }

        public CLRDataDirectory GetResources()
        {
            return new CLRDataDirectory(header.Resources);
        }

        public CLRDataDirectory GetStrongNameSignature()
        {
            return new CLRDataDirectory(header.StrongNameSignature);
        }

        public CLRDataDirectory GetCodeManagerTable()
        {
            return new CLRDataDirectory(header.CodeManagerTable);
        }

        public CLRDataDirectory GetVTableFixups()
        {
            return new CLRDataDirectory(header.VTableFixups);
        }

        public CLRDataDirectory GetExportAddressTableJumps()
        {
            return new CLRDataDirectory(header.ExportAddressTableJumps);
        }

        public CLRDataDirectory GetManagedNativeHeader()
        {
            return new CLRDataDirectory(header.ManagedNativeHeader);
        }

        #endregion

        #region Properties

        public CLRContent Content
        {
            get
            {
                return content;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        public Section Section
        {
            get
            {
                return section;
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
