using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE;

namespace Workshell.PE
{

    public sealed class GenericLocatable : ISupportsLocation
    {

        private Location _location;

        public GenericLocatable(Location location)
        {
            _location = new Workshell.PE.Location(location.FileOffset, location.RelativeVirtualAddress, location.VirtualAddress, location.FileSize, location.VirtualSize);
        }

        public GenericLocatable(ulong fileOffset, uint rva, ulong va, uint fileSize, uint virtualSize)
        {
            _location = new Workshell.PE.Location(fileOffset, rva, va, fileSize, virtualSize);
        }

        #region Properties

        public Location Location
        {
            get
            {
                return _location;
            }
        }

        #endregion

    }

}
