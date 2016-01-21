using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public class RelocationBlock : IEnumerable<Relocation>, ILocationSupport
    {

        private RelocationContent content;
        private StreamLocation location;
        private IMAGE_BASE_RELOCATION relocation;
        private SectionTableEntry relocation_section;
        private List<Relocation> list;

        internal RelocationBlock(RelocationContent relocContent, long offset, long size, IMAGE_BASE_RELOCATION baseRelocation, List<ushort> relocList)
        {
            content = relocContent;
            location = new StreamLocation(offset,size);
            relocation = baseRelocation;
            relocation_section = relocContent.Section.Sections.Reader.RVAToSectionTableEntry(baseRelocation.VirtualAddress);
            list = new List<Relocation>();

            long reloc_offset = offset + 8;

            foreach(ushort value in relocList)
                list.Add(new Relocation(this,reloc_offset,value));
        }

        #region Methods

        public IEnumerator<Relocation> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("0x{0:X8}+{1} -> {2}",relocation.VirtualAddress,relocation.SizeOfBlock,relocation_section.Name);
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

        public uint PageRVA
        {
            get
            {
                return relocation.VirtualAddress;
            }
        }

        public uint BlockSize
        {
            get
            {
                return relocation.SizeOfBlock;
            }
        }

        public SectionTableEntry RelocationSection
        {
            get
            {
                return relocation_section;
            }
        }

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public Relocation this[int index]
        {
            get
            {
                return list[index];
            }
        }

        #endregion

    }

}
