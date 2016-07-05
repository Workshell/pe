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
    internal struct ICONDIR
    {

        public byte Width;
        public byte Height;
        public byte ColorCount;
        public byte Reserved;

    }

}
