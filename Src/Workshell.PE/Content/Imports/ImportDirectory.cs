using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class ImportDirectoryEntry : ISupportsLocation, ISupportsBytes
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
            int size = Utils.SizeOf<IMAGE_IMPORT_DESCRIPTOR>();
            byte[] buffer = new byte[size];

            Utils.Write<IMAGE_IMPORT_DESCRIPTOR>(descriptor, buffer, 0, size);

            return buffer;
        }
        
        public DateTime GetTimeDateStamp()
        {
            return Utils.ConvertTimeDateStamp(descriptor.TimeDateStamp);
        }

        public string GetName()
        {
            if (name == null)
            {           
                StringBuilder builder = new StringBuilder(256);
                LocationCalculator calc = directory.DataDirectory.Directories.Image.GetCalculator();
                Stream stream = directory.DataDirectory.Directories.Image.GetStream();
                ulong offset = calc.RVAToOffset(descriptor.Name);

                stream.Seek(offset.ToInt64(), SeekOrigin.Begin);

                name = Utils.ReadString(stream);
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

    public sealed class ImportDirectory : ExecutableImageContent, IEnumerable<ImportDirectoryEntry>, ISupportsBytes
    {

        private ImportDirectoryEntry[] entries;

        internal ImportDirectory(DataDirectory dataDirectory, Location dataLocation, IMAGE_IMPORT_DESCRIPTOR[] importDescriptors) : base(dataDirectory,dataLocation)
        {
            entries = new ImportDirectoryEntry[0];

            LocationCalculator calc = DataDirectory.Directories.Image.GetCalculator();
            ulong image_base = dataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            ulong offset = dataLocation.FileOffset;
            uint size = Utils.SizeOf<IMAGE_IMPORT_DESCRIPTOR>().ToUInt32();

            entries = new ImportDirectoryEntry[importDescriptors.Length];

            for(var i = 0; i < importDescriptors.Length; i++)
            {
                IMAGE_IMPORT_DESCRIPTOR descriptor = importDescriptors[i];
                uint rva = calc.OffsetToRVA(dataLocation.Section, offset);
                Section entry_section = calc.RVAToSection(rva);
                Location entry_location = new Location(offset, rva, image_base + rva, size, size, entry_section);
                ImportDirectoryEntry entry = new ImportDirectoryEntry(this, descriptor, entry_location);

                entries[i] = entry;
                offset += size;
            }
        }

        #region Static Methods

        public static ImportDirectory Get(ExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.ImportTable))
                return null;

            DataDirectory directory = image.NTHeaders.DataDirectories[DataDirectoryType.ImportTable];

            if (DataDirectory.IsNullOrEmpty(directory))
                return null;

            LocationCalculator calc = directory.Directories.Image.GetCalculator();
            Section section = calc.RVAToSection(directory.VirtualAddress);
            ulong file_offset = calc.RVAToOffset(section, directory.VirtualAddress);          
            Stream stream = directory.Directories.Image.GetStream();

            stream.Seek(file_offset.ToInt64(), SeekOrigin.Begin);

            int size = Utils.SizeOf<IMAGE_IMPORT_DESCRIPTOR>();
            List<IMAGE_IMPORT_DESCRIPTOR> descriptors = new List<IMAGE_IMPORT_DESCRIPTOR>();

            while (true)
            {
                IMAGE_IMPORT_DESCRIPTOR descriptor = Utils.Read<IMAGE_IMPORT_DESCRIPTOR>(stream, size);

                if (descriptor.OriginalFirstThunk == 0 && descriptor.FirstThunk == 0)
                    break;

                descriptors.Add(descriptor);
            }

            ulong image_base = directory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            uint total_size = Convert.ToUInt32((descriptors.Count + 1) * size);
            Location location = new Location(file_offset, directory.VirtualAddress, image_base + directory.VirtualAddress, total_size, total_size, section);
            ImportDirectory result = new ImportDirectory(directory, location, descriptors.ToArray());

            return result;
        }

        #endregion

        #region Methods

        public IEnumerator<ImportDirectoryEntry> GetEnumerator()
        {
            for(var i = 0; i < entries.Length; i++)
            {
                yield return entries[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public byte[] GetBytes()
        {
            Stream stream = DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,Location);

            return buffer;
        }

        #endregion

        #region Properties

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
