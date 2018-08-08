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
    public sealed class ExceptionTable64 : ExceptionTable
    {
        private ExceptionTable64(PortableExecutableImage image, DataDirectory dataDirectory, Location location, ExceptionTableEntry[] entries) : base(image, dataDirectory, location, entries)
        {
        }

        #region Static Methods

        internal static async Task<ExceptionTable> GetAsync(PortableExecutableImage image, DataDirectory dataDirectory, Location location)
        {
            var calc = image.GetCalculator();
            var stream = image.GetStream();
            var offset = calc.RVAToOffset(dataDirectory.VirtualAddress);
            var rva = dataDirectory.VirtualAddress;      

            stream.Seek(offset.ToInt64(), SeekOrigin.Begin);

            var entrySize = Marshal.SizeOf<IMAGE_RUNTIME_FUNCTION_64>();
            var entries = new List<ExceptionTableEntry>();

            while (true)
            {
                var entryData = await stream.ReadStructAsync<IMAGE_RUNTIME_FUNCTION_64>(entrySize).ConfigureAwait(false);

                if (entryData.StartAddress == 0 && entryData.EndAddress == 0)
                    break;

                var entryLocation = new Location(calc, offset, rva, image.NTHeaders.OptionalHeader.ImageBase + rva, entrySize.ToUInt32(), entrySize.ToUInt32());
                var entry = new ExceptionTableEntry64(image, entryLocation, entryData);

                entries.Add(entry);

                offset += entrySize.ToUInt32();
                rva += entrySize.ToUInt32();
            }

            var table = new ExceptionTable64(image, dataDirectory, location, entries.ToArray());

            return table;
        }

        #endregion
    }
}
