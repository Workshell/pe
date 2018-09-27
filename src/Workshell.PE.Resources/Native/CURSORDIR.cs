using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Workshell.PE.Resources.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct CURSORDIR
    {
        public ushort Width;
        public ushort Height;
    }
}
