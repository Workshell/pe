using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Resources.Graphics
{
    internal static class GraphicUtils
    {
        #region Methods

        public static bool IsPNG(byte[] data)
        {
            if (data.Length < 8)
                return false;

            var signature = BitConverter.ToUInt64(data, 0);

            return (signature == 727905341920923785L);
        }

        public static bool IsPNG(Stream stream)
        {
            return IsPNGAsync(stream).GetAwaiter().GetResult();
        }

        public static async Task<bool> IsPNGAsync(Stream stream)
        {
            var buffer = new byte[sizeof(ulong)];
            var numRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            if (numRead < sizeof(ulong))
                return false;

            return IsPNG(buffer);
        }

        #endregion
    }
}
