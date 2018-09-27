using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Workshell.PE.Resources.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct CURSOR_RESDIR
    {
        public CURSORDIR Cursor;
        public ushort Planes;
        public ushort BitCount;
        public uint BytesInRes;
        public ushort CursorId;
    }
}
