using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public class RelocationContent : SectionContent, IEnumerable<RelocationBlock>
    {

        private List<RelocationBlock> blocks;

        internal RelocationContent(Stream stream, DataDirectory dataDirectory, Section owningSection) : base(dataDirectory,owningSection)
        {
            stream.Seek(owningSection.Location.Offset,SeekOrigin.Begin);

            blocks = new List<RelocationBlock>();

            long offset = owningSection.Location.Offset;
            long size = 0;

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

                if (size >= dataDirectory.Size)
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
