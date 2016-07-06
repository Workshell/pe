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

    public enum IconFormat
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

            byte[] data = resource.ToBytes(language);

            if (!IconUtils.IsPNG(data))
            {
                using (MemoryStream mem = new MemoryStream(data))
                {
                    BITMAPINFOHEADER header = Utils.Read<BITMAPINFOHEADER>(mem);

                    ushort width = Convert.ToUInt16(header.biWidth);
                    ushort height = Convert.ToUInt16(header.biHeight / 2);
                    byte color_count = Convert.ToByte(header.biBitCount);
                    byte[] dib = mem.ToArray();

                    IconResource icon = new IconResource(resource, language, width, height, color_count, dib, false);

                    return icon;
                }
            }
            else
            {
                using (MemoryStream mem = new MemoryStream(data))
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
                using (MemoryStream mem = new MemoryStream(dib))
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
            Save(fileName, IconFormat.Icon);
        }

        public void Save(Stream stream)
        {
            Save(stream, IconFormat.Icon);
        }

        public void Save(string fileName, IconFormat format)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(file, format);
                file.Flush();
            }
        }

        public void Save(Stream stream, IconFormat format)
        {
            switch (format)
            {
                case IconFormat.Raw:
                    SaveRaw(stream);
                    break;
                case IconFormat.Resource:
                    SaveResource(stream);
                    break;
                case IconFormat.Icon:
                    SaveIcon(stream);
                    break;
            }
        }

        private void SaveRaw(Stream stream)
        {
            byte[] data = resource.ToBytes(language_id);

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

            int colors = 0;

            switch (color_count)
            {
                case 1:
                    colors = 2;
                    break;
                case 2:
                    colors = 4;
                    break;
                case 3:
                    colors = 8;
                    break;
                case 4:
                    colors = 16;
                    break;
                case 5:
                    colors = 32;
                    break;
                case 6:
                    colors = 64;
                    break;
                case 7:
                    colors = 128;
                    break;
                //case 8:
                //    colors = 256;
                //    break;
            }

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
