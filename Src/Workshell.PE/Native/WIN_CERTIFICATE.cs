using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Native
{

    [StructLayout(LayoutKind.Sequential)]
    public struct WIN_CERTIFICATE
    {

        public uint dwLength;
        public ushort wRevision;
        public ushort wCertificateType;

    }

}
