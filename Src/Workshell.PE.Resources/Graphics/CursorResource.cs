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

namespace Workshell.PE
{

    public enum CursorSaveFormat
    {
        Raw,
        Resource,
        Cursor
    }

    public class CursorResource
    {

        private Resource resource;
        private uint language_id;
        private ushort hotspot_x;
        private ushort hotspot_y;
        private byte[] dib;

        internal CursorResource(Resource sourceResource, uint languageId, ushort hotspotX, ushort hotspotY, byte[] dibData)
        {
            resource = sourceResource;
            language_id = languageId;
            hotspot_x = hotspotX;
            hotspot_y = hotspotY;
            dib = dibData;
        }

        #region Static Methods

        public static CursorResource Load(Resource resource)
        {
            return Load(resource, Resource.DEFAULT_LANGUAGE);
        }

        public static CursorResource Load(Resource resource, uint language)
        {
            if (!resource.Languages.Contains(language))
                return null;

            if (resource.Type.Id != ResourceType.RT_CURSOR)
                return null;

            byte[] data = resource.GetBytes(language);
            MemoryStream mem = resource.Type.Resources.Image.MemoryStreamProvider.GetStream(data);

            using (mem)
            {
                ushort hotspot_x = Utils.ReadUInt16(mem);
                ushort hotspot_y = Utils.ReadUInt16(mem);
                byte[] dib;

                using (MemoryStream dib_mem = resource.Type.Resources.Image.MemoryStreamProvider.GetStream())
                {
                    mem.CopyTo(dib_mem, 4096);

                    dib = mem.ToArray();
                }

                CursorResource cursor = new CursorResource(resource, language, hotspot_x, hotspot_y, dib);

                return cursor;
            }
        }

        #endregion

        #region Methods

        public Cursor ToCursor()
        {
            MemoryStream mem = resource.Type.Resources.Image.MemoryStreamProvider.GetStream();

            using (mem)
            {
                Save(mem);
                mem.Seek(0, SeekOrigin.Begin);

                Cursor cursor = new Cursor(mem);

                return cursor;
            }
        }

        public Bitmap ToBitmap()
        {
            return ToBitmap(Color.Transparent);
        }

        public Bitmap ToBitmap(Color backgroundColor)
        {
            using (Cursor cursor = ToCursor())
            {
                Rectangle rect = new Rectangle(0, 0, cursor.Size.Width, cursor.Size.Height);
                Bitmap bitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);

                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    using (SolidBrush brush = new SolidBrush(backgroundColor))
                    {
                        graphics.FillRectangle(brush, rect);
                        cursor.Draw(graphics, rect);
                    }
                }

                return bitmap;
            }
        }

        public void Save(string fileName)
        {
            Save(fileName, CursorSaveFormat.Cursor);
        }

        public void Save(Stream stream)
        {
            Save(stream, CursorSaveFormat.Cursor);
        }

        public void Save(string fileName, CursorSaveFormat format)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(file, format);
                file.Flush();
            }
        }

        public void Save(Stream stream, CursorSaveFormat format)
        {
            switch (format)
            {
                case CursorSaveFormat.Raw:
                    SaveRaw(stream);
                    break;
                case CursorSaveFormat.Resource:
                    SaveResource(stream);
                    break;
                case CursorSaveFormat.Cursor:
                    SaveCursor(stream);
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

        private void SaveCursor(Stream stream)
        {
            MemoryStream mem = resource.Type.Resources.Image.MemoryStreamProvider.GetStream(dib);

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
                Utils.Write(hotspot_x, stream);
                Utils.Write(hotspot_y, stream);
                Utils.Write(dib.Length, stream);
                Utils.Write(22, stream);

                Utils.Write(dib, stream);
            }
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

        public ushort HotspotX
        {
            get
            {
                return hotspot_x;
            }
        }

        public ushort HotspotY
        {
            get
            {
                return hotspot_y;
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
