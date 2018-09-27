using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Resources.Version
{
    public sealed class VersionString
    {
        internal VersionString(string key, string value)
        {
            Key = key;
            Value = value;
        }

        #region Methods

        public override string ToString()
        {
            return $"{Key} = {Value}";
        }

        #endregion

        #region Properties

        public string Key { get; }
        public string Value { get; }

        #endregion
    }
}
