using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Attributes;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public enum DebugDirectoryType
    {
        [EnumAnnotation("IMAGE_DEBUG_TYPE_UNKNOWN")]
        Unknown = 0,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_COFF")]
        COFF = 1,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_CODEVIEW")]
        CodeView = 2,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_FPO")]
        FPO = 3,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_MISC")]
        Misc = 4,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_EXCEPTION")]
        Exception = 5,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_FIXUP")]
        Fixup = 6,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_OMAP_TO_SRC")]
        OMAPToSrc = 7,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_OMAP_FROM_SRC")]
        OMAPFromSrc = 8,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_BORLAND")]
        Bolrand = 9,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_RESERVED10")]
        Reserved = 10,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_CLSID")]
        CLSID = 11,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_VC_FEATURE")]
        VCFeature = 12,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_POGO")]
        POGO = 13,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_ILTCG")]
        ILTCG = 14,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_MPX")]
        MPX = 15
    }

    public class DebugDirectory : ILocationSupport, IRawDataSupport
    {

        public static readonly int size = Utils.SizeOf<IMAGE_DEBUG_DIRECTORY>();

        private DebugContent content;
        private StreamLocation location;
        private IMAGE_DEBUG_DIRECTORY directory;

        internal DebugDirectory(DebugContent debugContent, long directoryOffset, IMAGE_DEBUG_DIRECTORY debugDirectory)
        {
            content = debugContent;
            location = new StreamLocation(directoryOffset,DebugDirectory.Size);
            directory = debugDirectory;
        }

        #region Methods

        public override string ToString()
        {
            return String.Format("Debug Type: {0}",GetDirectoryType());
        }

        public byte[] GetBytes()
        {
            Stream stream = Content.Section.Sections.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,directory.PointerToRawData,directory.SizeOfData);

            return buffer;
        }

        public DateTime GetTimeDateStamp()
        {
            return Utils.ConvertTimeDateStamp(directory.TimeDateStamp);
        }

        public DebugDirectoryType GetDirectoryType()
        {
            return (DebugDirectoryType)directory.Type;
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

        public DebugContent Content
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

        public uint Characteristics
        {
            get
            {
                return directory.Characteristics;
            }
        }

        public uint TimeDateStamp
        {
            get
            {
                return directory.TimeDateStamp;
            }
        }

        public ushort MajorVersion
        {
            get
            {
                return directory.MajorVersion;
            }
        }

        public ushort MinorVersion
        {
            get
            {
                return directory.MinorVersion;
            }
        }

        public uint Type
        {
            get
            {
                return directory.Type;
            }
        }

        public uint SizeOfData
        {
            get
            {
                return directory.SizeOfData;
            }
        }

        public uint AddressOfRawData
        {
            get
            {
                return directory.AddressOfRawData;
            }
        }

        public uint PointerToRawData
        {
            get
            {
                return directory.PointerToRawData;
            }
        }

        #endregion

    }

}
