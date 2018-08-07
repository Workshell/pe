using System;
using System.Collections.Generic;
using System.Text;
using Workshell.PE.Annotations;

namespace Workshell.PE.Content.Exceptions
{
    public enum ExceptionUnwindCode : byte
    {
        [EnumAnnotation("UWOP_PUSH_NONVOL")]
        PushNonVolatile = 0,
        [EnumAnnotation("UWOP_ALLOC_LARGE ")]
        AllocLarge = 1,
        [EnumAnnotation("UWOP_ALLOC_SMALL")]
        AllocSmall = 2,
        [EnumAnnotation("UWOP_SET_FPREG")]
        SetFramePointerRegister = 3,
        [EnumAnnotation("UWOP_SAVE_NONVOL")]
        SaveNonVolatile = 4,
        [EnumAnnotation("UWOP_SAVE_NONVOL_FAR ")] 
        SaveNonVolatileFar = 5,
        [EnumAnnotation("UWOP_SAVE_XMM128")]
        SaveXMM128 = 8,
        [EnumAnnotation("UWOP_SAVE_XMM128_FAR")]
        SaveXMM128Far = 9,
        [EnumAnnotation("UWOP_PUSH_MACHFRAME")]
        PushMachineFrame = 10
    }

    public sealed class ExceptionUnwindInfoCode
    {
        internal ExceptionUnwindInfoCode(ushort value)
        {
            var bytes = BitConverter.GetBytes(value);

            Value = value;
            Offset = bytes[0];
            Code = (ExceptionUnwindCode)Convert.ToByte(bytes[1] & ((1 << 4) - 1));
            Operation = Convert.ToByte(bytes[1] >> 4 & ((1 << 4) - 1));
        }

        #region Methods

        public override string ToString()
        {
            return $"Offset: 0x{Offset:X2}, Code: {Code}, Operation: 0x{Operation:X2}";
        }

        #endregion

        #region Properties

        public ushort Value { get; }
        public byte Offset { get; }
        public ExceptionUnwindCode Code { get; }
        public byte Operation { get; }

        #endregion
    }
}
