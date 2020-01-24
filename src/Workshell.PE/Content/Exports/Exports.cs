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
        private sealed class ExportInfo
        {
            #if DEBUG
            public override string ToString()
            {
                return $"Address: 0x{Address:X8}, Ordinal: 0x{Ordinal:X4}, Name Address: 0x{NameAddress:X8}, Name: {Name}, Forward Name: {ForwardName}";
            }
            #endif

            #region Properties

            public uint Address { get; set; }
            public uint Ordinal { get; set; }
            public uint NameAddress { get; set; }
            public string Name { get; set; }
            public string ForwardName { get; set; }

            #endregion
        }

        private readonly Export[] _exports;

        internal Exports(IEnumerable<Export> exports, ExportDirectory directory, ExportTable<uint> functionAddresses, ExportTable<uint> nameAddresses, ExportTable<ushort> ordinals)
        {
            _exports = exports.ToArray();

            Directory = directory;
            FunctionAddressTable = functionAddresses;
            NameAddressTable = nameAddresses;
            NameOrdinalsTable = ordinals;
            Count = _exports.Length;
        }

        #region Static Methods

        public static Exports Get(PortableExecutableImage image)
        {
            return GetAsync(image).GetAwaiter().GetResult();
        }

        public static Exports Get(PortableExecutableImage image, ExportDirectory directory, ExportTable<uint> functionAddresses, ExportTable<uint> nameAddresses, ExportTable<ushort> ordinals)
        {
            return GetAsync(image, directory, functionAddresses, nameAddresses, ordinals).GetAwaiter().GetResult();
        }

        public static async Task<Exports> GetAsync(PortableExecutableImage image)
        {
            var directory = await ExportDirectory.GetAsync(image).ConfigureAwait(false);

            if (directory == null)
            {
                return null;
            }

            var functionAddresses = await ExportTable.GetFunctionAddressTableAsync(image, directory).ConfigureAwait(false);

            if (functionAddresses == null)
            {
                return null;
            }

            var nameAddresses = await ExportTable.GetNameAddressTableAsync(image, directory).ConfigureAwait(false);

            if (nameAddresses == null)
            {
                return null;
            }

            var ordinals = await ExportTable.GetOrdinalTableAsync(image, directory).ConfigureAwait(false);

            if (ordinals == null)
            {
                return null;
            }

            return await GetAsync(image, directory, functionAddresses, nameAddresses, ordinals).ConfigureAwait(false);
        }

        public static async Task<Exports> GetAsync(PortableExecutableImage image, ExportDirectory directory, ExportTable<uint> functionAddresses, ExportTable<uint> nameAddresses, ExportTable<ushort> ordinals)
        {
            var calc = image.GetCalculator();
            var stream = image.GetStream();
            var dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.ExportTable];
            var infos = new List<ExportInfo>();
            var currentOrdinal = directory.Base;

            foreach (var address in functionAddresses)
            {
                var info = new ExportInfo()
                {
                    Address = address,
                    Ordinal = currentOrdinal
                };

                if (address >= dataDirectory.VirtualAddress && address <= (dataDirectory.VirtualAddress + dataDirectory.Size))
                {
                    var offset = calc.RVAToOffset(address).ToInt64();

                    info.ForwardName = await GetStringAsync(stream, offset).ConfigureAwait(false);
                }

                infos.Add(info);

                currentOrdinal++;
            }

            for (var i = 0; i < nameAddresses.Count; i++)
            {
                var nameAddress = nameAddresses[i];
                var ordinal = ordinals[i];
                var offset = calc.RVAToOffset(nameAddress).ToInt64();
                var info = infos[ordinal];

                info.NameAddress = nameAddress;
                info.Name = await GetStringAsync(stream, offset);
            }

            var exports = new List<Export>(infos.Count);

            foreach (var info in infos)
            {
                var export = new Export(info.Address, info.Name, info.Ordinal, info.ForwardName);

                exports.Add(export);
            }

            var result = new Exports(exports.OrderBy(e => e.Ordinal), directory, functionAddresses, nameAddresses, ordinals);

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
            {
                yield return export;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public ExportDirectory Directory { get; }
        public ExportTable<uint> FunctionAddressTable { get; }
        public ExportTable<uint> NameAddressTable { get; }
        public ExportTable<ushort> NameOrdinalsTable { get; }

        public int Count { get; }
        public Export this[int index] => _exports[index];

        #endregion
    }
}
