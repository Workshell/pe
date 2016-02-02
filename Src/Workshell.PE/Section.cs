using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class Section
    {

        private Sections _sections;
        private SectionTableEntry _table_entry;
        private Location _location;

        internal Section(Sections sections, SectionTableEntry tableEntry, ulong imageBase)
        {
            _sections = sections;
            _table_entry = tableEntry;
            _location = new Location(tableEntry.PointerToRawData,tableEntry.VirtualAddress,imageBase + tableEntry.VirtualAddress,tableEntry.SizeOfRawData,tableEntry.VirtualSizeOrPhysicalAddress);
        }

        #region Methods

        public override string ToString()
        {
            return _table_entry.Name;
        }

        public byte[] GetBytes()
        {
            return null;
        }

        #endregion

        #region Properties

        public Sections Sections
        {
            get
            {
                return _sections;
            }
        }

        public SectionTableEntry TableEntry
        {
            get
            {
                return _table_entry;
            }
        }

        public Location Location
        {
            get
            {
                return _location;
            }
        }

        public string Name
        {
            get
            {
                return _table_entry.Name;
            }
        }

        #endregion

    }

    public class Sections : IEnumerable<Section>
    {

        private ImageReader reader;
        private SectionTable table;
        private List<Section> sections;

        internal Sections(ImageReader exeReader, SectionTable sectionTable, ulong imageBase)
        {
            reader = exeReader;
            table = sectionTable;
            sections = new List<Section>();

            foreach(SectionTableEntry entry in sectionTable)
            {
                Section section = new Section(this,entry,imageBase);

                sections.Add(section);
            }
        }

        #region Methods

        public IEnumerator<Section> GetEnumerator()
        {
            return sections.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public ImageReader Reader
        {
            get
            {
                return reader;
            }
        }

        public SectionTable Table
        {
            get
            {
                return table;
            }
        }

        public int Count
        {
            get
            {
                return table.Count;
            }
        }

        public Section this[int index]
        {
            get
            {
                if (index < 0 || index > (table.Count - 1))
                    return null;

                SectionTableEntry entry = table[index];

                return this[entry];
            }
        }

        public Section this[string sectionName]
        {
            get
            {
                SectionTableEntry entry = table.FirstOrDefault(e => String.Compare(sectionName,e.Name,StringComparison.OrdinalIgnoreCase) == 0);

                return this[entry];
            }
        }
        
        public Section this[SectionTableEntry tableEntry]
        {
            get
            {
                Section section = sections.FirstOrDefault(s => s.TableEntry == tableEntry);

                return section;
            }
        }

        #endregion

    }

}
