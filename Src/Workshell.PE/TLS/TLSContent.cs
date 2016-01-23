using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    internal class TLSContentProvider : ISectionContentProvider
    {

        #region Methods

        public SectionContent Create(DataDirectory directory, Section section)
        {
            return new TLSContent(directory,section);
        }

        #endregion

        #region Properties

        public DataDirectoryType DirectoryType
        {
            get
            {
                return DataDirectoryType.TLSTable;
            }
        }

        #endregion

    }

    public class TLSContent : SectionContent, ILocationSupport, IRawDataSupport
    {

        private StreamLocation location;
        private TLSDirectory directory;

        internal TLSContent(DataDirectory dataDirectory, Section section) : base(dataDirectory,section)
        {
            long offset = section.RVAToOffset(dataDirectory.VirtualAddress);
            location = new StreamLocation(offset,dataDirectory.Size);
        }

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = Section.Sections.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        private TLSDirectory LoadDirectory()
        {
            TLSDirectory result;
            Stream stream = Section.Sections.Reader.GetStream();

            stream.Seek(location.Offset,SeekOrigin.Begin);

            if (Section.Sections.Reader.Is32Bit)
            {
                IMAGE_TLS_DIRECTORY32 tls_directory = Utils.Read<IMAGE_TLS_DIRECTORY32>(stream,TLSDirectory.Size32);
                result = new TLSDirectory32(this,location.Offset,tls_directory);
            }
            else
            {
                IMAGE_TLS_DIRECTORY64 tls_directory = Utils.Read<IMAGE_TLS_DIRECTORY64>(stream,TLSDirectory.Size64);
                result = new TLSDirectory64(this,location.Offset,tls_directory);
            }

            return result;
        }

        #endregion

        #region Properties

        public StreamLocation Location
        {
            get
            {
                return location;
            }
        }

        public TLSDirectory Directory
        {
            get
            {
                if (directory == null)
                    directory = LoadDirectory();

                return directory;
            }
        }

        #endregion

    }

}

