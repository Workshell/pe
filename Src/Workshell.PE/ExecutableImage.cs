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

    public class ExecutableImage : IDisposable, ISupportsBytes
    {

        private class PreloadedInformation
        {

            public IMAGE_DOS_HEADER DOSHeader;
            public ulong StubOffset;
            public uint StubSize;
            public IMAGE_FILE_HEADER FileHeader;
            public IMAGE_OPTIONAL_HEADER32? OptHeader32;
            public IMAGE_OPTIONAL_HEADER64? OptHeader64;
            public IMAGE_DATA_DIRECTORY[] DataDirectories;
            public IMAGE_SECTION_HEADER[] SectionHeaders;

            public bool Is32Bit;
            public bool Is64Bit;
            public bool IsCLR;
            public bool IsSigned;
        
        }

        private bool _disposed;
        private Stream _stream;
        private bool _own_stream;
        private LocationCalculator _calc;
        private IMemoryStreamProvider _memory_stream_provider;

        private DOSHeader _dos_header;
        private DOSStub _dos_stub;
        private NTHeaders _nt_headers;
        private SectionTable _section_table;
        private Sections _sections;

        private bool _is_32bit;
        private bool _is_64bit;
        private bool _is_clr;
        private bool _is_signed;

        private ExecutableImage(Stream sourceStream, bool ownStream)
        {
            _disposed = false;
            _stream = sourceStream;
            _own_stream = ownStream;
            _calc = null;
            _memory_stream_provider = null;

            _dos_header = null;
            _dos_stub = null;
            _nt_headers = null;
            _section_table = null;
            _sections = null;

            _is_32bit = false;
            _is_64bit = false;
            _is_clr = false;
            _is_signed = false;

            Load();
        }

        #region Static Methods

        public static bool IsValid(Stream stream)
        {
            string error_message;

            return IsValid(stream,out error_message);
        }

        public static bool IsValid(Stream stream, out string errorMessage)
        {
            PreloadedInformation preload_info = TryPreload(stream,out errorMessage);

            return (preload_info != null);
        }

        public static bool IsValid(string fileName)
        {
            string error_message;

            return IsValid(fileName,out error_message);
        }

        public static bool IsValid(string fileName, out string errorMessage)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                return IsValid(file, out errorMessage);
            }
        }

        public static ExecutableImage FromFile(string fileName)
        {
            FileStream file = new FileStream(fileName,FileMode.Open,FileAccess.Read);

            return FromStream(file,true);
        }

        public static ExecutableImage FromStream(Stream stream)
        {
            return FromStream(stream,false);
        }

        public static ExecutableImage FromStream(Stream stream, bool ownStream)
        {
            return new ExecutableImage(stream,ownStream);
        }

        private static PreloadedInformation TryPreload(Stream stream, out string errorMessage)
        {
            errorMessage = String.Empty;

            if (!stream.CanSeek)
            {
                errorMessage = "Cannot seek in stream.";

                return null;
            }

            if (!stream.CanRead)
            {
                errorMessage = "Cannot read from stream.";

                return null;
            }

            stream.Seek(0,SeekOrigin.Begin);

            IMAGE_DOS_HEADER dos_header = Utils.Read<IMAGE_DOS_HEADER>(stream,DOSHeader.Size);

            if (dos_header.e_magic != DOSHeader.DOS_MAGIC_MZ)
            {
                errorMessage = "Incorrect magic number specified in MS-DOS header.";

                return null;
            }

            if (dos_header.e_lfanew == 0)
            {
                errorMessage = "No new header location specified in MS-DOS header, most likely a 16-bit executable.";

                return null;
            }

            if (dos_header.e_lfanew >= (256 * (1024 * 1024)))
            {           
                errorMessage = "New header location specified in MS-DOS header is beyond 256mb boundary (see RtlImageNtHeaderEx).";

                return null;
            }

            if (dos_header.e_lfanew % 4 != 0)
            {
                errorMessage = "New header location specified in MS-DOS header is not properly aligned.";

                return null;
            }

            if (dos_header.e_lfanew < DOSHeader.Size)
            {
                errorMessage = "New header location specified is invalid.";

                return null;
            }

            long stub_offset = DOSHeader.Size;
            int stub_size = dos_header.e_lfanew - DOSHeader.Size;

            if ((stub_offset + stub_size) > stream.Length)
            {
                errorMessage = "Cannot read beyond end of stream.";

                return null;
            }

            stream.Seek(dos_header.e_lfanew,SeekOrigin.Begin);

            uint pe_sig = Utils.ReadUInt32(stream);

            if (pe_sig != NTHeaders.PE_MAGIC_MZ)
            {
                errorMessage = "Incorrect PE signature found in NT Header.";

                return null;
            }

            if ((stream.Position + FileHeader.Size) > stream.Length)
            {
                errorMessage = "Cannot read beyond end of stream.";

                return null;
            }

            IMAGE_FILE_HEADER file_header = Utils.Read<IMAGE_FILE_HEADER>(stream,FileHeader.Size);

            ushort magic = 0;
            long position = stream.Position;

            try
            {
                magic = Utils.ReadUInt16(stream);
            }
            finally
            {
                stream.Seek(position, SeekOrigin.Begin);
            }

            bool is_32bit = (magic == (ushort)MagicType.PE32);
            bool is_64bit = (magic == (ushort)MagicType.PE32plus);

            if (!is_32bit && !is_64bit)
                throw new ExecutableImageException("Unknown PE type.");

            if ((stream.Position + file_header.SizeOfOptionalHeader) > stream.Length)
            {
                errorMessage = "Cannot read beyond end of stream.";

                return null;
            }

            IMAGE_OPTIONAL_HEADER32 opt_header_32 = new IMAGE_OPTIONAL_HEADER32();
            IMAGE_OPTIONAL_HEADER64 opt_header_64 = new IMAGE_OPTIONAL_HEADER64();
            int data_dir_count = 0;

            if (!is_64bit)
            {
                opt_header_32 = Utils.Read<IMAGE_OPTIONAL_HEADER32>(stream,OptionalHeader.Size32);
                data_dir_count = Convert.ToInt32(opt_header_32.NumberOfRvaAndSizes);
            }
            else
            {
                opt_header_64 = Utils.Read<IMAGE_OPTIONAL_HEADER64>(stream,OptionalHeader.Size64);
                data_dir_count = Convert.ToInt32(opt_header_64.NumberOfRvaAndSizes);
            }

            IMAGE_DATA_DIRECTORY[] data_directories = new IMAGE_DATA_DIRECTORY[data_dir_count];

            for(int i = 0; i < data_dir_count; i++)
                data_directories[i] = Utils.Read<IMAGE_DATA_DIRECTORY>(stream,Utils.SizeOf<IMAGE_DATA_DIRECTORY>());

            long section_table_size = Utils.SizeOf<IMAGE_SECTION_HEADER>() * file_header.NumberOfSections;

            if ((stream.Position + section_table_size) > stream.Length)
            {
                errorMessage = "Cannot read beyond end of stream.";

                return null;
            }

            IMAGE_SECTION_HEADER[] section_table_headers = new IMAGE_SECTION_HEADER[file_header.NumberOfSections];

            for(int i = 0; i < file_header.NumberOfSections; i++)
                section_table_headers[i] = Utils.Read<IMAGE_SECTION_HEADER>(stream,Utils.SizeOf<IMAGE_SECTION_HEADER>());

            foreach(IMAGE_SECTION_HEADER section_header in section_table_headers)
            {
                if ((section_header.PointerToRawData + section_header.SizeOfRawData) > stream.Length)
                {
                    errorMessage = "Section data is beyond end of stream.";

                    return null;
                }

                stream.Seek(section_header.PointerToRawData,SeekOrigin.Begin);
            }

            bool is_clr = false;
            int clr_dir_index = (int)DataDirectoryType.CLRRuntimeHeader;

            if (clr_dir_index < 0 || clr_dir_index > (data_directories.Length - 1))
            {
                IMAGE_DATA_DIRECTORY clr_dir = data_directories[clr_dir_index];

                if (clr_dir.VirtualAddress > 0 && clr_dir.Size > 0)
                    is_clr = true;
            }

            bool is_signed = false;
            int cert_dir_index = (int)DataDirectoryType.CertificateTable;

            if (cert_dir_index < 0 || cert_dir_index > (data_directories.Length - 1))
            {
                IMAGE_DATA_DIRECTORY cert_dir = data_directories[cert_dir_index];

                if (cert_dir.VirtualAddress > 0 && cert_dir.Size > 0)
                    is_signed = true;
            }

            PreloadedInformation info = new PreloadedInformation() {
                DOSHeader = dos_header,
                StubOffset = Convert.ToUInt64(stub_offset),
                StubSize = Convert.ToUInt32(stub_size),
                FileHeader = file_header,
                OptHeader32 = opt_header_32,
                OptHeader64 = opt_header_64,
                DataDirectories = data_directories,
                SectionHeaders = section_table_headers,

                Is32Bit = is_32bit,
                Is64Bit = is_64bit,
                IsCLR = is_clr,
                IsSigned = is_signed
            };

            return info;
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

        public Stream GetStream()
        {
            return _stream;
        }

        public byte[] GetBytes()
        {
            using (var mem = _memory_stream_provider.GetStream())
            {
                _stream.Seek(0,SeekOrigin.Begin);
                _stream.CopyTo(mem,4096);

                return mem.ToArray();
            }
        }

        public LocationCalculator GetCalculator()
        {
            if (_calc == null)
                _calc = new LocationCalculator(this);

            return _calc;
        }

        private void Load()
        {
            string error_message = String.Empty;
            PreloadedInformation preload_info = TryPreload(_stream,out error_message);

            if (preload_info == null)
                throw new ExecutableImageException(error_message);

            ulong image_base = 0;

            if (preload_info.OptHeader32 != null)
                image_base = preload_info.OptHeader32.Value.ImageBase;

            if (preload_info.OptHeader64 != null)
                image_base = preload_info.OptHeader64.Value.ImageBase;

            _dos_header = new DOSHeader(this,preload_info.DOSHeader,image_base);
            _dos_stub = new DOSStub(this,preload_info.StubOffset,preload_info.StubSize,image_base);
           
            FileHeader file_header = new FileHeader(this,preload_info.FileHeader,_dos_stub.Location.FileOffset + _dos_stub.Location.FileSize + 4,image_base);
            OptionalHeader opt_header;

            if (preload_info.Is32Bit)
            {
                opt_header = new OptionalHeader32(this,preload_info.OptHeader32.Value,file_header.Location.FileOffset + file_header.Location.FileSize,image_base);
            }
            else
            {
                opt_header = new OptionalHeader64(this,preload_info.OptHeader64.Value,file_header.Location.FileOffset + file_header.Location.FileSize,image_base);
            }

            DataDirectoryCollection data_dirs = new DataDirectoryCollection(this,opt_header,preload_info.DataDirectories);
            
            _nt_headers = new NTHeaders(this,file_header.Location.FileOffset - 4,image_base,file_header,opt_header,data_dirs);
            _section_table = new SectionTable(this,preload_info.SectionHeaders,_nt_headers.Location.FileOffset + _nt_headers.Location.FileSize,image_base);
            _sections = new Sections(_section_table);

            _is_32bit = preload_info.Is32Bit;
            _is_64bit = preload_info.Is64Bit;
            _is_clr = preload_info.IsCLR;
            _is_signed = preload_info.IsSigned;
        }

        #endregion

        #region Properties

        public IMemoryStreamProvider MemoryStreamProvider
        {
            get
            {
                if (_memory_stream_provider == null)
                    _memory_stream_provider = new DefaultMemoryStreamProvider();

                return _memory_stream_provider;
            }
            set
            {
                _memory_stream_provider = value;
            }
        }

        public bool Is32Bit
        {
            get
            {
                return _is_32bit;
            }
        }

        public bool Is64Bit
        {
            get
            {
                return _is_64bit;
            }
        }

        public bool IsCLR
        {
            get
            {
                return _is_clr;
            }
        }

        public bool IsSigned
        {
            get
            {
                return _is_signed;
            }
        }

        public DOSHeader DOSHeader
        {
            get
            {
                return _dos_header;
            }
        }

        public DOSStub DOSStub
        {
            get
            {
                return _dos_stub;
            }
        }

        public NTHeaders NTHeaders
        {
            get
            {
                return _nt_headers;
            }
        }

        public SectionTable SectionTable
        {
            get
            {
                return _section_table;
            }
        }

        public Sections Sections
        {
            get
            {
                return _sections;
            }
        }

        #endregion

    }

}
