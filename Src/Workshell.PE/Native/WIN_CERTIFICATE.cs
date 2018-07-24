using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Workshell.PE.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WIN_CERTIFICATE
    {
        public uint dwLength;
        public ushort wRevision;
        public ushort wCertificateType;
    }
}
