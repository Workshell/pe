using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Content;

namespace Workshell.PE.Resources
{
    public sealed class ResourceData : DataContent
    {
        internal ResourceData(PortableExecutableImage image, DataDirectory dataDirectory, Location location, ResourceDataEntry entry) : base(image, dataDirectory, location)
        {
            Entry = entry;
        }

        #region Methods

        public void CopyTo(Stream stream)
        {
            CopyToAsync(stream).GetAwaiter().GetResult();
        }

        public async Task CopyToAsync(Stream stream)
        {
            var buffer = await GetBytesAsync().ConfigureAwait(false);

            await stream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
        }

        #endregion

        #region Properties

        public ResourceDataEntry Entry { get; }

        #endregion
    }
}
