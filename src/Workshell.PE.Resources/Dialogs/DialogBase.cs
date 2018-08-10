using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Resources.Dialogs
{
    public abstract class DialogBase
    {
        protected internal DialogBase(DialogResource resource, uint language, bool isExtended)
        {
            Resource = resource;
            Language = language;
            IsExtended = isExtended;
        }

        #region Properties

        public DialogResource Resource { get; }
        public uint Language { get; }
        public bool IsExtended { get; }

        #endregion
    }
}
