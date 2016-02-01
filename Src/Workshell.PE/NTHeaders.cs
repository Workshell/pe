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

        internal NTHeaders(ImageReader exeReader, ulong headerOffset, ulong imageBase, FileHeader fileHeader, OptionalHeader optHeader)
        {
            uint size = fileHeader.Location.FileSize + optHeader.Location.FileSize;

            reader = exeReader;
            location = new Location(headerOffset,Convert.ToUInt32(headerOffset),imageBase + headerOffset,size,size);
            file_header = fileHeader;
            opt_header = optHeader;
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

        #endregion

    }

}
