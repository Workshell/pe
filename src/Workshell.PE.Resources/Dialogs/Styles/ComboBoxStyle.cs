using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Resources.Dialogs.Styles
{
    [Flags]
    public enum ComboBoxStyle : uint
    {
        CBS_SIMPLE            = 0x0001,
        CBS_DROPDOWN          = 0x0002,
        CBS_DROPDOWNLIST      = 0x0003,
        CBS_OWNERDRAWFIXED    = 0x0010,
        CBS_OWNERDRAWVARIABLE = 0x0020,
        CBS_AUTOHSCROLL       = 0x0040,
        CBS_OEMCONVERT        = 0x0080,
        CBS_SORT              = 0x0100,
        CBS_HASSTRINGS        = 0x0200,
        CBS_NOINTEGRALHEIGHT  = 0x0400,
        CBS_DISABLENOSCROLL   = 0x0800,
        CBS_UPPERCASE         = 0x2000,
        CBS_LOWERCASE         = 0x4000,
    }
}
