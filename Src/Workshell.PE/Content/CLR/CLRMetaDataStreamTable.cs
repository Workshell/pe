using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public sealed class CLRMetaDataStreamTable : IEnumerable<CLRMetaDataStreamTableEntry>, ISupportsLocation, ISupportsBytes
    {
        private readonly PortableExecutableImage _image;
        private readonly CLRMetaDataStreamTableEntry[] _entries;

        internal CLRMetaDataStreamTable(PortableExecutableImage image, Location location, CLRMetaDataStreamTableEntry[] entries)
        {
            _image = image;
            _entries = entries;

            Location = location;
            Count = _entries.Length;
        }

        #region Static Methods

        public static async Task<CLRMetaDataStreamTable> LoadAsync(PortableExecutableImage image, CLRMetaDataHeader header)
        {
            try
            {
                var calc = image.GetCalculator();
                var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
                var offset = header.Location.FileOffset + header.Location.FileSize;
                var rva = calc.OffsetToRVA(offset);
                var va = imageBase + rva;
                var entries = await LoadTableAsync(image, header, offset, imageBase).ConfigureAwait(false);
                ulong size = 0;

                foreach (var strm in entries)
                    size += strm.Location.FileSize;

                var section = calc.RVAToSection(rva);
                var location = new Location(offset,rva,va,size,size,section);

                return new CLRMetaDataStreamTable(image, location, entries);
            }
            catch (Exception ex)
            {
                throw new PortableExecutableImageException(image, "Could not read CLR meta-data streams table from stream.", ex);
            }
        }

        private static async Task<CLRMetaDataStreamTableEntry[]> LoadTableAsync(PortableExecutableImage image, CLRMetaDataHeader header, ulong baseOffset, ulong imageBase)
        {
            var stream = image.GetStream();

            stream.Seek(baseOffset.ToInt64(),SeekOrigin.Begin);

            var entries = new List<CLRMetaDataStreamTableEntry>();
            var offset = baseOffset;

            for(var i = 0; i < header.Streams; i++)
            {
                var size = 0U;
                var streamOffset = await stream.ReadUInt32Async().ConfigureAwait(false);

                size += sizeof(uint);

                var streamSize = await stream.ReadUInt32Async().ConfigureAwait(false);

                size += sizeof(uint);

                var streamName = new StringBuilder(256);

                while (true)
                {
                    var b = await stream.ReadByteAsync().ConfigureAwait(false);

                    size += 1;

                    if (b <= 0)
                        break;

                    streamName.Append((char)b);
                }

                while (true)
                {
                    if (stream.Position % 4 != 0)
                    {
                        await stream.ReadByteAsync().ConfigureAwait(false);
                        size += 1;
                    }
                    else
                    {
                        break;
                    }
                }

                var calc = image.GetCalculator();
                var rva = calc.OffsetToRVA(offset);
                var va = imageBase + rva;
                var section = calc.RVAToSection(rva);
                var location = new Location(offset, rva, va, size, size, section);
                var entry = new CLRMetaDataStreamTableEntry(image, location, streamOffset, streamSize, streamName.ToString());

                entries.Add(entry);

                offset += size;
            }

            return entries.ToArray();
        }

        #endregion

        #region Methods

        public IEnumerator<CLRMetaDataStreamTableEntry> GetEnumerator()
        {
            foreach (var entry in _entries)
                yield return entry;
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
        public CLRMetaDataStreamTableEntry this[int index] => _entries[index];
        public CLRMetaDataStreamTableEntry this[string name] => _entries.SingleOrDefault(entry => string.Compare(name, entry.Name, StringComparison.OrdinalIgnoreCase) == 0);

        #endregion
    }
}
