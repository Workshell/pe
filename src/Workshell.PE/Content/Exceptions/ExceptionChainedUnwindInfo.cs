using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Annotations;
using Workshell.PE.Extensions;

namespace Workshell.PE.Content.Exceptions
{
    public sealed class ExceptionChainedUnwindInfo
    {
        internal ExceptionChainedUnwindInfo(uint startAddress, uint endAddress, uint infoAddress)
        {
            StartAddress = startAddress;
            EndAddress = endAddress;
            UnwindInfoAddress = infoAddress;
        }

        #region Properties

        [FieldAnnotation("Start Address")]
        public uint StartAddress { get; }

        [FieldAnnotation("End Address")]
        public uint EndAddress { get; }

        [FieldAnnotation("Unwind Info Address")]
        public uint UnwindInfoAddress { get; }

        #endregion
    }
}
