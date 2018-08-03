using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Content
{
    public sealed class ImportHintNameEntry : ImportHintNameEntryBase
    {
        internal ImportHintNameEntry(PortableExecutableImage image, ulong offset, uint size, ushort entryHint, string entryName, bool isPadded) : base(image, offset, size, entryHint, entryName, isPadded, false)
        {
        }
    }
}
