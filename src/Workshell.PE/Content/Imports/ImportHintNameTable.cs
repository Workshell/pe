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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public sealed class ImportHintNameTable : ImportHintNameTableBase<ImportHintNameEntry>
    {
        internal ImportHintNameTable(PortableExecutableImage image, DataDirectory dataDirectory, Location location, IEnumerable<Tuple<long, uint, ushort, string, bool>> entries) : base(image, dataDirectory, location, entries, false)
        {
        }

        #region Static Methods

        public static ImportHintNameTable Get(PortableExecutableImage image, ImportDirectory directory = null)
        {
            return GetAsync(image, directory).GetAwaiter().GetResult();
        }

        public static async Task<ImportHintNameTable> GetAsync(PortableExecutableImage image, ImportDirectory directory = null)
        {
            if (directory == null)
            {
                directory = await ImportDirectory.GetAsync(image).ConfigureAwait(false);
            }

            var entries = new Dictionary<uint, Tuple<long, uint, ushort, string, bool>>();
            var ilt = await ImportAddressTables.GetLookupTablesAsync(image, directory).ConfigureAwait(false);
            var calc = image.GetCalculator();
            var stream = image.GetStream();

            foreach (var table in ilt)
            {
                foreach (var entry in table)
                {
                    if (entry.Address == 0 || entries.ContainsKey(entry.Address))
                    {
                        continue;
                    }

                    if (!entry.IsOrdinal)
                    {
                        var offset = calc.RVAToOffset(entry.Address);
                        var size = 0u;
                        var isPadded = false;
                        ushort hint = 0;
                        var name = new StringBuilder(256);

                        stream.Seek(offset, SeekOrigin.Begin);

                        hint = await stream.ReadUInt16Async().ConfigureAwait(false);
                        size += sizeof(ushort);

                        while (true)
                        {
                            var b = await stream.ReadByteAsync().ConfigureAwait(false);

                            size++;

                            if (b <= 0)
                            {
                                break;
                            }

                            name.Append((char)b);
                        }

                        if (size % 2 != 0)
                        {
                            isPadded = true;
                            size++;
                        }

                        var tuple = new Tuple<long, uint, ushort, string, bool>(offset, size, hint, name.ToString(), isPadded);

                        entries.Add(entry.Address, tuple);
                    }
                }
            }

            Location location;

            if (entries.Count > 0)
            {
                var firstEntry = entries.Values.MinBy(tuple => tuple.Item1);
                var lastEntry = entries.Values.MaxBy(tuple => tuple.Item1);
                var tableOffset = firstEntry.Item1;
                var tableSize = ((lastEntry.Item1 + lastEntry.Item2) - tableOffset).ToUInt32();
                var tableRVA = calc.OffsetToRVA(tableOffset);
                var tableVA = image.NTHeaders.OptionalHeader.ImageBase + tableRVA;
                var tableSection = calc.RVAToSection(tableRVA);

                location = new Location(image, tableOffset, tableRVA, tableVA, tableSize, tableSize, tableSection);
            }
            else
            {
                location = new Location(image, 0, 0, 0, 0, 0, null);
            }

            var result = new ImportHintNameTable(image, directory.DataDirectory, location, entries.Values.ToArray());

            return result;
        }

        #endregion
    }
}
