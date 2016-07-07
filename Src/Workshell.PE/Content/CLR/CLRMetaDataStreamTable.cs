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

using Workshell.PE.Extensions;

namespace Workshell.PE
{

    public sealed class CLRMetaDataStreamTableEntry : ISupportsLocation, ISupportsBytes
    {

        internal CLRMetaDataStreamTableEntry(CLRMetaDataStreamTable streamTable, Location streamLocation, uint streamOffset, uint streamSize, string streamName)
        {
            Table = streamTable;
            Location = streamLocation;
            Offset = streamOffset;
            Size = streamSize;
            Name = streamName;
        }

        #region Methods

        public override string ToString()
        {
            return String.Format("Offset: 0x{0:X8}, Size: {1}, Name: {2}",Offset,Size,Name);
        } 

        public byte[] GetBytes()
        {
            Stream stream = Table.MetaData.CLR.DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,Location);

            return buffer;
        }

        #endregion

        #region Properties

        public CLRMetaDataStreamTable Table
        {
            get;
            private set;
        }

        public Location Location
        {
            get;
            private set;
        }

        public uint Offset
        {
            get;
            private set;
        }

        public uint Size
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        #endregion

    }

    public sealed class CLRMetaDataStreamTable : IEnumerable<CLRMetaDataStreamTableEntry>, ISupportsLocation, ISupportsBytes
    {

        private CLRMetaData meta_data;
        private CLRMetaDataStreamTableEntry[] entries;
        private Location location;

        internal CLRMetaDataStreamTable(CLRMetaData metaData)
        {
            LocationCalculator calc = metaData.CLR.DataDirectory.Directories.Image.GetCalculator();
            Stream stream = metaData.CLR.DataDirectory.Directories.Image.GetStream();
            ulong image_base = metaData.CLR.DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            ulong offset = metaData.Header.Location.FileOffset + metaData.Header.Location.FileSize;

            meta_data = metaData;
            entries = LoadTable(metaData,calc,stream,offset,image_base);

            uint rva = calc.OffsetToRVA(offset);
            ulong va = image_base + rva;
            ulong size = 0;

            foreach (var strm in entries)
                size += strm.Location.FileSize;

            Section section = calc.RVAToSection(rva);

            location = new Location(offset,rva,va,size,size,section);
        }

        #region Methods

        public IEnumerator<CLRMetaDataStreamTableEntry> GetEnumerator()
        {
            for(var i = 0; i < entries.Length; i++)
            {
                yield return entries[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("Stream Entry Count: {0}",entries.Length);
        }

        public byte[] GetBytes()
        {
            Stream stream = meta_data.CLR.DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        private CLRMetaDataStreamTableEntry[] LoadTable(CLRMetaData metaData, LocationCalculator calc, Stream stream, ulong baseOffset, ulong imageBase)
        {
            stream.Seek(baseOffset.ToInt64(),SeekOrigin.Begin);

            List<CLRMetaDataStreamTableEntry> entries = new List<CLRMetaDataStreamTableEntry>();
            ulong offset = baseOffset;

            for(int i = 0; i < metaData.Header.Streams; i++)
            {
                uint size = 0;
                uint stream_offset = Utils.ReadUInt32(stream);

                size += sizeof(uint);

                uint stream_size = Utils.ReadUInt32(stream);

                size += sizeof(uint);

                StringBuilder stream_name = new StringBuilder(256);

                while (true)
                {
                    int b = stream.ReadByte();
                    size += 1;

                    if (b <= 0)
                        break;

                    stream_name.Append((char)b);
                }

                while (true)
                {
                    if (stream.Position % 4 != 0)
                    {
                        stream.ReadByte();
                        size += 1;
                    }
                    else
                    {
                        break;
                    }
                }

                uint rva = calc.OffsetToRVA(offset);
                ulong va = imageBase + rva;
                Section section = calc.RVAToSection(rva);
                Location entry_location = new Location(offset,rva,va,size,size,section);
                CLRMetaDataStreamTableEntry entry = new CLRMetaDataStreamTableEntry(this,entry_location,stream_offset,stream_size,stream_name.ToString());

                entries.Add(entry);
                offset += size;
            }

            return entries.ToArray();
        }

        #endregion

        #region Properties

        public CLRMetaData MetaData
        {
            get
            {
                return meta_data;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        public int Count
        {
            get
            {
                return entries.Length;
            }
        }

        public CLRMetaDataStreamTableEntry this[int index]
        {
            get
            {
                return entries[index];
            }
        }

        public CLRMetaDataStreamTableEntry this[string name]
        {
            get
            {
                CLRMetaDataStreamTableEntry entry = entries.FirstOrDefault(stream => String.Compare(name,stream.Name,StringComparison.OrdinalIgnoreCase) == 0);

                return entry;
            }
        }

        #endregion
        
    }

}
