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
