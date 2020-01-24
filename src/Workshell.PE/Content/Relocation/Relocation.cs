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
using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public enum RelocationType : byte
    {
        [EnumAnnotation("IMAGE_REL_BASED_ABSOLUTE")]
        Absolute = 0,
        [EnumAnnotationAttribute("IMAGE_REL_BASED_HIGH")]
        High = 1,
        [EnumAnnotationAttribute("IMAGE_REL_BASED_LOW")]
        Low = 2,
        [EnumAnnotationAttribute("IMAGE_REL_BASED_HIGHLOW")]
        HighLow = 3,
        [EnumAnnotationAttribute("IMAGE_REL_BASED_HIGHADJ")]
        HighAdj = 4,
        [EnumAnnotationAttribute("IMAGE_REL_BASED_MIPS_JMPADDR")]
        MIPSJmpAddr = 5,
        [EnumAnnotationAttribute("IMAGE_REL_BASED_ARM_MOV32A")]
        ARMMov32a = 6,
        [EnumAnnotationAttribute("IMAGE_REL_BASED_ARM_MOV32T")]
        ARMMov32t = 7,
        [EnumAnnotationAttribute("IMAGE_REL_BASED_MIPS_JMPADDR16")]
        MIPSJmpAddr16 = 9,
        [EnumAnnotationAttribute("IMAGE_REL_BASED_DIR64")]
        Dir64 = 10
    }

    public sealed class Relocation
    {
        internal Relocation(RelocationBlock relocBlock, ushort relocValue)
        {
            Block = relocBlock;
            
            var relocType = relocValue >> 12;
            var relocOffset = relocValue & 0xFFF;

            Type = (RelocationType)relocType;
            Offset = relocOffset.ToUInt16();
            Value = relocValue;
            ComputedRVA = Block.PageRVA;

            switch (Type)
            {
                case RelocationType.Absolute:
                    break;
                case RelocationType.HighLow:
                    ComputedRVA += Offset;
                    break;
                case RelocationType.Dir64:
                    ComputedRVA += Offset;
                    break;
                default:
                    ComputedRVA = 0;
                    break;
            }
        }

        #region Methods

        public override string ToString()
        {
            return $"Type: {Type}, Computed RVA: 0x{ComputedRVA:X8}";
        }

        #endregion

        #region Properties

        public RelocationBlock Block { get; }
        public RelocationType Type { get; }
        public ushort Offset { get; }
        public ushort Value { get; }
        public uint ComputedRVA { get; }

        #endregion
    }
}
