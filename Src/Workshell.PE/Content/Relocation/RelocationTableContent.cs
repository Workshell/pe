using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class RelocationTableContent : DataDirectoryContent
    {

        private Section section;
        private RelocationBlocks relocations;

        internal RelocationTableContent(DataDirectory dataDirectory, ulong imageBase) : base(dataDirectory, imageBase)
        {
            LocationCalculator calc = dataDirectory.Directories.Reader.GetCalculator();
            Stream stream = dataDirectory.Directories.Reader.GetStream();

            section = calc.RVAToSection(dataDirectory.VirtualAddress);

            LoadRelocations(calc, stream, imageBase);
        }

        #region Methods

        private void LoadRelocations(LocationCalculator calc, Stream stream, ulong imageBase)
        {
            ulong offset = calc.RVAToOffset(section, DataDirectory.VirtualAddress);
            Location location = new Location(offset, DataDirectory.VirtualAddress, imageBase + DataDirectory.VirtualAddress, DataDirectory.Size, DataDirectory.Size);

            stream.Seek(Convert.ToInt64(offset), SeekOrigin.Begin);

            ulong block_offset = offset;
            uint block_size = 0;
            List<Tuple<ulong,uint,uint,ushort[]>> block_relocs = new List<Tuple<ulong,uint,uint,ushort[]>>();

            while (true)
            {
                IMAGE_BASE_RELOCATION base_reloc = Utils.Read<IMAGE_BASE_RELOCATION>(stream);

                block_offset += sizeof(ulong);
                block_size += sizeof(ulong);

                long count = (base_reloc.SizeOfBlock - 8) / 2;
                List<ushort> relocs = new List<ushort>();

                for(long i = 0; i < count; i++)
                {
                    ushort reloc = Utils.ReadUInt16(stream);

                    relocs.Add(reloc);

                    block_offset += sizeof(ushort);
                    block_size += sizeof(ushort);
                }

                Tuple<ulong,uint,uint,ushort[]> block = new Tuple<ulong,uint,uint,ushort[]>(block_offset,block_size,base_reloc.VirtualAddress,relocs.ToArray());

                block_relocs.Add(block);

                if (block_size >= DataDirectory.Size)
                    break;
            }

            relocations = new RelocationBlocks(this,location,block_relocs,imageBase);
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

        public RelocationBlocks Relocations
        {
            get
            {
                return relocations;
            }
        }

        #endregion

    }

}
