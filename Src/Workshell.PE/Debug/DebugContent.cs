using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    internal class DebugContentProvider : ISectionContentProvider
    {

        #region Methods

        public SectionContent Create(DataDirectory directory, Section section)
        {
            return new DebugContent(directory,section);
        }

        #endregion

        #region Properties

        public DataDirectoryType DirectoryType
        {
            get
            {
                return DataDirectoryType.Debug;
            }
        }

        #endregion

    }

    public class DebugContent : SectionContent, ILocationSupport, IRawDataSupport, IEnumerable<DebugDirectory>
    {

        private StreamLocation location;
        private List<DebugDirectory> directories;

        internal DebugContent(DataDirectory dataDirectory, Section section) : base(dataDirectory,section)
        {
            long offset = section.RVAToOffset(dataDirectory.VirtualAddress);
            location = new StreamLocation(offset,dataDirectory.Size);
            directories = new List<DebugDirectory>();

            LoadDirectory();
        }

        #region Methods

        public IEnumerator<DebugDirectory> GetEnumerator()
        {
            return directories.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public byte[] GetBytes()
        {
            Stream stream = Section.Sections.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        private void LoadDirectory()
        {
            Stream stream = Section.Sections.Reader.GetStream();

            stream.Seek(location.Offset,SeekOrigin.Begin);

            long count = location.Size / DebugDirectory.Size;
            long offset = location.Offset;

            for(var i = 0; i < count; i++)
            {
                IMAGE_DEBUG_DIRECTORY native_directory = Utils.Read<IMAGE_DEBUG_DIRECTORY>(stream,DebugDirectory.Size);
                DebugDirectory directory = new DebugDirectory(this,offset,native_directory);

                directories.Add(directory);

                offset += DebugDirectory.Size;
            }
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

        public int Count
        {
            get
            {
                return directories.Count;
            }
        }

        public DebugDirectory this[int index]
        {
            get
            {
                if (index < 0 || index > (directories.Count - 1))
                    return null;

                return directories[index];
            }
        }

        #endregion

    }

}

