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
