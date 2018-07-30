using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Content
{
    public sealed class Export
    {
        internal Export(uint entryPoint, string name, uint ord, string forwardName)
        {
            EntryPoint = entryPoint;
            Name = name;
            Ordinal = ord;
            ForwardName = forwardName;
        }

        #region Methods

        public override string ToString()
        {
            string result;

            if (string.IsNullOrWhiteSpace(ForwardName))
            {
                result = $"0x{EntryPoint:X8} {Ordinal:D4} {Name}";
            }
            else
            {
                result = $"0x{EntryPoint:X8} {Ordinal:D4} {Name} -> {ForwardName}";
            }

            return result;
        }

        #endregion

        #region Properties

        public uint EntryPoint { get; }
        public string Name { get; }
        public uint Ordinal { get; }
        public string ForwardName { get; }

        #endregion
    }
}
