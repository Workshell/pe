using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Stream stream = Table.MetaData.Content.DataDirectory.Directories.Reader.GetStream();
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

    public sealed class CLRMetaDataStreamTable : IEnumerable<CLRMetaDataStreamTableEntry>, IReadOnlyCollection<CLRMetaDataStreamTableEntry>, ISupportsLocation, ISupportsBytes
    {

        private CLRMetaData meta_data;
        private List<CLRMetaDataStreamTableEntry> streams;
        private Location location;

        internal CLRMetaDataStreamTable(CLRMetaData metaData, ulong imageBase)
        {
            LocationCalculator calc = metaData.Content.DataDirectory.Directories.Reader.GetCalculator();
            Stream stream = metaData.Content.DataDirectory.Directories.Reader.GetStream();
            ulong offset = metaData.Header.Location.FileOffset + metaData.Header.Location.FileSize;

            meta_data = metaData;
            streams = new List<CLRMetaDataStreamTableEntry>();

            LoadTable(metaData,calc,stream,offset,imageBase);

            uint rva = calc.OffsetToRVA(offset);
            ulong va = imageBase + rva;
            ulong size = 0;

            foreach (var s in streams)
                size += s.Location.FileSize;

            location = new Location(offset,rva,va,size,size);
        }

        #region Methods

        public IEnumerator<CLRMetaDataStreamTableEntry> GetEnumerator()
        {
            return streams.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("Stream Entry Count: {0}",streams.Count);
        }

        public byte[] GetBytes()
        {
            Stream stream = meta_data.Content.DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        private void LoadTable(CLRMetaData metaData, LocationCalculator calc, Stream stream, ulong baseOffset, ulong imageBase)
        {
            stream.Seek(Convert.ToInt64(baseOffset),SeekOrigin.Begin);

            ulong offset = baseOffset;

            for(int i = 0; i < metaData.Header.Streams; i++)
            {
                uint size = 0;
                uint stream_offset = Utils.ReadUInt32(stream);

                size += sizeof(uint);

                uint stream_size = Utils.ReadUInt32(stream);

                size += sizeof(uint);

                StringBuilder stream_name = new StringBuilder();

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

                Location entry_location = new Location(offset,rva,va,size,size);
                CLRMetaDataStreamTableEntry entry = new CLRMetaDataStreamTableEntry(this,entry_location,stream_offset,stream_size,stream_name.ToString());

                streams.Add(entry);
                offset += size;
            }
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
                return streams.Count;
            }
        }

        public CLRMetaDataStreamTableEntry this[int index]
        {
            get
            {
                return streams[index];
            }
        }

        public CLRMetaDataStreamTableEntry this[string name]
        {
            get
            {
                CLRMetaDataStreamTableEntry entry = streams.FirstOrDefault(stream => String.Compare(name,stream.Name,StringComparison.OrdinalIgnoreCase) == 0);

                return entry;
            }
        }

        #endregion
        
    }

}
