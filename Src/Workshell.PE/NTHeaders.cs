using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class NTHeaders
    {

        public const uint PE_MAGIC_MZ = 17744;

        private ImageReader reader;
        private Location location;
        private FileHeader file_header;
        private OptionalHeader opt_header;
        private DataDirectories data_dirs;

        internal NTHeaders(ImageReader exeReader, ulong headerOffset, ulong imageBase, FileHeader fileHeader, OptionalHeader optHeader, DataDirectories dataDirs)
        {
            uint size = 4 + fileHeader.Location.FileSize + optHeader.Location.FileSize + dataDirs.Location.FileSize;

            reader = exeReader;
            location = new Location(headerOffset,Convert.ToUInt32(headerOffset),imageBase + headerOffset,size,size);
            file_header = fileHeader;
            opt_header = optHeader;
            data_dirs = dataDirs;
        }

        #region Methods

        public override string ToString()
        {
            return "NT Headers";
        }

        public byte[] GetBytes()
        {
            return null;
        }

        #endregion

        #region Properties

        public Location Location
        {
            get
            {
                return location;
            }
        }

        public FileHeader FileHeader
        {
            get
            {
                return file_header;
            }
        }

        public OptionalHeader OptionalHeader
        {
            get
            {
                return opt_header;
            }
        }

        public DataDirectories DataDirectories
        {
            get
            {
                return data_dirs;
            }
        }

        #endregion

    }

}
