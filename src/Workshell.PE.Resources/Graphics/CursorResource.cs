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
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;
using Workshell.PE.Resources.Native;

namespace Workshell.PE.Resources.Graphics
{
    public sealed class CursorResource : Resource
    {
        public CursorResource(PortableExecutableImage image, ResourceType type, ResourceDirectoryEntry entry, ResourceId id) : base(image, type, entry, id)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            var typeId = new ResourceId(ResourceType.Cursor);

            return ResourceType.Register<CursorResource>(typeId);
        }

        #endregion

        #region Methods

        public CursorInfo GetInfo()
        {
            return GetInfo(ResourceLanguage.English.UnitedStates);
        }

        public CursorInfo GetInfo(ResourceLanguage language)
        {
            return GetInfoAsync(language).GetAwaiter().GetResult();
        }

        public async Task<CursorInfo> GetInfoAsync()
        {
            return await GetInfoAsync(ResourceLanguage.English.UnitedStates).ConfigureAwait(false);
        }

        public async Task<CursorInfo> GetInfoAsync(ResourceLanguage language)
        {
            var data = await GetCursorDataAsync(language).ConfigureAwait(false);
            var result = new CursorInfo(this, language, data.HotspotX, data.HotspotY, data.Width, data.Height, data.ColorCount, data.DIB, data.IsPNG);

            return result;
        }

        public Icon GetIcon()
        {
            return GetIcon(ResourceLanguage.English.UnitedStates);
        }

        public Icon GetIcon(ResourceLanguage language)
        {
            return GetIconAsync(language).GetAwaiter().GetResult();
        }

        public async Task<Icon> GetIconAsync()
        {
            return await GetIconAsync(ResourceLanguage.English.UnitedStates).ConfigureAwait(false);
        }

        public async Task<Icon> GetIconAsync(ResourceLanguage language)
        {
            var data = await GetCursorDataAsync(language).ConfigureAwait(false);
            Icon icon;

            if (!data.IsPNG)
            {
                using (var dibMem = new MemoryStream(data.DIB))
                {
                    var header = await dibMem.ReadStructAsync<BITMAPINFOHEADER>().ConfigureAwait(false);

                    var width = header.biWidth.ToUInt16();
                    var height = (header.biHeight / 2).ToUInt16();
                    var colorCount = header.biBitCount;

                    using (var mem = new MemoryStream())
                    {
                        await mem.WriteUInt16Async(0).ConfigureAwait(false);
                        await mem.WriteUInt16Async(1).ConfigureAwait(false);
                        await mem.WriteUInt16Async(1).ConfigureAwait(false);

                        var colors = Convert.ToUInt64(Math.Pow(2, colorCount));

                        if (colors >= 256)
                            colors = 0;

                        await mem.WriteByteAsync(Convert.ToByte(width >= 256 ? 0 : width)).ConfigureAwait(false);
                        await mem.WriteByteAsync(Convert.ToByte(height >= 256 ? 0 : height)).ConfigureAwait(false);
                        await mem.WriteByteAsync(Convert.ToByte(colors)).ConfigureAwait(false);
                        await mem.WriteByteAsync(0).ConfigureAwait(false);
                        await mem.WriteUInt16Async(1).ConfigureAwait(false);
                        await mem.WriteUInt16Async(colorCount).ConfigureAwait(false);
                        await mem.WriteInt32Async(data.DIB.Length).ConfigureAwait(false);
                        await mem.WriteInt32Async(22).ConfigureAwait(false);
                        await mem.WriteBytesAsync(data.DIB).ConfigureAwait(false);

                        mem.Seek(0, SeekOrigin.Begin);

                        icon = new Icon(mem);
                    }
                }
            }
            else
            {
                using (var dibMem = new MemoryStream(data.DIB))
                {
                    using (var png = System.Drawing.Image.FromStream(dibMem))
                    {
                        using (var mem = new MemoryStream())
                        {
                            await mem.WriteUInt16Async(0).ConfigureAwait(false);
                            await mem.WriteUInt16Async(1).ConfigureAwait(false);
                            await mem.WriteUInt16Async(1).ConfigureAwait(false);
                            await mem.WriteByteAsync(Convert.ToByte(png.Width >= 256 ? 0 : png.Width)).ConfigureAwait(false);
                            await mem.WriteByteAsync(Convert.ToByte(png.Height >= 256 ? 0 : png.Height)).ConfigureAwait(false);
                            await mem.WriteByteAsync(0).ConfigureAwait(false); // 32-bit (16m colors) so 0
                            await mem.WriteByteAsync(0).ConfigureAwait(false);
                            await mem.WriteUInt16Async(1).ConfigureAwait(false);
                            await mem.WriteUInt16Async(32).ConfigureAwait(false);
                            await mem.WriteInt32Async(data.DIB.Length).ConfigureAwait(false);
                            await mem.WriteInt32Async(22).ConfigureAwait(false);
                            await mem.WriteBytesAsync(data.DIB).ConfigureAwait(false);

                            mem.Seek(0, SeekOrigin.Begin);

                            icon = new Icon(mem);
                        }
                    }
                }
            }

            return icon;
        }
   
