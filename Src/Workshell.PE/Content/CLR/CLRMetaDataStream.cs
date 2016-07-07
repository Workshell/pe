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

using Workshell.MoreLinq;

namespace Workshell.PE
{

    public sealed class CLRMetaDataStream : ISupportsLocation, ISupportsBytes
    {

        internal CLRMetaDataStream(CLRMetaDataStreams streams, CLRMetaDataStreamTableEntry tableEntry, ulong imageBase)
        {
            Streams = streams;
            TableEntry = tableEntry;

            LocationCalculator calc = streams.MetaData.CLR.DataDirectory.Directories.Image.GetCalculator();
            ulong offset = streams.MetaData.Location.FileOffset + tableEntry.Offset;
            uint rva = calc.OffsetToRVA(offset);
            ulong va = imageBase + rva;

            Location = new Location(offset,rva,va,tableEntry.Size,tableEntry.Size);
            Name = tableEntry.Name;
        }

        #region Methods

        public override string ToString()
        {
            return Name;
        } 

        public byte[] GetBytes()
        {
            Stream stream = Streams.MetaData.CLR.DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,Location);

            return buffer;
        }

        #endregion

        #region Properties

        public CLRMetaDataStreams Streams
        {
            get;
            private set;
        }

        public CLRMetaDataStreamTableEntry TableEntry
        {
            get;
            private set;
        }

        public Location Location
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

    public sealed class CLRMetaDataStreams : IEnumerable<CLRMetaDataStream>, ISupportsLocation, ISupportsBytes
    {

        private CLRMetaData meta_data;
        private CLRMetaDataStream[] streams;
        private Location location;

        internal CLRMetaDataStreams(CLRMetaData metaData)
        {
            ulong image_base = metaData.CLR.DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;

            meta_data = metaData;
            streams = new CLRMetaDataStream[metaData.StreamTable.Count];

            for(var i = 0; i < streams.Length; i++)
            {
                CLRMetaDataStreamTableEntry entry = metaData.StreamTable[i];
                CLRMetaDataStream stream = new CLRMetaDataStream(this, entry, image_base);

                streams[i] = stream;
            }

            Section section = null;
            uint rva = 0;
            ulong va = 0;
            ulong offset = 0;
            ulong size = 0;

            if (streams.Length > 0)
            {
                CLRMetaDataStream stream = streams.MinBy(s => s.Location.FileOffset);

                section = stream.Location.Section;
                rva = stream.Location.RelativeVirtualAddress;
                va = stream.Location.VirtualAddress;
                offset = stream.Location.FileOffset;
            }

            foreach (var stream in streams)
                size += stream.Location.FileSize;

            location = new Location(offset, rva, va, size, size, section);
        }

        #region Methods

        public IEnumerator<CLRMetaDataStream> GetEnumerator()
        {
            for(var i = 0; i < streams.Length; i++)
            {
                yield return streams[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("Stream Count: {0}",streams.Length);
        }

        public byte[] GetBytes()
        {
            Stream stream = meta_data.CLR.DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
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
                return streams.Length;
            }
        }

        public CLRMetaDataStream this[int index]
        {
            get
            {
                return streams[index];
            }
        }

        public CLRMetaDataStream this[string name]
        {
            get
            {
                CLRMetaDataStream stream = streams.FirstOrDefault(s => String.Compare(name,s.Name,StringComparison.OrdinalIgnoreCase) == 0);

                return stream;
            }
        }

        #endregion
        
    }

}
