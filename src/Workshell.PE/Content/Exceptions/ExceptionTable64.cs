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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE.Content
{
    public sealed class ExceptionTable64 : ExceptionTable
    {
        private ExceptionTable64(PortableExecutableImage image, DataDirectory dataDirectory, Location location, ExceptionTableEntry[] entries) : base(image, dataDirectory, location, entries)
        {
        }

        #region Static Methods

        internal static async Task<ExceptionTable> GetAsync(PortableExecutableImage image, DataDirectory dataDirectory, Location location)
        {
            var calc = image.GetCalculator();
            var stream = image.GetStream();
            var offset = calc.RVAToOffset(dataDirectory.VirtualAddress);
            var rva = dataDirectory.VirtualAddress;      

            stream.Seek(offset, SeekOrigin.Begin);

            var entrySize = Utils.SizeOf<IMAGE_RUNTIME_FUNCTION_64>();
            var entries = new List<ExceptionTableEntry>();

            while (true)
            {
                var entryData = await stream.ReadStructAsync<IMAGE_RUNTIME_FUNCTION_64>(entrySize).ConfigureAwait(false);

                if (entryData.StartAddress == 0 && entryData.EndAddress == 0)
                    break;

                var entryLocation = new Location(image, offset, rva, image.NTHeaders.OptionalHeader.ImageBase + rva, entrySize.ToUInt32(), entrySize.ToUInt32());
                var entry = new ExceptionTableEntry64(image, entryLocation, entryData);

                entries.Add(entry);

                offset += entrySize.ToUInt32();
                rva += entrySize.ToUInt32();
            }

            var table = new ExceptionTable64(image, dataDirectory, location, entries.ToArray());

            return table;
        }

        #endregion
    }
}
