using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Content
{
    public sealed class ImportHintNameTable : ImportHintNameTableBase<ImportHintNameEntry>
    {
        internal ImportHintNameTable(PortableExecutableImage image, DataDirectory directory, Location location, IEnumerable<Tuple<ulong,uint,ushort,string,bool>> entries) : base(image, directory, location, entries, false)
        {
        }

        #region Static Methods

        public static async Task<ImportHintNameTable> LoadAsync(PortableExecutableImage image)
        {
            var ilt = await ImportAddressTables.GetLookupTableAsync(image).ConfigureAwait(false);

            return await LoadAsync<ImportHintNameTable, ImportHintNameEntry>(image, ilt).ConfigureAwait(false);
        }

        #endregion
    }
}
