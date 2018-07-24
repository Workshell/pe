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

            Location = new Location(image.GetCalculator(), 0, 0, imageBase, Size.ToUInt32(), Size.ToUInt32());
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

        public static int Size { get; } = Marshal.SizeOf<IMAGE_DOS_HEADER>();

        #endregion

        #region Properties

        public Location Location { get; }

        [FieldAnnotation("Signature")]
        public ushort Magic => _header.e_magic;

        [FieldAnnotation("Bytes on last page of file")]
        public ushort BytesOnLastPage => _header.e_cblp;

        [FieldAnnotation("Pages in file")]
        public ushort PagesInFile => _header.e_cp;

        [FieldAnnotation("Relocations")]
        public ushort Relocations => _header.e_crlc;

        [FieldAnnotation("Size of header in paragraphs")]
        public ushort SizeHeaderParagraphs => _header.e_cparhdr;

        [FieldAnnotation("Minimum extra paragraphs needed")]
        public ushort MinExtraParagraphs => _header.e_minalloc;

        [FieldAnnotation("Maximum extra paragraphs needed")]
        public ushort MaxExtraParagraphs => _header.e_maxalloc;

        [FieldAnnotation("Initial (relative) CS value")]
        public ushort InitialSSValue => _header.e_ss;

        [FieldAnnotation("Initial SP value")]
        public ushort InitialSPValue => _header.e_sp;

        [FieldAnnotation("Checksum")]
        public ushort Checksum => _header.e_csum;

        [FieldAnnotation("Initial SP value")]
        public ushort InitialIPValue => _header.e_ip;

        [FieldAnnotation("Initial (relative) CS value")]
        public ushort InitialCSValue => _header.e_cs;

        [FieldAnnotation("File address of relocation table")]
        public ushort FileAddressRelocationTable => _header.e_lfarlc;

        [FieldAnnotation("Overlay number")]
        public ushort OverlayNumber => _header.e_ovno;

        [FieldAnnotation("Reserved",ArrayLength = 4)]
        public ushort[] Reserved1 => _header.e_res_1;

        [FieldAnnotation("OEM identifier")]
        public ushort OEMIdentifier => _header.e_oemid;

        [FieldAnnotation("OEM information")]
        public ushort OEMInformation => _header.e_oeminfo;

        [FieldAnnotation("Reserved",ArrayLength = 10)]
        public ushort[] Reserved2 => _header.e_res_2;

        [FieldAnnotation("File address of new header")]
        public int FileAddressNewHeader => _header.e_lfanew;

        #endregion
    }
}
