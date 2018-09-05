using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Workshell.PE.Resources.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MESSAGE_RESOURCE_DATA
    {
        public uint NumberOfBlocks;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MESSAGE_RESOURCE_BLOCK
    {
        public uint LowId;
        public uint HighId;
        public uint OffsetToEntries;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MESSAGE_RESOURCE_ENTRY
    {
        public ushort Length;
        public ushort Flags;
    }
}
