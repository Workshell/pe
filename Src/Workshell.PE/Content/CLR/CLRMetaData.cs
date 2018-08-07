using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public sealed class CLRMetaData : ISupportsLocation, ISupportsBytes
    {
        private readonly PortableExecutableImage _image;

        internal CLRMetaData(PortableExecutableImage image, Location location, CLRMetaDataHeader header, CLRMetaDataStreamTable streamTable,
            CLRMetaDataStreams streams)
        {
            _image = image;

            Location = location;
            Header = header;
            StreamTable = streamTable;
            Streams = streams;
        }

        #region Static Methods

        internal static async Task<CLRMetaData> GetAsync(PortableExecutableImage image, CLRHeader header)
        {
            var calc = image.GetCalculator();
            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
            var rva = header.MetaDataAddress;
            var va = imageBase + rva;
            var offset = calc.RVAToOffset(rva);
            var size = header.MetaDataSize;
            var section = calc.RVAToSection(rva);
            var location = new Location(offset, rva, va, size, size, section);
            var metaDataHeader = await CLRMetaDataHeader.LoadAsync(image, location).ConfigureAwait(false);
            var metaDataStreamTable = await CLRMetaDataStreamTable.LoadAsync(image, metaDataHeader).ConfigureAwait(false);
            var metaDataStreams = await CLRMetaDataStreams.LoadAsync(image, location, metaDataStreamTable).ConfigureAwait(false);
            var metaData = new CLRMetaData(image, location, metaDataHeader, metaDataStreamTable, metaDataStreams);

            return metaData;
        }

        #endregion

        #region Methods

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
        public CLRMetaDataHeader Header { get; }
        public CLRMetaDataStreamTable StreamTable { get; }
        public CLRMetaDataStreams Streams { get; }

        #endregion
    }
}
