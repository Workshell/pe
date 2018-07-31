using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Content
{
    public sealed class DelayedImportAddressTables : ImportAddressTablesBase<DelayedImportAddressTable, DelayedImportAddressTableEntry, DelayedImportDirectoryEntry>
    {
        internal DelayedImportAddressTables(PortableExecutableImage image, DataDirectory directory, Location location, Tuple<uint, DelayedImportDirectoryEntry, ulong[]>[]  tables) : base(image, directory, location, tables)
        {
        }

        #region Static Methods

        public static async Task<DelayedImportAddressTables> GetLookupTableAsync(PortableExecutableImage image, DelayedImportDirectory directory = null)
        {
            if (directory == null)
                directory = await DelayedImportDirectory.LoadAsync(image).ConfigureAwait(false);

            var tables = await LoadAsync<DelayedImportAddressTable, DelayedImportAddressTableEntry, DelayedImportDirectoryEntry, DelayedImportAddressTables>(
                image,
                directory,
                entry => entry.DelayNameTable
            ).ConfigureAwait(false);

            return tables;
        }

        public static async Task<DelayedImportAddressTables> GetAddressTableAsync(PortableExecutableImage image, DelayedImportDirectory directory = null)
        {
            if (directory == null)
                directory = await DelayedImportDirectory.LoadAsync(image).ConfigureAwait(false);

            var tables = await LoadAsync<DelayedImportAddressTable, DelayedImportAddressTableEntry, DelayedImportDirectoryEntry, DelayedImportAddressTables>(
                image,
                directory,
                entry => entry.DelayAddressTable
            ).ConfigureAwait(false);

            return tables;
        }

        #endregion
    }
}
