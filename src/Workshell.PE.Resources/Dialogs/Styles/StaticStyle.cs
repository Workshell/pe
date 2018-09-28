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
using System.Text;

namespace Workshell.PE.Resources.Dialogs.Styles
{
    [Flags]
    public enum StaticStyle
    {
        SS_LEFT = 0x00000000,
        SS_CENTER = 0x00000001,
        SS_RIGHT = 0x00000002,
        SS_ICON = 0x00000003,
        SS_BLACKRECT = 0x00000004,
        SS_GRAYRECT = 0x00000005,
        SS_WHITERECT = 0x00000006,
        SS_BLACKFRAME = 0x00000007,
        SS_GRAYFRAME = 0x00000008,
        SS_WHITEFRAME = 0x00000009,
        SS_USERITEM = 0x0000000A,
        SS_SIMPLE = 0x0000000B,
        SS_LEFTNOWORDWRAP = 0x0000000C,
        SS_OWNERDRAW = 0x0000000D,
        SS_BITMAP = 0x0000000E,
        SS_ENHMETAFILE = 0x0000000F,
        SS_ETCHEDHORZ = 0x00000010,
        SS_ETCHEDVERT = 0x00000011,
        SS_ETCHEDFRAME = 0x00000012,
        SS_TYPEMASK = 0x0000001F,
        SS_REALSIZECONTROL = 0x00000040,
        SS_NOPREFIX = 0x00000080,
        SS_NOTIFY = 0x00000100,
        SS_CENTERIMAGE = 0x00000200,
        SS_RIGHTJUST = 0x00000400,
        SS_REALSIZEIMAGE = 0x00000800,
        SS_SUNKEN = 0x00001000,
        SS_EDITCONTROL = 0x00002000,
        SS_ENDELLIPSIS = 0x00004000,
        SS_PATHELLIPSIS = 0x00008000,
        SS_WORDELLIPSIS = 0x0000C000,
        SS_ELLIPSISMASK = 0x0000C000,
    }
}
