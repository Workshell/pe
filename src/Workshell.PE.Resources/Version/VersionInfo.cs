using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Resources.Version
{
    public sealed class VersionInfo
    {
        internal VersionInfo(VersionResource resource, uint languageId, FixedFileInfo fixedInfo, StringFileInfo stringInfo, VarFileInfo varInfo)
        {
            Resource = resource;
            Language = languageId;
            Fixed = fixedInfo;
            Strings = stringInfo;
            Variables = varInfo;
        }

        #region Properties

        public VersionResource Resource { get; }
        public ResourceLanguage Language { get; }
        public FixedFileInfo Fixed { get; }
        public StringFileInfo Strings { get; }
        public VarFileInfo Variables { get; }

        #endregion
    }
}
