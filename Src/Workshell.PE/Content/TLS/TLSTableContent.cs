using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class TLSTableContent : DataDirectoryContent
    {

        private Section section;
        private TLSDirectory directory;

        internal TLSTableContent(DataDirectory dataDirectory, ulong imageBase) : base(dataDirectory, imageBase)
        {
            LocationCalculator calc = DataDirectory.Directories.Reader.GetCalculator();
            Stream stream = DataDirectory.Directories.Reader.GetStream();

            section = calc.RVAToSection(dataDirectory.VirtualAddress);

            LoadDirectory(calc, stream, imageBase);
        }

        #region Methods

        private void LoadDirectory(LocationCalculator calc, Stream stream, ulong imageBase)
        {
            bool is_64bit = DataDirectory.Directories.Reader.Is64Bit;
            ulong offset = calc.RVAToOffset(section, DataDirectory.VirtualAddress);
            Location location = new Location(offset, DataDirectory.VirtualAddress, imageBase + DataDirectory.VirtualAddress, DataDirectory.Size, DataDirectory.Size);

            stream.Seek(Convert.ToInt64(offset), SeekOrigin.Begin);

            if (!is_64bit)
            {
                IMAGE_TLS_DIRECTORY32 tls_dir = Utils.Read<IMAGE_TLS_DIRECTORY32>(stream);

                directory = new TLSDirectory32(this,location,tls_dir);
            }
            else
            {
                IMAGE_TLS_DIRECTORY64 tls_dir = Utils.Read<IMAGE_TLS_DIRECTORY64>(stream);

                directory = new TLSDirectory64(this,location,tls_dir);
            }
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

        public TLSDirectory Directory
        {
            get
            {
                return directory;
            }
        }

        #endregion

    }

}
