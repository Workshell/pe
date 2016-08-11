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

    public enum IconSaveFormat
    {
        Raw,
        Resource,
        Icon
    }

    public class IconResource
    {

        private Resource resource;
        private uint language_id;
        private ushort width;
        private ushort height;
        private byte color_count;
        private byte[] dib;
        private bool is_png;

        internal IconResource(Resource sourceResource, uint languageId, ushort iconWidth, ushort iconHeight, byte colorCount, byte[] dibData, bool isPNG)
        {
            resource = sourceResource;
            language_id = languageId;
            width = iconWidth;
            height = iconHeight;
            color_count = colorCount;
            dib = dibData;
            is_png = isPNG;
        }

        #region Static Methods

        public static IconResource Load(Resource resource)
        {
            return Load(resource, Resource.DEFAULT_LANGUAGE);
        }

        public static IconResource Load(Resource resource, uint language)
        {
            if (!resource.Languages.Contains(language))
                return null;

            if (resource.Type.Id != ResourceType.RT_ICON)
                return null;

            byte[] data = resource.GetBytes(language);
            MemoryStream mem = resource.Type.Resources.Image.MemoryStreamProvider.GetStream(data);

            using (mem)
            {
                if (!GraphicResources.IsPNG(data))
                {
                    BITMAPINFOHEADER header = Utils.Read<BITMAPINFOHEADER>(mem);

                    ushort width = Convert.ToUInt16(header.biWidth);
                    ushort height = Convert.ToUInt16(header.biHeight / 2);
                    byte color_count = Convert.ToByte(header.biBitCount);
                    byte[] dib = mem.ToArray();

                    IconResource icon = new IconResource(resource, language, width, height, color_count, dib, false);

                    return icon;
                }
                else
                {
                    using (Image png = Image.FromStream(mem))
                    {
                        ushort width = Convert.ToUInt16(png.Width);
                        ushort height = Convert.ToUInt16(png.Height);
                        byte color_count = 32;
                        byte[] dib = mem.ToArray();

                        IconResource icon = new IconResource(resource, language, width, height, color_count, dib, true);

                        return icon;
                    }
                }
            }
        }

        #endregion

        #region Methods

        public Icon ToIcon()
        {
            MemoryStream mem = resource.Type.Resources.Image.MemoryStreamProvider.GetStream();

            using (mem)
            {
                Save(mem);
                mem.Seek(0, SeekOrigin.Begin);

                Icon icon = new Icon(mem);

                return icon;
            }
        }

        public Bitmap ToBitmap()
        {
            return ToBitmap(Color.Transparent);
        }

        public Bitmap ToBitmap(Color backgroundColor)
        {
            if (!is_png)
            {
                using (Icon icon = ToIcon())
                {
                    Rectangle rect = new Rectangle(0, 0, icon.Size.Width, icon.Size.Height);
                    Bitmap bitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);

                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        using (SolidBrush brush = new SolidBrush(backgroundColor))
                        {
                            graphics.FillRectangle(brush, rect);
                            graphics.DrawIcon(icon, rect);
                        }
                    }

                    return bitmap;
                }
            }
            else
            {
                MemoryStream mem = resource.Type.Resources.Image.MemoryStreamProvider.GetStream(dib);

                using (mem)
                {
                    using (Image png = Image.FromStream(mem))
                    {
                        Rectangle rect = new Rectangle(0, 0, width, height);
                        Bitmap bitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);

                        using (Graphics graphics = Graphics.FromImage(bitmap))
                        {
                            using (SolidBrush brush = new SolidBrush(backgroundColor))
                            {
                                graphics.FillRectangle(brush, rect);
                                graphics.DrawImage(png, rect);
                            }
                        }

                        return bitmap;
                    }
                }
            }
        }

        public void Save(string fileName)
        {
            Save(fileName, IconSaveFormat.Icon);
        }

        public void Save(Stream stream)
        {
            Save(stream, IconSaveFormat.Icon);
        }

        public void Save(string fileName, IconSaveFormat format)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(file, format);
                file.Flush();
            }
        }

        public void Save(Stream stream, IconSaveFormat format)
        {
            switch (format)
            {
                case IconSaveFormat.Raw:
                    SaveRaw(stream);
                    break;
                case IconSaveFormat.Resource:
                    SaveResource(stream);
                    break;
                case IconSaveFormat.Icon:
                    SaveIcon(stream);
                    break;
            }
        }

        private void SaveRaw(Stream stream)
        {
            byte[] data = resource.GetBytes(language_id);

            stream.Write(data, 0, data.Length);
        }

        private void SaveResource(Stream stream)
        {
            throw new NotImplementedException();
        }

        private void SaveIcon(Stream stream)
        {
            Utils.Write(Convert.ToUInt16(0), stream);
            Utils.Write(Convert.ToUInt16(1), stream);
            Utils.Write(Convert.ToUInt16(1), stream);

            uint colors = 0;

            if (color_count != 0 && color_count < 8)
                colors = Convert.ToUInt32(Math.Pow(2,color_count));

            Utils.Write(Convert.ToByte(width >= 256 ? 0 : width), stream);
            Utils.Write(Convert.ToByte(height >= 256 ? 0 : height), stream);
            Utils.Write(Convert.ToByte(colors), stream);
            Utils.Write(Convert.ToByte(0), stream);
            Utils.Write(Convert.ToUInt16(1), stream);
            Utils.Write(Convert.ToUInt16(color_count), stream);
            Utils.Write(dib.Length, stream);
            Utils.Write(22, stream);

            Utils.Write(dib, stream);
        }

        #endregion

        #region Properties

        public Resource Resource
        {
            get
            {
                return resource;
            }
        }

        public uint Language
        {
            get
            {
                return language_id;
            }
        }

        public ushort Width
        {
            get
            {
                return width;
            }
        }

        public ushort Height
        {
            get
            {
                return height;
            }
        }

        public byte ColorCount
        {
            get
            {
                return color_count;
            }
        }

        public byte[] DIB
        {
            get
            {
                return dib;
            }
        }

        #endregion

    }

}
