using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public sealed class DelayedImports : ImportsBase<DelayedImportLibrary>
    {
        internal DelayedImports(DelayedImportLibrary[] libraries) : base(libraries)
        {
        }

        #region Static Methods

        public static async Task<DelayedImports> GetAsync(PortableExecutableImage image)
        {
            var directory = await DelayedImportDirectory.LoadAsync(image).ConfigureAwait(false);

            if (directory == null)
                return null;

            var ilt = await DelayedImportAddressTables.GetLookupTableAsync(image, directory).ConfigureAwait(false);

            if (ilt == null)
                return null;

            var hnt = await DelayedImportHintNameTable.GetAsync(image, directory).ConfigureAwait(false);

            if (hnt == null)
                return null;

            return await GetAsync(image, ilt, hnt).ConfigureAwait(false);
        }

        public static async Task<DelayedImports> GetAsync(PortableExecutableImage image, DelayedImportAddressTables ilt, DelayedImportHintNameTable hnt)
        {
            var libraries = new List<DelayedImportLibrary>();
            var calc = image.GetCalculator();
            var stream = image.GetStream();

            foreach (var table in ilt)
            {
                var builder = new StringBuilder(256);
                var offset = calc.RVAToOffset(table.DirectoryEntry.Name);

                stream.Seek(offset.ToInt64(), SeekOrigin.Begin);

                while (true)
                {
                    var b = await stream.ReadByteAsync().ConfigureAwait(false);

                    if (b <= 0)
                        break;

                    builder.Append((char)b);
                }

                var name = builder.ToString();
                var functions = new List<ImportLibraryFunction>(table.Count);

                foreach (var entry in table)
                {
                    ImportLibraryFunction function = null;

                    if (entry.IsOrdinal)
                    {
                        function = new ImportLibraryOrdinalFunction(entry, entry.Ordinal);
                    }
                    else
                    {
                        var hintEntry = hnt.FirstOrDefault(e => e.Location.RelativeVirtualAddress == entry.Address);

                        if (hintEntry != null)
                            function = new ImportLibraryNamedFunction(entry, hintEntry);
                    }

                    if (function != null)
                        functions.Add(function);
                }

                var library = new DelayedImportLibrary(functions.ToArray(), name);

                libraries.Add(library);
            }

            var imports = new DelayedImports(libraries.ToArray());

            return imports;
        }

        #endregion
    }
}
