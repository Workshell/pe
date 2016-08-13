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

using Workshell.PE.Resources.Native;

namespace Workshell.PE.Resources
{

    public sealed class CursorInfo
    {

        internal CursorInfo(CursorResource cursor, uint languageId, ushort hotspotX, ushort hotspotY, byte[] dib)
        {
            Cursor = cursor;
            Language = languageId;
            Hotspot = new Point(hotspotX, hotspotY);
            DIB = dib;
        }

        #region Properties

        public CursorResource Cursor
        {
            get;
            private set;
        }

        public uint Language
        {
            get;
            private set;
        }

        public Point Hotspot
        {
            get;
            private set;
        }

        public byte[] DIB
        {
            get;
            private set;
        }

        #endregion

    }

    public enum CursorSaveFormat
    {
        Raw,
        Cursor,
        Icon,
        Bitmap
    }

    public class CursorResource : Resource
    {

        public CursorResource(ResourceType owningType, ResourceDirectoryEntry directoryEntry) : base(owningType, directoryEntry)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            ResourceId resource_type = new ResourceId(ResourceType.RT_CURSOR);

            return ResourceType.Register(resource_type, typeof(CursorResource));
        }

        #endregion

        #region Methods

        public CursorInfo GetInfo()
        {
            return GetInfo(DEFAULT_LANGUAGE);
        }

        public CursorInfo GetInfo(uint languageId)
        {
            Tuple<ushort, ushort, byte[]> tuple = Load(languageId);
            CursorInfo result = new CursorInfo(this, languageId, tuple.Item1, tuple.Item2, tuple.Item3);

            return result;
        }

        public Icon ToIcon()
        {
            return ToIcon(Color.Transparent);
        }

        public Icon ToIcon(Color backgroundColor)
        {
            return ToIcon(DEFAULT_LANGUAGE, Color.Transparent);
        }

        public Icon ToIcon(uint languageId)
        {
            return ToIcon(languageId, Color.Transparent);
        }

        public Icon ToIcon(uint languageId, Color backgroundColor)
        {
            Tuple<ushort, ushort, byte[]> tuple = Load(languageId);
            MemoryStream dib_mem = new MemoryStream(tuple.Item3);

            using (dib_mem)
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

                    uint colors = 0;

                    if (color_count != 0 && color_count < 8)
                        colors = Convert.ToUInt32(Math.Pow(2, color_count));

                    Utils.Write(Convert.ToByte(width >= 256 ? 0 : width), mem);
                    Utils.Write(Convert.ToByte(height >= 256 ? 0 : height), mem);
                    Utils.Write(Convert.ToByte(colors), mem);
                    Utils.Write(Convert.ToByte(0), mem);
                    Utils.Write(Convert.ToUInt16(1), mem);
                    Utils.Write(Convert.ToUInt16(color_count), mem);
                    Utils.Write(tuple.Item3.Length, mem);
                    Utils.Write(22, mem);

                    Utils.Write(tuple.Item3, mem);

                    mem.Seek(0, SeekOrigin.Begin);

                    Icon icon = new Icon(mem);

                    return icon;
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
            using (Icon icon = ToIcon(languageId, backgroundColor))
            {
                Rectangle rect = new Rectangle(0, 0, icon.Width, icon.Height);
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

        public void Save(string fileName, uint languageId, CursorSaveFormat format)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(file, languageId, format);
                file.Flush();
            }
        }

        public void Save(Stream stream, uint languageId, CursorSaveFormat format)
        {
            switch (format)
            {
                case CursorSaveFormat.Raw:
                    Save(stream, languageId);
                    break;
                case CursorSaveFormat.Icon:
                    {
                        using (Icon icon = ToIcon(languageId))
                        {
                            icon.Save(stream);
                        }

                        break;
                    }
                case CursorSaveFormat.Bitmap:
                    {
                        using (Bitmap bitmap = ToBitmap(languageId))
                        {
                            bitmap.Save(stream, ImageFormat.Bmp);
                        }

                        break;
                    }
                case CursorSaveFormat.Cursor:
                default:
                    {
                        Tuple<ushort, ushort, byte[]> tuple = Load(languageId);
                        MemoryStream mem = new MemoryStream(tuple.Item3);

                        using (mem)
                        {
                            BITMAPINFOHEADER header = Utils.Read<BITMAPINFOHEADER>(mem);

                            Utils.Write(Convert.ToUInt16(0), stream);
                            Utils.Write(Convert.ToUInt16(2), stream);
                            Utils.Write(Convert.ToUInt16(1), stream);

                            Utils.Write(Convert.ToByte(header.biWidth), stream);
                            Utils.Write(Convert.ToByte(header.biHeight), stream);
                            Utils.Write(Convert.ToByte(0), stream);
                            Utils.Write(Convert.ToByte(0), stream);
                            Utils.Write(tuple.Item1, stream);
                            Utils.Write(tuple.Item2, stream);
                            Utils.Write(tuple.Item3.Length, stream);
                            Utils.Write(22, stream);

                            Utils.Write(tuple.Item3, stream);
                        }

                        break;
                    }
            }
        }

        private Tuple<ushort, ushort, byte[]> Load(uint languageId)
        {
            byte[] data = GetBytes(languageId);

            using (MemoryStream mem = new MemoryStream(data))
            {
                ushort hotspot_x = Utils.ReadUInt16(mem);
                ushort hotspot_y = Utils.ReadUInt16(mem);
                byte[] dib;

                using (MemoryStream dib_mem = new MemoryStream(data.Length - (sizeof(ushort) * 2)))
                {
                    mem.CopyTo(dib_mem, 4096);

                    dib = dib_mem.ToArray();
                }

                Tuple<ushort, ushort, byte[]> tuple = new Tuple<ushort, ushort, byte[]>(hotspot_x, hotspot_y, dib);

                return tuple;
            }
        }

        #endregion

    }

}
