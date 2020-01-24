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
    public enum ButtonStyle : uint
    {
        BS_PUSHBUTTON = 0x00000000,
        BS_DEFPUSHBUTTON = 0x00000001,
        BS_CHECKBOX = 0x00000002,
        BS_AUTOCHECKBOX = 0x00000003,
        BS_RADIOBUTTON = 0x00000004,
        BS_3STATE = 0x00000005,
        BS_AUTO3STATE = 0x00000006,
        BS_GROUPBOX = 0x00000007,
        BS_USERBUTTON = 0x00000008,
        BS_AUTORADIOBUTTON = 0x00000009,
        BS_PUSHBOX = 0x0000000A,
        BS_OWNERDRAW = 0x0000000B,
        BS_TYPEMASK = 0x0000000F,
        BS_LEFTTEXT = 0x00000020,
        BS_TEXT = 0x00000000,
        BS_ICON = 0x00000040,
        BS_BITMAP = 0x00000080,
        BS_LEFT = 0x00000100,
        BS_RIGHT = 0x00000200,
        BS_CENTER = 0x00000300,
        BS_TOP = 0x00000400,
        BS_BOTTOM = 0x00000800,
        BS_VCENTER = 0x00000C00,
        BS_PUSHLIKE = 0x00001000,
        BS_MULTILINE = 0x00002000,
        BS_NOTIFY = 0x00004000,
        BS_FLAT = 0x00008000,
        BS_RIGHTBUTTON = BS_LEFTTEXT
    }

}
