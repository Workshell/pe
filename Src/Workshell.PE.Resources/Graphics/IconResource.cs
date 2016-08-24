#region License
//  Copyright(c) 2016, Workshell Ltd
//  All rights reserved.
//  
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Workshell Ltd nor the names of its contributors
//  may be used to endorse or promote products
//  derived from this software without specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED.IN NO EVENT SHALL WORKSHELL BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Workshell.PE.Resources.Native;

namespace Workshell.PE.Resources
{

    public sealed class IconInfo
    {

        internal IconInfo(IconResource resource, uint languageId, ushort width, ushort height, byte colors, byte[] dib, bool isPNG)
        {
            Icon = resource;
            Language = languageId;
            Size = new Size(width, height);
            Colors = colors;
            DIB = dib;
            IsPNG = isPNG;
        }

        #region Properties

        public IconResource Icon
        {
            get;
            private set;
        }

        public uint Language
        {
            get;
            private set;
        }

        public Size Size
        {
            get;
            private set;
        }

        public byte Colors
        {
            get;
            private set;
        }

        public byte[] DIB
        {
            get;
            private set;
        }

        public bool IsPNG
        {
            get;
            private set;
        }

        #endregion

    }

    public enum IconSaveFormat
    {
        Raw,
        Icon,
        Bitmap,
        PNG
    }

    public class IconResource : Resource
    {

        public IconResource(ResourceType owningType, ResourceDirectoryEntry directoryEntry) : base(owningType, directoryEntry)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            ResourceId resource_type = new ResourceId(ResourceType.RT_ICON);

            return ResourceType.Register(resource_type, typeof(IconResource));
        }

        #endregion

        #region Methods

        public IconInfo GetInfo()
        {
            return GetInfo(DEFAULT_LANGUAGE);
        }

        public IconInfo GetInfo(uint languageId)
        {
            Tuple<ushort, ushort, byte, byte[], bool> tuple = Load(languageId);
            IconInfo result = new IconInfo(this, languageId, tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5);

            return result;
        }

        public Icon ToIcon()
        {
            return ToIcon(DEFAULT_LANGUAGE);
        }

        public Icon ToIcon(uint languageId)
        {
            Tuple<ushort, ushort, byte, byte[], bool> tuple = Load(languageId);

            if (!tuple.Item5)
            {
                using (MemoryStream dib_mem = new MemoryStream(tuple.Item4))
                {
                    BITMAPINFOHEADER header = Utils.Read<BITMAPINFOHEADER>(dib_mem);

                    ushort width = Convert.ToUInt16(header.biWidth);
                    ushort height = Convert.ToUInt16(header.biHeight / 2);
                    byte color_count = Convert.ToByte(header.biBitCount);

                    MemoryStream mem = new MemoryStream();

                    using (mem)
                    {
                        Utils.Write(Convert.ToUInt16(0), mem);
                        Utils.Write(Convert.ToUInt16(1), mem);
                        Utils.Write(Convert.ToUInt16(1), mem);

                        ulong colors = Convert.ToUInt64(Math.Pow(2, color_count));

                        if (colors >= 256)
                            colors = 0;

                        Utils.Write(Convert.ToByte(width >= 256 ? 0 : width), mem);
                        Utils.Write(Convert.ToByte(height >= 256 ? 0 : height), mem);
                        Utils.Write(Convert.ToByte(colors), mem);
                        Utils.Write(Convert.ToByte(0), mem);
                        Utils.Write(Convert.ToUInt16(1), mem);
                        Utils.Write(Convert.ToUInt16(color_count), mem);
                        Utils.Write(tuple.Item4.Length, mem);
                        Utils.Write(22, mem);

                        Utils.Write(tuple.Item4, mem);

                        mem.Seek(0, SeekOrigin.Begin);

                        Icon icon = new Icon(mem);

                        return icon;
                    }
                }
            }
            else
            {
                using (MemoryStream dib_mem = new MemoryStream(tuple.Item4))
                {
                    using (Image png = Image.FromStream(dib_mem))
                    {
                        ushort width = Convert.ToUInt16(png.Width);
                        ushort height = Convert.ToUInt16(png.Height);
                        byte color_count = 32;

                        MemoryStream mem = new MemoryStream();

                        using (mem)
                        {
                            Utils.Write(Convert.ToUInt16(0), mem);
                            Utils.Write(Convert.ToUInt16(1), mem);
                            Utils.Write(Convert.ToUInt16(1), mem);

                            Utils.Write(Convert.ToByte(width >= 256 ? 0 : width), mem);
                            Utils.Write(Convert.ToByte(height >= 256 ? 0 : height), mem);
                            Utils.Write(Convert.ToByte(0), mem); // 32-bit (16m colors) so 0
                            Utils.Write(Convert.ToByte(0), mem);
                            Utils.Write(Convert.ToUInt16(1), mem);
                            Utils.Write(Convert.ToUInt16(color_count), mem);
                            Utils.Write(tuple.Item4.Length, mem);
                            Utils.Write(22, mem);

                            Utils.Write(tuple.Item4, mem);

                            mem.Seek(0, SeekOrigin.Begin);

                            Icon icon = new Icon(mem);

                            return icon;
                        }
                    }
                }
            }
        }

