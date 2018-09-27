using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Workshell.PE.Resources.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct ICONDIR
    {
        public byte Width;
        public byte Height;
        public byte ColorCount;
        public byte Reserved;
    }
}
