using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class LoadConfigTableContent : DataDirectoryContent
    {

        private LoadConfigDirectory directory;

        internal LoadConfigTableContent(DataDirectory dataDirectory, ulong imageBase) : base(dataDirectory, imageBase)
        {
            LocationCalculator calc = DataDirectory.Directories.Reader.GetCalculator();
            Stream stream = DataDirectory.Directories.Reader.GetStream();

            LoadDirectory(calc, stream, imageBase);
        }

        #region Methods

        private void LoadDirectory(LocationCalculator calc, Stream stream, ulong imageBase)
        {
            bool is_64bit = DataDirectory.Directories.Reader.Is64Bit;
            Section section = calc.RVAToSection(DataDirectory.VirtualAddress);
            ulong offset = calc.RVAToOffset(section, DataDirectory.VirtualAddress);
            Location location = new Location(offset, DataDirectory.VirtualAddress, imageBase + DataDirectory.VirtualAddress, DataDirectory.Size, DataDirectory.Size);

            stream.Seek(Convert.ToInt64(offset), SeekOrigin.Begin);

            if (!is_64bit)
            {
                IMAGE_LOAD_CONFIG_DIRECTORY32 config_dir = Utils.Read<IMAGE_LOAD_CONFIG_DIRECTORY32>(stream);

                directory = new LoadConfigDirectory32(this, location, section, config_dir);
            }
            else
            {
                IMAGE_LOAD_CONFIG_DIRECTORY64 config_dir = Utils.Read<IMAGE_LOAD_CONFIG_DIRECTORY64>(stream);

                directory = new LoadConfigDirectory64(this, location, section, config_dir);
            }
        }

        #endregion

        #region Properties

        public LoadConfigDirectory Directory
        {
            get
            {
                return directory;
            }
        }

        #endregion

    }

}
