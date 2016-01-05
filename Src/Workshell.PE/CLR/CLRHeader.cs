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

    public class CLRHeader : ILocatable
    {

        private static readonly int size = Utils.SizeOf<IMAGE_COR20_HEADER>();

        private CLRContent content;
        private IMAGE_COR20_HEADER header;
        private StreamLocation location;

        internal CLRHeader(Stream stream, CLRContent clrContent, long headerOffset)
        {
            header = Utils.Read<IMAGE_COR20_HEADER>(stream,size);
            location = new StreamLocation(headerOffset,size);
        }

        #region Methods

        internal IMAGE_COR20_HEADER GetNativeHeader()
        {
            return header;
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

        public StreamLocation Location
        {
            get
            {
                return location;
            }
        }

        public Version RuntimeVersion
        {
            get
            {
                return new Version(header.MajorRuntimeVersion,header.MinorRuntimeVersion);
            }
        }

        public DataDirectory MetaData
        {
            get
            {
                return new DataDirectory(DataDirectoryType.None,header.MetaData);
            }
        }

        public COMImageFlags Flags
        {
            get
            {
                return (COMImageFlags)header.Flags;
            }
        }

        public uint EntryPointToken
        {
            get
            {
                if ((Flags & COMImageFlags.NativeEntryPoint) != COMImageFlags.NativeEntryPoint)
                {
                    return header.EntryPointTokenOrRVA;
                }
                else
                {
                    return 0;
                }
            }
        }

        public uint EntryPointRVA
        {
            get
            {
                if ((Flags & COMImageFlags.NativeEntryPoint) == COMImageFlags.NativeEntryPoint)
                {
                    return header.EntryPointTokenOrRVA;
                }
                else
                {
                    return 0;
                }
            }
        }

        public DataDirectory Resources
        {
            get
            {
                return new DataDirectory(DataDirectoryType.None,header.Resources);
            }
        }

        public DataDirectory StrongNameSignature
        {
            get
            {
                return new DataDirectory(DataDirectoryType.None,header.StrongNameSignature);
            }
        }

        public DataDirectory CodeManagerTable
        {
            get
            {
                return new DataDirectory(DataDirectoryType.None,header.CodeManagerTable);
            }
        }

        public DataDirectory VTableFixups
        {
            get
            {
                return new DataDirectory(DataDirectoryType.None,header.VTableFixuups);
            }
        }

        public DataDirectory ExportAddressTableJumps
        {
            get
            {
                return new DataDirectory(DataDirectoryType.None,header.ExportAddressTableJumps);
            }
        }

        public DataDirectory ManagedNativeHeader
        {
            get
            {
                return new DataDirectory(DataDirectoryType.None,header.ManagedNativeHeader);
            }
        }

        #endregion

    }

}
