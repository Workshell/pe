#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

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
