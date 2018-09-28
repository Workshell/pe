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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public sealed class Exports : IEnumerable<Export>
    {
        private readonly Export[] _exports;

        internal Exports(Export[] exports)
        {
            _exports = exports;

            Count = exports.Length;
        }

        #region Static Methods

        public static async Task<Exports> GetAsync(PortableExecutableImage image)
        {
            var directory = await ExportDirectory.GetAsync(image).ConfigureAwait(false);

            if (directory == null)
                return null;

            var functionAddresses = await ExportTable.GetFunctionAddressTableAsync(image, directory).ConfigureAwait(false);

            if (functionAddresses == null)
                return null;

            var nameAddresses = await ExportTable.GetNameAddressTableAsync(image, directory).ConfigureAwait(false);

            if (nameAddresses == null)
                return null;

            var ordinals = await ExportTable.GetOrdinalTableAsync(image, directory).ConfigureAwait(false);

            if (ordinals == null)
                return null;

            return await GetAsync(image, directory, functionAddresses, nameAddresses, ordinals).ConfigureAwait(false);
        }

        public static async Task<Exports> GetAsync(PortableExecutableImage image, ExportDirectory directory, ExportTable<uint> functionAddresses, ExportTable<uint> nameAddresses, ExportTable<ushort> ordinals)
        {
            var calc = image.GetCalculator();
            var stream = image.GetStream();
            var dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.ExportTable];
            var fullNameAddresses = new List<Tuple<int, uint, uint, ushort, string, string>>();

            for(var i = 0; i < nameAddresses.Count; i++)
            {
                var nameAddress = nameAddresses[i];
                var ordinal = ordinals[i];
                var functionAddress = functionAddresses[ordinal];
                var offset = calc.RVAToOffset(nameAddress).ToInt64();
                var name = await GetStringAsync(stream, offset).ConfigureAwait(false);
                var fwdName = string.Empty;

                if (functionAddress >= dataDirectory.VirtualAddress && functionAddress <= (dataDirectory.VirtualAddress + dataDirectory.Size))
                {
                    offset = calc.RVAToOffset(functionAddress).ToInt64();
                    fwdName = await GetStringAsync(stream, offset).ConfigureAwait(false);
                }

                var tuple = new Tuple<int, uint, uint, ushort, string, string>(i, functionAddress, nameAddress, ordinal, name, fwdName);

                fullNameAddresses.Add(tuple);
            }

            var exports = new List<Export>();

            for(var i = 0; i < functionAddresses.Count; i++)
            {
                var functionAddress = functionAddresses[i];
                var isOrdinal = fullNameAddresses.All(t => t.Item2 != functionAddress);

                if (!isOrdinal)
                {
                    var tuple = fullNameAddresses.First(t => t.Item2 == functionAddress);
                    var export = new Export(functionAddress, tuple.Item5, directory.Base + tuple.Item4, tuple.Item6);

                    exports.Add(export);
                }
                else
                {
                    var export = new Export(functionAddress, string.Empty, (directory.Base + i).ToUInt32(), string.Empty);

                    exports.Add(export);
                }
            }

            var result = new Exports(exports.OrderBy(e => e.Ordinal).ToArray());

            return result;
        }

        private static async Task<string> GetStringAsync(Stream stream, long offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);

            return await stream.ReadStringAsync().ConfigureAwait(false);
        }

        #endregion

        #region Methods

        public IEnumerator<Export> GetEnumerator()
        {
            foreach (var export in _exports)
                yield return export;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public int Count { get; }
        public Export this[int index] => _exports[index];

        #endregion
    }
}
