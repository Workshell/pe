using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Content
{
    public sealed class ExceptionTableEntry32 : ExceptionTableEntry
    {
        internal ExceptionTableEntry32(PortableExecutableImage image, Location location) : base(image, location)
        {
        }
    }
}
