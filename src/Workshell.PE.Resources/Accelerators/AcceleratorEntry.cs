using System;
using System.Collections.Generic;
using System.Text;
using Workshell.PE.Annotations;

namespace Workshell.PE.Resources.Accelerators
{
    [Flags]
    public enum AcceleratorFlags : ushort
    {
        [EnumAnnotation("FVIRTKEY")]
        VirtualKey = 0x0001,
        [EnumAnnotation("FNOINVERT")]
        NoInvert = 0x0002,
        [EnumAnnotation("FSHIFT")]
        Shift = 0x0004,
        [EnumAnnotation("FCONTROL")]
        Control = 0x0008,
        [EnumAnnotation("FALT")]
        Alt = 0x0010,
        End = 0x0080
    }

    public sealed class AcceleratorEntry
    {
        internal AcceleratorEntry(ushort flags, ushort key, ushort id)
        {
            Flags = flags;
            Key = key;
            Id = id;
        }

        #region Methods

        public override string ToString()
        {
            return $"Id: {Id}; Key: {GetKey()}; Flags: {GetFlags()}";
        }

        public AcceleratorFlags GetFlags()
        {
            return (AcceleratorFlags)Flags;
        }

        public AcceleratorKeys GetKey()
        {
            return (AcceleratorKeys)Key;
        }

        #endregion

        #region Properties

        public ushort Flags { get; }
        public ushort Key { get; }
        public ushort Id {get;}

        #endregion
    }
}
