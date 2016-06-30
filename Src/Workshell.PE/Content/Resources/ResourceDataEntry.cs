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

    public sealed class ResourceDataEntry : ExecutableImageContent, ISupportsBytes
    {

        private ResourceDirectoryEntry directory_entry;
        private IMAGE_RESOURCE_DATA_ENTRY entry;
        private ResourceData data;

        internal ResourceDataEntry(DataDirectory dataDirectory, Location dataLocation, ResourceDirectoryEntry directoryEntry) : base(dataDirectory,dataLocation)
        {
            Stream stream = directoryEntry.Directory.DataDirectory.Directories.Image.GetStream();

            stream.Seek(dataLocation.FileOffset.ToInt64(),SeekOrigin.Begin);

            directory_entry = directoryEntry;
            entry = Utils.Read<IMAGE_RESOURCE_DATA_ENTRY>(stream);
            data = GetData();
        }

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,Location);

            return buffer;
        }

        public ResourceData GetData()
        {
            if (data == null)
            {
                LocationCalculator calc = DataDirectory.Directories.Image.GetCalculator();
                uint rva = entry.OffsetToData;
                ulong va = DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase + rva;
                ulong file_offset = calc.VAToOffset(va);
                ulong size = entry.Size;
                Section section = calc.RVAToSection(rva);
                Location location = new Location(file_offset, rva, va, size, size, section);

                data = new ResourceData(DataDirectory, location, this);
            }

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
