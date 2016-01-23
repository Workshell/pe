using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public class ExeReader : IDisposable, IRawDataSupport
    {

        private bool _disposed;
        private Stream _stream;
        private bool _own_stream;

        private DOSHeader _dos_header;
        private DOSStub _dos_stub;
        private NTHeaders _nt_headers;
        private SectionTable _section_table;
        private Sections _sections;

        private bool is_32bit;
        private bool is_64bit;
        private bool is_clr;

        private ExeReader(Stream sourceStream, bool ownStream)
        {
            if (!sourceStream.CanRead)
                throw new IOException("Cannot read from stream.");

            if (!sourceStream.CanSeek)
                throw new IOException("Cannot seek in stream.");

            _disposed = false;
            _stream = sourceStream;
            _own_stream = ownStream;

            _dos_header = null;
            _dos_stub = null;
            _nt_headers = null;
            _section_table = null;
            _sections = null;

            is_32bit = false;
            is_64bit = false;
            is_clr = false;
        }

        #region Static Methods

        public static bool IsValid(Stream stream)
        {
            string error_message;

            return IsValid(stream,out error_message);
        }

        public static bool IsValid(Stream stream, out string errorMessage)
        {
            return Validate(stream,out errorMessage);
        }

        public static bool IsValid(string fileName)
        {
            string error_message;

            return IsValid(fileName,out error_message);
        }

        public static bool IsValid(string fileName, out string errorMessage)
        {
            FileStream file = new FileStream(fileName,FileMode.Open,FileAccess.Read);

            return IsValid(file,out errorMessage);  
        }

        public static ExeReader FromFile(string fileName)
        {
            FileStream file = new FileStream(fileName,FileMode.Open,FileAccess.Read);

            return FromStream(file,true);
        }

        public static ExeReader FromStream(Stream stream)
        {
            return FromStream(stream,false);
        }

        public static ExeReader FromStream(Stream stream, bool ownStream)
        {
            return new ExeReader(stream,ownStream);
        }

        private static bool Validate(Stream stream, out string errorMessage)
        {
            errorMessage = String.Empty;

            if (!stream.CanSeek)
            {
                errorMessage = "Cannot seek in stream.";

                return false;
            }

            if (!stream.CanRead)
            {
                errorMessage = "Cannot read from stream.";

                return false;
            }

            stream.Seek(0,SeekOrigin.Begin);

            IMAGE_DOS_HEADER dos_header = Utils.Read<IMAGE_DOS_HEADER>(stream,DOSHeader.Size);

            if (dos_header.e_magic != DOSHeader.DOS_MAGIC_MZ)
            {
                errorMessage = "Incorrect magic number specified in MS-DOS header.";

                return false;
            }

            if (dos_header.e_lfanew == 0)
            {
                errorMessage = "No new header location specified in MS-DOS header, most likely a 16-bit executable.";

                return false;
            }

            if (dos_header.e_lfanew >= (256 * (1024 * 1024)))
            {           
                errorMessage = "New header location specified in MS-DOS header is beyond 256mb boundary (see RtlImageNtHeaderEx).";

                return false;
            }

            if (dos_header.e_lfanew % 4 != 0)
            {
                errorMessage = "New header location specified in MS-DOS header is not properly aligned.";

                return false;
            }

            if (dos_header.e_lfanew < DOSHeader.Size)
            {
                errorMessage = "New header location specified is invalid.";

                return false;
            }

            long stub_offset = DOSHeader.Size;
            int stub_size = dos_header.e_lfanew - DOSHeader.Size;

            if ((stub_offset + stub_size) > stream.Length)
            {
                errorMessage = "Cannot read beyond end of stream.";

                return false;
            }

            stream.Seek(dos_header.e_lfanew,SeekOrigin.Begin);

            uint pe_sig = Utils.ReadUInt32(stream);

            if (pe_sig != NTHeaders.PE_MAGIC_MZ)
            {
                errorMessage = "Incorrect PE signature found in NT Header.";

                return false;
            }

            if ((stream.Position + FileHeader.Size) > stream.Length)
            {
                errorMessage = "Cannot read beyond end of stream.";

                return false;
            }

            IMAGE_FILE_HEADER file_header = Utils.Read<IMAGE_FILE_HEADER>(stream,FileHeader.Size);
            CharacteristicsType characteristics = (CharacteristicsType)file_header.Characteristics;
            bool is_32bit = ((characteristics & CharacteristicsType.Supports32Bit) == CharacteristicsType.Supports32Bit);
            bool is_64bit = !is_32bit;

            if ((stream.Position + file_header.SizeOfOptionalHeader) > stream.Length)
            {
                errorMessage = "Cannot read beyond end of stream.";

                return false;
            }

            IMAGE_OPTIONAL_HEADER32 opt_header_32 = new IMAGE_OPTIONAL_HEADER32();
            IMAGE_OPTIONAL_HEADER64 opt_header_64 = new IMAGE_OPTIONAL_HEADER64();

            if (!is_64bit)
            {
                opt_header_32 = Utils.Read<IMAGE_OPTIONAL_HEADER32>(stream,OptionalHeader.Size32);

                if (stream.Length < opt_header_32.SizeOfImage)
                {
                    errorMessage = "Stream is smaller than specified image size.";

                    return false;
                }
            }
            else
            {
                opt_header_64 = Utils.Read<IMAGE_OPTIONAL_HEADER64>(stream,OptionalHeader.Size64);

                if (stream.Length < opt_header_64.SizeOfImage)
                {
                    errorMessage = "Stream is smaller than specified image size.";

                    return false;
                }
            }

            long section_table_size = SectionTableEntry.Size * file_header.NumberOfSections;

            if ((stream.Position + section_table_size) > stream.Length)
            {
                errorMessage = "Cannot read beyond end of stream.";

                return false;
            }

            List<IMAGE_SECTION_HEADER> section_table_headers = new List<IMAGE_SECTION_HEADER>();

            for(int i = 0; i < file_header.NumberOfSections; i++)
            {
                IMAGE_SECTION_HEADER header = Utils.Read<IMAGE_SECTION_HEADER>(stream,SectionTableEntry.Size);

                section_table_headers.Add(header);
            }

            foreach(IMAGE_SECTION_HEADER section_header in section_table_headers)
            {
                if ((section_header.PointerToRawData + section_header.SizeOfRawData) > stream.Length)
                {
                    StringBuilder builder = new StringBuilder();

                    foreach(char c in section_header.Name)
                    {
                        if (c == '\0')
                            break;

                        builder.Append(c);
                    }

                    errorMessage = String.Format("Section {0} data is beyond end of stream.",builder.ToString());

                    return false;
                }

                stream.Seek(section_header.PointerToRawData,SeekOrigin.Begin);
            }

            return true;
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_own_stream)
                    _stream.Dispose();
            
                _disposed = true;
            }
        }

        public byte[] GetBytes()
        {
            using (MemoryStream mem = new MemoryStream())
            {
                _stream.Seek(0,SeekOrigin.Begin);
                _stream.CopyTo(mem,64 * 1024);

                return mem.ToArray();
            }
        }

        private void LoadDOSHeader()
        {
            _stream.Seek(0,SeekOrigin.Begin);

            IMAGE_DOS_HEADER native_dos_header = Utils.Read<IMAGE_DOS_HEADER>(_stream,DOSHeader.Size);

            if (native_dos_header.e_magic != DOSHeader.DOS_MAGIC_MZ)
                throw new ExeReaderException("Incorrect magic number specified in MS-DOS header.");

            if (native_dos_header.e_lfanew == 0)
                throw new ExeReaderException("No new header location specified in MS-DOS header, most likely a 16-bit executable.");

            if (native_dos_header.e_lfanew >= (256 * (1024 * 1024)))
                throw new ExeReaderException("New header location specified in MS-DOS header is beyond 256mb boundary (see RtlImageNtHeaderEx).");

            if (native_dos_header.e_lfanew % 4 != 0)
                throw new ExeReaderException("New header location specified in MS-DOS header is not properly aligned.");

            if (native_dos_header.e_lfanew < DOSHeader.Size)
                throw new ExeReaderException("New header location specified is invalid.");

            StreamLocation location = new StreamLocation(0,DOSHeader.Size);

            _dos_header = new DOSHeader(this,native_dos_header,location);
        }

        private void LoadDOSStub()
        {
            if (_dos_header == null)
                LoadDOSHeader();

            long offset = _dos_header.Location.Offset + _dos_header.Location.Size;
            int size = _dos_header.FileAddressNewHeader - DOSHeader.Size;

            if ((offset + size) > _stream.Length)
                throw new ExeReaderException("Cannot read beyond end of stream.");

            StreamLocation location = new StreamLocation(offset,size);

            _dos_stub = new DOSStub(this,location);
        }

        private void LoadNTHeaders()
        {
            if (_dos_header == null)
                LoadDOSHeader();

            _stream.Seek(_dos_header.FileAddressNewHeader,SeekOrigin.Begin);

            LoadPESignature();

            FileHeader file_header = LoadFileHeader(_dos_header);
            OptionalHeader opt_header = LoadOptionalHeader(file_header);
            StreamLocation location = new StreamLocation(_dos_header.FileAddressNewHeader,4 + file_header.Location.Size + opt_header.Location.Size);

            _nt_headers = new NTHeaders(this,location,file_header,opt_header);

            if (_nt_headers.FileHeader != null)
            {
                CharacteristicsType characteristics = _nt_headers.FileHeader.GetCharacteristics();

                is_32bit = ((characteristics & CharacteristicsType.Supports32Bit) == CharacteristicsType.Supports32Bit);
                is_64bit = !is_32bit;
            }

            if (_nt_headers.OptionalHeader != null)
            {
                is_32bit = (_nt_headers.OptionalHeader.GetMagic() == MagicType.PE32);
                is_64bit = !is_32bit;
            }

            DataDirectory clr_dir = _nt_headers.OptionalHeader.DataDirectories[DataDirectoryType.CLRRuntimeHeader];

            is_clr = (clr_dir != null && clr_dir.VirtualAddress > 0 && clr_dir.Size > 0);
        }

        private void LoadPESignature()
        {
            uint sig = Utils.ReadUInt32(_stream);

            if (sig != NTHeaders.PE_MAGIC_MZ)
                throw new ExeReaderException("Incorrect magic number specified in PE header.");
        }

        private FileHeader LoadFileHeader(DOSHeader dosHeader)
        {
            IMAGE_FILE_HEADER native_file_header = Utils.Read<IMAGE_FILE_HEADER>(_stream,FileHeader.Size);
            StreamLocation location = new StreamLocation(dosHeader.FileAddressNewHeader + 4,FileHeader.Size);

            return new FileHeader(this,native_file_header,location);
        }

        private OptionalHeader LoadOptionalHeader(FileHeader fileHeader)
        {
            CharacteristicsType characteristics = fileHeader.GetCharacteristics();
            bool is_x64 = !((characteristics & CharacteristicsType.Supports32Bit) == CharacteristicsType.Supports32Bit);
            IMAGE_OPTIONAL_HEADER32 opt_header_32 = new IMAGE_OPTIONAL_HEADER32();
            IMAGE_OPTIONAL_HEADER64 opt_header_64 = new IMAGE_OPTIONAL_HEADER64();
            StreamLocation location = null;
            OptionalHeader opt_header = null;

            if (!is_x64)
            {
                opt_header_32 = Utils.Read<IMAGE_OPTIONAL_HEADER32>(_stream,OptionalHeader.Size32);
                location = new StreamLocation(fileHeader.Location.Offset + fileHeader.Location.Size,OptionalHeader.Size32);
                opt_header = new OptionalHeader32(this,opt_header_32,location);
            }
            else
            {
                opt_header_64 = Utils.Read<IMAGE_OPTIONAL_HEADER64>(_stream,OptionalHeader.Size64);
                location = new StreamLocation(fileHeader.Location.Offset + fileHeader.Location.Size,OptionalHeader.Size64);
                opt_header = new OptionalHeader64(this,opt_header_64,location);
            }

            return opt_header;
        }

        private void LoadSectionTable()
        {
            if (_nt_headers == null)
                LoadNTHeaders();

            long offset = _nt_headers.Location.Offset + _nt_headers.Location.Size;

            _stream.Seek(offset,SeekOrigin.Begin);

            List<IMAGE_SECTION_HEADER> headers = new List<IMAGE_SECTION_HEADER>();

            for(var i = 0; i < _nt_headers.FileHeader.NumberOfSections; i++)
            {
                IMAGE_SECTION_HEADER header = Utils.Read<IMAGE_SECTION_HEADER>(_stream,SectionTableEntry.Size);

                headers.Add(header);
            }

            long size = headers.Count * SectionTableEntry.Size;
            StreamLocation location = new StreamLocation(offset,size);

            _section_table = new SectionTable(this,location,headers);
        }

        private void LoadSections()
        {
            if (_section_table == null)
                LoadSectionTable();

            _sections = new Sections(this,_section_table);
        }

        #endregion

        internal Stream GetStream()
        {
            return _stream;
        }

        #region Location Conversion Methods

        /* VA */

        public Section VAToSection(ulong va)
        {
            if (_nt_headers == null)
                LoadNTHeaders();

            ulong image_base = _nt_headers.OptionalHeader.ImageBase;
            uint rva = Convert.ToUInt32(va - image_base);

            return RVAToSection(rva);
        }

        public SectionTableEntry VAToSectionTableEntry(ulong va)
        {
            if (_nt_headers == null)
                LoadNTHeaders();

            ulong image_base = _nt_headers.OptionalHeader.ImageBase;
            uint rva = Convert.ToUInt32(va - image_base);

            return RVAToSectionTableEntry(rva);
        }

        public long VAToOffset(ulong va)
        {
            if (_nt_headers == null)
                LoadNTHeaders();

            ulong image_base = _nt_headers.OptionalHeader.ImageBase;
            uint rva = Convert.ToUInt32(va - image_base);

            return RVAToOffset(rva);
        }

        public long VAToOffset(Section section, ulong va)
        {
            return VAToOffset(section.TableEntry,va);
        }

        public long VAToOffset(SectionTableEntry section, ulong va)
        {
            if (_nt_headers == null)
                LoadNTHeaders();

            ulong image_base = _nt_headers.OptionalHeader.ImageBase;
            uint rva = Convert.ToUInt32(va - image_base);

            return RVAToOffset(section,rva);
        }

        public ulong OffsetToVA(long offset)
        {
            if (_section_table == null)
                LoadSectionTable();

            foreach(SectionTableEntry entry in _section_table)
            {
                if (offset >= entry.PointerToRawData && offset < (entry.PointerToRawData + entry.SizeOfRawData))
                    return OffsetToVA(entry,offset);
            }

            return 0;
        }

        public ulong OffsetToVA(Section section, long offset)
        {
            return OffsetToVA(section.TableEntry,offset);
        }

        public ulong OffsetToVA(SectionTableEntry section, long offset)
        {
            if (_nt_headers == null)
                LoadNTHeaders();

            ulong image_base = _nt_headers.OptionalHeader.ImageBase;
            uint rva = Convert.ToUInt32((offset + section.VirtualAddress) - section.PointerToRawData);

            return image_base + rva;
        }

        /* RVA */

        public Section RVAToSection(uint rva)
        {
            if (_sections == null)
                LoadSections();

            SectionTableEntry entry = RVAToSectionTableEntry(rva);

            if (entry == null)
                return null;

            return _sections[entry];
        }

        public SectionTableEntry RVAToSectionTableEntry(uint rva)
        {
            if (_section_table == null)
                LoadSectionTable();

            foreach(SectionTableEntry entry in _section_table)
            {
                if (rva >= entry.VirtualAddress && rva <= (entry.VirtualAddress + entry.SizeOfRawData))
                    return entry;
            }

            return null;
        }

        public long RVAToOffset(uint rva)
        {
            if (_section_table == null)
                LoadSectionTable();

            foreach(SectionTableEntry entry in _section_table)
            {
                if (rva >= entry.VirtualAddress && rva < (entry.VirtualAddress + entry.SizeOfRawData))
                    return RVAToOffset(entry,rva);
            }

            return 0;
        }

        public long RVAToOffset(Section section, uint rva)
        {
            return RVAToOffset(section.TableEntry,rva);
        }

        public long RVAToOffset(SectionTableEntry section, uint rva)
        {
            long offset = (rva - section.VirtualAddress) + section.PointerToRawData;

            return offset;
        }

        public uint OffsetToRVA(long offset)
        {
            if (_section_table == null)
                LoadSectionTable();

            foreach(SectionTableEntry entry in _section_table)
            {
                if (offset >= entry.PointerToRawData && offset < (entry.PointerToRawData + entry.SizeOfRawData))
                    return OffsetToRVA(entry,offset);
            }

            return 0;
        }

        public uint OffsetToRVA(Section section, long offset)
        {
            return OffsetToRVA(section.TableEntry,offset);
        }

        public uint OffsetToRVA(SectionTableEntry section, long offset)
        {
            uint rva = Convert.ToUInt32((offset + section.VirtualAddress) - section.PointerToRawData);

            return rva;
        }

        #endregion

        #region Properties

        public bool Is32Bit
        {
            get
            {
                if (_nt_headers == null)
                    LoadNTHeaders();

                return is_32bit;
            }
        }

        public bool Is64Bit
        {
            get
            {
                if (_nt_headers == null)
                    LoadNTHeaders();

                return is_64bit;
            }
        }

        public bool IsCLR
        {
            get
            {
                if (_nt_headers == null)
                    LoadNTHeaders();

                return is_clr;
            }
        }

        public DOSHeader DOSHeader
        {
            get
            {
                if (_dos_header == null)
                    LoadDOSHeader();

                return _dos_header;
            }
        }

        public DOSStub DOSStub
        {
            get
            {
                if (_dos_stub == null)
                    LoadDOSStub();

                return _dos_stub;
            }
        }

        public NTHeaders NTHeaders
        {
            get
            {
                if (_nt_headers == null)
                    LoadNTHeaders();

                return _nt_headers;
            }
        }

        public SectionTable SectionTable
        {
            get
            {
                if (_section_table == null)
                    LoadSectionTable();

                return _section_table;
            }
        }

        public Sections Sections
        {
            get
            {
                if (_sections == null)
                    LoadSections();

                return _sections;
            }
        }

        #endregion

    }

}
