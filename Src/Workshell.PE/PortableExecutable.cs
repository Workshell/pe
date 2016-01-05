using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class PortableExecutable : IDisposable
    {

        private bool disposed;
        private Stream stream;
        private bool own_stream;
        private string file_name;
        private DOSHeader dos_header;
        private DOSStub dos_stub;
        private NTHeaders nt_headers;
        private SectionTable section_table;
        private Sections sections;

        private PortableExecutable(Stream stream, bool ownStream, string fileName)
        {
            this.disposed = false;
            this.stream = stream;
            this.own_stream = ownStream;
            this.file_name = fileName;

            dos_header = new DOSHeader(this,0);
            dos_stub = new DOSStub(this);
            nt_headers = new NTHeaders(stream,this);
            section_table = new SectionTable(stream,this);
            sections = new Sections(this,section_table);

            LoadRelocationTable();
            LoadCLRData();
        }

        #region Static Methods

        public static PortableExecutable Open(string fileName)
        {
            FileStream file = new FileStream(fileName,FileMode.Open,FileAccess.Read);

            return new PortableExecutable(file,true,fileName);
        }

        public static PortableExecutable Open(byte[] bytes, string fileName)
        {
            MemoryStream mem = new MemoryStream(bytes);

            return new PortableExecutable(mem,true,fileName);
        }

        public static PortableExecutable Open(Stream stream, string fileName)
        {
            return Open(stream,false,fileName);
        }

        public static PortableExecutable Open(Stream stream, bool ownStream, string fileName)
        {
            return new PortableExecutable(stream,ownStream,fileName);
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if (!disposed)
            {
                if (own_stream)
                    stream.Close();

                disposed = true;
            }
        }

        private void LoadRelocationTable()
        {
            DataDirectory directory = nt_headers.OptionalHeader.DataDirectories[DataDirectoryType.BaseRelocationTable];

            if (DataDirectory.IsNullOrEmpty(directory))
                return;

            Section section = sections.RVAToSection(directory.VirtualAddress);

            if (section == null)
                throw new PortableExecutableException(); 

            RelocationContent content = new RelocationContent(stream,directory,section);

            section.AttachContent(content);
        }

        private void LoadCLRData()
        {
            DataDirectory directory = nt_headers.OptionalHeader.DataDirectories[DataDirectoryType.CLRRuntimeHeader];

            if (DataDirectory.IsNullOrEmpty(directory))
                return;

            Section section = sections.RVAToSection(directory.VirtualAddress);

            if (section == null)
                throw new PortableExecutableException(); 

            CLRContent content = new CLRContent(stream,directory,section);

            section.AttachContent(content);
        }

        #endregion

        #region Properties

        public Stream Stream
        {
            get
            {
                return stream;
            }
        }

        public bool OwnStream
        {
            get
            {
                return own_stream;
            }
        }

        public string Filename
        {
            get
            {
                return file_name;
            }
        }

        public bool Is32Bit
        {
            get
            {
                bool result = false;

                if (nt_headers.FileHeader != null)
                {
                    CharacteristicsType characteristics = nt_headers.FileHeader.GetCharacteristics();

                    result = ((characteristics & CharacteristicsType.Supports32Bit) == CharacteristicsType.Supports32Bit);
                }

                if (nt_headers.OptionalHeader != null)
                    result = (nt_headers.OptionalHeader.GetMagic() == MagicType.PE32);

                return result;
            }
        }

        public bool Is64Bit
        {
            get
            {
                bool result = false;

                if (nt_headers.FileHeader != null)
                {
                    CharacteristicsType characteristics = nt_headers.FileHeader.GetCharacteristics();

                    result = ((characteristics & CharacteristicsType.Supports32Bit) != CharacteristicsType.Supports32Bit);
                }

                if (nt_headers.OptionalHeader != null)
                    result = (nt_headers.OptionalHeader.GetMagic() == MagicType.PE32plus);

                return result;
            }
        }

        public DOSHeader DOSHeader
        {
            get
            {
                return dos_header;
            }
        }

        public DOSStub DOSStub
        {
            get
            {
                return dos_stub;
            }
        }

        public NTHeaders NTHeaders
        {
            get
            {
                return nt_headers;
            }
        }

        public SectionTable SectionTable
        {
            get
            {
                return section_table;
            }
        }

        public Sections Sections
        {
            get
            {
                return sections;
            }
        }

        #endregion

    }

}
