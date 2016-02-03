using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public abstract class DataDirectoryContent
    {

        private DataDirectory data_dir;

        public DataDirectoryContent(DataDirectory dataDirectory, ulong imageBase)
        {
            data_dir = dataDirectory;
        }

        #region Properties

        public DataDirectory DataDirectory
        {
            get
            {
                return data_dir;
            }
        }

        #endregion

    }

}
