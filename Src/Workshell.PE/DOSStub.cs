using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class DOSStub : ILocationSupport, IRawDataSupport
    {

        private ExeReader reader;
        private StreamLocation location;

        internal DOSStub(ExeReader exeReader, StreamLocation streamLoc)
        {
            reader = exeReader;
            location = streamLoc;
        }

        #region Methods

        public override string ToString()
        {
            return "MS-DOS Stub";
        }

        public byte[] GetBytes()
        {
            Stream stream = reader.GetStream();
            long position = stream.Position;

            try
            {
                byte[] buffer = new byte[location.Size];

                stream.Seek(location.Offset,SeekOrigin.Begin);
                stream.Read(buffer,0,buffer.Length);

                return buffer;
            }
            finally
            {
                stream.Seek(position,SeekOrigin.Begin);
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

        #endregion

    }

}
