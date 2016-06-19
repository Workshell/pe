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

        internal CLRMetaData(CLR clr, Location location, CLRHeader header)
        {
            CLR = clr;
            Location = location;
            Header = new CLRMetaDataHeader(this);
            StreamTable = new CLRMetaDataStreamTable(this);
            Streams = new CLRMetaDataStreams(this);
        }

        #region Static Methods

        public static CLRMetaData Get(CLRHeader header)
        {
            LocationCalculator calc = header.CLR.DataDirectory.Directories.Image.GetCalculator();
            ulong image_base = header.CLR.DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            uint rva = header.MetaDataAddress;
            ulong va = image_base + rva;
            ulong offset = calc.RVAToOffset(rva);
            uint size = header.MetaDataSize;
            Section section = calc.RVAToSection(rva);
            Location location = new Location(offset, rva, va, size, size, section);
            CLRMetaData meta_data = new CLRMetaData(header.CLR, location, header);

            return meta_data;
        }

        #endregion

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = CLR.DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,Location);

            return buffer;
        }

        #endregion

        #region Properties

        public CLR CLR
        {
            get;
            private set;
        }

        public Location Location
        {
            get;
            private set;
        }

        public CLRMetaDataHeader Header
        {
            get;
            private set;
        }

        public CLRMetaDataStreamTable StreamTable
        {
            get;
            private set;
        }

        public CLRMetaDataStreams Streams
        {
            get;
            private set;
        }

        #endregion

    }

}
