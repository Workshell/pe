using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public sealed class CLRMetaDataStream : ISupportsLocation, ISupportsBytes
    {
        private readonly PortableExecutableImage _image;

        internal CLRMetaDataStream(PortableExecutableImage image, Location mdLocation, ulong imageBase, CLRMetaDataStreamTableEntry tableEntry)
        {
            _image = image;

            var calc = image.GetCalculator();
            var offset = mdLocation.FileOffset + tableEntry.Offset;
            var rva = calc.OffsetToRVA(offset);
            var va = imageBase + rva;

            Location = new Location(calc, offset, rva, va, tableEntry.Size, tableEntry.Size);
            TableEntry = tableEntry;
            Name = tableEntry.Name;
        }

        #region Methods

        public override string ToString()
        {
            return Name;
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

        public Stream GetStream()
        {
            var buffer = GetBytes();

            return new MemoryStream(buffer);
        }

        public async Task<Stream> GetStreamAsync()
        {
            var buffer = await GetBytesAsync().ConfigureAwait(false);

            return new MemoryStream(buffer);
        }

        public long CopyTo(Stream stream, int bufferSize = 4096)
        {
            return CopyToAsync(stream, bufferSize).GetAwaiter().GetResult();
        }

        public async Task<long> CopyToAsync(Stream stream, int bufferSize = 4096)
        {
            if (bufferSize < 1)
                bufferSize = 4096;

            var count = 0L;

            using (var mem = await GetStreamAsync().ConfigureAwait(false))
            {
                var buffer = new byte[bufferSize];
                var numRead = await mem.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

                while (numRead > 0)
                {
                    await stream.WriteAsync(buffer, 0, numRead).ConfigureAwait(false);

                    count += numRead;
                    numRead = await mem.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                }
            }

            return count;
        }

        #endregion

        #region Properties

        public Location Location { get; }
        public CLRMetaDataStreamTableEntry TableEntry { get; }
        public string Name { get; }

        #endregion
    }
}
