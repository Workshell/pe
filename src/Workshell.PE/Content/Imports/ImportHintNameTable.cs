using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Content
{
    public sealed class ImportHintNameTable : ImportHintNameTableBase<ImportHintNameEntry>
    {
        internal ImportHintNameTable(PortableExecutableImage image, DataDirectory directory, Location location, Tuple<ulong, uint, ushort, string, bool>[] entries) : base(image, directory, location, entries, false)
        {
        }

        #region Static Methods

        #endregion
    }
}
