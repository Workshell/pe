using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    internal static class IconUtils
    {

        #region Methods

        public static bool IsPNG(byte[] data)
        {
            if (data.Length < 8)
                return false;

            ulong signature = BitConverter.ToUInt64(data, 0);

            return (signature == 727905341920923785L);
        }

        public static bool IsPNG(Stream stream)
        {
            byte[] buffer = new byte[sizeof(ulong)];
            int num_read = stream.Read(buffer, 0, buffer.Length);

            if (num_read < sizeof(ulong))
                return false;

            return IsPNG(buffer);
        }

        #endregion

    }

}
