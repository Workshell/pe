using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    internal class ImportContentProvider : ISectionContentProvider
    {

        #region Methods

        public SectionContent Create(DataDirectory directory, Section section)
        {
            return new ImportContent(directory,section);
        }

        #endregion

        #region Properties

        public DataDirectoryType DirectoryType
        {
            get
            {
                return DataDirectoryType.ImportTable;
            }
        }

        #endregion

    }

    public class ImportContent : SectionContent, ILocationSupport, IRawDataSupport, IEnumerable<ImportLibrary>
    {

        private StreamLocation location;
        private ImportDirectory directory;
        private ImportLookupTables ilt;
        private ImportLookupTables iat;
        private ImportHintNameTable hint_name_table;
        private List<ImportLibrary> libraries;

        internal ImportContent(DataDirectory dataDirectory, Section section) : base(dataDirectory,section)
        {
            long offset = section.RVAToOffset(dataDirectory.VirtualAddress);

            location = new StreamLocation(offset,dataDirectory.Size);

            Stream stream = Section.Sections.Reader.Stream;

            LoadDirectory(stream);
            LoadILT(stream);
            LoadIAT(stream);
            LoadHintNameTable(stream);
            LoadLibraries(stream);
        }

        #region Methods

        public IEnumerator<ImportLibrary> GetEnumerator()
        {
            return libraries.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public byte[] GetBytes()
        {
            Stream stream = Section.Sections.Reader.Stream;
            byte[] buffer = new byte[location.Size];
            
            stream.Seek(location.Offset,SeekOrigin.Begin);
            stream.Read(buffer,0,buffer.Length);

            return buffer;
        }

        private void LoadDirectory(Stream stream)
        {
            stream.Seek(location.Offset,SeekOrigin.Begin);

            List<IMAGE_IMPORT_DESCRIPTOR> descriptors = new List<IMAGE_IMPORT_DESCRIPTOR>();

            while (true)
            {
                IMAGE_IMPORT_DESCRIPTOR descriptor = Utils.Read<IMAGE_IMPORT_DESCRIPTOR>(stream);

                if (descriptor.OriginalFirstThunk == 0 && descriptor.FirstThunk == 0)
                    break;

                descriptors.Add(descriptor);
            }

            StreamLocation directory_location = new StreamLocation(location.Offset,(descriptors.Count + 1) * ImportDirectoryEntry.Size);

            directory = new ImportDirectory(this,descriptors,directory_location);
        }

        private void LoadILT(Stream stream)
        {
            ilt = new ImportLookupTables(this);

            for(int i = 0; i < directory.Count; i++)
            {
                ImportDirectoryEntry entry = directory[i];
                List<ulong> ilt_entries = new List<ulong>();
                long ilt_offset = 0;

                if (entry.OriginalFirstThunk != 0)
                    ilt_offset = Section.RVAToOffset(entry.OriginalFirstThunk);
                
                if (ilt_offset == 0)
                    return;

                stream.Seek(ilt_offset,SeekOrigin.Begin);

                while (true)
                {
                    if (!Section.Sections.Reader.Is64Bit)
                    {
                        uint ilt_entry = Utils.ReadUInt32(stream);

                        if (ilt_entry == 0)
                            break;

                        ilt_entries.Add(ilt_entry);
                    }
                    else
                    {
                        ulong ilt_entry = Utils.ReadUInt64(stream);

                        if (ilt_entry == 0)
                            break;

                        ilt_entries.Add(ilt_entry);
                    }
                }

                long ilt_size = (ilt_entries.Count + 1) * (Section.Sections.Reader.Is64Bit ? sizeof(ulong) : sizeof(uint));

                ilt.Create(entry,ilt_offset,ilt_size,ilt_entries);
            }
        }

        private void LoadIAT(Stream stream)
        {
            iat = new ImportLookupTables(this);

            for(int i = 0; i < directory.Count; i++)
            {
                ImportDirectoryEntry entry = directory[i];
                List<ulong> iat_entries = new List<ulong>();
                long iat_offset = 0;

                if (entry.FirstThunk != 0)
                    iat_offset = Section.RVAToOffset(entry.FirstThunk);
                
                if (iat_offset == 0)
                    return;

                stream.Seek(iat_offset,SeekOrigin.Begin);

                while (true)
                {
                    if (!Section.Sections.Reader.Is64Bit)
                    {
                        uint iat_entry = Utils.ReadUInt32(stream);

                        if (iat_entry == 0)
                            break;

                        iat_entries.Add(iat_entry);
                    }
                    else
                    {
                        ulong iat_entry = Utils.ReadUInt64(stream);

                        if (iat_entry == 0)
                            break;

                        iat_entries.Add(iat_entry);
                    }
                }

                long iat_size = (iat_entries.Count + 1) * (Section.Sections.Reader.Is64Bit ? sizeof(ulong) : sizeof(uint));

                iat.Create(entry,iat_offset,iat_size,iat_entries);
            }
        }

        private void LoadHintNameTable(Stream stream)
        {
            hint_name_table = new ImportHintNameTable(this);

            foreach(ImportLookupTable table in ilt)
            {
                foreach(ImportLookupTableEntry entry in table)
                {
                    if (!entry.IsOrdinal)
                    {
                        long offset = Section.RVAToOffset(Convert.ToUInt32(entry.Address));
                        long size = 0;
                        bool is_padded = false;
                        ushort hint = 0;
                        StringBuilder name = new StringBuilder();

                        stream.Seek(offset,SeekOrigin.Begin);

                        hint = Utils.ReadUInt16(stream);
                        size += sizeof(ushort);

                        while (true)
                        {
                            int b = stream.ReadByte();

                            size++;

                            if (b <= 0)
                                break;

                            name.Append((char)b);
                        }

                        if ((size % 2) != 0)
                        {
                            is_padded = true;
                            size++;
                        }

                        hint_name_table.Create(offset,size,hint,name.ToString(),is_padded);
                    }
                }
            }
        }

        private void LoadLibraries(Stream stream)
        {
            libraries = new List<ImportLibrary>();

            foreach(ImportLookupTable table in ilt)
            {
                StringBuilder library_name = new StringBuilder();
                long library_name_offset = Section.RVAToOffset(table.DirectoryEntry.Name);

                stream.Seek(library_name_offset,SeekOrigin.Begin);

                while (true)
                {
                    int b = stream.ReadByte();

                    if (b <= 0)
                        break;

                    library_name.Append((char)b);
                }

                ImportLibrary library = new ImportLibrary(this,table,library_name.ToString());

                libraries.Add(library);

                foreach(ImportLookupTableEntry entry in table)
                {
                    if (entry.IsOrdinal)
                    {
                        library.Add(entry,entry.Ordinal);
                    }
                    else
                    {
                        long hint_offset = Section.RVAToOffset(Convert.ToUInt32(entry.Address));
                        ImportHintNameEntry hint_entry = hint_name_table.FirstOrDefault(hne => hne.Location.Offset == hint_offset);

                        if (hint_entry != null)
                            library.Add(entry,hint_entry);
                    }
                }
            }
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

        public ImportDirectory Directory
        {
            get
            {
                return directory;
            }
        }

        public ImportLookupTables ILT
        {
            get
            {
                return ilt;
            }
        }

        public ImportLookupTables IAT
        {
            get
            {
                return iat;
            }
        }

        public ImportHintNameTable HintNameTable
        {
            get
            {
                return hint_name_table;
            }
        }

        public int Count
        {
            get
            {
                return libraries.Count;
            }
        }

        public ImportLibrary this[int index]
        {
            get
            {
                return libraries[index];
            }
        }

        public ImportLibrary this[string libraryName]
        {
            get
            {
                ImportLibrary library = libraries.FirstOrDefault(lib => String.Compare(libraryName,lib.Name,StringComparison.OrdinalIgnoreCase) == 0);

                return library;
            }
        }

        #endregion

    }

}

