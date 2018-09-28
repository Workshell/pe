using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Workshell.PE.Resources.Graphics
{
    public sealed class IconInfo
    {
        internal IconInfo(IconResource resource, ResourceLanguage language, ushort width, ushort height, byte colors, byte[] dib, bool isPNG)
        {
            Icon = resource;
            Language = language;
            Size = new Size(width, height);
            Colors = colors;
            DIB = dib;
            IsPNG = isPNG;
        }

        #region Properties

        public IconResource Icon { get; }
        public ResourceLanguage Language { get; }
        public Size Size { get; }
        public byte Colors { get; }
        public byte[] DIB { get; }
        public bool IsPNG { get; }

        #endregion
    }
}
