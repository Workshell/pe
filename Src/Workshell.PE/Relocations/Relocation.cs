using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Attributes;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public enum RelocationType
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

    public class Relocation : ILocatable
    {

        private const int size = sizeof(ushort);

        private RelocationBlock block;
        private RelocationType type;
        private int offset;
        private ushort value;
        private StreamLocation location;

        internal Relocation(RelocationBlock relocBlock, long relocOffset, ushort relocValue)
        {
            block = relocBlock;

            int reloc_type = relocValue >> 12;
            int reloc_offset = relocValue & 0xFFF;
			
            type = (RelocationType)reloc_type;
            offset = reloc_offset;
            value = relocValue;
            location = new StreamLocation(relocOffset,size);
        }

        #region Methods

        public override string ToString()
        {
            return String.Format("{0} (0x{0:X4})",value);
        }

        #endregion

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

        public int Offset
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

        #endregion

    }

}
