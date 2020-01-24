#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

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
    public sealed class DOSHeader : ISupportsLocation, ISupportsBytes
    {
        public const ushort DOS_MAGIC_MZ = 23117;

        private readonly PortableExecutableImage _image;
        private readonly IMAGE_DOS_HEADER _header;
        
        internal DOSHeader(PortableExecutableImage image, IMAGE_DOS_HEADER dosHeader, ulong imageBase)
        {
            _image = image;
            _header = dosHeader;

            Location = new Location(image, 0, 0, imageBase, Size, Size);
        }

        #region Methods

        public override string ToString()
        {
            return "MS-DOS Header";
        }

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

        #endregion

        #region Static Properties

        public static uint Size { get; } = Utils.SizeOf<IMAGE_DOS_HEADER>().ToUInt32();

        #endregion

        #region Properties

        public Location Location { get; }

        [FieldAnnotation("Signature", Order = 1)]
        public ushort Magic => _header.e_magic;

        [FieldAnnotation("Bytes on last page of file", Order = 2)]
        public ushort BytesOnLastPage => _header.e_cblp;

        [FieldAnnotation("Pages in file", Order = 3)]
        public ushort PagesInFile => _header.e_cp;

        [FieldAnnotation("Relocations", Order = 4)]
        public ushort Relocations => _header.e_crlc;

        [FieldAnnotation("Size of header in paragraphs", Order = 5)]
        public ushort SizeHeaderParagraphs => _header.e_cparhdr;

        [FieldAnnotation("Minimum extra paragraphs needed", Order = 6)]
        public ushort MinExtraParagraphs => _header.e_minalloc;

        [FieldAnnotation("Maximum extra paragraphs needed", Order = 7)]
        public ushort MaxExtraParagraphs => _header.e_maxalloc;

        [FieldAnnotation("Initial (relative) CS value", Order = 8)]
        public ushort InitialSSValue => _header.e_ss;

        [FieldAnnotation("Initial SP value", Order = 9)]
        public ushort InitialSPValue => _header.e_sp;

        [FieldAnnotation("Checksum", Order = 10)]
        public ushort Checksum => _header.e_csum;

        [FieldAnnotation("Initial SP value", Order = 11)]
        public ushort InitialIPValue => _header.e_ip;

        [FieldAnnotation("Initial (relative) CS value", Order = 12)]
        public ushort InitialCSValue => _header.e_cs;

        [FieldAnnotation("File address of relocation table", Order = 13)]
        public ushort FileAddressRelocationTable => _header.e_lfarlc;

        [FieldAnnotation("Overlay number", Order = 14)]
        public ushort OverlayNumber => _header.e_ovno;

        [FieldAnnotation("Reserved", ArrayLength = 4, Order = 15)]
        public ushort[] Reserved1 => _header.e_res_1;

        [FieldAnnotation("OEM identifier", Order = 16)]
        public ushort OEMIdentifier => _header.e_oemid;

        [FieldAnnotation("OEM information", Order = 17)]
        public ushort OEMInformation => _header.e_oeminfo;

        [FieldAnnotation("Reserved", ArrayLength = 10, Order = 18)]
        public ushort[] Reserved2 => _header.e_res_2;

        [FieldAnnotation("File address of new header", Order = 19)]
        public uint FileAddressNewHeader => _header.e_lfanew;

        #endregion
    }
}
