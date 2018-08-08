using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE.Content
{
    public sealed class ExceptionTable32 : ExceptionTable
    {
        private ExceptionTable32(PortableExecutableImage image, DataDirectory dataDirectory, Location location, ExceptionTableEntry[] entries) : base(image, dataDirectory, location, entries)
        {
        }

        #region Static Methods

        internal static Task<ExceptionTable> GetAsync(PortableExecutableImage image, DataDirectory dataDirectory, Location location)
        {
            ExceptionTable table = new ExceptionTable32(image, dataDirectory, location, new ExceptionTableEntry[0]);

            return Task.FromResult(table);
        }

        #endregion
    }
}
