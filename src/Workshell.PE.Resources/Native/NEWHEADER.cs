using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Workshell.PE.Resources.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct NEWHEADER
    {
        public const ushort RES_ICON = 1;
        public const ushort RES_CURSOR = 2;

        public ushort Reserved;
        public ushort ResType;
        public ushort ResCount;
    }
}
