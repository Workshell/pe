using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE.Content
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

    public sealed class ResourceDirectoryEntry : DataContent
    {
        private string _name;
        private ResourceDirectory _directory;
        private ResourceDataEntry _data;

        internal ResourceDirectoryEntry(PortableExecutableImage image, DataDirectory dataDirectory, Location location, ResourceDirectory parentDirectory, IMAGE_RESOURCE_DIRECTORY_ENTRY entry) : base(image, dataDirectory, location)
        {
            _name = null;
            _directory = null;
            _data = null;

            ParentDirectory = parentDirectory;
            NameType = ((entry.Name & 0x80000000) == 0x80000000 ? NameType.Name : NameType.ID);
            OffsetType = ((entry.OffsetToData & 0x80000000) == 0x80000000 ? OffsetType.Directory : OffsetType.Data);

            Name = entry.Name;
            OffsetToData = entry.OffsetToData;
        }

        #region Methods

        public uint GetId()
        {
            if ((Name & 0x80000000) == 0x80000000)
                return 0;
            
            return Name;
        }

        public string GetName()
        {
            return GetNameAsync().GetAwaiter().GetResult();
        }

        public async Task<string> GetNameAsync()
        {
            if (_name == null)
            {
                if ((Name & 0x80000000) == 0x80000000)
                {
                    var calc = Image.GetCalculator();
                    var stream = Image.GetStream();
                    var offset = Name & 0x7fffffff;
                    var rva = DataDirectory.VirtualAddress + offset;
                    var fileOffset = calc.RVAToOffset(rva);

                    stream.Seek(fileOffset.ToInt64(), SeekOrigin.Begin);

                    var count = await stream.ReadUInt16Async().ConfigureAwait(false);
                    var builder = new StringBuilder(count);

                    for(int i = 0; i < count; i++)
                    {
                        var value = await stream.ReadUInt16Async().ConfigureAwait(false);
                        var chr = Convert.ToChar(value);

                        builder.Append(chr);
                    }

                    _name = builder.ToString();
                }
                else
                {
                    _name = string.Empty;
                }
            }

            return _name;
        }

        public ResourceDirectory GetDirectory()
        {
            return GetDirectoryAsync().GetAwaiter().GetResult();
        }

        public async Task<ResourceDirectory> GetDirectoryAsync()
        {
            if (_directory == null && (OffsetToData & 0x80000000) == 0x80000000)
            {
                var calc = Image.GetCalculator();
                var offset = OffsetToData & 0x7fffffff;
                var rva = DataDirectory.VirtualAddress + offset;
                var va = Image.NTHeaders.OptionalHeader.ImageBase + rva;
                var fileOffset = calc.RVAToOffset(rva);
                var size = Marshal.SizeOf<IMAGE_RESOURCE_DIRECTORY>().ToUInt32();
                var section = calc.RVAToSection(rva);
                var location = new Location(fileOffset, rva, va, size, size, section);

                _directory = new ResourceDirectory(Image, DataDirectory, location, this);

                await _directory.LoadAsync().ConfigureAwait(false);
            }

            return _directory;
        }

        public ResourceDataEntry GetDataEntry()
        {
            return GetDataEntryAsync().GetAwaiter().GetResult();
        }

        public async Task<ResourceDataEntry> GetDataEntryAsync()
        {
            if (_data == null && (OffsetToData & 0x80000000) != 0x80000000)
            {
                var calc = Image.GetCalculator();
                var offset = OffsetToData & 0x7fffffff;
                var rva = DataDirectory.VirtualAddress + offset;
                var va = Image.NTHeaders.OptionalHeader.ImageBase + rva;
                var fileOffset = calc.RVAToOffset(rva);
                var size = Marshal.SizeOf<IMAGE_RESOURCE_DATA_ENTRY>().ToUInt32();
                var section = calc.RVAToSection(rva);
                var location = new Location(fileOffset, rva, va, size, size, section);

                _data = new ResourceDataEntry(Image, DataDirectory, location, this);

                await _data.LoadAsync().ConfigureAwait(false);
            }

            return _data;
        }

        #endregion

        #region Properties

        public ResourceDirectory ParentDirectory { get; }
        public NameType NameType { get; }
        public OffsetType OffsetType { get; }

        [FieldAnnotation("Name")]
        public uint Name { get; }

        [FieldAnnotation("Offset to Data")]
        public uint OffsetToData { get; }

        #endregion
    }
}
