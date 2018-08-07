using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public sealed class DelayedImportAddressTables: ImportAddressTablesBase<DelayedImportAddressTable, DelayedImportAddressTableEntry>
    {
        internal DelayedImportAddressTables(PortableExecutableImage image, DataDirectory directory, Location location, Tuple<uint, ulong[], ImportDirectoryEntryBase>[] tables) : base(image, directory, location, tables, false)
        {
        }

        #region Static Methods

        public static async Task<DelayedImportAddressTables> GetLookupTableAsync(PortableExecutableImage image, DelayedImportDirectory directory = null)
        {
            return await GetTableAsync(image, directory, (entry) => entry.DelayNameTable).ConfigureAwait(false);
        }

        public static async Task<DelayedImportAddressTables> GetAddressTableAsync(PortableExecutableImage image, DelayedImportDirectory directory = null)
        {
            return await GetTableAsync(image, directory, (entry) => entry.DelayAddressTable).ConfigureAwait(false);
        }

        private static async Task<DelayedImportAddressTables> GetTableAsync(PortableExecutableImage image, DelayedImportDirectory directory, Func<DelayedImportDirectoryEntry, uint> thunkHandler)
        {
            if (directory == null)
                directory = await DelayedImportDirectory.GetAsync(image).ConfigureAwait(false);

            var calc = image.GetCalculator();          
            var stream = image.GetStream();
            var tables = new List<Tuple<uint, ulong[], ImportDirectoryEntryBase>>();

            foreach (var dirEntry in directory)
            {
                var thunk = thunkHandler(dirEntry);

                if (thunk == 0)
                    continue;

                var entries = new List<ulong>();
                var offset = calc.RVAToOffset(thunk);

                stream.Seek(offset.ToInt64(), SeekOrigin.Begin);

                while (true)
                {
                    var entry = (!image.Is64Bit ? await stream.ReadUInt32Async().ConfigureAwait(false) : await stream.ReadUInt64Async().ConfigureAwait(false));

                    entries.Add(entry);

                    if (entry == 0)
                        break;
                }

                var table = new Tuple<uint, ulong[], ImportDirectoryEntryBase>(thunk, entries.ToArray(), dirEntry);

                tables.Add(table);
            }

            var rva = 0u;

            if (tables.Count > 0)
                rva = tables.MinBy(table => table.Item1).Item1;

            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
            var va = imageBase + rva;
            var fileOffset = calc.RVAToOffset(rva);
            var fileSize = 0ul;

            foreach (var table in tables)
            {
                var size = (table.Item2.Length + 1) * (!image.Is64Bit ? sizeof(uint) : sizeof(ulong));

                fileSize += size.ToUInt32();
            }

            var section = calc.RVAToSection(rva);
            var location = new Location(fileOffset, rva, va, fileSize, fileSize, section);
            var result = new DelayedImportAddressTables(image, directory.Directory, location, tables.ToArray());

            return result;
        }

        #endregion
    }
}
