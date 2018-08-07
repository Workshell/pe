using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public abstract class ExceptionTableEntry : ISupportsLocation, ISupportsBytes
    {
        protected internal ExceptionTableEntry(PortableExecutableImage image, Location location)
        {
            Image = image;
            Location = location;
        }

        #region Methods

        public byte[] GetBytes()
        {
            return GetBytesAsync().GetAwaiter().GetResult();
        }

        public async Task<byte[]> GetBytesAsync()
        {
            var stream = Image.GetStream();
            var buffer = await stream.ReadBytesAsync(Location).ConfigureAwait(false);

            return buffer;
        }

        #endregion

        #region Properties

        public Location Location { get; }
        protected PortableExecutableImage Image { get; }

        #endregion
    }
}
