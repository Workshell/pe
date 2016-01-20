using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class CLRMetaDataStreamTableEntry : ILocationSupport, IRawDataSupport
    {

        internal CLRMetaDataStreamTableEntry(CLRMetaDataStreamTable streamTable, StreamLocation streamLocation, uint offset, uint size, string name)
        {
            Table = streamTable;
            Location = streamLocation;
            Offset = offset;
            Size = size;
            Name = name;
        }

        #region Methods

        public override string ToString()
        {
            return Name;
        }

        public byte[] GetBytes()
        {
            Stream stream = Table.MetaData.Content.Section.Sections.Reader.GetStream();
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

        public StreamLocation Location
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

    public class CLRMetaDataStreamTable : ILocationSupport, IRawDataSupport, IEnumerable<CLRMetaDataStreamTableEntry>
    {

        private CLRMetaData meta_data;
        private List<CLRMetaDataStreamTableEntry> streams;
        private StreamLocation location;

        public CLRMetaDataStreamTable(CLRMetaData metaData)
        {
            meta_data = metaData;
            streams = new List<CLRMetaDataStreamTableEntry>();

            long offset = meta_data.Header.Location.Offset + meta_data.Header.Location.Size;

            LoadTable(offset);

            long size = streams.Sum(entry => entry.Location.Size);
            location = new StreamLocation(offset,size);
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

        public byte[] GetBytes()
        {
            Stream stream = meta_data.Content.Section.Sections.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        private void LoadTable(long baseOffset)
        {          
            Stream stream = meta_data.Content.Section.Sections.Reader.GetStream();

            stream.Seek(baseOffset,SeekOrigin.Begin);

            long offset = baseOffset;

            for(int i = 0; i < meta_data.Header.Streams; i++)
            {
                long size = 0;

                uint stream_offset = Utils.ReadUInt32(stream);
                size += 4;

                uint stream_size = Utils.ReadUInt32(stream);
                size += 4;

                StringBuilder name = new StringBuilder();

                while (true)
                {
                    int b = stream.ReadByte();
                    size += 1;

                    if (b <= 0)
                        break;

                    name.Append((char)b);
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

                StreamLocation entry_location = new StreamLocation(offset,size);
                CLRMetaDataStreamTableEntry entry = new CLRMetaDataStreamTableEntry(this,entry_location,stream_offset,stream_size,name.ToString());

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

        public StreamLocation Location
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
                if (index < 0 || index > (streams.Count - 1))
                    return null;

                return streams[index];
            }
        }

        public CLRMetaDataStreamTableEntry this[string name]
        {
            get
            {
                CLRMetaDataStreamTableEntry stream = streams.FirstOrDefault(strm => String.Compare(name,strm.Name,StringComparison.OrdinalIgnoreCase) == 0);

                return stream;
            }
        }

        #endregion

    }

}
