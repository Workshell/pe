using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Workshell.PE.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct IMAGE_LOAD_CONFIG_DIRECTORY32
    {
        public uint Size;
        public uint TimeDateStamp;
        public ushort MajorVersion;
        public ushort MinorVersion;
        public uint GlobalFlagsClear;
        public uint GlobalFlagsSet;
        public uint CriticalSectionDefaultTimeout;
        public uint DeCommitFreeBlockThreshold;
        public uint DeCommitTotalFreeThreshold;
        public uint LockPrefixTable; 
        public uint MaximumAllocationSize;
        public uint VirtualMemoryThreshold;
        public uint ProcessHeapFlags;
        public uint ProcessAffinityMask;
        public ushort CSDVersion;
        public ushort Reserved1;
        public uint EditList;
        public uint SecurityCookie;
        public uint SEHandlerTable;
        public uint SEHandlerCount;
        public uint GuardCFCheckFunctionPointer;
        public uint Reserved2;
        public uint GuardCFFunctionTable;
        public uint GuardCFFunctionCount;
        public uint GuardFlags;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct IMAGE_LOAD_CONFIG_DIRECTORY64 
    {
        public uint Size;
        public uint TimeDateStamp;
        public ushort MajorVersion;
        public ushort MinorVersion;
        public uint GlobalFlagsClear;
        public uint GlobalFlagsSet;
        public uint CriticalSectionDefaultTimeout;
        public ulong DeCommitFreeBlockThreshold;
        public ulong DeCommitTotalFreeThreshold;
        public ulong LockPrefixTable;
        public ulong MaximumAllocationSize;
        public ulong VirtualMemoryThreshold;
        public ulong ProcessAffinityMask;
        public uint ProcessHeapFlags;
        public ushort CSDVersion;
        public ushort Reserved1;
        public ulong EditList;
        public ulong SecurityCookie;
        public ulong SEHandlerTable;
        public ulong SEHandlerCount;
        public ulong GuardCFCheckFunctionPointer;
        public ulong Reserved2;
        public ulong GuardCFFunctionTable;
        public ulong GuardCFFunctionCount;
        public uint GuardFlags;
    }
}
