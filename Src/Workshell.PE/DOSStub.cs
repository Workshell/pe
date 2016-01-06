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
            long position = reader.Stream.Position;

            try
            {
                byte[] buffer = new byte[location.Size];

                reader.Stream.Seek(location.Offset,SeekOrigin.Begin);
                reader.Stream.Read(buffer,0,buffer.Length);

                return buffer;
            }
            finally
            {
                reader.Stream.Seek(position,SeekOrigin.Begin);
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
