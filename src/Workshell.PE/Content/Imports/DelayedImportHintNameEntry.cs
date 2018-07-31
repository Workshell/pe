using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Content
{
    public sealed class DelayedImportHintNameEntry : ImportHintNameEntryBase
    {
        internal DelayedImportHintNameEntry(PortableExecutableImage image, ulong offset, uint size, ushort hint, string name, bool isPadded) : base(image, offset, size, hint, name, isPadded, true)
        {
        }
    }
}
