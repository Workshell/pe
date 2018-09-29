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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE
{
    public sealed class PortableExecutableImage : IDisposable
    {
        private readonly Stream _stream;
        private readonly bool _ownStream;
        private bool _disposed;
        private LocationCalculator _calc;

        private PortableExecutableImage(Stream stream, bool ownStream)
        {
            _stream = stream;
            _ownStream = ownStream;
            _disposed = false;
            _calc = null;
        }

        #region Static Methods

        public static PortableExecutableImage FromFile(string fileName)
        {
            var file = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            return FromStream(file);
        }

        public static async Task<PortableExecutableImage> FromFileAsync(string fileName)
        {
            var file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            
            return await FromStreamAsync(file).ConfigureAwait(false);
        }

        public static PortableExecutableImage FromStream(Stream stream, bool ownStream = true)
        {
            return FromStreamAsync(stream, ownStream).GetAwaiter().GetResult();
        }

        public static async Task<PortableExecutableImage> FromStreamAsync(Stream stream, bool ownStream = true)
        {
            var image = new PortableExecutableImage(stream, ownStream);

            await image.LoadAsync().ConfigureAwait(false);

            return image;
        }

        public static bool IsValid(string fileName)
        {
            var file = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            return IsValid(file);
        }

        public static async Task<bool> IsValidAsync(string fileName)
        {
            var file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            
            return await IsValidAsync(file).ConfigureAwait(false);
        }

        public static bool IsValid(Stream stream, bool ownStream = true)
        {
            return IsValidAsync(stream, ownStream).GetAwaiter().GetResult();
        }

        public static async Task<bool> IsValidAsync(Stream stream, bool ownStream = true)
        {     
            try
            {
                using (var image = await PortableExecutableImage.FromStreamAsync(stream, ownStream).ConfigureAwait(false))
                {
                    return true;
                }
            }
            catch (PortableExecutableImageException)
            {
                return false;
            }
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_ownStream)
                    _stream.Dispose();

                _disposed = true;
            }
        }

        public LocationCalculator GetCalculator()
        {
            if (_calc == null)
                _calc = new LocationCalculator(this);

            return _calc;
        }

        public Stream GetStream()
        {
            return _stream;
        }

        private async Task LoadAsync()
        {
            if (!_stream.CanSeek)
                throw new PortableExecutableImageException(this, "Cannot seek in stream.");

            if (!_stream.CanRead)
                throw new PortableExecutableImageException(this, "Cannot read from stream.");

            IMAGE_DOS_HEADER dosHeader;

            try
            {
                dosHeader = await _stream.ReadStructAsync<IMAGE_DOS_HEADER>(DOSHeader.Size).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new PortableExecutableImageException(this, "Cannot read DOS header from stream.", ex);
            }

            if (dosHeader.e_magic != DOSHeader.DOS_MAGIC_MZ)
                throw new PortableExecutableImageException(this, "Incorrect magic number specified in DOS header.");

            if (dosHeader.e_lfanew == 0)
                throw new PortableExecutableImageException(this, "No new header location specified in DOS header, most likely a 16-bit executable.");

            if (dosHeader.e_lfanew >= (256 * (1024 * 1024)))
                throw new PortableExecutableImageException(this, "New header location specified in MS-DOS header is beyond 256mb boundary (see RtlImageNtHeaderEx).");

            if (dosHeader.e_lfanew % 4 != 0)
                throw new PortableExecutableImageException(this, "New header location specified in MS-DOS header is not properly aligned.");

            if (dosHeader.e_lfanew < DOSHeader.Size)
                throw new PortableExecutableImageException(this, "New header location specified is invalid.");

            var stubOffset = DOSHeader.Size;
            var stubSize = dosHeader.e_lfanew - DOSHeader.Size;
            var stubRead = await _stream.SkipBytesAsync(stubSize).ConfigureAwait(false);

            if (stubRead < stubSize)
                throw new PortableExecutableImageException(this, "Could not read DOS stub from stream.");

            _stream.Seek(dosHeader.e_lfanew, SeekOrigin.Begin);

            var ntOffset = _stream.Position;
            uint peSig;

            try
            {
                peSig = await _stream.ReadUInt32Async().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new PortableExecutableImageException(this, "Could not read PE signature from stream.", ex);
            }

            if (peSig != NTHeaders.PE_MAGIC_MZ)
                throw new PortableExecutableImageException(this, "Incorrect PE signature found in NT Header.");

            IMAGE_FILE_HEADER fileHdr;

            try
            {
                fileHdr = await _stream.ReadStructAsync<IMAGE_FILE_HEADER>(FileHeader.Size).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new PortableExecutableImageException(this, "Could not read NT Header from stream.", ex);
            }

            ushort magic = 0;

            try
            {
                magic = await _stream.ReadUInt16Async().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new PortableExecutableImageException(this, "Could not read Optional Header magic number from stream.", ex);
            }

            Is32Bit = (magic == (ushort)MagicType.PE32);
            Is64Bit = (magic == (ushort)MagicType.PE32plus);

            if (!Is32Bit && !Is64Bit)
                throw new PortableExecutableImageException(this, "Unknown PE type.");

            byte[] optionalHeaderBytes;

            try
            {
                var optionalHeaderSize = (Is32Bit ? Utils.SizeOf<IMAGE_OPTIONAL_HEADER32>() : Utils.SizeOf<IMAGE_OPTIONAL_HEADER64>());

                optionalHeaderBytes = await _stream.ReadBytesAsync(optionalHeaderSize).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new PortableExecutableImageException(this, "Could not read Optional Header from stream.", ex);
            }

            IMAGE_OPTIONAL_HEADER32 optionalHeader32 = new IMAGE_OPTIONAL_HEADER32();
            IMAGE_OPTIONAL_HEADER64 optionalHeader64 = new IMAGE_OPTIONAL_HEADER64();
            var dirCount = 0;

            if (Is32Bit)
            {
                optionalHeader32 = Utils.Read<IMAGE_OPTIONAL_HEADER32>(optionalHeaderBytes);

                dirCount = optionalHeader32.NumberOfRvaAndSizes.ToInt32();
            }
            else
            {
                optionalHeader64 = Utils.Read<IMAGE_OPTIONAL_HEADER64>(optionalHeaderBytes);

                dirCount = optionalHeader64.NumberOfRvaAndSizes.ToInt32();
            }

            var dataDirs = new IMAGE_DATA_DIRECTORY[dirCount];
            var dataDirSize = Utils.SizeOf<IMAGE_DATA_DIRECTORY>();

            for (var i = 0; i < dirCount; i++)
            {
                try
                {
                    dataDirs[i] = await _stream.ReadStructAsync<IMAGE_DATA_DIRECTORY>(dataDirSize).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new PortableExecutableImageException(this, "Could not read data directory from stream.", ex);
                }
            }

            var sectionTableEntrySize = Utils.SizeOf<IMAGE_SECTION_HEADER>();
            var sectionTable = new IMAGE_SECTION_HEADER[fileHdr.NumberOfSections];

            for (var i = 0; i < fileHdr.NumberOfSections; i++)
            {
                try
                {
                    sectionTable[i] = await _stream.ReadStructAsync<IMAGE_SECTION_HEADER>(sectionTableEntrySize).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new PortableExecutableImageException(this, "Could not read section table entry from stream.", ex);
                }
            }

            IsCLR = false;

            var clrIndex = (int)DataDirectoryType.CLRRuntimeHeader;

            if (clrIndex >= 0 && clrIndex <= (dataDirs.Length - 1))
            {
                var clrDirectory = dataDirs[clrIndex];

                if (clrDirectory.VirtualAddress > 0 && clrDirectory.Size > 0)
                    IsCLR = true;
            }

            IsSigned = false;

            var certIndex = (int)DataDirectoryType.CertificateTable;

            if (certIndex >= 0 && certIndex <= (dataDirs.Length - 1))
            {
                var certDirectory = dataDirs[certIndex];

                if (certDirectory.VirtualAddress > 0 && certDirectory.Size > 0)
                    IsSigned = true;
            }

            var imageBase = (Is32Bit ? optionalHeader32.ImageBase : optionalHeader64.ImageBase);

            DOSHeader = new DOSHeader(this, dosHeader, imageBase);
            DOSStub = new DOSStub(this, stubOffset.ToUInt64(), stubSize.ToUInt32(), imageBase);

            var fileHeader = new FileHeader(this, fileHdr, DOSStub.Location.FileOffset + DOSStub.Location.FileSize + 4, imageBase);
            OptionalHeader optionalHeader;

            if (Is32Bit)
            {
                optionalHeader = new OptionalHeader32(this, optionalHeader32, fileHeader.Location.FileOffset + fileHeader.Location.FileSize, imageBase, magic);
            }
            else
            {
                optionalHeader = new OptionalHeader64(this, optionalHeader64, fileHeader.Location.FileOffset + fileHeader.Location.FileSize, imageBase, magic);
            }

            var dataDirectories = new DataDirectories(this, optionalHeader, dataDirs);

            NTHeaders = new NTHeaders(this, ntOffset.ToUInt64(), imageBase, fileHeader, optionalHeader, dataDirectories);
            SectionTable = new SectionTable(this, sectionTable, NTHeaders.Location.FileOffset + NTHeaders.Location.FileSize, imageBase);
            Sections = new Sections(this, SectionTable);
        }

        #endregion

        #region Properties

        public bool Is32Bit { get; private set; }
        public bool Is64Bit { get; private set; }
        public bool IsCLR { get; private set; }
        public bool IsSigned { get; private set; }
        public DOSHeader DOSHeader { get; private set; }
        public DOSStub DOSStub { get; private set; }
        public NTHeaders NTHeaders { get; private set; }
        public SectionTable SectionTable { get; private set; }
        public Sections Sections { get; private set; }

        #endregion
    }
}
