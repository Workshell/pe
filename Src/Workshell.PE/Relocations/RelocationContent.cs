using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    internal class RelocationContentProvider : ISectionContentProvider
    {

        #region Methods

        public SectionContent Create(DataDirectory directory, Section section)
        {
            return new RelocationContent(directory,section);
        }

        #endregion

        #region Properties

        public DataDirectoryType DirectoryType
        {
            get
            {
                return DataDirectoryType.BaseRelocationTable;
            }
        }

        #endregion

    }

    public class RelocationContent : SectionContent, ILocationSupport, IRawDataSupport, IEnumerable<RelocationBlock>
    {

        private List<RelocationBlock> blocks;
        private StreamLocation location;

        internal RelocationContent(DataDirectory directory, Section section) : base(directory,section)
        {
            blocks = new List<RelocationBlock>();

            long base_offset = section.RVAToOffset(directory.VirtualAddress);
            long offset = base_offset;
            long size = 0;
            Stream stream = Section.Sections.Reader.GetStream();

            stream.Seek(offset,SeekOrigin.Begin);

            while (true)
            {
                long block_offset = offset;
                IMAGE_BASE_RELOCATION block_data = Utils.Read<IMAGE_BASE_RELOCATION>(stream);

                offset += 8;
                size += 8;

                long num_relocations = (block_data.SizeOfBlock - 8) / 2;
                List<ushort> relocations = new List<ushort>();

                for(long i = 0; i < num_relocations; i++)
                {
                    ushort relocation = Utils.ReadUInt16(stream);

                    relocations.Add(relocation);

                    offset += 2;
                    size += 2;
                }

                RelocationBlock block = new RelocationBlock(this,block_offset,block_data.SizeOfBlock,block_data,relocations);
                
                blocks.Add(block);

                if (size >= directory.Size)
                    break;
            }

            location = new StreamLocation(base_offset,size);
        }

        #region Methods

        public IEnumerator<RelocationBlock> GetEnumerator()
        {
            return blocks.GetEnumerator();
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
                return blocks.Count;
            }
        }

        public RelocationBlock this[int index]
        {
            get
            {
                return blocks[index];
            }
        }

        #endregion

    }

}
