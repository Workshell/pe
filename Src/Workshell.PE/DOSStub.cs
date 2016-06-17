using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public sealed class DOSStub : ISupportsLocation, ISupportsBytes
    {

        private ExecutableImage image;
        private Location location;

        internal DOSStub(ExecutableImage exeImage, ulong stubOffset, uint stubSize, ulong imageBase)
        {
            image = exeImage;
            location = new Location(stubOffset,Convert.ToUInt32(stubOffset),imageBase + stubOffset,stubSize,stubSize);
        }

        #region Methods

        public override string ToString()
        {
            return "MS-DOS Stub";
        }

        public byte[] GetBytes()
        {
            Stream stream = image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        #endregion

        #region Properties

        public ExecutableImage Image
        {
            get
            {
                return image;
            }
        }

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
