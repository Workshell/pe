using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public sealed class ImportAddressTableEntry : ImportAddressTableEntryBase
    {
        internal ImportAddressTableEntry(PortableExecutableImage image, ulong offset, ulong value, uint address, ushort ordinal, bool isOrdinal) : base(image, offset, value, address, ordinal, isOrdinal, false)
        {
        }
    }
}
