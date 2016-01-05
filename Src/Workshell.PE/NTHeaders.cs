using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class NTHeaders : ILocatable
    {

        private PortableExecutable exe;
        private FileHeader file_header;
        private OptionalHeader optional_header;
        private StreamLocation location;

        internal NTHeaders(Stream stream, PortableExecutable portableExecutable)
        {
            exe = portableExecutable;

            LoadSignature(stream);
            
            file_header = LoadFileHeader(stream);
            optional_header = LoadOptionalHeader(stream,file_header);

            location = new StreamLocation(exe.DOSHeader.FileAddressNewHeader,4 + file_header.Location.Size + optional_header.Location.Size);
        }

        #region Methods

        private void LoadSignature(Stream stream)
        {
            long offset = exe.DOSStub.Location.Offset + exe.DOSStub.Location.Size;
            byte[] signature = new byte[4];
            int sig_read = stream.Read(signature,0,signature.Length);

            if (sig_read < signature.Length)
                throw new PortableExecutableException("Could not read NT header from stream.");

            if (signature[0] != 80 && signature[1] != 69)
                throw new PortableExecutableException("Incorrect signature specified in the NT header.");
        }

        private FileHeader LoadFileHeader(Stream stream)
        {
            long offset = (exe.DOSStub.Location.Offset + exe.DOSStub.Location.Size) + 4;
            FileHeader result = new FileHeader(stream,offset);

            return result;
        }

        private OptionalHeader LoadOptionalHeader(Stream stream, FileHeader fileHeader)
        {
            long offset = fileHeader.Location.Offset + fileHeader.Location.Size;
            OptionalHeader header;

            if ((fileHeader.GetCharacteristics() & CharacteristicsType.Supports32Bit) == CharacteristicsType.Supports32Bit)
            {
                header = new OptionalHeader32(stream,offset);
            }
            else
            {
                header = new OptionalHeader64(stream,offset);
            }

            return header;
        }

        #endregion

        #region Properties

        public PortableExecutable Executable
        {
            get
            {
                return exe;
            }
        }

        public StreamLocation Location
        {
            get
            {
                return location;
            }
        }

        public FileHeader FileHeader
        {
            get
            {
                return file_header;
            }
        }

        public OptionalHeader OptionalHeader
        {
            get
            {
                return optional_header;
            }
        }

        #endregion

    }

}
