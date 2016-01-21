using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class Section : IEnumerable<SectionContent>, ILocationSupport, IRawDataSupport
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

        public byte[] GetBytes()
        {
            byte[] buffer = new byte[location.Size];

            sections.Reader.Stream.Seek(location.Offset,SeekOrigin.Begin);
            sections.Reader.Stream.Read(buffer,0,buffer.Length);

            return buffer;
        }

        internal void Attach(SectionContent content)
        {
            if (!contents.Contains(content))
                contents.Add(content);
        }

        #endregion

        #region Location Conversion Methods

        public long VAToOffset(ulong va)
        {
            return sections.Reader.VAToOffset(this,va);
        }

        public ulong OffsetToVA(long offset)
        {
            return sections.Reader.OffsetToVA(this,offset);
        }

        public long RVAToOffset(uint rva)
        {
            return sections.Reader.RVAToOffset(this,rva);
        }

        public uint OffsetToRVA(long offset)
        {
            return sections.Reader.OffsetToRVA(this,offset);
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

        private ExeReader reader;
        private SectionTable table;
        private Dictionary<DataDirectoryType,ISectionContentProvider> content_providers;
        private Dictionary<SectionTableEntry,Section> cache;

        internal Sections(ExeReader exeReader, SectionTable sectionTable)
        {
            reader = exeReader;
            table = sectionTable;
            content_providers = new Dictionary<DataDirectoryType,ISectionContentProvider>();
            cache = new Dictionary<SectionTableEntry,Section>();

            RegisterAssemblyContentProviders();
        }

        #region Methods

        public IEnumerator<Section> GetEnumerator()
        {
            return table.Select(entry => CreateSection(entry)).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool RegisterContentProvider(ISectionContentProvider contentProvider)
        {
            return RegisterContentProvider(contentProvider,false);
        }

        public bool RegisterContentProvider(ISectionContentProvider contentProvider, bool allowReplace)
        {
            if (content_providers.ContainsKey(contentProvider.DirectoryType))
                return false;

            content_providers[contentProvider.DirectoryType] = contentProvider;

            return true;
        }

        public void UnregisterContentProvider(DataDirectoryType directoryType)
        {
            if (content_providers.ContainsKey(directoryType))
                content_providers.Remove(directoryType);
        }

        public void UnregisterContentProvider(ISectionContentProvider contentProvider)
        {
            foreach(KeyValuePair<DataDirectoryType,ISectionContentProvider> kvp in content_providers)
            {
                if (kvp.Value == contentProvider)
                {
                    content_providers.Remove(kvp.Key);

                    break;
                }
            }
        }

        private void RegisterAssemblyContentProviders()
        {
            Type iface_type = typeof(ISectionContentProvider);
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            foreach(Type type in types)
            {
                Type[] type_ifaces = type.GetInterfaces();

                if (type_ifaces.Contains(iface_type))
                {
                    ISectionContentProvider content_provider = (ISectionContentProvider)Activator.CreateInstance(type);

                    RegisterContentProvider(content_provider);
                }
            }
        }

        private Section CreateSection(SectionTableEntry entry)
        {
            if (entry.VirtualAddress == 0)
                return null;

            if (cache.ContainsKey(entry))
                return cache[entry];

            Section section = new Section(this,entry);
            Dictionary<DataDirectoryType,DataDirectory> data_directories = new Dictionary<DataDirectoryType,DataDirectory>();

            foreach(DataDirectory directory in reader.NTHeaders.OptionalHeader.DataDirectories)
            {
                if (DataDirectory.IsNullOrEmpty(directory))
                    continue;

                if (directory.VirtualAddress >= entry.VirtualAddress && directory.VirtualAddress < ((entry.VirtualAddress + entry.SizeOfRawData) - directory.Size))
                    data_directories[directory.DirectoryType] = directory;
            }

            foreach(KeyValuePair<DataDirectoryType,ISectionContentProvider> kvp in content_providers)
            {
                if (!data_directories.ContainsKey(kvp.Key))
                    continue;

                DataDirectory data_directory = data_directories[kvp.Key];
                SectionContent content = kvp.Value.Create(data_directory,section);

                section.Attach(content);
            }

            cache.Add(entry,section);

            return section;
        }

        #endregion

        #region Properties

        public ExeReader Reader
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
                if (tableEntry == null)
                    return null;

                return CreateSection(tableEntry);
            }
        }

        #endregion

    }

}
