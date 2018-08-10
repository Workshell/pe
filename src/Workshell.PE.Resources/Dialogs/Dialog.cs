using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Resources.Dialogs
{
    public sealed class Dialog : DialogBase
    {
        internal Dialog(DialogResource resource, uint language) : base(resource, language, false)
        {
        }
    }
}
