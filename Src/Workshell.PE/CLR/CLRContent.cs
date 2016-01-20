using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    internal class CLRContentProvider : ISectionContentProvider
    {

        #region Methods

        public SectionContent Create(DataDirectory directory, Section section)
        {
            return new CLRContent(directory,section);
        }

        #endregion

        #region Properties

        public DataDirectoryType DirectoryType
        {
            get
            {
                return DataDirectoryType.CLRRuntimeHeader;
            }
        }

        #endregion

    }

    public class CLRContent : SectionContent
    {

        private StreamLocation location;
        private CLRHeader header;

        internal CLRContent(DataDirectory dataDirectory, Section section) : base(dataDirectory,section)
        {
            long offset = section.RVAToOffset(dataDirectory.VirtualAddress);
            location = new StreamLocation(offset,dataDirectory.Size);

            Stream stream = Section.Sections.Reader.Stream;

            LoadHeader(stream);
        }

        #region Methods

        private void LoadHeader(Stream stream)
        {
            stream.Seek(location.Offset,SeekOrigin.Begin);

            IMAGE_COR20_HEADER native_clr_header = Utils.Read<IMAGE_COR20_HEADER>(stream);
            header = new CLRHeader(this,location.Offset,native_clr_header);
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

        public CLRHeader Header
        {
            get
            {
                return header;
            }
        }

        #endregion

    }

}
