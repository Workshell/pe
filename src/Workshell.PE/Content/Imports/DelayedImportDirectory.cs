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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE.Content
{
    public sealed class DelayedImportDirectory : ImportDirectoryBase<DelayedImportDirectoryEntry>
    {
        private DelayedImportDirectory(PortableExecutableImage image, DataDirectory dataDirectory, Location location, DelayedImportDirectoryEntry[] entries) : base(image, dataDirectory, location, entries)
        {
        }

        #region Static Methods

        public static DelayedImportDirectory Get(PortableExecutableImage image)
        {
            return GetAsync(image).GetAwaiter().GetResult();
        }

        public static async Task<DelayedImportDirectory> GetAsync(PortableExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.DelayImportDescriptor))
            {
                return null;
            }

            var dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.DelayImportDescriptor];

            if (DataDirectory.IsNullOrEmpty(dataDirectory))
            {
                return null;
            }

            var calc = image.GetCalculator();
            var section = calc.RVAToSection(dataDirectory.VirtualAddress);
            var fileOffset = calc.RVAToOffset(section, dataDirectory.VirtualAddress);          
            var stream = image.GetStream();

            stream.Seek(fileOffset, SeekOrigin.Begin);

            var size = Utils.SizeOf<IMAGE_DELAY_IMPORT_DESCRIPTOR>();
            var descriptors = new List<Tuple<long, IMAGE_DELAY_IMPORT_DESCRIPTOR>>();

            try
            {
                var offset = 0L;

                while (true)
                {
                    var descriptor = await stream.ReadStructAsync<IMAGE_DELAY_IMPORT_DESCRIPTOR>(size).ConfigureAwait(false);

                    if (descriptor.Name == 0 && descriptor.ModuleHandle == 0)
                    {
                        break;
                    }

                    var tuple = new Tuple<long, IMAGE_DELAY_IMPORT_DESCRIPTOR>(offset, descriptor);

                    offset += size;

                    descriptors.Add(tuple);
                }
            }
            catch (Exception ex)
            {
                throw new PortableExecutableImageException(image, "Could not read delay import descriptor from stream.", ex);
            }

            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
            var totalSize = (descriptors.Count + 1) * size;
            var location = new Location(image, fileOffset, dataDirectory.VirtualAddress, imageBase + dataDirectory.VirtualAddress, totalSize.ToUInt32(), totalSize.ToUInt32(), section);
            var entries = new DelayedImportDirectoryEntry[descriptors.Count];

            for (var i = 0; i < descriptors.Count; i++)
            {
                try
                {
                    var entryOffset = fileOffset + descriptors[i].Item1;
                    var entryRVA = calc.OffsetToRVA(entryOffset);
                    var entryVA = imageBase + entryRVA;
                    var entryLocation = new Location(image, entryOffset, entryRVA, entryVA, size.ToUInt32(), size.ToUInt32());
                    var name = await GetNameAsync(calc, stream, descriptors[i].Item2.Name).ConfigureAwait(false);

                    entries[i] = new DelayedImportDirectoryEntry(image, entryLocation, descriptors[i].Item2, name);
                }
                catch (Exception ex)
                {
                    throw new PortableExecutableImageException(image, "Could not read delay import library name from stream.", ex);
                }
            }
            
            var result = new DelayedImportDirectory(image, dataDirectory, location, entries);

            return result;
        }

        #endregion
    }
}
