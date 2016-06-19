using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class CLR : ExecutableImageContent
    {

        internal CLR(DataDirectory dataDirectory, Location dataLocation) : base(dataDirectory,dataLocation)
        {
        }

        #region Static Methods

        public static CLR Get(ExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.CLRRuntimeHeader))
                return null;

            DataDirectory directory = image.NTHeaders.DataDirectories[DataDirectoryType.CLRRuntimeHeader];

            if (DataDirectory.IsNullOrEmpty(directory))
                return null;

            LocationCalculator calc = directory.Directories.Image.GetCalculator();
            Section section = calc.RVAToSection(directory.VirtualAddress);
            ulong file_offset = calc.RVAToOffset(section, directory.VirtualAddress);
            ulong image_base = directory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;         
            Location location = new Location(file_offset, directory.VirtualAddress, image_base + directory.VirtualAddress, directory.Size, directory.Size, section);
            CLR result = new CLR(directory, location);

            return result;
        }

        /*
        public static CLRMetaData GetMetaData(CLRHeader header)
        {
            LocationCalculator calc = header.CLR.DataDirectory.Directories.Image.GetCalculator();
            ulong image_base = header.CLR.DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            uint rva = header.MetaDataAddress;
            ulong va = image_base + rva;
            ulong offset = calc.RVAToOffset(rva);
            uint size = header.MetaDataSize;
            Section section = calc.RVAToSection(rva);
            Location location = new Location(offset, rva, va, size, size, section);

            //CLRMetaDataHeader meta_data_header = new CLRMetaDataHeader(this, header);
            //CLRMetaDataStreamTable stream_table = new CLRMetaDataStreamTable(this, imageBase);
            //CLRMetaDataStreamCollection streams = new CLRMetaDataStreamCollection(this, imageBase);
            CLRMetaData meta_data = new CLRMetaData(header.CLR, location, header);

            return meta_data;
        }
        */

        #endregion

    }

    /*

    public sealed class CLRContent : DataDirectoryContent
    {

        private CLRHeader header;
        private CLRMetaData meta_data;

        internal CLRContent(DataDirectory dataDirectory, ulong imageBase) : base(dataDirectory, imageBase)
        {
            LocationCalculator calc = DataDirectory.Directories.Reader.GetCalculator();
            Stream stream = DataDirectory.Directories.Reader.GetStream();

            LoadHeader(calc, stream, imageBase);
            LoadMetaData(calc, stream, imageBase);
        }

        #region Methods

        private void LoadHeader(LocationCalculator calc, Stream stream, ulong imageBase)
        {
            ulong offset = calc.RVAToOffset(DataDirectory.VirtualAddress);
            uint size = Convert.ToUInt32(Utils.SizeOf<IMAGE_COR20_HEADER>());
            Location location = new Location(offset,DataDirectory.VirtualAddress,imageBase + DataDirectory.VirtualAddress,size,size);
            Section section = calc.RVAToSection(DataDirectory.VirtualAddress);

            stream.Seek(Convert.ToInt64(offset),SeekOrigin.Begin);

            IMAGE_COR20_HEADER clr_header = Utils.Read<IMAGE_COR20_HEADER>(stream,Convert.ToInt32(size));

            header = new CLRHeader(this,clr_header,location,section);
        }

        private void LoadMetaData(LocationCalculator calc, Stream stream, ulong imageBase)
        {
            meta_data = new CLRMetaData(this,imageBase);
        }

        #endregion

        #region Properties

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
                return meta_data;
            }
        }

        #endregion

    }

    */

}
