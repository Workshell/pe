using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public sealed class RelocationBlock : IEnumerable<Relocation>, IReadOnlyCollection<Relocation>, ISupportsLocation, ISupportsBytes
    {

        private RelocationBlocks blocks;
        private Location location;
        private uint page_rva;
        private uint block_size;
        private Section reloc_section;
        private Relocation[] relocations;

        internal RelocationBlock(RelocationBlocks relocBlocks, Location blockLocation, uint pageRVA, uint blockSize, IEnumerable<ushort> relocs)
        {
            LocationCalculator calc = relocBlocks.Content.DataDirectory.Directories.Reader.GetCalculator();

            blocks = relocBlocks;
            location = blockLocation;
            page_rva = pageRVA;
            block_size = blockSize;
            reloc_section = calc.RVAToSection(pageRVA);

            List<Relocation> list = new List<Relocation>();

            foreach(ushort reloc_offset in relocs)
            {
                Relocation reloc = new Relocation(this,reloc_offset);

                list.Add(reloc);
            }

            relocations = list.ToArray();
        }

        #region Methods

        public IEnumerator<Relocation> GetEnumerator()
        {
            return relocations.Cast<Relocation>().GetEnumerator();
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
            Stream stream = blocks.Content.DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        #endregion

        #region Properties

        public RelocationBlocks Blocks
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

        public Section RelocationSection
        {
            get
            {
                return reloc_section;
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
