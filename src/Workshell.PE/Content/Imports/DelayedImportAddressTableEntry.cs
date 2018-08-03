using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Content
{
    public sealed class DelayedImportAddressTableEntry : ImportAddressTableEntryBase
    {
        internal DelayedImportAddressTableEntry(PortableExecutableImage image, ulong entryOffset, ulong entryValue, uint entryAddress, ushort entryOrdinal, bool isOrdinal) : base(image, entryOffset, entryValue, entryAddress, entryOrdinal, isOrdinal, true)
        {
        }
    }
}
