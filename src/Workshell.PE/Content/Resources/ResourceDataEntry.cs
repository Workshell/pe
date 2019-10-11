#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

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
                var location = new Location(Image, offset, rva, va, size, size, section);

                _data = new ResourceData(Image, DataDirectory, location, this);
            }

            return _data;
        }

        internal async Task LoadAsync()
        {
            var stream = Image.GetStream();

            stream.Seek(Location.FileOffset, SeekOrigin.Begin);

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

        [FieldAnnotation("Offset to Data", Order = 1)]
        public uint OffsetToData { get; private set; }

        [FieldAnnotation("Size", Order = 2)]
        public uint Size { get; private set; }

        [FieldAnnotation("Code Page", Order = 3)]
        public uint CodePage { get; private set; }

        [FieldAnnotation("Reserved", Order = 4)]
        public uint Reserved { get; private set; }

        #endregion
    }
}
