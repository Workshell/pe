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

    public sealed class ResourceDataEntry : ISupportsLocation, ISupportsBytes
    {

        private ResourceDirectoryEntry directory_entry;
        private IMAGE_RESOURCE_DATA_ENTRY entry;
        private Location location;
        private ulong image_base;
        private ResourceData data;

        internal ResourceDataEntry(ResourceDirectoryEntry directoryEntry, ulong entryOffset, ulong imageBase)
        {
            LocationCalculator calc = directoryEntry.Directory.Content.DataDirectory.Directories.Reader.GetCalculator();
            Stream stream = directoryEntry.Directory.Content.DataDirectory.Directories.Reader.GetStream();

            stream.Seek(Convert.ToInt64(entryOffset),SeekOrigin.Begin);

            uint rva = calc.OffsetToRVA(entryOffset);
            ulong va = imageBase + rva;
            uint size = Convert.ToUInt32(Utils.SizeOf<IMAGE_RESOURCE_DATA_ENTRY>());

            directory_entry = directoryEntry;
            entry = Utils.Read<IMAGE_RESOURCE_DATA_ENTRY>(stream);
            location = new Location(entryOffset,rva,va,size,size);
            image_base = imageBase;

            GetData();
        }

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = directory_entry.Directory.Content.DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        public ResourceData GetData()
        {
            if (data == null)
                data = new ResourceData(this,image_base);

            return data;
        }

        #endregion

        #region Properties

        public ResourceDirectoryEntry DirectoryEntry
        {
            get
            {
                return directory_entry;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        [FieldAnnotation("Offset to Data")]
        public uint OffsetToData
        {
            get
            {
                return entry.OffsetToData;
            }
        }

        [FieldAnnotation("Size")]
        public uint Size
        {
            get
            {
                return entry.Size;
            }
        }

        [FieldAnnotation("Code Page")]
        public uint CodePage
        {
            get
            {
                return entry.CodePage;
            }
        }

        [FieldAnnotation("Reserved")]
        public uint Reserved
        {
            get
            {
                return entry.Reserved;
            }
        }

        #endregion

    }

}