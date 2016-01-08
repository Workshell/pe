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

    public class RelocationContent : SectionContent, IEnumerable<RelocationBlock>
    {

        private List<RelocationBlock> blocks;

        internal RelocationContent(DataDirectory directory, Section section) : base(directory,section)
        {
            Stream stream = section.Sections.Reader.Stream;

            blocks = new List<RelocationBlock>();

            return;

            long offset = section.Location.Offset;
            long size = 0;

            stream.Seek(section.Location.Offset,SeekOrigin.Begin);

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

        #endregion

        #region Properties

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
