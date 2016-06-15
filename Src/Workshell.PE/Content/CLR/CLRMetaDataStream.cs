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

        internal CLRMetaDataStream(CLRMetaDataStreamCollection streams, CLRMetaDataStreamTableEntry tableEntry, ulong imageBase)
        {
            Streams = streams;
            TableEntry = tableEntry;

            LocationCalculator calc = streams.MetaData.Content.DataDirectory.Directories.Reader.GetCalculator();
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
            Stream stream = Streams.MetaData.Content.DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,Location);

            return buffer;
        }

        #endregion

        #region Properties

        public CLRMetaDataStreamCollection Streams
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

    public sealed class CLRMetaDataStreamCollection : IEnumerable<CLRMetaDataStream>, IReadOnlyCollection<CLRMetaDataStream>, ISupportsLocation, ISupportsBytes
    {

        private CLRMetaData meta_data;
        private List<CLRMetaDataStream> streams;
        private Location location;

        internal CLRMetaDataStreamCollection(CLRMetaData metaData, ulong imageBase)
        {
            meta_data = metaData;
            streams = new List<CLRMetaDataStream>();

            foreach(CLRMetaDataStreamTableEntry entry in metaData.StreamTable)
            {
                CLRMetaDataStream stream = new CLRMetaDataStream(this,entry,imageBase);

                streams.Add(stream);
            }

            LocationCalculator calc = metaData.Content.DataDirectory.Directories.Reader.GetCalculator();
            ulong offset = 0;

            if (streams.Count > 0)
            {
                CLRMetaDataStream stream = streams.MinBy(s => s.Location.FileOffset);

                offset = stream.Location.FileOffset;
            }

            ulong size = 0;

            foreach (var stream in streams)
                size += stream.Location.FileSize;

            uint rva = calc.OffsetToRVA(offset);
            ulong va = imageBase + rva;

            location = new Location(offset,rva,va,size,size);
        }

        #region Methods

        public IEnumerator<CLRMetaDataStream> GetEnumerator()
        {
            return streams.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("Stream Count: {0}",streams.Count);
        }

        public byte[] GetBytes()
        {
            Stream stream = meta_data.Content.DataDirectory.Directories.Reader.GetStream();
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
                return streams.Count;
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
