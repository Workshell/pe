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

namespace Workshell.PE.Resources
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
