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

    internal struct ILTTable
    {

        public long Offset;
        public long Size;
        public List<ulong> Entries;

    }

    public class ImportContent : SectionContent, ILocationSupport
    {

        private StreamLocation location;
        private ImportDirectory directory;
        private ImportLookupTables ilt;

        internal ImportContent(DataDirectory dataDirectory, Section section) : base(dataDirectory,section)
        {
            long offset = Convert.ToInt64(section.RVAToOffset(dataDirectory.VirtualAddress));

            location = new StreamLocation(offset,dataDirectory.Size);

            Stream stream = Section.Sections.Reader.Stream;

            LoadDirectory(stream);
            LoadLookupTables(stream);
        }

        #region Methods

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
            long offset = Convert.ToInt64(Section.RVAToOffset(DataDirectory.VirtualAddress));

            stream.Seek(offset,SeekOrigin.Begin);

            List<IMAGE_IMPORT_DESCRIPTOR> descriptors = new List<IMAGE_IMPORT_DESCRIPTOR>();

            while (true)
            {
                IMAGE_IMPORT_DESCRIPTOR descriptor = Utils.Read<IMAGE_IMPORT_DESCRIPTOR>(stream);

                if (descriptor.OriginalFirstThunk == 0 && descriptor.FirstThunk == 0)
                    break;

                descriptors.Add(descriptor);
            }

            StreamLocation directory_location = new StreamLocation(offset,(descriptors.Count + 1) * ImportDirectoryEntry.Size);

            directory = new ImportDirectory(this,descriptors,directory_location);
        }

        private void LoadLookupTables(Stream stream)
        {
            Dictionary<int,ILTTable> ilt_tables = new Dictionary<int,ILTTable>();

            for(int i = 0; i < directory.Count; i++)
            {
                ImportDirectoryEntry entry = directory[i];
                List<ulong> ilt_entries = new List<ulong>();
                long ilt_offset = 0;

                if (entry.OriginalFirstThunk != 0)
                {
                    ilt_offset = Convert.ToInt32(Section.RVAToOffset(entry.OriginalFirstThunk));
                }
                else
                {
                    ilt_offset = Convert.ToInt32(Section.RVAToOffset(entry.FirstThunk));
                }

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

                ILTTable ilt_table = new ILTTable() {
                    Offset = ilt_offset,
                    Size = ilt_size,
                    Entries = ilt_entries
                };

                ilt_tables.Add(i,ilt_table);
            }

            ilt = new ImportLookupTables(this,ilt_tables);
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

        public ImportLookupTables LookupTables
        {
            get
            {
                return ilt;
            }
        }

        #endregion

    }

}

