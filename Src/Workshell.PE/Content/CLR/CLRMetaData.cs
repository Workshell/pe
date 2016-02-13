using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public sealed class CLRMetaData : ISupportsLocation, ISupportsBytes
    {

        private CLRContent content;
        private Location location;
        private Section section;
        private ulong image_base;
        private CLRMetaDataHeader header;
        private CLRMetaDataStreamTable stream_table;
        private CLRMetaDataStreamCollection streams;

        internal CLRMetaData(CLRContent mdContent, ulong imageBase)
        {
            LocationCalculator calc = mdContent.DataDirectory.Directories.Reader.GetCalculator();
            uint rva = mdContent.Header.MetaDataAddress;
            ulong va = imageBase + rva;
            ulong offset = calc.RVAToOffset(rva);
            uint size = mdContent.Header.MetaDataSize;

            content = mdContent;
            location = new Location(offset,rva,va,size,size);
            section = calc.RVAToSection(rva);
            image_base = imageBase;
            header = new CLRMetaDataHeader(this,imageBase);
            stream_table = new CLRMetaDataStreamTable(this,imageBase);
            streams = new CLRMetaDataStreamCollection(this,imageBase);
        }

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = content.DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        #endregion

        #region Properties

        public CLRContent Content
        {
            get
            {
                return content;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        public Section Section
        {
            get
            {
                return section;
            }
        }

        public CLRMetaDataHeader Header
        {
            get
            {
                return header;
            }
        }

        public CLRMetaDataStreamTable StreamTable
        {
            get
            {
                return stream_table;
            }
        }

        public CLRMetaDataStreamCollection Streams
        {
            get
            {
                return streams;
            }
        }

        #endregion

    }

}