        public Bitmap GetBitmap(Color backgroundColor)
        {
            return GetBitmap(backgroundColor, ResourceLanguage.English.UnitedStates);
        }

        public Bitmap GetBitmap(ResourceLanguage language)
        {
            return GetBitmap(Color.Transparent, language);
        }

        public Bitmap GetBitmap(Color backgroundColor, ResourceLanguage language)
        {
            return GetBitmapAsync(backgroundColor, language).GetAwaiter().GetResult();
        }

        public async Task<Bitmap> GetBitmapAsync()
        {
            return await GetBitmapAsync(Color.Transparent, ResourceLanguage.English.UnitedStates).ConfigureAwait(false);
        }

        public async Task<Bitmap> GetBitmapAsync(Color backgroundColor)
        {
            return await GetBitmapAsync(backgroundColor, ResourceLanguage.English.UnitedStates).ConfigureAwait(false);
        }

        public async Task<Bitmap> GetBitmapAsync(ResourceLanguage language)
        {
            return await GetBitmapAsync(Color.Transparent, language).ConfigureAwait(false);
        }

        public async Task<Bitmap> GetBitmapAsync(Color backgroundColor, ResourceLanguage language)
        {
            var data = await GetCursorDataAsync(language).ConfigureAwait(false);
            Bitmap result;

            if (!data.IsPNG)
            {
                using (var icon = await GetIconAsync(language).ConfigureAwait(false))
                {
                    var rect = new Rectangle(0, 0, icon.Width, icon.Height);

                    result = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);

                    using (var graphics = System.Drawing.Graphics.FromImage(result))
                    {

                        using (var brush = new SolidBrush(backgroundColor))
                            graphics.FillRectangle(brush, rect);

                        graphics.DrawIcon(icon, rect);
                    }

                    result.MakeTransparent(backgroundColor);
                }
            }
            else
            {
                using (var dibStream = new MemoryStream(data.DIB))
                {
                    using (var png = System.Drawing.Image.FromStream(dibStream))
                        result = new Bitmap(png);
                }
            }

            return result;
        }

        private async Task<(ushort HotspotX, ushort HotspotY, ushort Width, ushort Height, byte ColorCount, byte[] DIB, bool IsPNG)> GetCursorDataAsync(ResourceLanguage language)
        {
            var buffer = await GetBytesAsync(language).ConfigureAwait(false);
            byte[] dib;
            bool isPNG;
            ushort hotspotX;
            ushort hotspotY;
            ushort width;
            ushort height;
            byte colorCount;

            using (var mem = new MemoryStream(buffer))
            {
                hotspotX = await mem.ReadUInt16Async().ConfigureAwait(false);
                hotspotY = await mem.ReadUInt16Async().ConfigureAwait(false);

                using (var dibMem = new MemoryStream(buffer.Length))
                {
                    mem.CopyTo(dibMem, 4096);
                    dibMem.Seek(0, SeekOrigin.Begin);

                    dib = dibMem.ToArray();
                    isPNG = GraphicUtils.IsPNG(dib);

                    if (!isPNG)
                    {
                        var header = await dibMem.ReadStructAsync<BITMAPINFOHEADER>().ConfigureAwait(false);

                        width = header.biWidth.ToUInt16();
                        height = (header.biHeight / 2).ToUInt16();
                        colorCount = Convert.ToByte(header.biBitCount);
                    }
                    else
                    {
                        using (var png = System.Drawing.Image.FromStream(dibMem))
                        {
                            width = png.Width.ToUInt16();
                            height = png.Height.ToUInt16();
                            colorCount = 32;
                        }
                    }
                }
            }

            return (hotspotX, hotspotY, width, height, colorCount, dib, isPNG);
        }

        #endregion
    }
}
