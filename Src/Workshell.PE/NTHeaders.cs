using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;

namespace Workshell.PE
{
    public sealed class NTHeaders : ISupportsLocation, ISupportsBytes
    {
        public const uint PE_MAGIC_MZ = 17744;

        private readonly PortableExecutableImage _image;

        internal NTHeaders(PortableExecutableImage image, ulong headerOffset, ulong imageBase, FileHeader fileHeader, OptionalHeader optHeader, DataDirectories dataDirs)
        {
            _image = image;

            var size = (4U + fileHeader.Location.FileSize + optHeader.Location.FileSize + dataDirs.Location.FileSize).ToUInt32();

            Location = new Location(image.GetCalculator(), headerOffset, headerOffset.ToUInt32(), imageBase + headerOffset, size, size);
            FileHeader = fileHeader;
            OptionalHeader = optHeader;
            DataDirectories = dataDirs;
        }

        #region Methods

        public override string ToString()
        {
            return "NT Headers";
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
        public FileHeader FileHeader { get; }
        public OptionalHeader OptionalHeader { get; }
        public DataDirectories DataDirectories { get; }

        #endregion
    }
}
