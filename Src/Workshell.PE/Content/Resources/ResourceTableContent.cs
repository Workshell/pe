using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class ResourceTableContent : DataDirectoryContent
    {

        private Section section;
        private ResourceDirectory root_directory;

        internal ResourceTableContent(DataDirectory dataDirectory, ulong imageBase) : base(dataDirectory, imageBase)
        {
            LocationCalculator calc = DataDirectory.Directories.Reader.GetCalculator();
            Stream stream = DataDirectory.Directories.Reader.GetStream();
            ulong offset = calc.RVAToOffset(dataDirectory.VirtualAddress);

            section = calc.RVAToSection(dataDirectory.VirtualAddress);
            root_directory = new ResourceDirectory(this,offset,imageBase,null);
        }

        #region Methods

        #endregion

        #region Properties

        public Section Section
        {
            get
            {
                return section;
            }
        }

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
