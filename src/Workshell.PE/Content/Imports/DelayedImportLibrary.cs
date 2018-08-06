using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Workshell.PE.Content
{
    public sealed class DelayedImportLibrary : ImportLibraryBase
    {
        internal DelayedImportLibrary(ImportLibraryFunction[] functions, string name) : base(functions, name, true)
        {
        }
    }
}
