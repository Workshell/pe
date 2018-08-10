using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Content;
using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE.Resources
{
    public sealed class ResourceDirectory : DataContent, IEnumerable<ResourceDirectoryEntry>
    {
        private ResourceDirectoryEntry[] _entries;

        internal ResourceDirectory(PortableExecutableImage image, DataDirectory dataDirectory, Location location, ResourceDirectoryEntry directoryEntry) : base(image, dataDirectory, location)
        {
            _entries = new ResourceDirectoryEntry[0];

            DirectoryEntry = directoryEntry;
        }

        #region Static Methods

        public static ResourceDirectory Get(PortableExecutableImage image)
        {
            return GetAsync(image).GetAwaiter().GetResult();
        }

        public static async Task<ResourceDirectory> GetAsync(PortableExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.ResourceTable))
                return null;

            var dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.ResourceTable];

            if (DataDirectory.IsNullOrEmpty(dataDirectory))
                return null;

            var calc = image.GetCalculator();
            var rva = dataDirectory.VirtualAddress;
            var va = image.NTHeaders.OptionalHeader.ImageBase + rva;
            var offset = calc.RVAToOffset(rva);
            var size = Marshal.SizeOf<IMAGE_RESOURCE_DIRECTORY>().ToUInt32();
            var section = calc.RVAToSection(rva);
            var location = new Location(offset, rva, va, size, size, section);
            var directory = new ResourceDirectory(image, dataDirectory, location, null);

            await directory.LoadAsync().ConfigureAwait(false);

            return directory;
        }

        #endregion

        #region Methods

        public IEnumerator<ResourceDirectoryEntry> GetEnumerator()
        {
            foreach (var entry in _entries)
                yield return entry;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public DateTime GetTimeDateStamp()
        {
            return Utils.ConvertTimeDateStamp(TimeDateStamp);
        }

        public Version GetVersion()
        {
            return new Version(MajorVersion, MinorVersion);
        }

        internal async Task LoadAsync()
        {

            var calc = Image.GetCalculator();
            var stream = Image.GetStream();

            stream.Seek(Location.FileOffset.ToInt64(),SeekOrigin.Begin);

            var directorySize = Marshal.SizeOf<IMAGE_RESOURCE_DIRECTORY>();
            IMAGE_RESOURCE_DIRECTORY directory;

            try
            {
                directory = await stream.ReadStructAsync<IMAGE_RESOURCE_DIRECTORY>(directorySize).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new PortableExecutableImageException(Image, "Could not read resource directory from stream.", ex);
            }

            var count = directory.NumberOfNamedEntries + directory.NumberOfIdEntries;
            var entries = new List<ResourceDirectoryEntry>(count);
            var offset = Location.FileOffset + directorySize.ToUInt32();
            var entrySize = Marshal.SizeOf<IMAGE_RESOURCE_DIRECTORY_ENTRY>();

            for (int i = 0; i < count; i++)
            {
                stream.Seek(offset.ToInt64(),SeekOrigin.Begin);

                var entry = await stream.ReadStructAsync<IMAGE_RESOURCE_DIRECTORY_ENTRY>(entrySize).ConfigureAwait(false);
                var rva = calc.OffsetToRVA(offset);
                var va = Image.NTHeaders.OptionalHeader.ImageBase + rva;
                var section = calc.RVAToSection(rva);
                var location = new Location(offset, rva, va, entrySize.ToUInt32(), entrySize.ToUInt32(), section);
                var directoryEntry = new ResourceDirectoryEntry(Image, DataDirectory, location, this, entry);

                entries.Add(directoryEntry);

                offset += entrySize.ToUInt32();
            }

            _entries = entries.ToArray();

            Characteristics = directory.Characteristics;
            TimeDateStamp = directory.TimeDateStamp;
            MajorVersion = directory.MajorVersion;
            MinorVersion = directory.MinorVersion;
            NumberOfNamedEntries = directory.NumberOfNamedEntries;
            NumberOfIdEntries = directory.NumberOfIdEntries;
        }

        #endregion

        #region Properties

        public ResourceDirectoryEntry DirectoryEntry { get; }
        public int Count => _entries.Length;
        public ResourceDirectoryEntry this[int index] => _entries[index];

        [FieldAnnotation("Characteristics")]
        public uint Characteristics { get; private set; }

        [FieldAnnotation("Date/Time Stamp")]
        public uint TimeDateStamp { get; private set; }

        [FieldAnnotation("Major Version")]
        public ushort MajorVersion { get; private set; }

        [FieldAnnotation("Minor Version")]
        public ushort MinorVersion { get; private set; }

        [FieldAnnotation("Number of Named Entries")]
        public ushort NumberOfNamedEntries { get; private set; }

        [FieldAnnotation("Number of ID Entries")]
        public ushort NumberOfIdEntries { get; private set; }



        #endregion
    }
}
