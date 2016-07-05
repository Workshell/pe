using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Native
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
