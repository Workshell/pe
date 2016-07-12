using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Resources.Native
{

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct ACCELTABLEENTRY
    {

        public ushort fFlags;
        public ushort wAnsi;
        public ushort wId;
        public ushort padding;

    }

}
