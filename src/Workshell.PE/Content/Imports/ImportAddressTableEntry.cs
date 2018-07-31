using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Content
{
    public sealed class ImportAddressTableEntry : ImportAddressTableEntryBase
    {
        internal ImportAddressTableEntry(PortableExecutableImage image, ulong entryOffset, ulong entryValue, uint entryAddress, ushort entryOrdinal, bool isOrdinal) : base(image, entryOffset, entryValue, entryAddress, entryOrdinal, isOrdinal, false)
        {
        }
    }
}
