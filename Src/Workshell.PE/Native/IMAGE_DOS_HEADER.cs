using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Native
{

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct IMAGE_DOS_HEADER 
    {

        public ushort e_magic;              // Magic number
        public ushort e_cblp;               // Bytes on last page of file
        public ushort e_cp;                 // Pages in file
        public ushort e_crlc;               // Relocations
        public ushort e_cparhdr;            // Size of header in paragraphs
        public ushort e_minalloc;           // Minimum extra paragraphs needed
        public ushort e_maxalloc;           // Maximum extra paragraphs needed
        public ushort e_ss;                 // Initial (relative) SS value
        public ushort e_sp;                 // Initial SP value
        public ushort e_csum;               // Checksum
        public ushort e_ip;                 // Initial IP value
        public ushort e_cs;                 // Initial (relative) CS value
        public ushort e_lfarlc;             // File address of relocation table
        public ushort e_ovno;               // Overlay number
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] e_res_1;            // Reserved words
        public ushort e_oemid;              // OEM identifier (for e_oeminfo)
        public ushort e_oeminfo;            // OEM information; e_oemid specific
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public ushort[] e_res_2;             // Reserved words
        public int e_lfanew;                // File address of new exe header

    }

}
