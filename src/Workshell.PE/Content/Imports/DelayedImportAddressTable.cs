using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Content
{
    public sealed class DelayedImportAddressTable : ImportAddressTableBase<DelayedImportAddressTableEntry, DelayedImportDirectoryEntry>
    {
        internal DelayedImportAddressTable(PortableExecutableImage image, DelayedImportDirectoryEntry directoryEntry,  uint tableRVA, ulong[] tableEntries) : base(image, directoryEntry, tableRVA, tableEntries, false)
        {
        }
    }
}
