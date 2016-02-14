using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class DebugContent : DataDirectoryContent
    {

        private Section section;
        private DebugDirectoryCollection directories;
        private DebugDataCollection data;

        internal DebugContent(DataDirectory dataDirectory, ulong imageBase) : base(dataDirectory,imageBase)
        {
            LocationCalculator calc = dataDirectory.Directories.Reader.GetCalculator();
            Stream stream = dataDirectory.Directories.Reader.GetStream();

            section = calc.RVAToSection(dataDirectory.VirtualAddress);

            LoadDirectories(calc,stream,imageBase);
            LoadData(calc,stream,imageBase);
        }

        #region Methods

        private void LoadDirectories(LocationCalculator calc, Stream stream, ulong imageBase)
        {
            ulong offset = calc.RVAToOffset(DataDirectory.VirtualAddress);
            Location location = new Location(offset, DataDirectory.VirtualAddress, imageBase + DataDirectory.VirtualAddress, DataDirectory.Size, DataDirectory.Size);
            uint size = Convert.ToUInt32(Utils.SizeOf<IMAGE_DEBUG_DIRECTORY>());
            long count = DataDirectory.Size / size;
            List<Tuple<ulong,IMAGE_DEBUG_DIRECTORY>> dirs = new List<Tuple<ulong,IMAGE_DEBUG_DIRECTORY>>();

            stream.Seek(Convert.ToInt64(offset), SeekOrigin.Begin);

            for(var i = 0; i < count; i++)
            {
                IMAGE_DEBUG_DIRECTORY entry = Utils.Read<IMAGE_DEBUG_DIRECTORY>(stream, Convert.ToInt32(size));

                dirs.Add(new Tuple<ulong, IMAGE_DEBUG_DIRECTORY>(offset,entry));

                offset += size;
            }

            directories = new DebugDirectoryCollection(this,location,dirs);
        }

        private void LoadData(LocationCalculator calc, Stream stream, ulong imageBase)
        {
            data = new DebugDataCollection(this,imageBase);
        }

        #endregion

        #region Properties

        public Section Section
        {
            get
            {
                return section;
            }
        }

        public DebugDirectoryCollection Directories
        {
            get
            {
                return directories;
            }
        }

        public DebugDataCollection Data
        {
            get
            {
                return data;
            }
        }

        #endregion

    }

}
