using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class CLRDataDirectory
    {

        private IMAGE_DATA_DIRECTORY data_dir;

        internal CLRDataDirectory(IMAGE_DATA_DIRECTORY dataDirectory)
        {
            data_dir = dataDirectory;
        }

        #region Static Methods

        public static bool IsNullOrEmpty(DataDirectory dataDirectory)
        {
            if (dataDirectory == null)
                return true;

            if (dataDirectory.VirtualAddress == 0)
                return true;

            if (dataDirectory.Size == 0)
                return true;

            return false;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return String.Format("0x{0:X8}+{1}",data_dir.VirtualAddress,data_dir.Size);
        }

        #endregion

        #region Properties

        public uint VirtualAddress
        {
            get
            {
                return data_dir.VirtualAddress;
            }
        }

        public uint Size
        {
            get
            {
                return data_dir.Size;
            }
        }

        #endregion

    }

}
