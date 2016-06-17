using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;

namespace Workshell.PE
{

    public class NTHeaders : ISupportsLocation, ISupportsBytes
    {

        public const uint PE_MAGIC_MZ = 17744;

        private ExecutableImage image;
        private Location location;
        private FileHeader file_header;
        private OptionalHeader opt_header;
        private DataDirectoryCollection data_dirs;

        internal NTHeaders(ExecutableImage exeImage, ulong headerOffset, ulong imageBase, FileHeader fileHeader, OptionalHeader optHeader, DataDirectoryCollection dataDirs)
        {
            uint size = (4U + fileHeader.Location.FileSize + optHeader.Location.FileSize + dataDirs.Location.FileSize).ToUInt32();

            image = exeImage;
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
            Stream stream = image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        #endregion

        #region Properties

        public ExecutableImage Image
        {
            get
            {
                return image;
            }
        }

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

        public DataDirectoryCollection DataDirectories
        {
            get
            {
                return data_dirs;
            }
        }

        #endregion

    }

}
