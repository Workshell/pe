using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class CLRMetaData : ILocationSupport
    {

        private CLRContent content;
        private StreamLocation location;

        internal CLRMetaData(Stream stream, CLRContent clrContent)
        {
            long offset = Convert.ToInt64(clrContent.Section.RVAToOffset(clrContent.Header.MetaData.VirtualAddress));

            stream.Seek(offset,SeekOrigin.Begin);

            content = clrContent;
            location = new StreamLocation(offset,0);
        }

        #region Properties

        public StreamLocation Location
        {
            get
            {
                return location;
            }
        }

        #endregion

    }

}
