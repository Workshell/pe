using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public class DataContent : ISupportsLocation, ISupportsBytes
    {
        public DataContent(PortableExecutableImage image, DataDirectory dataDirectory, Location location)
        {
            DataDirectory = dataDirectory;
            Location = location;
            Image = image;
        }

        #region Methods

        public byte[] GetBytes()
        {
            return GetBytesAsync().GetAwaiter().GetResult();
        }

        public async Task<byte[]> GetBytesAsync()
        {
            var stream = Image.GetStream();
            var result = await stream.ReadBytesAsync(Location).ConfigureAwait(false);

            return result;
        }

        #endregion

        #region Properties

        public DataDirectory DataDirectory { get; }
        public Location Location { get; }
        protected PortableExecutableImage Image { get; }

        #endregion
    }
}
