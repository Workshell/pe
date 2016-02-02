using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Attributes;
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

    public class CLRHeader : ILocationSupport, IRawDataSupport
    {

        private static readonly int size = Utils.SizeOf<IMAGE_COR20_HEADER>();

        private CLRContent content;
        private IMAGE_COR20_HEADER header;
        private StreamLocation location;

        internal CLRHeader(CLRContent clrContent, DataDirectory dataDirectory)
        {
            long offset = clrContent.Section.RVAToOffset(dataDirectory.VirtualAddress);
            Stream stream = clrContent.Section.Sections.Reader.GetStream();

            stream.Seek(offset,SeekOrigin.Begin);

            content = clrContent;
            location = new StreamLocation(offset,size);
            header = Utils.Read<IMAGE_COR20_HEADER>(stream);        
        }

        #region Methods

        public byte[] GetBytes()
        {
            byte[] buffer = new byte[size];

            Utils.Write<IMAGE_COR20_HEADER>(header,buffer,0,buffer.Length);

            return buffer;
        }

        public Version GetRuntimeVersion()
        {
            return new Version(header.MajorRuntimeVersion,header.MinorRuntimeVersion);
        }

        public DataDirectory GetMetaData()
        {
            return new DataDirectory(DataDirectoryType.Unknown,header.MetaData);
        }

        public COMImageFlags GetFlags()
        {
            return (COMImageFlags)header.Flags;
        }

        public DataDirectory GetResources()
        {
            return new DataDirectory(DataDirectoryType.Unknown,header.Resources);
        }

        public DataDirectory GetStrongNameSignature()
        {
            return new DataDirectory(DataDirectoryType.Unknown,header.StrongNameSignature);
        }

        public DataDirectory GetCodeManagerTable()
        {
            return new DataDirectory(DataDirectoryType.Unknown,header.CodeManagerTable);
        }

        public DataDirectory GetVTableFixups()
        {
            return new DataDirectory(DataDirectoryType.Unknown,header.VTableFixups);
        }

        public DataDirectory GetExportAddressTableJumps()
        {
            return new DataDirectory(DataDirectoryType.Unknown,header.ExportAddressTableJumps);
        }

        public DataDirectory GetManagedNativeHeader()
        {
            return new DataDirectory(DataDirectoryType.Unknown,header.ManagedNativeHeader);
        }

        #endregion

        #region Static Properties

        public static int Size
        {
            get
            {
                return size;
            }
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

        public StreamLocation Location
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
