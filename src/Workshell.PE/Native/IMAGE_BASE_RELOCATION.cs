using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Workshell.PE.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct IMAGE_BASE_RELOCATION
    {
        public uint VirtualAddress;
        public uint SizeOfBlock;
    }
}
