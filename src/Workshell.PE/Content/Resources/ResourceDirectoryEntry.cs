#region License
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
            {
                return 0;
            }

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

                    stream.Seek(fileOffset, SeekOrigin.Begin);

                    var count = await stream.ReadUInt16Async().ConfigureAwait(false);
                    var builder = new StringBuilder(count);

                    for(var i = 0; i < count; i++)
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
                var size = Utils.SizeOf<IMAGE_RESOURCE_DIRECTORY>().ToUInt32();
                var section = calc.RVAToSection(rva);
                var location = new Location(Image, fileOffset, rva, va, size, size, section);

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
                var size = Utils.SizeOf<IMAGE_RESOURCE_DATA_ENTRY>().ToUInt32();
                var section = calc.RVAToSection(rva);
                var location = new Location(Image, fileOffset, rva, va, size, size, section);

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

        [FieldAnnotation("Name", Order = 1)]
        public uint Name { get; }

        [FieldAnnotation("Offset to Data", Order = 2)]
        public uint OffsetToData { get; }

        #endregion
    }
}
