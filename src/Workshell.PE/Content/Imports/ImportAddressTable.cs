using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Content
{
    public sealed class ImportAddressTable : ImportAddressTableBase<ImportAddressTableEntry, ImportDirectoryEntry>
    {
        internal ImportAddressTable(PortableExecutableImage image, ImportDirectoryEntry directoryEntry,  uint tableRVA, ulong[] tableEntries) : base(image, directoryEntry, tableRVA, tableEntries, false)
        {
        }
    }
}
