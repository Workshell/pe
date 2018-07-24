using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;

namespace Workshell.PE
{
    public sealed class Section : ISupportsLocation, ISupportsBytes
    {
        private readonly PortableExecutableImage _image;

        internal Section(PortableExecutableImage image, Sections sections, SectionTableEntry tableEntry)
        {
            _image = image;

            var imageBase = _image.NTHeaders.OptionalHeader.ImageBase;

            Sections = sections;
            TableEntry = tableEntry;
            Location = new Location(image.GetCalculator(), tableEntry.PointerToRawData, tableEntry.VirtualAddress, imageBase + tableEntry.VirtualAddress, tableEntry.SizeOfRawData, tableEntry.VirtualSizeOrPhysicalAddress);
        }

        #region Methods

        public override string ToString()
        {
            return TableEntry.Name;
        }

        public byte[] GetBytes()
        {
            return GetBytesAsync().GetAwaiter().GetResult();
        }

        public async Task<byte[]> GetBytesAsync()
        {
            var stream = _image.GetStream();
            var buffer = await stream.ReadBytesAsync(Location).ConfigureAwait(false);

            return buffer;
        }

        #endregion

        #region Properties

        public Sections Sections { get; }
        public SectionTableEntry TableEntry { get; }
        public Location Location { get; }
        public string Name => TableEntry.Name;

        #endregion
    }

    public sealed class Sections : IEnumerable<Section>
    {
        private readonly PortableExecutableImage _image;
        private readonly Dictionary<SectionTableEntry,Section> _sections;

        internal Sections(PortableExecutableImage image, SectionTable sectionTable)
        {
            _image = image;
            _sections = new Dictionary<SectionTableEntry, Section>(sectionTable.Count);

            Table = sectionTable;
        }

        #region Methods

        public IEnumerator<Section> GetEnumerator()
        {
            foreach (var entry in Table)
            {
                var section = GetSection(entry);

                yield return section;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Section GetSection(SectionTableEntry tableEntry)
        {
            if (!_sections.ContainsKey(tableEntry))
            {
                var section = new Section(_image, this, tableEntry);

                _sections[tableEntry] = section;
            }

            return _sections[tableEntry];
        }

        #endregion

        #region Properties

        public SectionTable Table { get; }
        public int Count => Table.Count;

        public Section this[int index]
        {
            get
            {
                var entry = Table[index];

                return this[entry];
            }
        }

        public Section this[string sectionName]
        {
            get
            {
                var entry = Table.FirstOrDefault(e => String.Compare(sectionName,e.Name,StringComparison.OrdinalIgnoreCase) == 0);

                return this[entry];
            }
        }
        
        public Section this[SectionTableEntry tableEntry] => GetSection(tableEntry);

        #endregion
    }
}
