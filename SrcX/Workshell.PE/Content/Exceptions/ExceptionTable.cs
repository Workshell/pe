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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Content
{
    public abstract class ExceptionTable : DataContent, IEnumerable<ExceptionTableEntry>
    {
        private readonly ExceptionTableEntry[] _entries;

        protected internal ExceptionTable(PortableExecutableImage image, DataDirectory dataDirectory, Location location, ExceptionTableEntry[] entries) : base(image, dataDirectory, location)
        {
            _entries = entries;

            Count = _entries.Length;
        }

        #region Static Methods

        public static async Task<ExceptionTable> GetAsync(PortableExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.ExceptionTable))
                return null;

            var dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.ExceptionTable];

            if (DataDirectory.IsNullOrEmpty(dataDirectory))
                return null;

            var calc = image.GetCalculator();
            var section = calc.RVAToSection(dataDirectory.VirtualAddress);
            var fileOffset = calc.RVAToOffset(section, dataDirectory.VirtualAddress);
            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;           
            var location = new Location(image, fileOffset, dataDirectory.VirtualAddress, imageBase + dataDirectory.VirtualAddress, dataDirectory.Size, dataDirectory.Size, section);
            ExceptionTable table;

            if (image.Is64Bit)
            {
                table = await ExceptionTable64.GetAsync(image, dataDirectory, location).ConfigureAwait(false);
            }
            else
            {
                table = await ExceptionTable32.GetAsync(image, dataDirectory, location).ConfigureAwait(false);
            }

            return table;
        }

        #endregion

        #region Methods

        public IEnumerator<ExceptionTableEntry> GetEnumerator()
        {
            foreach (var function in _entries)
                yield return function;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
        
        #region Properties

        public int Count { get; }
        public ExceptionTableEntry this[int index] => _entries[index];

        #endregion
    }
}
