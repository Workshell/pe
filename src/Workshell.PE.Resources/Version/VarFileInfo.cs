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
    public sealed class VarFileInfo : IEnumerable<VersionVariable>
    {
        private readonly VersionVariable[] _vars;

        private VarFileInfo(string key, VersionVariable[] vars)
        {
            _vars = vars;

            Key = key;
            Count = _vars.Length;
        }

        #region Static Methods

        internal static async Task<VarFileInfo> LoadAsync(Stream stream, ushort len, ushort valLen, ushort type, string key)
        {
            var count = 3 * sizeof(ushort);

            count += (key.Length + 1) * sizeof(ushort);
            count += await VersionResource.AlignWordBoundaryAsync(stream).ConfigureAwait(false);

            var variables = new List<VersionVariable>();
            var data = await stream.ReadBytesAsync(len - count).ConfigureAwait(false);

            using (var mem = new MemoryStream(data))
            {
                while (mem.Position < mem.Length)
                {
                    var variable = await LoadVariableAsync(mem).ConfigureAwait(false);

                    variables.Add(variable);
                }
            }

            var result = new VarFileInfo(key, variables.ToArray());

            return result;
        }

        private static async Task<VersionVariable> LoadVariableAsync(Stream stream)
        {
            var count = 3 * sizeof(ushort);

            await stream.ReadBytesAsync(count).ConfigureAwait(false);

            var key = await stream.ReadUnicodeStringAsync().ConfigureAwait(false);

            await VersionResource.AlignWordBoundaryAsync(stream).ConfigureAwait(false);

            var value = await stream.ReadUInt32Async().ConfigureAwait(false);
            var result = new VersionVariable(key, value);

            return result;
        }

        #endregion
        
        #region Methods

        public override string ToString()
        {
            return $"Key: {Key}, Vars: {Count:n0}";
        }

        public IEnumerator<VersionVariable> GetEnumerator()
        {
            foreach (var v in _vars)
                yield return v;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public string Key { get; }
        public int Count { get; }
        public VersionVariable this[int index] => _vars[index];
        public VersionVariable this[string key] => _vars.FirstOrDefault(v => string.Compare(key, v.Key, StringComparison.OrdinalIgnoreCase) == 0);

        #endregion
    }
}
