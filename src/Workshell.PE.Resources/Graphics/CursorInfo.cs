using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Workshell.PE.Resources.Graphics
{
    public sealed class CursorInfo
    {
        internal CursorInfo(CursorResource cursor, ResourceLanguage language, ushort hotspotX, ushort hotspotY, ushort width, ushort height, byte colors, byte[] dib, bool isPNG)
        {
            Cursor = cursor;
            Language = language;
            Hotspot = new Point(hotspotX, hotspotY);
            Size = new Size(width, height);
            Colors = colors;
            DIB = dib;
            IsPNG = isPNG;
        }

        #region Properties

        public CursorResource Cursor { get; }
        public ResourceLanguage Language { get; }
        public Point Hotspot { get; }
        public Size Size { get; }
        public byte Colors { get; }
        public byte[] DIB { get; }
        public bool IsPNG { get; }

        #endregion
    }
}
