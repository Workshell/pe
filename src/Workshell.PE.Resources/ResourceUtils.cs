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

namespace Workshell.PE.Resources
{
    internal static class ResourceUtils
    {
        #region Methods

        public static async Task<(ushort Num, string Str)> OrdOrSzAsync(Stream stream)
        {
            var value = await stream.ReadUInt16Async().ConfigureAwait(false);

            if (value == 0)
                return (0, string.Empty);

            if (value == 0xFFFF)
            {
                value = await stream.ReadUInt16Async().ConfigureAwait(false);

                return (value, string.Empty);
            }

            var builder = new StringBuilder(256);

            while (true)
            {
                if (value == 0)
                    break;

                builder.Append((char)value);

                value = await stream.ReadUInt16Async().ConfigureAwait(false);
            }

            return (value, builder.ToString());
        }

        #endregion
    }
}
