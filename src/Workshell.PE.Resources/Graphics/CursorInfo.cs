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
