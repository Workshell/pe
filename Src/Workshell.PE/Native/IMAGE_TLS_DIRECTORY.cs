using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Workshell.PE.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct IMAGE_TLS_DIRECTORY32
    {
        public uint StartAddress;
        public uint EndAddress;
        public uint AddressOfIndex;
        public uint AddressOfCallbacks;
        public uint SizeOfZeroFill;
        public uint Characteristics;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct IMAGE_TLS_DIRECTORY64
    {
        public ulong StartAddress;
        public ulong EndAddress;
        public ulong AddressOfIndex;
        public ulong AddressOfCallbacks;
        public uint SizeOfZeroFill;
        public uint Characteristics;   
    }
}
