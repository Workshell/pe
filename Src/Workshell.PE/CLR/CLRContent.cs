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
        private CLRMetaData meta_data;

        internal CLRContent(DataDirectory dataDirectory, Section section) : base(dataDirectory,section)
        {
            long offset = section.RVAToOffset(dataDirectory.VirtualAddress);

            location = new StreamLocation(offset,dataDirectory.Size);
            header = new CLRHeader(this,dataDirectory);
            meta_data = null;
        }

        #region Methods

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

        public CLRMetaData MetaData
        {
            get
            {
                if (meta_data == null)
                    meta_data = new CLRMetaData(this);

                return meta_data;
            }
        }

        #endregion

    }

}
