#region License
//  Copyright(c) 2016, Workshell Ltd
//  All rights reserved.
//  
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Workshell Ltd nor the names of its contributors
//  may be used to endorse or promote products
//  derived from this software without specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED.IN NO EVENT SHALL WORKSHELL BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

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
            ulong image_base = tableEntry.Table.Image.NTHeaders.OptionalHeader.ImageBase;

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
            Stream stream = _table_entry.Table.Image.GetStream();
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

    public sealed class Sections : IEnumerable<Section>
    {

        private SectionTable table;
        private Dictionary<SectionTableEntry,Section> sections;

        internal Sections(SectionTable sectionTable)
        {
            table = sectionTable;
            sections = new Dictionary<SectionTableEntry, Section>();
        }

        #region Methods

        public IEnumerator<Section> GetEnumerator()
        {
            for(var i = 0; i < table.Count; i++)
            {
                Section section = GetSection(table[i]);

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
