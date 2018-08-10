using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Resources.Dialogs.Styles
{
    [Flags]
    public enum ListBoxStyle : uint
    {
        LBS_NOTIFY = 0x0001,
        LBS_SORT = 0x0002,
        LBS_NOREDRAW = 0x0004,
        LBS_MULTIPLESEL = 0x0008,
        LBS_OWNERDRAWFIXED = 0x0010,
        LBS_OWNERDRAWVARIABLE = 0x0020,
        LBS_HASSTRINGS = 0x0040,
        LBS_USETABSTOPS = 0x0080,
        LBS_NOINTEGRALHEIGHT = 0x0100,
        LBS_MULTICOLUMN = 0x0200,
        LBS_WANTKEYBOARDINPUT = 0x0400,
        LBS_EXTENDEDSEL = 0x0800,
        LBS_DISABLENOSCROLL = 0x1000,
        LBS_NODATA = 0x2000,
        LBS_NOSEL = 0x4000,
        LBS_COMBOBOX = 0x8000,
        LBS_STANDARD = (LBS_NOTIFY | LBS_SORT | WindowStyle.WS_VSCROLL | WindowStyle.WS_BORDER),
    }
}
