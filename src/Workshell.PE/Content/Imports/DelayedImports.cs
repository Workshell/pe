#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

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
            var directory = await DelayedImportDirectory.GetAsync(image).ConfigureAwait(false);

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
