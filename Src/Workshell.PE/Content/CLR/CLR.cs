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
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Content
{
    public sealed class CLR : DataContent
    {
        private CLR(PortableExecutableImage image, DataDirectory dataDirectory, Location location, CLRHeader header, CLRMetaData metaData) : base(image, dataDirectory, location)
        {
            Header = header;
            MetaData = metaData;
        }

        #region Static Methods

        public static CLR Get(PortableExecutableImage image)
        {
            return GetAsync(image).GetAwaiter().GetResult();
        }

        public static async Task<CLR> GetAsync(PortableExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.CLRRuntimeHeader))
                return null;

            var dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.CLRRuntimeHeader];

            if (DataDirectory.IsNullOrEmpty(dataDirectory))
                return null;

            var calc = image.GetCalculator();
            var section = calc.RVAToSection(dataDirectory.VirtualAddress);
            var fileOffset = calc.RVAToOffset(section, dataDirectory.VirtualAddress);
            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;         
            var location = new Location(fileOffset, dataDirectory.VirtualAddress, imageBase + dataDirectory.VirtualAddress, dataDirectory.Size, dataDirectory.Size, section);
            var header = await CLRHeader.GetAsync(image, location).ConfigureAwait(false);
            var metaData = await CLRMetaData.GetAsync(image, header).ConfigureAwait(false);

            return new CLR(image, dataDirectory, location, header, metaData);
        }

        #endregion

        #region Properties

        public CLRHeader Header { get; }
        public CLRMetaData MetaData { get; }

        #endregion
    }
}
