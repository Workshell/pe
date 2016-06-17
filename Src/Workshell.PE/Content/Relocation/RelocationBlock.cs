using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public sealed class RelocationBlock : IEnumerable<Relocation>, ISupportsLocation, ISupportsBytes
    {

        private RelocationTable blocks;
        private Location location;
        private uint page_rva;
        private uint block_size;
        private Lazy<Section> reloc_section;
        private Relocation[] relocations;

        internal RelocationBlock(RelocationTable relocBlocks, Location blockLocation, uint pageRVA, uint blockSize, ushort[] relocs)
        {
            blocks = relocBlocks;
            location = blockLocation;
            page_rva = pageRVA;
            block_size = blockSize;
            reloc_section = new Lazy<Section>(GetSection);
            relocations = new Relocation[relocs.Length];

            for(var i = 0; i < relocs.Length; i++)
            {
                ushort offset = relocs[i];
                Relocation reloc = new Relocation(this, offset);

                relocations[i] = reloc;
            }
        }

        #region Methods

        public IEnumerator<Relocation> GetEnumerator()
        {
            for(var i = 0; i < relocations.Length; i++)
            {
                yield return relocations[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("Page RVA: 0x{0:X8}, Block Size: {1}, Relocations: {2}",page_rva,block_size,relocations.Length);
        }

        public byte[] GetBytes()
        {
            Stream stream = blocks.DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        private Section GetSection()
        {
            LocationCalculator calc = blocks.DataDirectory.Directories.Image.GetCalculator();
            Section section = calc.RVAToSection(page_rva);

            return section;
        }

        #endregion

        #region Properties

        public RelocationTable Blocks
        {
            get
            {
                return blocks;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        public uint PageRVA
        {
            get
            {
                return page_rva;
            }
        }

        public uint BlockSize
        {
            get
            {
                return block_size;
            }
        }

        public Section Section
        {
            get
            {
                return reloc_section.Value;
            }
        }

        public int Count
        {
            get
            {
                return relocations.Length;
            }
        }

        public Relocation this[int index]
        {
            get
            {
                return relocations[index];
            }
        }

        #endregion

    }

}
