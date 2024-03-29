﻿#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

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
        private ResourceDirectory _root;

        internal ResourceDirectory(PortableExecutableImage image, DataDirectory dataDirectory, Location location, ResourceDirectoryEntry directoryEntry) : base(image, dataDirectory, location)
        {
            _entries = new ResourceDirectoryEntry[0];
            _root = null;

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
            var size = Utils.SizeOf<IMAGE_RESOURCE_DIRECTORY>().ToUInt32();
            var section = calc.RVAToSection(rva);
            var location = new Location(image, offset, rva, va, size, size, section);
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

            stream.Seek(Location.FileOffset,SeekOrigin.Begin);

            var directorySize = Utils.SizeOf<IMAGE_RESOURCE_DIRECTORY>();
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
            var entrySize = Utils.SizeOf<IMAGE_RESOURCE_DIRECTORY_ENTRY>();

            for (int i = 0; i < count; i++)
            {
                stream.Seek(offset,SeekOrigin.Begin);

                var entry = await stream.ReadStructAsync<IMAGE_RESOURCE_DIRECTORY_ENTRY>(entrySize).ConfigureAwait(false);
                var rva = calc.OffsetToRVA(offset);
                var va = Image.NTHeaders.OptionalHeader.ImageBase + rva;
                var section = calc.RVAToSection(rva);
                var location = new Location(Image, offset, rva, va, entrySize.ToUInt32(), entrySize.ToUInt32(), section);
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

        [FieldAnnotation("Characteristics", Order = 1)]
        public uint Characteristics { get; private set; }

        [FieldAnnotation("Date/Time Stamp", Order = 2)]
        public uint TimeDateStamp { get; private set; }

        [FieldAnnotation("Major Version", Order = 3)]
        public ushort MajorVersion { get; private set; }

        [FieldAnnotation("Minor Version", Order = 4)]
        public ushort MinorVersion { get; private set; }

        [FieldAnnotation("Number of Named Entries", Order = 5)]
        public ushort NumberOfNamedEntries { get; private set; }

        [FieldAnnotation("Number of ID Entries", Order = 6)]
        public ushort NumberOfIdEntries { get; private set; }

        #endregion
    }
}
