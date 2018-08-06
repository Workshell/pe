using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Content
{
    public sealed class ImportAddressTable : ImportAddressTableBase<ImportAddressTableEntry>
    {
        internal ImportAddressTable(PortableExecutableImage image, uint rva, ulong[] entries, ImportDirectoryEntryBase directoryEntry) : base(image, rva, entries, directoryEntry, false)
        {
        }
    }
}