        public Bitmap ToBitmap()
        {
            return ToBitmap(Color.Transparent);
        }

        public Bitmap ToBitmap(Color backgroundColor)
        {
            return ToBitmap(DEFAULT_LANGUAGE, Color.Transparent);
        }

        public Bitmap ToBitmap(uint languageId)
        {
            return ToBitmap(languageId, Color.Transparent);
        }

        public Bitmap ToBitmap(uint languageId, Color backgroundColor)
        {
            Tuple<ushort, ushort, byte, byte[], bool> tuple = Load(languageId);

            if (!tuple.Item5)
            {
                using (Icon icon = ToIcon(languageId))
                {
                    Rectangle rect = new Rectangle(0, 0, icon.Width, icon.Height);
                    Bitmap bitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);

                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {

                        using (SolidBrush brush = new SolidBrush(backgroundColor))
                            graphics.FillRectangle(brush, rect);

                        graphics.DrawIcon(icon, rect);
                    }

                    bitmap.MakeTransparent(backgroundColor);

                    return bitmap;
                }
            }
            else
            {
                using (MemoryStream dib_mem = new MemoryStream(tuple.Item4))
                {
                    using (Image png = Image.FromStream(dib_mem))
                    {
                        Bitmap bitmap = new Bitmap(png);

                        return bitmap;
                    }
                }
            }
        }

        public void Save(string fileName, uint languageId, IconSaveFormat format)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(file, languageId, format);
                file.Flush();
            }
        }

        public void Save(Stream stream, uint languageId, IconSaveFormat format)
        {
            switch (format)
            {
                case IconSaveFormat.Raw:
                    Save(stream, languageId);
                    break;
                case IconSaveFormat.Icon:
                default:
                    {
                        using (Icon icon = ToIcon(languageId))
                        {
                            icon.Save(stream);
                        }

                        break;
                    }
                case IconSaveFormat.Bitmap:
                    {
                        using (Bitmap bitmap = ToBitmap(languageId))
                        {
                            bitmap.Save(stream, ImageFormat.Bmp);
                        }

                        break;
                    }
                case IconSaveFormat.PNG:
                    {
                        Tuple<ushort, ushort, byte, byte[], bool> tuple = Load(languageId);

                        if (!tuple.Item5)
                        {
                            using (Bitmap bitmap = ToBitmap(languageId))
                            {
                                bitmap.Save(stream, ImageFormat.Png);
                            }
                        }
                        else
                        {
                            stream.Write(tuple.Item4, 0, tuple.Item4.Length);
                        }

                        break;
                    }
            }
        }

        private Tuple<ushort, ushort, byte, byte[], bool> Load(uint languageId)
        {
            byte[] data = GetBytes(languageId);
            ushort width;
            ushort height;
            byte color_count;
            byte[] dib;
            bool is_png;

            using (MemoryStream mem = new MemoryStream(data))
            {
                if (!GraphicResources.IsPNG(data))
                {
                    BITMAPINFOHEADER header = Utils.Read<BITMAPINFOHEADER>(mem);

                    width = Convert.ToUInt16(header.biWidth);
                    height = Convert.ToUInt16(header.biHeight / 2);
                    color_count = Convert.ToByte(header.biBitCount);
                    dib = mem.ToArray();
                    is_png = false;
                }
                else
                {
                    using (Image png = Image.FromStream(mem))
                    {
                        width = Convert.ToUInt16(png.Width);
                        height = Convert.ToUInt16(png.Height);
                        color_count = 32;
                        dib = mem.ToArray();
                        is_png = true;
                    }
                }
            }

            Tuple<ushort, ushort, byte, byte[], bool> tuple = new Tuple<ushort, ushort, byte, byte[], bool>(width, height, color_count, dib, is_png);

            return tuple;
        }

        #endregion

    }

}
