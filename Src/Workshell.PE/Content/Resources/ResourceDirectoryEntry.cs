#region License
//  Copyright(c) 2016, Workshell Ltd
//  All rights reserved.
//  
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Workshell Ltd nor the names of its contributors
//  may be used to endorse or promote products
//  derived from this software without specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED.IN NO EVENT SHALL WORKSHELL BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

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

    public sealed class ResourceDirectoryEntry : ExecutableImageContent, ISupportsBytes
    {

        private ResourceDirectory parent_directory;
        private IMAGE_RESOURCE_DIRECTORY_ENTRY entry;
        private string name;
        private ResourceDirectory directory;
        private ResourceDataEntry data;

        internal ResourceDirectoryEntry(DataDirectory dataDirectory, Location dataLocation, ResourceDirectory parentDirectory, IMAGE_RESOURCE_DIRECTORY_ENTRY directoryEntry) : base(dataDirectory,dataLocation)
        {
            parent_directory = parentDirectory;
            entry = directoryEntry;
            name = null;
            directory = null;
            data = null;
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
            if (name == null)
            {
                if ((entry.Name & 0x80000000) == 0x80000000)
                {
                    LocationCalculator calc = DataDirectory.Directories.Image.GetCalculator();
                    Stream stream = DataDirectory.Directories.Image.GetStream();

                    uint offset = entry.Name & 0x7fffffff;
                    uint rva = DataDirectory.VirtualAddress + offset;
                    ulong file_offset = calc.RVAToOffset(rva);

                    stream.Seek(file_offset.ToInt64(),SeekOrigin.Begin);

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
                LocationCalculator calc = DataDirectory.Directories.Image.GetCalculator();
                Stream stream = DataDirectory.Directories.Image.GetStream();

                uint offset = entry.OffsetToData & 0x7fffffff;
                uint rva = DataDirectory.VirtualAddress + offset;
                ulong va = DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase + rva;
                ulong file_offset = calc.RVAToOffset(rva);
                uint size = Utils.SizeOf<IMAGE_RESOURCE_DIRECTORY>().ToUInt32();
                Section section = calc.RVAToSection(rva);
                Location location = new Location(file_offset, rva, va, size, size, section);

                directory = new ResourceDirectory(parent_directory.DataDirectory,location,parent_directory);
            }

            return directory;
        }

        public ResourceDataEntry GetDataEntry()
        {
            if (data == null && ((entry.OffsetToData & 0x80000000) != 0x80000000))
            {
                LocationCalculator calc = DataDirectory.Directories.Image.GetCalculator();

                uint offset = entry.OffsetToData & 0x7fffffff;
                uint rva = DataDirectory.VirtualAddress + offset;
                ulong va = DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase + rva;
                ulong file_offset = calc.RVAToOffset(rva);
                uint size = Utils.SizeOf<IMAGE_RESOURCE_DATA_ENTRY>().ToUInt32();
                Section section = calc.RVAToSection(rva);
                Location location = new Location(file_offset, rva, va, size, size, section);

                data = new ResourceDataEntry(DataDirectory,location,this);
            }

            return data;
        }

        public byte[] GetBytes()
        {
            Stream stream = DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,Location);

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
