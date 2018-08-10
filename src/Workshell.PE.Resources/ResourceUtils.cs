using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;

namespace Workshell.PE.Resources
{
    internal static class ResourceUtils
    {
        #region Methods

        public static async Task<Tuple<ushort, string>> OrdOrSzAsync(Stream stream)
        {
            var value = await stream.ReadUInt16Async().ConfigureAwait(false);

            if (value == 0)
                return new Tuple<ushort, string>(0, string.Empty);

            if (value == 0xFFFF)
            {
                value = await stream.ReadUInt16Async().ConfigureAwait(false);

                return new Tuple<ushort, string>(value, string.Empty);
            }

            var builder = new StringBuilder(256);

            while (true)
            {
                if (value == 0)
                    break;

                builder.Append((char)value);

                value = await stream.ReadUInt16Async().ConfigureAwait(false);
            }

            return new Tuple<ushort, string>(value, builder.ToString());
        }

        #endregion
    }
}
