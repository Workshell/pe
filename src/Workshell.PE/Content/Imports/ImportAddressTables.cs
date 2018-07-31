using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Content
{
    public sealed class ImportAddressTables : ImportAddressTablesBase<ImportAddressTable, ImportAddressTableEntry, ImportDirectoryEntry>
    {
        internal ImportAddressTables(PortableExecutableImage image, DataDirectory directory, Location location, Tuple<uint, ImportDirectoryEntry, ulong[]>[]  tables) : base(image, directory, location, tables)
        {
        }

        #region Static Methods

        public static async Task<ImportAddressTables> GetLookupTableAsync(PortableExecutableImage image, ImportDirectory directory = null)
        {
            if (directory == null)
                directory = await ImportDirectory.LoadAsync(image).ConfigureAwait(false);

            var tables = await LoadAsync<ImportAddressTable, ImportAddressTableEntry, ImportDirectoryEntry, ImportAddressTables>(
                image,
                directory,
                entry => entry.OriginalFirstThunk
            ).ConfigureAwait(false);

            return tables;
        }

        public static async Task<ImportAddressTables> GetAddressTableAsync(PortableExecutableImage image, ImportDirectory directory = null)
        {
            if (directory == null)
                directory = await ImportDirectory.LoadAsync(image).ConfigureAwait(false);

            var tables = await LoadAsync<ImportAddressTable, ImportAddressTableEntry, ImportDirectoryEntry, ImportAddressTables>(
                image,
                directory,
                entry => entry.FirstThunk
            ).ConfigureAwait(false);

            return tables;
        }

        #endregion
    }
}
