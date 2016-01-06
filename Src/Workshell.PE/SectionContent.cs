using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public interface ISectionContentProvider
    {

        #region Methods

        SectionContent Create(DataDirectory directory, Section section);

        #endregion

        #region Properties

        DataDirectoryType DirectoryType
        {
            get;
        }

        #endregion

    }

    public abstract class SectionContent
    {

        private DataDirectory _data_directory;
        private Section _section;

        internal SectionContent(DataDirectory directory, Section section)
        {
            _data_directory = directory;
            _section = section;
        }

        #region Properties

        public Section Section
        {
            get
            {
                return _section;
            }
        }

        public DataDirectory DataDirectory
        {
            get
            {
                return _data_directory;
            }
        }

        #endregion

    }

}
