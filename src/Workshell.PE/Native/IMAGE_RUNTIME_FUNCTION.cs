using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Workshell.PE.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct IMAGE_RUNTIME_FUNCTION_32
    {
        public uint BeginAddress;
        public uint EndAddress;
        public uint ExceptionHandler;
        public uint ExceptionData;
        public uint PrologEndAddress;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct IMAGE_RUNTIME_FUNCTION_64
    {
        public uint StartAddress;
        public uint EndAddress;
        public uint UnwindInfoAddress;
    }
}
