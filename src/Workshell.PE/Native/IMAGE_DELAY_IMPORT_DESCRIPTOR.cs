using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Workshell.PE.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct IMAGE_DELAY_IMPORT_DESCRIPTOR
    {
        public uint Attributes;
        public uint Name;
        public uint ModuleHandle;
        public uint DelayAddressTable;
        public uint DelayNameTable;
        public uint BoundDelayIAT;
        public uint UnloadDelayIAT;
        public uint TimeDateStamp;
    }
}
