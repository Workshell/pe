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
    public enum WindowStyleEx : uint
    {
        WS_EX_DLGMODALFRAME     = 0x00000001,
        WS_EX_NOPARENTNOTIFY    = 0x00000004,
        WS_EX_TOPMOST           = 0x00000008,
        WS_EX_ACCEPTFILES       = 0x00000010,
        WS_EX_TRANSPARENT       = 0x00000020,
        WS_EX_MDICHILD          = 0x00000040,
        WS_EX_TOOLWINDOW        = 0x00000080,
        WS_EX_WINDOWEDGE        = 0x00000100,
        WS_EX_CLIENTEDGE        = 0x00000200,
        WS_EX_CONTEXTHELP       = 0x00000400,
        WS_EX_RIGHT             = 0x00001000,
        WS_EX_LEFT              = 0x00000000,
        WS_EX_RTLREADING        = 0x00002000,
        WS_EX_LTRREADING        = 0x00000000,
        WS_EX_LEFTSCROLLBAR     = 0x00004000,
        WS_EX_RIGHTSCROLLBAR    = 0x00000000,
        WS_EX_CONTROLPARENT     = 0x00010000,
        WS_EX_STATICEDGE        = 0x00020000,
        WS_EX_APPWINDOW         = 0x00040000,
        WS_EX_OVERLAPPEDWINDOW  = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
        WS_EX_PALETTEWINDOW     = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),
        WS_EX_LAYERED           = 0x00080000,
        WS_EX_NOINHERITLAYOUT   = 0x00100000, // Disable inheritence of mirroring by children
        WS_EX_LAYOUTRTL         = 0x00400000, // Right to left mirroring
        WS_EX_COMPOSITED        = 0x02000000,
        WS_EX_NOACTIVATE        = 0x08000000
    }
}
