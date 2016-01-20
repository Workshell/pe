using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Native
{

    [StructLayout(LayoutKind.Sequential)]
    internal struct IMAGE_COR20_HEADER
    {

        public uint cb;
        public ushort MajorRuntimeVersion;
        public ushort MinorRuntimeVersion;
        public IMAGE_DATA_DIRECTORY MetaData;
        public uint Flags;
        public uint EntryPointTokenOrRVA;
        public IMAGE_DATA_DIRECTORY Resources;
        public IMAGE_DATA_DIRECTORY StrongNameSignature;
        public IMAGE_DATA_DIRECTORY CodeManagerTable;
        public IMAGE_DATA_DIRECTORY VTableFixups;
        public IMAGE_DATA_DIRECTORY ExportAddressTableJumps;
        public IMAGE_DATA_DIRECTORY ManagedNativeHeader;

    }

}
