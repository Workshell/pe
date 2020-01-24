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
    public enum DialogStyle : uint
    {
        DS_ABSALIGN         = 0x01,
        DS_SYSMODAL         = 0x02,
        DS_LOCALEDIT        = 0x20,
        DS_SETFONT          = 0x40,
        DS_MODALFRAME       = 0x80,
        DS_NOIDLEMSG        = 0x100,
        DS_SETFOREGROUND    = 0x200,

        DS_3DLOOK           = 0x0004,
        DS_FIXEDSYS         = 0x0008,
        DS_NOFAILCREATE     = 0x0010,
        DS_CONTROL          = 0x0400,
        DS_CENTER           = 0x0800,
        DS_CENTERMOUSE      = 0x1000,
        DS_CONTEXTHELP      = 0x2000,

        DS_SHELLFONT        = (DS_SETFONT | DS_FIXEDSYS),
        DS_USEPIXELS        = 0x8000
    }

}
