using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class DOSStub : ILocatable
    {

        private PortableExecutable exe;
        private StreamLocation location;

        internal DOSStub(PortableExecutable portableExecutable)
        {
            exe = portableExecutable;

            long offset = exe.DOSHeader.Location.Offset + exe.DOSHeader.Location.Size;
            long size = exe.DOSHeader.FileAddressNewHeader - DOSHeader.Size;

            location = new StreamLocation(offset,size);

            long num_skipped = Utils.SkipBytes(portableExecutable.Stream,Convert.ToInt32(size));

            if (num_skipped < size)
                throw new PortableExecutableException("Could not read MS-DOS stub from stream.");
        }

        #region Methods

        public override string ToString()
        {
            if (location == null)
            {
                return base.ToString();
            }
            else
            {
                return location.ToString();
            }
        }

        public byte[] GetBytes()
        {
            byte[] buffer = new byte[location.Size];

            exe.Stream.Seek(location.Offset,SeekOrigin.Begin);
            exe.Stream.Read(buffer,0,buffer.Length);

            return buffer;
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

        #endregion

    }

}
