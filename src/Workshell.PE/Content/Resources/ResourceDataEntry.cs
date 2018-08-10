using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Content;
using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE.Resources
{
    public sealed class ResourceDataEntry : DataContent
    {
        private ResourceData _data;

        internal ResourceDataEntry(PortableExecutableImage image, DataDirectory dataDirectory, Location location, ResourceDirectoryEntry directoryEntry) : base(image, dataDirectory, location)
        {
            _data = null;

            DirectoryEntry = directoryEntry;
        }

        #region Methods

        public ResourceData GetData()
        {
            if (_data == null)
            {
                var calc = Image.GetCalculator();
                var rva = OffsetToData;
                var va = Image.NTHeaders.OptionalHeader.ImageBase + rva;
                var offset = calc.RVAToOffset(rva);
                var size = Size;
                var section = calc.RVAToSection(rva);
                var location = new Location(offset, rva, va, size, size, section);

                _data = new ResourceData(Image, DataDirectory, location, this);
            }

            return _data;
        }

        internal async Task LoadAsync()
        {
            var stream = Image.GetStream();

            stream.Seek(Location.FileOffset.ToInt64(), SeekOrigin.Begin);

            try
            {
                var entry = await stream.ReadStructAsync<IMAGE_RESOURCE_DATA_ENTRY>().ConfigureAwait(false);

                OffsetToData = entry.OffsetToData;
                Size = entry.Size;
                CodePage = entry.CodePage;
                Reserved = entry.Reserved;
            }
            catch (Exception ex)
            {
                throw new PortableExecutableImageException(Image, "Could not read resource data entry from stream.", ex);
            }
        }

        #endregion

        #region Properties

        public ResourceDirectoryEntry DirectoryEntry { get; }

        [FieldAnnotation("Offset to Data")]
        public uint OffsetToData { get; private set; }

        [FieldAnnotation("Size")]
        public uint Size { get; private set; }

        [FieldAnnotation("Code Page")]
        public uint CodePage { get; private set; }

        [FieldAnnotation("Reserved")]
        public uint Reserved { get; private set; }

        #endregion
    }
}
