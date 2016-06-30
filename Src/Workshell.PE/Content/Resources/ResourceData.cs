using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class ResourceData : ExecutableImageContent, ISupportsBytes
    {

        private ResourceDataEntry data_entry;

        internal ResourceData(DataDirectory dataDirectory, Location dataLocation, ResourceDataEntry dataEntry) : base(dataDirectory,dataLocation)
        {
            data_entry = dataEntry;
        }

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,Location);

            return buffer;
        }

        #endregion

        #region Properties

        public ResourceDataEntry DataEntry
        {
            get
            {
                return data_entry;
            }
        }

        #endregion

    }

}
