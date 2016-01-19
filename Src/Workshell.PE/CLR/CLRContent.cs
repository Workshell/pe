using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public class CLRContent : SectionContent
    {

        private CLRHeader header;

        internal CLRContent(Stream stream, DataDirectory dataDirectory, Section owningSection) : base(dataDirectory,owningSection)
        {
            //long offset = Convert.ToInt64(owningSection.RVAToOffset(dataDirectory.VirtualAddress));
            long offset = 0;

            stream.Seek(offset,SeekOrigin.Begin);

            header = new CLRHeader(stream,this,offset);
        }

        #region Methods

        #endregion

        #region Properties

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
