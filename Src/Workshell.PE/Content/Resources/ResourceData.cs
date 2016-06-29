using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class ResourceData : ISupportsLocation, ISupportsBytes
    {

        private ResourceDataEntry data_entry;
        private Location location;

        internal ResourceData(ResourceDataEntry dataEntry, ulong imageBase)
        {
            LocationCalculator calc = dataEntry.DirectoryEntry.Directory.Resources.DataDirectory.Directories.Image.GetCalculator();
            ulong va = imageBase + dataEntry.OffsetToData;
            ulong file_offset = calc.VAToOffset(va);

            data_entry = dataEntry;
            location = new Location(file_offset,dataEntry.OffsetToData,va,dataEntry.Size,dataEntry.Size);
        }

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = data_entry.DirectoryEntry.Directory.Resources.DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

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
