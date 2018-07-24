using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public sealed class CLRMetaDataStreamTableEntry : ISupportsLocation, ISupportsBytes
    {
        private readonly PortableExecutableImage _image;

        internal CLRMetaDataStreamTableEntry(PortableExecutableImage image, Location location, uint offset, uint size, string name)
        {
            _image = image;

            Location = location;
            Offset = offset;
            Size = size;
            Name = name;
        }

        #region Methods

        public override string ToString()
        {
            return $"Offset: 0x{Offset:X8}, Size: {Size}, Name: {Name}";
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
        public uint Offset { get; }
        public uint Size { get; }
        public string Name { get; }

        #endregion
    }
}
