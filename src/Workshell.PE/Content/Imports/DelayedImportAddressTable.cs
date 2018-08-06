using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Content
{
    public sealed class DelayedImportAddressTable : ImportAddressTableBase<DelayedImportAddressTableEntry>
    {
        internal DelayedImportAddressTable(PortableExecutableImage image, uint rva, ulong[] entries, ImportDirectoryEntryBase directoryEntry) : base(image, rva, entries, directoryEntry, true)
        {
        }
    }
}
