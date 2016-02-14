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

    public enum NameType
    {
        ID,
        Name
    }

    public enum OffsetType
    {
        Directory,
        Data
    }

    public sealed class ResourceDirectoryEntry : ISupportsLocation, ISupportsBytes
    {

        private ResourceDirectory parent_directory;
        private IMAGE_RESOURCE_DIRECTORY_ENTRY entry;
        private Location location;
        private ulong image_base;
        private string name;
        private ResourceDirectory directory;
        private ResourceDataEntry data;

        internal ResourceDirectoryEntry(ResourceDirectory parentDirectory, ulong entryOffset, IMAGE_RESOURCE_DIRECTORY_ENTRY directoryEntry, ulong imageBase)
        {
            LocationCalculator calc = parentDirectory.Content.DataDirectory.Directories.Reader.GetCalculator();
            Stream stream = parentDirectory.Content.DataDirectory.Directories.Reader.GetStream();

            uint rva = calc.OffsetToRVA(entryOffset);
            ulong va = imageBase + rva;
            uint size = Convert.ToUInt32(Utils.SizeOf<IMAGE_RESOURCE_DIRECTORY_ENTRY>());

            parent_directory = parentDirectory;
            entry = directoryEntry;
            location = new Location(entryOffset,rva,va,size,size);
            image_base = imageBase;
            name = String.Empty;
            directory = null;
        }

        #region Methods

        public uint GetId()
        {
            if ((entry.Name & 0x80000000) == 0x80000000)
            {
                return 0;
            }
            else
            {
                return entry.Name;
            }
        }

        public string GetName()
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                if ((entry.Name & 0x80000000) == 0x80000000)
                {
                    LocationCalculator calc = parent_directory.Content.DataDirectory.Directories.Reader.GetCalculator();
                    Stream stream = parent_directory.Content.DataDirectory.Directories.Reader.GetStream();
                    uint offset = entry.Name & 0x7fffffff;
                    uint rva = parent_directory.Content.DataDirectory.VirtualAddress + offset;
                    ulong file_offset = calc.RVAToOffset(rva);

                    stream.Seek(Convert.ToInt64(file_offset),SeekOrigin.Begin);

                    ushort count = Utils.ReadUInt16(stream);
                    StringBuilder builder = new StringBuilder(count);

                    for(int i = 0; i < count; i++)
                    {
                        ushort value = Utils.ReadUInt16(stream);
                        char c = Convert.ToChar(value);

                        builder.Append(c);
                    }

                    name = builder.ToString();
                }
                else
                {
                    name = String.Empty;
                }
            }

            return name;
        }

        public ResourceDirectory GetDirectory()
        {
            if (directory == null && ((entry.OffsetToData & 0x80000000) == 0x80000000))
            {
                LocationCalculator calc = parent_directory.Content.DataDirectory.Directories.Reader.GetCalculator();
                Stream stream = parent_directory.Content.DataDirectory.Directories.Reader.GetStream();

                uint offset = entry.OffsetToData & 0x7fffffff;
                uint rva = parent_directory.Content.DataDirectory.VirtualAddress + offset;
                ulong file_offset = calc.RVAToOffset(rva);

                directory = new ResourceDirectory(parent_directory.Content,file_offset,image_base,parent_directory);
            }

            return directory;
        }

        public ResourceDataEntry GetDataEntry()
        {
            if (data == null && ((entry.OffsetToData & 0x80000000) != 0x80000000))
            {
                LocationCalculator calc = parent_directory.Content.DataDirectory.Directories.Reader.GetCalculator();
                Stream stream = parent_directory.Content.DataDirectory.Directories.Reader.GetStream();

                uint offset = entry.OffsetToData & 0x7fffffff;
                uint rva = parent_directory.Content.DataDirectory.VirtualAddress + offset;
                ulong file_offset = calc.RVAToOffset(rva);

                data = new ResourceDataEntry(this,file_offset,image_base);
            }

            return data;
        }

        public byte[] GetBytes()
        {
            Stream stream = parent_directory.Content.DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        #endregion

        #region Properties

        public ResourceDirectory Directory
        {
            get
            {
                return parent_directory;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        public NameType NameType
        {
            get
            {
                return ((entry.Name & 0x80000000) == 0x80000000 ? NameType.Name : NameType.ID);
            }
        }

        public OffsetType OffsetType
        {
            get
            {
                return ((entry.OffsetToData & 0x80000000) == 0x80000000 ? OffsetType.Directory : OffsetType.Data);
            }
        }

        [FieldAnnotation("Name")]
        public uint Name
        {
            get
            {
                return entry.Name;
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

        #endregion

    }

}
