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

    public sealed class ResourceDirectory : IEnumerable<ResourceDirectoryEntry>, ISupportsLocation, ISupportsBytes
    {

        private DataDirectory data_directory;
        private IMAGE_RESOURCE_DIRECTORY directory;
        private Location location;
        private ResourceDirectory parent_directory;
        private ResourceDirectoryEntry[] entries;

        internal ResourceDirectory(DataDirectory dataDirectory, ulong directoryOffset, ResourceDirectory parentDirectory)
        {
            data_directory = dataDirectory;

            LocationCalculator calc = data_directory.Directories.Image.GetCalculator();
            Stream stream = data_directory.Directories.Image.GetStream();

            stream.Seek(Convert.ToInt64(directoryOffset),SeekOrigin.Begin);

            uint rva = calc.OffsetToRVA(directoryOffset);
            ulong va = data_directory.Directories.Image.NTHeaders.OptionalHeader.ImageBase + rva;
            uint size = Utils.SizeOf<IMAGE_RESOURCE_DIRECTORY>().ToUInt32();
            Section section = calc.RVAToSection(rva);

            directory = Utils.Read<IMAGE_RESOURCE_DIRECTORY>(stream);
            location = new Location(directoryOffset,rva,va,size,size,section);
            parent_directory = parentDirectory;

            int count = directory.NumberOfNamedEntries + directory.NumberOfIdEntries;
            List<ResourceDirectoryEntry> list = new List<ResourceDirectoryEntry>(count);
            ulong offset = directoryOffset + size;
            uint entry_size = Utils.SizeOf<IMAGE_RESOURCE_DIRECTORY_ENTRY>().ToUInt32();

            for (int i = 0; i < count; i++)
            {
                stream.Seek(Convert.ToInt64(offset),SeekOrigin.Begin);

                IMAGE_RESOURCE_DIRECTORY_ENTRY directory_entry = Utils.Read<IMAGE_RESOURCE_DIRECTORY_ENTRY>(stream);
                ResourceDirectoryEntry entry = new ResourceDirectoryEntry(this,offset,directory_entry);

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
            Section section = calc.RVAToSection(directory.VirtualAddress);
            ulong file_offset = calc.RVAToOffset(section, directory.VirtualAddress);

            ResourceDirectory result = new ResourceDirectory(directory, file_offset, null);

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
            Stream stream = data_directory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        #endregion

        #region Properties

        public DataDirectory DataDirectory
        {
            get
            {
                return data_directory;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

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
