﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class ResourceDirectory
    {

        private ResourceTableContent content;
        private IMAGE_RESOURCE_DIRECTORY directory;
        private Location location;
        private ResourceDirectory parent_directory;
        private List<ResourceDirectoryEntry> entries;

        internal ResourceDirectory(ResourceTableContent resourceContent, ulong directoryOffset, ulong imageBase, ResourceDirectory parentDirectory)
        {
            LocationCalculator calc = resourceContent.DataDirectory.Directories.Reader.GetCalculator();
            Stream stream = resourceContent.DataDirectory.Directories.Reader.GetStream();

            stream.Seek(Convert.ToInt64(directoryOffset),SeekOrigin.Begin);

            uint rva = calc.OffsetToRVA(directoryOffset);
            ulong va = imageBase + rva;
            uint size = Convert.ToUInt32(Utils.SizeOf<IMAGE_RESOURCE_DIRECTORY>());

            content = resourceContent;
            directory = Utils.Read<IMAGE_RESOURCE_DIRECTORY>(stream);
            location = new Location(directoryOffset,rva,va,size,size);
            parent_directory = parentDirectory;

            int count = directory.NumberOfNamedEntries + directory.NumberOfIdEntries;

            entries = new List<ResourceDirectoryEntry>(count);

            ulong offset = directoryOffset + size;

            for(int i = 0; i < count; i++)
            {
                IMAGE_RESOURCE_DIRECTORY_ENTRY directory_entry = Utils.Read<IMAGE_RESOURCE_DIRECTORY_ENTRY>(stream);
                ResourceDirectoryEntry entry = new ResourceDirectoryEntry(this,offset,directory_entry,imageBase);

                entries.Add(entry);
                offset += Convert.ToUInt32(Utils.SizeOf<IMAGE_RESOURCE_DIRECTORY_ENTRY>());
            }
        }

        #region Methods

        #endregion

        #region Properties

        public ResourceTableContent Content
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

        public ResourceDirectory Parent
        {
            get
            {
                return parent_directory;
            }
        }

        public List<ResourceDirectoryEntry> Entries
        {
            get
            {
                return entries;
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
