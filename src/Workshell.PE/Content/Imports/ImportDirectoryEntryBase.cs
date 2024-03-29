﻿#region License
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

    public abstract class ImportDirectoryEntryBase : ISupportsLocation, ISupportsBytes
    {
        private readonly PortableExecutableImage _image;

        protected internal ImportDirectoryEntryBase(PortableExecutableImage image, Location location, bool isDelayed)
        {
            _image = image;

            Location = location;
            IsDelayed = isDelayed;
        }

        #region Methods

        public byte[] GetBytes()
        {
            return GetBytesAsync().GetAwaiter().GetResult();
        }

        public async Task<byte[]> GetBytesAsync()
        {
            var stream = _image.GetStream();
            var buffer = await stream.ReadBytesAsync(Location).ConfigureAwait(false);

            return buffer;
        }

        public string GetName()
        {
            return GetNameAsync().GetAwaiter().GetResult();
        }

        public async Task<string> GetNameAsync()
        {
            var stream = _image.GetStream();
            var calc = _image.GetCalculator();
            var builder = new StringBuilder(256);
            var offset = calc.RVAToOffset(Name);

            stream.Seek(offset, SeekOrigin.Begin);

            while (true)
            {
                var b = await stream.ReadByteAsync().ConfigureAwait(false);

                if (b <= 0)
                {
                    break;
                }

                builder.Append((char)b);
            }

            return builder.ToString();
        }

        #endregion

        #region Properties

        public Location Location { get; }
        public bool IsDelayed { get; }
        public abstract uint Name { get; }

        #endregion
    }
}
