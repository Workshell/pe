using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Resources.Messages
{
    public sealed class MessageTableEntry
    {
        internal MessageTableEntry(uint id, string message, bool isUnicode)
        {
            Id = id;
            Message = message;
            IsUnicode = isUnicode;
        }

        #region Methods

        public override string ToString()
        {
            return $"0x{Id:X8}: {Message}";
        }

        #endregion

        #region Properties

        public uint Id { get; }
        public string Message { get; }
        public bool IsUnicode { get; }

        #endregion
    }
}
