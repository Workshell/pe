using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public class ImportTableContent : DataDirectoryContent
    {

        private ulong image_base;
        private ImportDirectory dir;

        internal ImportTableContent(DataDirectory dataDirectory, ulong imageBase) : base(dataDirectory,imageBase)
        {
            image_base = imageBase;

            LocationCalculator calc = DataDirectory.Directories.Reader.GetCalculator();
            Stream stream = DataDirectory.Directories.Reader.GetStream();

            LoadDirectory(calc,stream);
        }

        #region Methods

        private void LoadDirectory(LocationCalculator calc, Stream stream)
        {
            List<IMAGE_IMPORT_DESCRIPTOR> descriptors = new List<IMAGE_IMPORT_DESCRIPTOR>();
            Section section = calc.RVAToSection(DataDirectory.VirtualAddress);
            ulong offset = calc.RVAToOffset(section,DataDirectory.VirtualAddress);
            int size = Utils.SizeOf<IMAGE_IMPORT_DESCRIPTOR>();

            stream.Seek(Convert.ToInt64(offset),SeekOrigin.Begin);

            while (true)
            {
                IMAGE_IMPORT_DESCRIPTOR descriptor = Utils.Read<IMAGE_IMPORT_DESCRIPTOR>(stream,size);

                if (descriptor.OriginalFirstThunk == 0 && descriptor.FirstThunk == 0)
                    break;

                descriptors.Add(descriptor);
            }

            uint total_size = Convert.ToUInt32((descriptors.Count + 1) * size);
            Location location = new Location(offset,DataDirectory.VirtualAddress,image_base + DataDirectory.VirtualAddress,total_size,total_size);

            dir = new ImportDirectory(this,location,section,descriptors,image_base);
        }

        #endregion

        #region Properties

        public ImportDirectory Directory
        {
            get
            {
                return dir;
            }
        }

        #endregion

    }

}
