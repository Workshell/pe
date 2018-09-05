using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Resources.Strings
{
    public sealed class StringTableEntry
    {
        internal StringTableEntry(ushort id, string value)
        {
            Id = id;
            Value = value;
        }

        #region Methods

        public override string ToString()
        {
            if (Id == 0)
                return "(Empty)";

            return $"{Id} = {Value}";
        }

        #endregion

        #region Properties

        public ushort Id { get; }
        public string Value { get; }
        public bool IsEmpty => (Id == 0);

        #endregion
    }
}
