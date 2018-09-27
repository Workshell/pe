using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Resources.Version
{
    public sealed class VersionVariable
    {
        internal VersionVariable(string key, uint value)
        {
            Key = key;
            Value = value;
        }

        #region Methods

        public override string ToString()
        {
            return $"{Key} = 0x{Value:X8}";
        }

        #endregion

        #region Properties

        public string Key { get; }
        public uint Value { get; }

        #endregion
    }
}
