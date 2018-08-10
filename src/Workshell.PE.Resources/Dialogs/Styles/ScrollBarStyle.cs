using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Resources.Dialogs.Styles
{
    [Flags]
    public enum ScrollBarStyle : uint
    {
        SBS_HORZ                    = 0x0000,
        SBS_VERT                    = 0x0001,
        SBS_TOPALIGN                = 0x0002,
        SBS_LEFTALIGN               = 0x0002,
        SBS_BOTTOMALIGN             = 0x0004,
        SBS_RIGHTALIGN              = 0x0004,
        SBS_SIZEBOXTOPLEFTALIGN     = 0x0002,
        SBS_SIZEBOXBOTTOMRIGHTALIGN = 0x0004,
        SBS_SIZEBOX                 = 0x0008,
        SBS_SIZEGRIP                = 0x0010
    }
}
