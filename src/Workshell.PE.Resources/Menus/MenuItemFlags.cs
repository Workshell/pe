using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Resources.Menus
{
    [Flags]
    public enum MenuItemFlags : ushort
    {
        Enabled = 0,
        Grayed = 0x0001,
        Disabled = 0x0002,
        Bitmap = 0x0004,
        OwnerDraw = 0x0100,
        Checked = 0x0008,
        Popup = 0x0010,
        MenubarBreak = 0x0020,
        MenuBreak = 0x0040,
        EndMenu = 0x0080,
        Seperator = 0x0800
    }
}
