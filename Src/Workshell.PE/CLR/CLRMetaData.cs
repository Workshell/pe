using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Attributes;

namespace Workshell.PE
{

    public class CLRMetaData : ILocationSupport
    {

        private CLRContent content;
        private StreamLocation location;
        private CLRMetaDataHeader header;
        private CLRMetaDataStreamTable stream_table;
        private CLRMetaDataStreams streams;

        internal CLRMetaData(CLRContent clrContent)
        {
            long offset = clrContent.Section.RVAToOffset(clrContent.Header.MetaDataAddress);

            content = clrContent;
            location = new StreamLocation(offset,clrContent.Header.MetaDataSize);
            header = null;
            stream_table = null;
            streams = null;
        }

        #region Properties

        public CLRContent Content
        {
            get
            {
                return content;
            }
        }

        public StreamLocation Location
        {
            get
            {
                return location;
            }
        }

        public CLRMetaDataHeader Header
        {
            get
            {
                if (header == null)
                    header = new CLRMetaDataHeader(this);

                return header;
            }
        }

        public CLRMetaDataStreamTable StreamTable
        {
            get
            {
                if (stream_table == null)
                    stream_table = new CLRMetaDataStreamTable(this);

                return stream_table;
            }
        }

        public CLRMetaDataStreams Streams
        {
            get
            {
                if (streams == null)
                    streams = new CLRMetaDataStreams(this);

                return streams;
            }
        }

        #endregion

    }

}
