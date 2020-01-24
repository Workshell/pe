#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

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
            Location = new Location(image, tableEntry.PointerToRawData, tableEntry.VirtualAddress, imageBase + tableEntry.VirtualAddress, tableEntry.SizeOfRawData, tableEntry.VirtualSizeOrPhysicalAddress);
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
