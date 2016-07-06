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

    public class IconResource
    {

        private byte width;
        private byte height;
        private byte color_count;
        private byte[] dib;

        internal IconResource(byte iconWidth, byte iconHeight, byte colorCount, byte[] dibData)
        {
            width = iconWidth;
            height = iconHeight;
            color_count = colorCount;
            dib = dibData;
        }

        #region Static Methods

        public static IconResource FromBytes(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                return FromStream(mem);
            }
        }

        public static IconResource FromStream(Stream stream)
        {
            byte width;
            byte height;
            byte color_count;
            byte[] dib;

            using (MemoryStream mem = new MemoryStream())
            {
                stream.CopyTo(mem, 4096);
                mem.Seek(0, SeekOrigin.Begin);

                dib = mem.ToArray();
            
                BITMAPINFOHEADER header = Utils.Read<BITMAPINFOHEADER>(mem);

                width = Convert.ToByte(header.biWidth);
                height = Convert.ToByte(header.biHeight / 2);
                color_count = Convert.ToByte(header.biBitCount);
            }

            IconResource icon = new IconResource(width, height, color_count, dib);
            
            return icon;
        }

        public static IconResource FromResource(Resource resource)
        {
            return FromResource(resource, Resource.DEFAULT_LANGUAGE);
        }

        public static IconResource FromResource(Resource resource, uint language)
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
                Utils.Write(Convert.ToUInt16(1), stream);
                Utils.Write(Convert.ToUInt16(1), stream);

                int colors = 0;

                switch (color_count)
                {
                    case 0:
                        colors = 0;
                        break;
                    case 1:
                        colors = 2;
                        break;
                    case 4:
                        colors = 16;
                        break;
                    case 8:
                        colors = 0;
                        break;
                    case 16:
                        colors = 0;
                        break;
                    case 24:
                        colors = 0;
                        break;
                    case 32:
                        colors = 0;
                        break;
                }

                Utils.Write(Convert.ToByte(width), stream);
                Utils.Write(Convert.ToByte(height), stream);
                Utils.Write(Convert.ToByte(colors), stream);
                Utils.Write(Convert.ToByte(0), stream);
                Utils.Write(Convert.ToUInt16(1), stream);
                Utils.Write(Convert.ToUInt16(color_count), stream);
                Utils.Write(dib.Length, stream);
                Utils.Write(22, stream);

                Utils.Write(dib, stream);
            }
        }

        public Icon ToIcon()
        {
            using (MemoryStream mem = new MemoryStream())
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

        #endregion

        #region Properties

        public byte Width
        {
            get
            {
                return width;
            }
        }

        public byte Height
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
