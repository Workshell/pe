using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{

    public abstract class ImportDirectoryEntryBase : ISupportsLocation, ISupportsBytes
    {
        private readonly PortableExecutableImage _image;

        protected internal ImportDirectoryEntryBase(PortableExecutableImage image, Location location, bool isDelayed)
        {
            _image = image;

            Location = location;
            IsDelayed = isDelayed;
        }

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
        public bool IsDelayed { get; }
        public abstract uint Name { get; }

        #endregion
    }
}
