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
    internal struct NEWHEADER
    {

        public const ushort RES_ICON = 1;
        public const ushort RES_CURSOR = 2;

        public ushort Reserved;
        public ushort ResType;
        public ushort ResCount;

    }

}
