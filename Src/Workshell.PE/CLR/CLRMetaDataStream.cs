using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.MoreLinq;

namespace Workshell.PE
{

    public class CLRMetaDataStream : ILocationSupport, IRawDataSupport
    {

        internal CLRMetaDataStream(CLRMetaDataStreams streams, CLRMetaDataStreamTableEntry tableEntry)
        {
            long offset = streams.MetaData.Location.Offset + tableEntry.Offset;

            Streams = streams;
            TableEntry = tableEntry;
            Location = new StreamLocation(offset,tableEntry.Size);
        }

        #region Methods

        public override string ToString()
        {
            return Name;
        }

        public byte[] GetBytes()
        {
            Stream stream = Streams.MetaData.Content.Section.Sections.Reader.GetStream();
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

        public StreamLocation Location
        {
            get;
            private set;
        }

        public string Name
        {
            get
            {
                return TableEntry.Name;
            }
        }

        #endregion

    }

    public class CLRMetaDataStreams : ILocationSupport, IRawDataSupport, IEnumerable<CLRMetaDataStream>
    {

        private CLRMetaData meta_data;
        private StreamLocation location;
        private Dictionary<CLRMetaDataStreamTableEntry,CLRMetaDataStream> cache;

        public CLRMetaDataStreams(CLRMetaData metaData)
        {
            meta_data = metaData;
            location = null;
            cache = new Dictionary<CLRMetaDataStreamTableEntry,CLRMetaDataStream>();
        }

        #region Methods

        public IEnumerator<CLRMetaDataStream> GetEnumerator()
        {
            IEnumerable<CLRMetaDataStream> streams = meta_data.StreamTable.Select(entry => CreateStream(entry));

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

        private StreamLocation CreateLocation()
        {
            long entry_offset = 0;

            if (meta_data.StreamTable.Count > 0)
            {
                CLRMetaDataStreamTableEntry entry = meta_data.StreamTable.MinBy(e => e.Offset);

                entry_offset = entry.Offset;
            }

            long offset = meta_data.Location.Offset + entry_offset;
            long size = meta_data.StreamTable.Sum(entry => entry.Size);

            return new StreamLocation(offset,size);
        }

        private CLRMetaDataStream CreateStream(CLRMetaDataStreamTableEntry tableEntry)
        {
            if (cache.ContainsKey(tableEntry))
                return cache[tableEntry];

            CLRMetaDataStream stream = new CLRMetaDataStream(this,tableEntry);

            cache.Add(tableEntry,stream);

            return stream;
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
                if (location == null)
                    location = CreateLocation();

                return location;
            }
        }

        public int Count
        {
            get
            {
                return meta_data.StreamTable.Count;
            }
        }

        public CLRMetaDataStream this[int index]
        {
            get
            {
                CLRMetaDataStreamTableEntry entry = meta_data.StreamTable[index];

                return this[entry];
            }
        }

        public CLRMetaDataStream this[string name]
        {
            get
            {
                CLRMetaDataStreamTableEntry entry = meta_data.StreamTable[name];

                return this[entry];
            }
        }

        public CLRMetaDataStream this[CLRMetaDataStreamTableEntry tableEntry]
        {
            get
            {
                if (tableEntry == null)
                    return null;

                return CreateStream(tableEntry);
            }
        }

        #endregion

    }

}
