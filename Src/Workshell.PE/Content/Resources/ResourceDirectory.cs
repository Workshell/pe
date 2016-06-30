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

    public sealed class ResourceDirectory : ExecutableImageContent, IEnumerable<ResourceDirectoryEntry>, ISupportsBytes
    {

        private IMAGE_RESOURCE_DIRECTORY directory;
        private ResourceDirectory parent_directory;
        private ResourceDirectoryEntry[] entries;

        internal ResourceDirectory(DataDirectory dataDirectory, Location dataLocation, ResourceDirectory parentDirectory) : base(dataDirectory,dataLocation)
        {
            LocationCalculator calc = DataDirectory.Directories.Image.GetCalculator();
            Stream stream = DataDirectory.Directories.Image.GetStream();

            stream.Seek(dataLocation.FileOffset.ToInt64(),SeekOrigin.Begin);

            directory = Utils.Read<IMAGE_RESOURCE_DIRECTORY>(stream);
            parent_directory = parentDirectory;

            int count = directory.NumberOfNamedEntries + directory.NumberOfIdEntries;
            List<ResourceDirectoryEntry> list = new List<ResourceDirectoryEntry>(count);
            uint size = Utils.SizeOf<IMAGE_RESOURCE_DIRECTORY>().ToUInt32();
            ulong offset = dataLocation.FileOffset + size;
            uint entry_size = Utils.SizeOf<IMAGE_RESOURCE_DIRECTORY_ENTRY>().ToUInt32();

            for (int i = 0; i < count; i++)
            {
                stream.Seek(offset.ToInt64(),SeekOrigin.Begin);

                IMAGE_RESOURCE_DIRECTORY_ENTRY directory_entry = Utils.Read<IMAGE_RESOURCE_DIRECTORY_ENTRY>(stream);

                uint rva = calc.OffsetToRVA(offset);
                ulong va = DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase + rva;
                Section section = calc.RVAToSection(rva);
                Location location = new Location(offset, rva, va, entry_size, entry_size, section);

                ResourceDirectoryEntry entry = new ResourceDirectoryEntry(DataDirectory, location,this,directory_entry);

                list.Add(entry);
                offset += entry_size;
            }

            entries = list.ToArray();
        }

        #region Static Methods

        public static ResourceDirectory GetRootDirectory(ExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.ResourceTable))
                return null;

            DataDirectory directory = image.NTHeaders.DataDirectories[DataDirectoryType.ResourceTable];

            if (DataDirectory.IsNullOrEmpty(directory))
                return null;

            LocationCalculator calc = directory.Directories.Image.GetCalculator();
            uint rva = directory.VirtualAddress;
            ulong va = directory.Directories.Image.NTHeaders.OptionalHeader.ImageBase + rva;
            ulong file_offset = calc.RVAToOffset(rva);
            uint size = Utils.SizeOf<IMAGE_RESOURCE_DIRECTORY>().ToUInt32();
            Section section = calc.RVAToSection(rva);
            Location location = new Location(file_offset, rva, va, size, size, section);
            ResourceDirectory result = new ResourceDirectory(directory, location, null);

            return result;
        }

        #endregion

        #region Methods

        public IEnumerator<ResourceDirectoryEntry> GetEnumerator()
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

        public ResourceDirectory Parent
        {
            get
            {
                return parent_directory;
            }
        }

        public int Count
        {
            get
            {
                return entries.Length;
            }
        }

        public ResourceDirectoryEntry this[int index]
        {
            get
            {
                return entries[index];
            }
        }

        [FieldAnnotation("Characteristics")]
        public uint Characteristics
        {
            get
            {
                return directory.Characteristics;
            }
        }

        [FieldAnnotation("Date/Time Stamp")]
        public uint TimeDateStamp
        {
            get
            {
                return directory.TimeDateStamp;
            }
        }

        [FieldAnnotation("Major Version")]
        public ushort MajorVersion
        {
            get
            {
                return directory.MajorVersion;
            }
        }

        [FieldAnnotation("Minor Version")]
        public ushort MinorVersion
        {
            get
            {
                return directory.MinorVersion;
            }
        }

        [FieldAnnotation("Number of Named Entries")]
        public ushort NumberOfNamedEntries
        {
            get
            {
                return directory.NumberOfNamedEntries;
            }
        }

        [FieldAnnotation("Number of ID Entries")]
        public ushort NumberOfIdEntries
        {
            get
            {
                return directory.NumberOfIdEntries;
            }
        }

        #endregion

    }

}
