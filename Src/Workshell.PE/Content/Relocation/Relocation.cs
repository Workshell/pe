#region License
//  Copyright(c) 2016, Workshell Ltd
//  All rights reserved.
//  
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Workshell Ltd nor the names of its contributors
//  may be used to endorse or promote products
//  derived from this software without specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED.IN NO EVENT SHALL WORKSHELL BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public enum RelocationType : byte
    {
        [EnumAnnotationAttribute("IMAGE_REL_BASED_ABSOLUTE")]
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

        private RelocationBlock block;
        private RelocationType type;
        private ushort offset;
        private ushort value;
        private uint computed_rva;

        internal Relocation(RelocationBlock relocBlock, ushort relocValue)
        {
            block = relocBlock;
            
            int reloc_type = relocValue >> 12;
            int reloc_offset = relocValue & 0xFFF;

            type = (RelocationType)reloc_type;
            offset = Convert.ToUInt16(reloc_offset);
            value = relocValue;
            computed_rva = block.PageRVA;

            switch (type)
            {
                case RelocationType.Absolute:
                    break;
                case RelocationType.HighLow:
                    computed_rva += offset;
                    break;
                case RelocationType.Dir64:
                    computed_rva += offset;
                    break;      
                case RelocationType.High:
                case RelocationType.Low:
                default:
                    computed_rva = 0;
                    break;
            }
        }

        #region Methods

        public override string ToString()
        {
            return String.Format("Type: {0}, Computed RVA: 0x{1:X8}",type,computed_rva);
        }

        #endregion

        #region Properties

        public RelocationBlock Block
        {
            get
            {
                return block;
            }
        }

        public RelocationType Type
        {
            get
            {
                return type;
            }
        }

        public ushort Offset
        {
            get
            {
                return offset;
            }
        }

        public ushort Value
        {
            get
            {
                return value;
            }
        }

        public uint ComputedRVA
        {
            get
            {
                return computed_rva;
            }
        }

        #endregion

    }

}
