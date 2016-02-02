using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class DOSStub : ISupportsLocation
    {

        private ImageReader reader;
        private Location location;

        internal DOSStub(ImageReader exeReader, ulong stubOffset, uint stubSize, ulong imageBase)
        {
            reader = exeReader;
            location = new Location(stubOffset,Convert.ToUInt32(stubOffset),imageBase + stubOffset,stubSize,stubSize);
        }

        #region Methods

        public override string ToString()
        {
            return "MS-DOS Stub";
        }

        public byte[] GetBytes()
        {
            return null;
        }

        #endregion

        #region Properties

        public Location Location
        {
            get
            {
                return location;
            }
        }

        #endregion

    }

}
