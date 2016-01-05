using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public class RelocationBlock : IEnumerable<Relocation>, ILocatable
    {

        private RelocationContent content;
        private StreamLocation location;
        private IMAGE_BASE_RELOCATION relocation;
        private Section relocation_section;
        private List<Relocation> list;

        internal RelocationBlock(RelocationContent relocContent, long offset, long size, IMAGE_BASE_RELOCATION baseRelocation, List<ushort> relocList)
        {
            content = relocContent;
            location = new StreamLocation(offset,size);
            relocation = baseRelocation;
            relocation_section = null;

            foreach(Section section in relocContent.Section.Sections)
            {
                if (relocation.VirtualAddress >= section.TableEntry.VirtualAddress && relocation.VirtualAddress <= (section.TableEntry.VirtualAddress + section.TableEntry.VirtualSizeOrPhysicalAddress))
                {
                    relocation_section = section;

                    break;
                }
            }

            list = new List<Relocation>();

            long reloc_offset = offset + 8;

            foreach(ushort value in relocList)
                list.Add(new Relocation(this,reloc_offset,value));
        }

        #region Methods

        public IMAGE_BASE_RELOCATION GetNativeRelocation()
        {
            return relocation;
        }

        public override string ToString()
        {
            return String.Format("0x{0:X8}+{1}",relocation.VirtualAddress,relocation_section.TableEntry.Name);
        }

        public IEnumerator<Relocation> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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

        public Section RelocationSection
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
