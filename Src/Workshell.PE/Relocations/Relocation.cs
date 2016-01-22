﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Attributes;
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

    public class Relocation : ILocationSupport
    {

        private const int size = sizeof(ushort);

        private RelocationBlock block;
        private RelocationType type;
        private ushort offset;
        private ushort value;
        private uint computed_rva;
        private StreamLocation location;

        internal Relocation(RelocationBlock relocBlock, long relocOffset, ushort relocValue)
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

            location = new StreamLocation(relocOffset,size);
        }

        #region Static Properties

        public static int Size
        {
            get
            {
                return size;
            }
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

        public StreamLocation Location
        {
            get
            {
                return location;
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
