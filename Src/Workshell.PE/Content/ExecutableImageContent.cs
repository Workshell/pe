using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public abstract class ExecutableImageContent : ISupportsLocation
    {

        private DataDirectory directory;
        private Location location;

        public ExecutableImageContent(DataDirectory dataDirectory, Location dataLocation)
        {
            directory = dataDirectory;
            location = dataLocation;
        }

        #region Properties

        public DataDirectory DataDirectory
        {
            get
            {
                return directory;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        #endregion

    }

}
