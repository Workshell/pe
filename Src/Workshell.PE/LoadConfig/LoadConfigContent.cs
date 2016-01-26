using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    internal class LoadConfigContentProvider : ISectionContentProvider
    {

        #region Methods

        public SectionContent Create(DataDirectory directory, Section section)
        {
            return new LoadConfigContent(directory,section);
        }

        #endregion

        #region Properties

        public DataDirectoryType DirectoryType
        {
            get
            {
                return DataDirectoryType.LoadConfigTable;
            }
        }

        #endregion

    }

    public class LoadConfigContent : SectionContent, ILocationSupport, IRawDataSupport
    {

        private StreamLocation location;
        private LoadConfigDirectory directory;

        internal LoadConfigContent(DataDirectory dataDirectory, Section section) : base(dataDirectory,section)
        {
            long offset = section.RVAToOffset(dataDirectory.VirtualAddress);
            location = new StreamLocation(offset,dataDirectory.Size);
        }

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = Section.Sections.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        private LoadConfigDirectory LoadDirectory()
        {
            LoadConfigDirectory result;
            Stream stream = Section.Sections.Reader.GetStream();

            stream.Seek(location.Offset,SeekOrigin.Begin);

            if (Section.Sections.Reader.Is32Bit)
            {
                IMAGE_LOAD_CONFIG_DIRECTORY32 directory = Utils.Read<IMAGE_LOAD_CONFIG_DIRECTORY32>(stream,LoadConfigDirectory.Size32);
                result = new LoadConfigDirectory32(this,location.Offset,directory);
            }
            else
            {
                IMAGE_LOAD_CONFIG_DIRECTORY64 directory = Utils.Read<IMAGE_LOAD_CONFIG_DIRECTORY64>(stream,LoadConfigDirectory.Size64);
                result = new LoadConfigDirectory64(this,location.Offset,directory);
            }

            return result;
        }

        #endregion

        #region Properties

        public StreamLocation Location
        {
            get
            {
                return location;
            }
        }

        public LoadConfigDirectory Directory
        {
            get
            {
                if (directory == null)
                    directory = LoadDirectory();

                return directory;
            }
        }

        #endregion

    }


}
