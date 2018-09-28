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
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;
using Workshell.PE.Resources.Native;

namespace Workshell.PE.Resources.Graphics
{
    public sealed class BitmapResource : Resource
    {
        private static readonly int BitmapInfoHeaderSize = Marshal.SizeOf<BITMAPINFOHEADER>();
        private static readonly int BitmapFileHeaderSize = Marshal.SizeOf<BITMAPFILEHEADER>();

        public BitmapResource(PortableExecutableImage image, ResourceType type, ResourceDirectoryEntry entry, ResourceId id) : base(image, type, entry, id)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            var typeId = new ResourceId(ResourceType.Bitmap);

            return ResourceType.Register<BitmapResource>(typeId);
        }

        #endregion

        #region Methods

        public Bitmap GetBitmap()
        {
            return GetBitmapAsync(ResourceLanguage.English.UnitedStates).GetAwaiter().GetResult();
        }

        public async Task<Bitmap> GetBitmapAsync()
        {
            return await GetBitmapAsync(ResourceLanguage.English.UnitedStates).ConfigureAwait(false);
        }

        public Bitmap GetBitmap(ResourceLanguage language)
        {
            return GetBitmapAsync(language).GetAwaiter().GetResult();
        }

        public async Task<Bitmap> GetBitmapAsync(ResourceLanguage language)
        {
            using (var mem = new MemoryStream())
            {
                var buffer = await GetBytesAsync(language).ConfigureAwait(false);

                using (var dibMem = new MemoryStream(buffer))
                {
                    var header = await dibMem.ReadStructAsync<BITMAPINFOHEADER>(BitmapInfoHeaderSize).ConfigureAwait(false);
                    var fileHeader = new BITMAPFILEHEADER()
                    {
                        Tag = 19778,
                        Size = (BitmapFileHeaderSize + buffer.Length).ToUInt32(),
                        Reserved1 = 0,
                        Reserved2 = 0,
                        BitmapOffset = (BitmapFileHeaderSize.ToUInt32() + header.biSize)
                    };

                    await mem.WriteStructAsync<BITMAPFILEHEADER>(fileHeader).ConfigureAwait(false);
                    await mem.WriteBytesAsync(buffer).ConfigureAwait(false);
                }

                mem.Seek(0, SeekOrigin.Begin);

                using (var bitmap = (Bitmap)System.Drawing.Image.FromStream(mem))
                {
                    var result = new Bitmap(bitmap);

                    return result;
                }
            }
        }

        #endregion
    }
}
