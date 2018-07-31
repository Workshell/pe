using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Content
{
    public sealed class DelayedImportHintNameTable : ImportHintNameTableBase<DelayedImportHintNameEntry>
    {
        internal DelayedImportHintNameTable(PortableExecutableImage image, DataDirectory directory, Location location, IEnumerable<Tuple<ulong, uint, ushort, string, bool>> entries) : base(image, directory, location, entries, true)
        {
        }

        #region Static Methods

        public static async Task<DelayedImportHintNameTable> LoadAsync(PortableExecutableImage image)
        {
            var ilt = await DelayedImportAddressTables.GetLookupTableAsync(image).ConfigureAwait(false);

            return await LoadAsync<DelayedImportAddressTables, DelayedImportHintNameTable, DelayedImportHintNameEntry>(image, ilt).ConfigureAwait(false);
        }

        #endregion
    }
}
