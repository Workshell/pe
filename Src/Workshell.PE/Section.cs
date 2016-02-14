using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class Section : ISupportsLocation, ISupportsBytes
    {

        private Sections _sections;
        private SectionTableEntry table_entry;
        private Location location;

        internal Section(Sections sections, SectionTableEntry tableEntry, ulong imageBase)
        {
            _sections = sections;
            table_entry = tableEntry;
            location = new Location(tableEntry.PointerToRawData,tableEntry.VirtualAddress,imageBase + tableEntry.VirtualAddress,tableEntry.SizeOfRawData,tableEntry.VirtualSizeOrPhysicalAddress);
        }

        #region Methods

        public override string ToString()
        {
            return table_entry.Name;
        }

        public byte[] GetBytes()
        {
            Stream stream = _sections.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
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
                return table_entry;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        public string Name
        {
            get
            {
                return table_entry.Name;
            }
        }

        #endregion

    }

    public sealed class Sections : IEnumerable<Section>, IReadOnlyCollection<Section>
    {

        private ImageReader reader;
        private SectionTable table;
        private Section[] sections;

        internal Sections(ImageReader exeReader, SectionTable sectionTable, ulong imageBase)
        {
            reader = exeReader;
            table = sectionTable;

            List<Section> list = new List<Section>();

            foreach(SectionTableEntry entry in sectionTable)
            {
                Section section = new Section(this,entry,imageBase);

                list.Add(section);
            }

            sections = list.ToArray();
        }

        #region Methods

        public IEnumerator<Section> GetEnumerator()
        {
            return sections.Cast<Section>().GetEnumerator();
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
