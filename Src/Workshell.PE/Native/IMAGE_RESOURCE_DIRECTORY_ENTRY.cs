using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Native
{

    [StructLayout(LayoutKind.Sequential)]
    internal struct IMAGE_RESOURCE_DIRECTORY_ENTRY
    {

        public uint Name;
        public uint OffsetToData;

    }

}
