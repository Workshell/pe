using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class Resources : ExecutableImageContent, ISupportsBytes
    {

        private ResourceDirectory root_directory;

        internal Resources(DataDirectory dataDirectory, Location dataLocation, ulong rootOffset) : base(dataDirectory, dataLocation)
        {
            root_directory = new ResourceDirectory(this, rootOffset, null);
        }

        #region Static Methods

        public static Resources Get(ExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.ResourceTable))
                return null;

            DataDirectory directory = image.NTHeaders.DataDirectories[DataDirectoryType.ResourceTable];

            if (DataDirectory.IsNullOrEmpty(directory))
                return null;

            LocationCalculator calc = directory.Directories.Image.GetCalculator();
            Section section = calc.RVAToSection(directory.VirtualAddress);
            ulong file_offset = calc.RVAToOffset(section, directory.VirtualAddress);
            ulong image_base = directory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            Location location = new Location(file_offset, directory.VirtualAddress, image_base + directory.VirtualAddress, directory.Size, directory.Size, section);
            Resources result = new Resources(directory, location, file_offset);

            return result;
        }

        #endregion

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream, Location);

            return buffer;
        }

        #endregion

        #region Properties

        public ResourceDirectory Root
        {
            get
            {
                return root_directory;
            }
        }

        #endregion

    }

}
