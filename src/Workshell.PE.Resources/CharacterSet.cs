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

using Workshell.PE.Annotations;

namespace Workshell.PE.Resources
{
    public enum CharacterSet : byte
    {
        [EnumAnnotation("ANSI_CHARSET")]
        ANSI = 0,
        [EnumAnnotation("DEFAULT_CHARSET")]
        Default = 0x01,
        [EnumAnnotation("SYMBOL_CHARSET")]
        Symbol = 0x02,
        [EnumAnnotation("MAC_CHARSET")]
        Mac = 0x4D,
        [EnumAnnotation("SHIFTJIS_CHARSET")]
        ShiftJis = 0x80,
        [EnumAnnotation("HANGUL_CHARSET")]
        Hangul = 0x81,
        [EnumAnnotation("JOHAB_CHARSET")]
        Johab = 0x82,
        [EnumAnnotation("GB2312_CHARSET")]
        GB2312 = 0x86,
        [EnumAnnotation("CHINESEBIG5_CHARSET")]
        ChineseBig5 = 0x88,
        [EnumAnnotation("GREEK_CHARSET")]
        Greek = 0xA1,
        [EnumAnnotation("TURKISH_CHARSET")]
        Turkish = 0xA2,
        [EnumAnnotation("VIETNAMESE_CHARSET")]
        Vietnamese = 0xA3,
        [EnumAnnotation("HEBREW_CHARSET")]
        Hebrew = 0xB1,
        [EnumAnnotation("ARABIC_CHARSET")]
        Arabic = 0xB2,
        [EnumAnnotation("BALTIC_CHARSET")]
        Baltic = 0xBA,
        [EnumAnnotation("RUSSIAN_CHARSET")]
        Russian = 0xCC,
        [EnumAnnotation("THAI_CHARSET")]
        Thai = 0xDE,
        [EnumAnnotation("EASTEUROPE_CHARSET")]
        EastEurope = 0xEE,
        [EnumAnnotation("OEM_CHARSET")]
        OEM = 0xFF
    }
}
