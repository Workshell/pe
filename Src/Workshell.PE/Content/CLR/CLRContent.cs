using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

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
