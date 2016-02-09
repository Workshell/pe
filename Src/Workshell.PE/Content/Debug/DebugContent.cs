﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public class DebugContent : DataDirectoryContent
    {

        private ulong image_base;
        private DebugDirectories directories;

        internal DebugContent(DataDirectory dataDirectory, ulong imageBase) : base(dataDirectory,imageBase)
        {
            image_base = imageBase;

            LocationCalculator calc = DataDirectory.Directories.Reader.GetCalculator();
            Stream stream = DataDirectory.Directories.Reader.GetStream();

            LoadDirectories(calc, stream, imageBase);
        }

        #region Methods

        private void LoadDirectories(LocationCalculator calc, Stream stream, ulong imageBase)
        {
            ulong offset = calc.RVAToOffset(DataDirectory.VirtualAddress);
            Section section = calc.RVAToSection(DataDirectory.VirtualAddress);
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

            directories = new DebugDirectories(this, location, section, dirs);
        }

        #endregion

        #region Properties

        public DebugDirectories Directories
        {
            get
            {
                return directories;
            }
        }

        #endregion

    }

}
