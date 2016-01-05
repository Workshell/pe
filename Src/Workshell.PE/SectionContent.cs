using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public abstract class SectionContent
    {

        private Section section;
        private DataDirectory data_directory;

        internal SectionContent(DataDirectory dataDirectory, Section owningSection)
        {
            section = owningSection;
            data_directory = dataDirectory;
        }

        #region Properties

        public Section Section
        {
            get
            {
                return section;
            }
        }

        public DataDirectory DataDirectory
        {
            get
            {
                return data_directory;
            }
        }

        #endregion

    }

}
