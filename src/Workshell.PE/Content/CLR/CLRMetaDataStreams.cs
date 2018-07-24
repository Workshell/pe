using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public sealed class CLRMetaDataStreams : IEnumerable<CLRMetaDataStream>, ISupportsLocation, ISupportsBytes
    {
        private readonly PortableExecutableImage _image;
        private readonly CLRMetaDataStream[] _streams;

        internal CLRMetaDataStreams(PortableExecutableImage image, Location location, CLRMetaDataStream[] streams)
        {
            _image = image;
            _streams = streams;

            Location = location;
            Count = _streams.Length;
        }
        
        #region Static Methods

        internal static Task<CLRMetaDataStreams> LoadAsync(PortableExecutableImage image, Location mdLocation, CLRMetaDataStreamTable streamTable)
        {
            try
            {
                var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
                var streams = new CLRMetaDataStream[streamTable.Count];

                for(var i = 0; i < streams.Length; i++)
                {
                    var entry = streamTable[i];
                    var stream = new CLRMetaDataStream(image, mdLocation, imageBase, entry);

                    streams[i] = stream;
                }

                uint rva = 0;
                ulong va = 0;
                ulong offset = 0;
                ulong size = 0;

                if (streams.Length > 0)
                {
                    var stream = streams.MinBy(s => s.Location.FileOffset);

                    rva = stream.Location.RelativeVirtualAddress;
                    va = stream.Location.VirtualAddress;
                    offset = stream.Location.FileOffset;
                }

                foreach (var stream in streams)
                    size += stream.Location.FileSize;

                var location = new Location(image.GetCalculator(), offset, rva, va, size, size);
                var result = new CLRMetaDataStreams(image, location, streams);

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new PortableExecutableImageException(image, "Could not load CLR meta-data streams from stream.", ex);
            }
        }

        #endregion

        #region Methods

        public IEnumerator<CLRMetaDataStream> GetEnumerator()
        {
            foreach (var strm in _streams)
                yield return strm;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public byte[] GetBytes()
        {
            return GetBytesAsync().GetAwaiter().GetResult();
        }

        public async Task<byte[]> GetBytesAsync()
        {
            var stream = _image.GetStream();
            var buffer = await stream.ReadBytesAsync(Location).ConfigureAwait(false);

            return buffer;
        }

        #endregion

        #region Properties

        public Location Location { get; }
        public int Count { get; }
        public CLRMetaDataStream this[int index] => _streams[index];
        public CLRMetaDataStream this[string name] => _streams.SingleOrDefault(strm => string.Compare(name, strm.Name, StringComparison.OrdinalIgnoreCase) == 0);

        #endregion
    }
}
