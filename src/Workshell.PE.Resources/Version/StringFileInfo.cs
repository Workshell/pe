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

namespace Workshell.PE.Resources.Version
{
    public sealed class StringFileInfo : IEnumerable<VersionStringTable>
    {
        private readonly VersionStringTable[] _tables;

        private StringFileInfo(string key, VersionStringTable[] tables)
        {
            _tables = tables;

            Key = key;
            Count = _tables.Length;
        }

        #region Static Methods

        internal static async Task<StringFileInfo> LoadAsync(Stream stream)
        {
            var count = 3 * sizeof(ushort);
            var len = await stream.ReadUInt16Async().ConfigureAwait(false);
            var valLen = await stream.ReadUInt16Async().ConfigureAwait(false);
            var type = await stream.ReadUInt16Async().ConfigureAwait(false);
            var key = await stream.ReadUnicodeStringAsync().ConfigureAwait(false);

            count += (key.Length + 1) * sizeof(ushort);
            count += await VersionResource.AlignWordBoundaryAsync(stream).ConfigureAwait(false);

            var tables = new List<VersionStringTable>();
            var buffer = await stream.ReadBytesAsync(len - count).ConfigureAwait(false);

            using (var mem = new MemoryStream(buffer))
            {
                while (mem.Position < mem.Length)
                {
                    var table = await LoadTableAsync(mem).ConfigureAwait(false);

                    tables.Add(table);
                }
            }

            var result = new StringFileInfo(key, tables.ToArray());

            return result;
        }

        private static async Task<VersionStringTable> LoadTableAsync(Stream stream)
        {
            var count = 3 * sizeof(ushort);
            var len = await stream.ReadUInt16Async().ConfigureAwait(false);
            var valLen = await stream.ReadUInt16Async().ConfigureAwait(false);
            var type = await stream.ReadUInt16Async().ConfigureAwait(false);
            var key = await stream.ReadUnicodeStringAsync().ConfigureAwait(false);

            count += (key.Length + 1) * sizeof(ushort);
            count += await VersionResource.AlignWordBoundaryAsync(stream).ConfigureAwait(false);

            var strings = new List<VersionString>();
            var buffer = await stream.ReadBytesAsync(len - count).ConfigureAwait(false);

            using (var mem = new MemoryStream(buffer))
            {
                while (mem.Position < mem.Length)
                {
                    var str = await LoadStringAsync(mem).ConfigureAwait(false);

                    strings.Add(str);

                    if (mem.Position % 4 != 0 && mem.Position < mem.Length)
                        await VersionResource.AlignWordBoundaryAsync(mem).ConfigureAwait(false);
                }
            }

            var result = new VersionStringTable(key, strings.ToArray());

            return result;
        }

        private static async Task<VersionString> LoadStringAsync(Stream stream)
        {
            var len = await stream.ReadUInt16Async().ConfigureAwait(false);
            var valLen = await stream.ReadUInt16Async().ConfigureAwait(false);
            var type = await stream.ReadUInt16Async().ConfigureAwait(false);
            var key = await stream.ReadUnicodeStringAsync().ConfigureAwait(false);

            await VersionResource.AlignWordBoundaryAsync(stream).ConfigureAwait(false);

            var value = await stream.ReadUnicodeStringAsync(valLen).ConfigureAwait(false);
            var result = new VersionString(key, value);

            return result;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"Key: {Key}, Tables: {Count:n0}";
        }

        public IEnumerator<VersionStringTable> GetEnumerator()
        {
            foreach (var table in _tables)
                yield return table;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public string Key { get; }
        public int Count { get; }
        public VersionStringTable this[int index] => _tables[index];
        public VersionStringTable this[string key] => _tables.FirstOrDefault(t => string.Compare(key, t.Key, StringComparison.OrdinalIgnoreCase) == 0);

        #endregion
    }
}
