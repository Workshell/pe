using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class DebugData : ISupportsLocation, ISupportsBytes
    {

        private DebugDataCollection collection;
        private DebugDirectory directory;
        private Location location;

        internal DebugData(DebugDataCollection dataCollection, DebugDirectory dataDirectory, Location dataLocation)
        {
            collection = dataCollection;
            directory = dataDirectory;
            location = dataLocation;
        }

        #region Methods

        public override string ToString()
        {
            string type = directory.GetDirectoryType().ToString();

            return String.Format("Debug Type: {0}, File Offset: 0x{1:X8}, Size: 0x{2:X8}",type,location.FileOffset,location.FileSize);
        }

        public byte[] GetBytes()
        {
            Stream stream = collection.Content.DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream, location);

            return buffer;
        }

        #endregion

        #region Properties

        public DebugDataCollection Collection
        {
            get
            {
                return collection;
            }
        }

        public DebugDirectory Directory
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

    public sealed class DebugDataCollection : IEnumerable<DebugData>, IReadOnlyList<DebugData>, IReadOnlyCollection<DebugData>
    {

        private DebugContent content;
        private Section section;
        private List<DebugData> debug_data;

        internal DebugDataCollection(DebugContent debugContent, Section debugSection, ulong imageBase)
        {
            content = debugContent;
            section = debugSection;
            debug_data = new List<DebugData>();

            Load(imageBase);
        }

        #region Methods

        public IEnumerator<DebugData> GetEnumerator()
        {
            return debug_data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("Debug Data Count: {0}", debug_data.Count);
        }

        private void Load(ulong imageBase)
        {
            foreach(DebugDirectory directory in content.Directories)
            {
                Location location = new Location(directory.PointerToRawData,directory.AddressOfRawData,imageBase + directory.AddressOfRawData,directory.SizeOfData,directory.SizeOfData);
                DebugData data = new DebugData(this,directory,location);

                debug_data.Add(data);
            }

            debug_data = debug_data.OrderBy(data => data.Location.FileOffset).ToList();
        }

        #endregion

        #region Properties

        public DebugContent Content
        {
            get
            {
                return content;
            }
        }

        public Section Section
        {
            get
            {
                return section;
            }
        }

        public int Count
        {
            get
            {
                return debug_data.Count;
            }
        }

        public DebugData this[int index]
        {
            get
            {
                return this[index];
            }
        }

        #endregion

    }

}
