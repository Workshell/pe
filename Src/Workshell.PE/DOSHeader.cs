#region License
//  Copyright(c) 2016, Workshell Ltd
//  All rights reserved.
//  
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Workshell Ltd nor the names of its contributors
//  may be used to endorse or promote products
//  derived from this software without specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED.IN NO EVENT SHALL WORKSHELL BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

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

    public sealed class DOSHeader : ISupportsLocation, ISupportsBytes
    {

        public const ushort DOS_MAGIC_MZ = 23117;

        private static readonly int size = Utils.SizeOf<IMAGE_DOS_HEADER>();

        private ExecutableImage image;
        private IMAGE_DOS_HEADER header;
        private Location location;

        internal DOSHeader(ExecutableImage exeImage, IMAGE_DOS_HEADER dosHeader, ulong imageBase)
        {
            image = exeImage;
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
            Stream stream = image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

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

        public ExecutableImage Image
        {
            get
            {
                return image;
            }
        }

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
