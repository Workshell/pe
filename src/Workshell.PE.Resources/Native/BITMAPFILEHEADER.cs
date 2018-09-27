using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Workshell.PE.Resources.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BITMAPFILEHEADER
    {
        public ushort Tag;
        public uint Size;
        public ushort Reserved1;
        public ushort Reserved2;
        public uint BitmapOffset;
    }
}
