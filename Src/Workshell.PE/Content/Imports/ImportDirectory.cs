using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public class ImportDirectoryEntry : ISupportsLocation
    {

        private ImportDirectory directory;
        private IMAGE_IMPORT_DESCRIPTOR descriptor;
        private Location location;
        private string name;

        internal ImportDirectoryEntry(ImportDirectory importDirectory, IMAGE_IMPORT_DESCRIPTOR entry, Location entryLocation)
        {
            directory = importDirectory;
            descriptor = entry;
            location = entryLocation;
            name = null;
        }

        #region Methods

        public override string ToString()
        {
            return GetName();
        }

        public byte[] GetBytes()
        {
            using (MemoryStream mem = new MemoryStream())
            {
                Utils.Write<IMAGE_IMPORT_DESCRIPTOR>(descriptor,mem);

                return mem.ToArray();
            }
        }
        
        public DateTime GetTimeDateStamp()
        {
            return Utils.ConvertTimeDateStamp(descriptor.TimeDateStamp);
        }

        public string GetName()
        {
            if (String.IsNullOrWhiteSpace(name))
            {           
                StringBuilder builder = new StringBuilder();
                LocationCalculator calc = directory.Content.DataDirectory.Directories.Reader.GetCalculator();
                Stream stream = directory.Content.DataDirectory.Directories.Reader.GetStream();
                ulong offset = calc.RVAToOffset(descriptor.Name);

                stream.Seek(Convert.ToInt64(offset),SeekOrigin.Begin);

                while (true)
                {
                    int b = stream.ReadByte();

                    if (b <= 0)
                        break;

                    builder.Append((char)b);
                }

                name = builder.ToString();
            }

            return name;
        }

        #endregion

        #region Properties

        public ImportDirectory Directory
        {
            get
            {
                return directory;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        [FieldAnnotation("Original First Thunk")]
        public uint OriginalFirstThunk
        {
            get
            {
                return descriptor.OriginalFirstThunk;
            }
        }

        [FieldAnnotation("Date/Time Stamp")]
        public uint TimeDateStamp
        {
            get
            {
                return descriptor.TimeDateStamp;
            }
        }

        [FieldAnnotation("Forwarder Chain")]
        public uint ForwarderChain
        {
            get
            {
                return descriptor.ForwarderChain;
            }
        }

        [FieldAnnotation("Name")]
        public uint Name
        {
            get
            {
                return descriptor.Name;
            }
        }

        [FieldAnnotation("First Thunk")]
        public uint FirstThunk
        {
            get
            {
                return descriptor.FirstThunk;
            }
        }

        #endregion

    }

    public class ImportDirectory : IEnumerable<ImportDirectoryEntry>, ISupportsLocation
    {

        private ImportTableContent content;
        private Location location;
        private Section section;
        private List<ImportDirectoryEntry> entries;

        internal ImportDirectory(ImportTableContent importContent, Location dirLocation, Section dirSection, IEnumerable<IMAGE_IMPORT_DESCRIPTOR> importDescriptors, ulong imageBase)
        {
            content = importContent;
            location = dirLocation;
            section = dirSection;
            entries = new List<ImportDirectoryEntry>();

            LocationCalculator calc = importContent.DataDirectory.Directories.Reader.GetCalculator();
            ulong offset = dirLocation.FileOffset;
            uint size = Convert.ToUInt32(Utils.SizeOf<IMAGE_IMPORT_DESCRIPTOR>());

            foreach(IMAGE_IMPORT_DESCRIPTOR descriptor in importDescriptors)
            {
                uint rva = calc.OffsetToRVA(section,offset);
                ulong va = imageBase + rva;
                Location descripter_location = new Location(offset,rva,va,size,size);

                ImportDirectoryEntry entry = new ImportDirectoryEntry(this,descriptor,descripter_location);

                entries.Add(entry);
                offset += size;
            }
        }

        #region Methods

        public IEnumerator<ImportDirectoryEntry> GetEnumerator()
        {
            return entries.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public byte[] GetBytes()
        {
            return null;
        }

        #endregion

        #region Properties

        public ImportTableContent Content
        {
            get
            {
                return content;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        public Section Section
        {
            get
            {
                return section;
            }
        }

        public int Count
        {
            get
            {
                return entries.Count;
            }
        }

        public ImportDirectoryEntry this[int index]
        {
            get
            {
                return entries[index];
            }
        }

        #endregion

    }

}
