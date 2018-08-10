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
