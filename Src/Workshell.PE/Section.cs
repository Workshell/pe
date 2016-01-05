using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class Section : IEnumerable<SectionContent>, ILocatable
    {

        private Sections sections;
        private SectionTableEntry table_entry;
        private StreamLocation location;
        private List<SectionContent> contents;

        internal Section(Sections sections, SectionTableEntry tableEntry)
        {
            this.sections = sections;
            this.table_entry = tableEntry;
            this.location = new StreamLocation(tableEntry.PointerToRawData,tableEntry.SizeOfRawData);
            this.contents = new List<SectionContent>();
        }

        #region Methods

        public IEnumerator<SectionContent> GetEnumerator()
        {
            return contents.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return table_entry.Name;
        }

        public ulong RVAToOffset(ulong rva)
        {
            ulong offset = (rva - table_entry.VirtualAddress) + table_entry.PointerToRawData;

            return offset;
        }

        public byte[] GetBytes()
        {
            byte[] buffer = new byte[location.Size];

            sections.Executable.Stream.Seek(location.Offset,SeekOrigin.Begin);
            sections.Executable.Stream.Read(buffer,0,buffer.Length);

            return buffer;
        }

        internal void AttachContent(SectionContent content)
        {
            contents.Add(content);
        }

        #endregion

        #region Properties

        public Sections Sections
        {
            get
            {
                return sections;
            }
        }

        public SectionTableEntry TableEntry
        {
            get
            {
                return table_entry;
            }
        }

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
                return contents.Count;
            }
        }

        public SectionContent this[int index]
        {
            get
            {
                return contents[index];
            }
        }

        public SectionContent this[DataDirectory dataDirectory]
        {
            get
            {
                SectionContent content = contents.FirstOrDefault(c => c.DataDirectory != null && c.DataDirectory.Equals(dataDirectory));

                return content;
            }
        }

        public SectionContent this[DataDirectoryType directoryType]
        {
            get
            {
                SectionContent content = contents.FirstOrDefault(c => c.DataDirectory != null && c.DataDirectory.DirectoryType == directoryType);

                return content;
            }
        }

        #endregion

    }

    public class Sections : IEnumerable<Section>
    {

        private PortableExecutable exe;
        private List<Section> list;

        internal Sections(PortableExecutable portableExecutable, SectionTable sectionTable)
        {
            exe = portableExecutable;
            list = new List<Section>();

            foreach(SectionTableEntry entry in sectionTable)
            {
                Section section = new Section(this,entry);

                list.Add(section);
            }
        }

        #region Methods

        public Section RVAToSection(ulong rva)
        {
            foreach(SectionTableEntry entry in exe.SectionTable)
            {
                if (rva >= entry.VirtualAddress && rva < (entry.VirtualAddress + entry.SizeOfRawData))
                    return this[entry];
            }

            return null;
        }

        public IEnumerator<Section> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public PortableExecutable Executable
        {
            get
            {
                return exe;
            }
        }

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public Section this[int index]
        {
            get
            {
                return list[index];
            }
        }

        public Section this[string sectionName]
        {
            get
            {
                Section section = list.FirstOrDefault(item => String.Compare(sectionName,item.TableEntry.Name,StringComparison.OrdinalIgnoreCase) == 0);

                return section;
            }
        }

        public Section this[SectionTableEntry tableEntry]
        {
            get
            {
                foreach(Section section in list)
                {
                    if (section.TableEntry.Equals(tableEntry))
                        return section;
                }

                return null;
            }
        }

        #endregion

    }

}
