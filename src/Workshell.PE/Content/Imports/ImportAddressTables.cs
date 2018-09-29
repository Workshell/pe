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

using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public sealed class ImportAddressTables: ImportAddressTablesBase<ImportAddressTable, ImportAddressTableEntry>
    {
        internal ImportAddressTables(PortableExecutableImage image, DataDirectory dataDirectory, Location location, Tuple<uint, ulong[], ImportDirectoryEntryBase>[] tables) : base(image, dataDirectory, location, tables, false)
        {
        }

        #region Static Methods

        public static async Task<ImportAddressTables> GetLookupTableAsync(PortableExecutableImage image, ImportDirectory directory = null)
        {
            return await GetTableAsync(image, directory, (entry) => entry.OriginalFirstThunk).ConfigureAwait(false);
        }

        public static async Task<ImportAddressTables> GetAddressTableAsync(PortableExecutableImage image, ImportDirectory directory = null)
        {
            return await GetTableAsync(image, directory, (entry) => entry.FirstThunk).ConfigureAwait(false);
        }

        private static async Task<ImportAddressTables> GetTableAsync(PortableExecutableImage image, ImportDirectory directory, Func<ImportDirectoryEntry, uint> thunkHandler)
        {
            if (directory == null)
                directory = await ImportDirectory.GetAsync(image).ConfigureAwait(false);

            var calc = image.GetCalculator();          
            var stream = image.GetStream();
            var tables = new List<Tuple<uint, ulong[], ImportDirectoryEntryBase>>();

            foreach (var dirEntry in directory)
            {
                var thunk = thunkHandler(dirEntry);

                if (thunk == 0)
                    continue;

                var entries = new List<ulong>();
                var offset = calc.RVAToOffset(thunk);

                stream.Seek(offset.ToInt64(), SeekOrigin.Begin);

                while (true)
                {
                    var entry = (!image.Is64Bit ? await stream.ReadUInt32Async().ConfigureAwait(false) : await stream.ReadUInt64Async().ConfigureAwait(false));

                    entries.Add(entry);

                    if (entry == 0)
                        break;
                }

                var table = new Tuple<uint, ulong[], ImportDirectoryEntryBase>(thunk, entries.ToArray(), dirEntry);

                tables.Add(table);
            }

            var rva = 0u;

            if (tables.Count > 0)
                rva = tables.MinBy(table => table.Item1).Item1;

            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
            var va = imageBase + rva;
            var fileOffset = calc.RVAToOffset(rva);
            var fileSize = 0ul;

            foreach (var table in tables)
            {
                var size = table.Item2.Length * (!image.Is64Bit ? sizeof(uint) : sizeof(ulong));

                fileSize += size.ToUInt32();
            }

            var section = calc.RVAToSection(rva);
            var location = new Location(fileOffset, rva, va, fileSize, fileSize, section);
            var result = new ImportAddressTables(image, directory.DataDirectory, location, tables.ToArray());

            return result;
        }

        #endregion
    }
}
