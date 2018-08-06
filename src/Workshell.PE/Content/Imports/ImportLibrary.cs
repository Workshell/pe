using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Workshell.PE.Content
{
    public sealed class ImportLibrary : ImportLibraryBase
    {
        internal ImportLibrary(ImportLibraryFunction[] functions, string name) : base(functions, name, false)
        {
        }
    }
}
