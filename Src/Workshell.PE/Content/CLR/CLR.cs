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

        private CLRHeader header;
        private CLRMetaData meta_data;

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

        #endregion

        #region Properties

        public CLRHeader Header
        {
            get
            {
                if (header == null)
                    header = CLRHeader.Get(this);

                return header;
            }
        }

        public CLRMetaData MetaData
        {
            get
            {
                if (meta_data == null)
                    meta_data = CLRMetaData.Get(Header);

                return meta_data;
            }
        }

        #endregion

    }

}
