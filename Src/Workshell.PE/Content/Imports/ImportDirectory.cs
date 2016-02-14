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

    public sealed class ImportDirectoryEntry : ISupportsLocation
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

    public sealed class ImportDirectory : IEnumerable<ImportDirectoryEntry>, IReadOnlyCollection<ImportDirectoryEntry>, ISupportsLocation
    {

        private ImportTableContent content;
        private Location location;
        private ImportDirectoryEntry[] entries;

        internal ImportDirectory(ImportTableContent importContent, Location dirLocation, IEnumerable<IMAGE_IMPORT_DESCRIPTOR> importDescriptors, ulong imageBase)
        {
            content = importContent;
            location = dirLocation;
            entries = new ImportDirectoryEntry[0];

            LocationCalculator calc = importContent.DataDirectory.Directories.Reader.GetCalculator();
            ulong offset = dirLocation.FileOffset;
            uint size = Convert.ToUInt32(Utils.SizeOf<IMAGE_IMPORT_DESCRIPTOR>());
            List<ImportDirectoryEntry> list = new List<ImportDirectoryEntry>();

            foreach(IMAGE_IMPORT_DESCRIPTOR descriptor in importDescriptors)
            {
                uint rva = calc.OffsetToRVA(content.Section,offset);
                ulong va = imageBase + rva;
                Location descripter_location = new Location(offset,rva,va,size,size);

                ImportDirectoryEntry entry = new ImportDirectoryEntry(this,descriptor,descripter_location);

                list.Add(entry);
                offset += size;
            }

            entries = list.ToArray();
        }

        #region Methods

        public IEnumerator<ImportDirectoryEntry> GetEnumerator()
        {
            return entries.Cast<ImportDirectoryEntry>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public byte[] GetBytes()
        {
            Stream stream = content.DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
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

        public int Count
        {
            get
            {
                return entries.Length;
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
