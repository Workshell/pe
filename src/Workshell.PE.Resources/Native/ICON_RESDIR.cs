using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Workshell.PE.Resources.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct ICON_RESDIR
    {
        public ICONDIR Icon;
        public ushort Planes;
        public ushort BitCount;
        public uint BytesInRes;
        public ushort IconId;
    }
}
