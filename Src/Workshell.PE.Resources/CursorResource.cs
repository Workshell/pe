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

    public class CursorResource
    {

        private ushort hotspot_x;
        private ushort hotspot_y;
        private byte[] dib;

        internal CursorResource(ushort hotspotX, ushort hotspotY, byte[] dibData)
        {
            hotspot_x = hotspotX;
            hotspot_y = hotspotY;
            dib = dibData;
        }

        #region Static Methods

        public static CursorResource FromBytes(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                return FromStream(mem);
            }
        }

        public static CursorResource FromStream(Stream stream)
        {
            ushort hotspot_x = Utils.ReadUInt16(stream);
            ushort hotspot_y = Utils.ReadUInt16(stream);
            byte[] dib;

            using (MemoryStream mem = new MemoryStream())
            {
                stream.CopyTo(mem, 4096);

                dib = mem.ToArray();
            }

            CursorResource cursor = new CursorResource(hotspot_x, hotspot_y, dib);

            return cursor;
        }

        public static CursorResource FromResource(Resource resource)
        {
            return FromResource(resource, Resource.DEFAULT_LANGUAGE);
        }

        public static CursorResource FromResource(Resource resource, uint language)
        {
            byte[] data = resource.ToBytes(language);

            return FromBytes(data);
        }

        #endregion

        #region Methods

        public void Save(string fileName)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(file);
                file.Flush();
            }
        }

        public void Save(Stream stream)
        {
            using (MemoryStream mem = new MemoryStream(dib))
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
                Utils.Write((uint)dib.Length, stream);
                Utils.Write((uint)22, stream);

                Utils.Write(dib, stream);
            }
        }

        public Cursor ToCursor()
        {
            using (MemoryStream mem = new MemoryStream())
            {
                Save(mem);
                mem.Seek(0, SeekOrigin.Begin);

                return new Cursor(mem);
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
                    SolidBrush brush = new SolidBrush(backgroundColor);

                    graphics.FillRectangle(brush, rect);
                    cursor.Draw(graphics, rect);
                }

                return bitmap;
            }
        }

        #endregion

        #region Properties

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
