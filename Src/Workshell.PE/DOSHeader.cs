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

    public class DOSHeader
    {

        public const ushort DOS_MAGIC_MZ = 23117;

        private static readonly int size = Utils.SizeOf<IMAGE_DOS_HEADER>();

        private ImageReader reader;
        private IMAGE_DOS_HEADER header;
        private Location location;

        internal DOSHeader(ImageReader exeReader, IMAGE_DOS_HEADER dosHeader, ulong imageBase)
        {
            reader = exeReader;
            header = dosHeader;
            location = new Location(0,0,imageBase,Convert.ToUInt32(DOSHeader.Size),Convert.ToUInt32(DOSHeader.Size));
        }

        #region Methods

        public override string ToString()
        {
            return "MS-DOS Header";
        }

        public byte[] GetBytes()
        {
            byte[] buffer = new byte[size];

            Utils.Write<IMAGE_DOS_HEADER>(header,buffer,0,buffer.Length);

            return buffer;
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

        public Location Location
        {
            get
            {
                return location;
            }
        }

        [FieldAnnotation("Signature")]
        public ushort Magic
        {
            get
            {
                return header.e_magic;
            }
        }

        [FieldAnnotation("Bytes on last page of file")]
        public ushort BytesOnLastPage
        {
            get
            {
                return header.e_cblp;
            }
        }

        [FieldAnnotation("Pages in file")]
        public ushort PagesInFile
        {
            get
            {
                return header.e_cp;
            }
        }

        [FieldAnnotation("Relocations")]
        public ushort Relocations
        {
            get
            {
                return header.e_crlc;
            }
        }

        [FieldAnnotation("Size of header in paragraphs")]
        public ushort SizeHeaderParagraphs
        {
            get
            {
                return header.e_cparhdr;
            }
        }

        [FieldAnnotation("Minimum extra paragraphs needed")]
        public ushort MinExtraParagraphs
        {
            get
            {
                return header.e_minalloc;
            }
        }

        [FieldAnnotation("Maximum extra paragraphs needed")]
        public ushort MaxExtraParagraphs
        {
            get
            {
                return header.e_maxalloc;
            }
        }

        [FieldAnnotation("Initial (relative) CS value")]
        public ushort InitialSSValue
        {
            get
            {
                return header.e_ss;
            }
        }

        [FieldAnnotation("Initial SP value")]
        public ushort InitialSPValue
        {
            get
            {
                return header.e_sp;
            }
        }

        [FieldAnnotation("Checksum")]
        public ushort Checksum
        {
            get
            {
                return header.e_csum;
            }
        }

        [FieldAnnotation("Initial SP value")]
        public ushort InitialIPValue
        {
            get
            {
                return header.e_ip;
            }
        }

        [FieldAnnotation("Initial (relative) CS value")]
        public ushort InitialCSValue
        {
            get
            {
                return header.e_cs;
            }
        }

        [FieldAnnotation("File address of relocation table")]
        public ushort FileAddressRelocationTable
        {
            get
            {
                return header.e_lfarlc;
            }
        }

        [FieldAnnotation("Overlay number")]
        public ushort OverlayNumber
        {
            get
            {
                return header.e_ovno;
            }
        }

        [FieldAnnotation("Reserved",ArrayLength = 4)]
        public ushort[] Reserved1
        {
            get
            {
                return header.e_res_1;
            }
        }

        [FieldAnnotation("OEM identifier")]
        public ushort OEMIdentifier
        {
            get
            {
                return header.e_oemid;
            }
        }

        [FieldAnnotation("OEM information")]
        public ushort OEMInformation
        {
            get
            {
                return header.e_oeminfo;
            }
        }

        [FieldAnnotation("Reserved",ArrayLength = 10)]
        public ushort[] Reserved2
        {
            get
            {
                return header.e_res_2;
            }
        }

        [FieldAnnotation("File address of new header")]
        public int FileAddressNewHeader
        {
            get
            {
                return header.e_lfanew;
            }
        }

        #endregion

    }

}
