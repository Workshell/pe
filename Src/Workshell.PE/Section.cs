using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public sealed class Section : ISupportsLocation, ISupportsBytes
    {

        private Sections _sections;
        private SectionTableEntry _table_entry;
        private Location _location;

        internal Section(Sections sections, SectionTableEntry tableEntry)
        {
            ulong image_base = sections.Reader.NTHeaders.OptionalHeader.ImageBase;

            _sections = sections;
            _table_entry = tableEntry;
            _location = new Location(tableEntry.PointerToRawData,tableEntry.VirtualAddress,image_base + tableEntry.VirtualAddress,tableEntry.SizeOfRawData,tableEntry.VirtualSizeOrPhysicalAddress);
        }

        #region Methods

        public override string ToString()
        {
            return _table_entry.Name;
        }

        public byte[] GetBytes()
        {
            Stream stream = _sections.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,_location);

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

    public sealed class Sections : IEnumerable<Section>, IReadOnlyCollection<Section>
    {

        private ImageReader reader;
        private SectionTable table;
        private Dictionary<SectionTableEntry,Section> sections;

        internal Sections(ImageReader exeReader, SectionTable sectionTable)
        {
            reader = exeReader;
            table = sectionTable;
            sections = new Dictionary<SectionTableEntry, Section>();
        }

        #region Methods

        public IEnumerator<Section> GetEnumerator()
        {
            for(var i = 0; i < table.Count; i++)
            {
                Section section = new Section(this, table[i]);

                yield return section;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Section GetSection(SectionTableEntry tableEntry)
        {
            if (!sections.ContainsKey(tableEntry))
            {
                Section section = new Section(this, tableEntry);

                sections[tableEntry] = section;
            }

            return sections[tableEntry];
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
                return GetSection(tableEntry);
            }
        }

        #endregion

    }

}
