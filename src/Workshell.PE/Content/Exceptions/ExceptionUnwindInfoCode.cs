#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

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
