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
    public sealed class StringFileInfo : IEnumerable<VerStringTable>
    {
        private readonly VerStringTable[] _tables;

        internal StringFileInfo(string key, VerStringTable[] tables)
        {
            _tables = tables;

            Key = key;
            Count = _tables.Length;
        }

        #region Static Methods

        internal static async Task<StringFileInfo> LoadAsync(Stream stream)
        {
            var count = 0;
            var len = await stream.ReadUInt16Async().ConfigureAwait(false);
            var valLen = await stream.ReadUInt16Async().ConfigureAwait(false);
            var type = await stream.ReadUInt16Async().ConfigureAwait(false);

            count += 3 * sizeof(ushort);

            var key = await stream.ReadUnicodeStringAsync().ConfigureAwait(false);

            count += (key.Length + 1) * sizeof(ushort);

            if (stream.Position % 4 != 0)
            {
                await stream.ReadUInt16Async().ConfigureAwait(false);

                count += sizeof(ushort);
            }

            var tables = new List<VerStringTable>();
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

        private static async Task<VerStringTable> LoadTableAsync(Stream stream)
        {
            var count = 0;
            var len = await stream.ReadUInt16Async().ConfigureAwait(false);
            var valLen = await stream.ReadUInt16Async().ConfigureAwait(false);
            var type = await stream.ReadUInt16Async().ConfigureAwait(false);

            count += 3 * sizeof(ushort);

            var key = await stream.ReadUnicodeStringAsync().ConfigureAwait(false);

            count += (key.Length + 1) * sizeof(ushort);

            if (stream.Position % 4 != 0)
            {
                await stream.ReadUInt16Async().ConfigureAwait(false);

                count += sizeof(ushort);
            }

            var strings = new List<VerString>();
            var buffer = await stream.ReadBytesAsync(len - count).ConfigureAwait(false);

            using (var mem = new MemoryStream(buffer))
            {
                while (mem.Position < mem.Length)
                {
                    var str = await LoadStringAsync(mem).ConfigureAwait(false);

                    strings.Add(str);

                    if (mem.Position % 4 != 0 && mem.Position < mem.Length)
                        await mem.ReadUInt16Async().ConfigureAwait(false);
                }
            }

            var result = new VerStringTable(key, strings.ToArray());

            return result;
        }

        private static async Task<VerString> LoadStringAsync(Stream stream)
        {
            var len = await stream.ReadUInt16Async().ConfigureAwait(false);
            var valLen = await stream.ReadUInt16Async().ConfigureAwait(false);
            var type = await stream.ReadUInt16Async().ConfigureAwait(false);
            var key = await stream.ReadUnicodeStringAsync().ConfigureAwait(false);

            if (stream.Position % 4 != 0)
                await stream.ReadUInt16Async().ConfigureAwait(false);

            var value = await stream.ReadUnicodeStringAsync(valLen);
            var result = new VerString(key, value);

            return result;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"Key: {Key}, Tables: {Count:n0}";
        }

        public IEnumerator<VerStringTable> GetEnumerator()
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
        public VerStringTable this[int index] => _tables[index];
        public VerStringTable this[string key] => _tables.FirstOrDefault(t => string.Compare(key, t.Key, StringComparison.OrdinalIgnoreCase) == 0);

        #endregion
    }
}
