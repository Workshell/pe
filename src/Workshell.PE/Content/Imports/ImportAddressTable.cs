using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Content.Imports
{
    public sealed class ImportAddressTable : ImportAddressTableBase<ImportAddressTableEntry>
    {
        internal ImportAddressTable(PortableExecutableImage image, uint rva, ulong[] entries) : base(image, rva, entries, false)
        {
        }
    }
}
