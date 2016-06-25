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

    public sealed class DelayImportDirectoryEntry : ISupportsLocation, ISupportsBytes
    {

        private DelayImportDirectory directory;
        private IMAGE_DELAY_IMPORT_DESCRIPTOR descriptor;
        private Location location;
        private string name;

        internal DelayImportDirectoryEntry(DelayImportDirectory importDirectory, IMAGE_DELAY_IMPORT_DESCRIPTOR entry, Location entryLocation)
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
            int size = Utils.SizeOf<IMAGE_DELAY_IMPORT_DESCRIPTOR>();
            byte[] buffer = new byte[size];

            Utils.Write<IMAGE_DELAY_IMPORT_DESCRIPTOR>(descriptor, buffer, 0, size);

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

        public DelayImportDirectory Directory
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

        [FieldAnnotation("Attributes")]
        public uint Attributes
        {
            get
            {
                return descriptor.Attributes;
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

        [FieldAnnotation("Module Handle")]
        public uint ModuleHandle
        {
            get
            {
                return descriptor.ModuleHandle;
            }
        }

        [FieldAnnotation("Delay Import Address Table")]
        public uint DelayAddressTable
        {
            get
            {
                return descriptor.DelayAddressTable;
            }
        }

        [FieldAnnotation("Delay Import Hint/Name Table")]
        public uint DelayNameTable
        {
            get
            {
                return descriptor.DelayNameTable;
            }
        }

        [FieldAnnotation("Bound Delay Import Address Table")]
        public uint BoundDelayIAT
        {
            get
            {
                return descriptor.Attributes;
            }
        }

        [FieldAnnotation("Unload Delay Import Address Table")]
        public uint UnloadDelayIAT
        {
            get
            {
                return descriptor.Attributes;
            }
        }

        [FieldAnnotation("Date/Time Stamp")]
        public uint TimeDateStamp
        {
            get
            {
                return descriptor.Attributes;
            }
        }

        #endregion

    }

    public sealed class DelayImportDirectory : ExecutableImageContent, IEnumerable<DelayImportDirectoryEntry>, ISupportsBytes
    {

        private DelayImportDirectoryEntry[] entries;

        internal DelayImportDirectory(DataDirectory dataDirectory, Location dataLocation, IMAGE_DELAY_IMPORT_DESCRIPTOR[] importDescriptors) : base(dataDirectory,dataLocation)
        {
            LocationCalculator calc = DataDirectory.Directories.Image.GetCalculator();
            ulong image_base = dataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            ulong offset = dataLocation.FileOffset;
            uint size = Utils.SizeOf<IMAGE_IMPORT_DESCRIPTOR>().ToUInt32();

            entries = new DelayImportDirectoryEntry[importDescriptors.Length];

            for(var i = 0; i < importDescriptors.Length; i++)
            {
                IMAGE_DELAY_IMPORT_DESCRIPTOR descriptor = importDescriptors[i];
                uint rva = calc.OffsetToRVA(dataLocation.Section, offset);
                Section entry_section = calc.RVAToSection(rva);
                Location entry_location = new Location(offset, rva, image_base + rva, size, size, entry_section);
                DelayImportDirectoryEntry entry = new DelayImportDirectoryEntry(this, descriptor, entry_location);

                entries[i] = entry;
                offset += size;
            }
        }

        #region Static Methods

        public static DelayImportDirectory Get(ExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.ImportTable))
                return null;

            DataDirectory directory = image.NTHeaders.DataDirectories[DataDirectoryType.DelayImportDescriptor];

            if (DataDirectory.IsNullOrEmpty(directory))
                return null;

            LocationCalculator calc = directory.Directories.Image.GetCalculator();
            Section section = calc.RVAToSection(directory.VirtualAddress);
            ulong file_offset = calc.RVAToOffset(section, directory.VirtualAddress);          
            Stream stream = directory.Directories.Image.GetStream();

            stream.Seek(file_offset.ToInt64(), SeekOrigin.Begin);

            int size = Utils.SizeOf<IMAGE_DELAY_IMPORT_DESCRIPTOR>();
            List<IMAGE_DELAY_IMPORT_DESCRIPTOR> descriptors = new List<IMAGE_DELAY_IMPORT_DESCRIPTOR>();

            while (true)
            {
                IMAGE_DELAY_IMPORT_DESCRIPTOR descriptor = Utils.Read<IMAGE_DELAY_IMPORT_DESCRIPTOR>(stream, size);

                if (descriptor.Name == 0 && descriptor.ModuleHandle == 0)
                    break;

                descriptors.Add(descriptor);
            }

            ulong image_base = directory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            uint total_size = Convert.ToUInt32((descriptors.Count + 1) * size);
            Location location = new Location(file_offset, directory.VirtualAddress, image_base + directory.VirtualAddress, total_size, total_size, section);
            DelayImportDirectory result = new DelayImportDirectory(directory, location, descriptors.ToArray());

            return result;
        }

        #endregion

        #region Methods

        public IEnumerator<DelayImportDirectoryEntry> GetEnumerator()
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

        public DelayImportDirectoryEntry this[int index]
        {
            get
            {
                return entries[index];
            }
        }

        #endregion

    }

}
