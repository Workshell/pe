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
    public sealed class ImportDirectory : ImportDirectoryBase<ImportDirectoryEntry>
    {
        internal ImportDirectory(PortableExecutableImage image, DataDirectory directory, Location location, ImportDirectoryEntry[] entries) : base(image, directory, location, entries)
        {
        }

        #region Static Methods

        internal static async Task<ImportDirectory> LoadAsync(PortableExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.ImportTable))
                return null;

            DataDirectory dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.ImportTable];

            if (DataDirectory.IsNullOrEmpty(dataDirectory))
                return null;

            var calc = image.GetCalculator();
            var section = calc.RVAToSection(dataDirectory.VirtualAddress);
            var fileOffset = calc.RVAToOffset(section, dataDirectory.VirtualAddress);          
            var stream = image.GetStream();

            stream.Seek(fileOffset.ToInt64(), SeekOrigin.Begin);

            var size = Marshal.SizeOf<IMAGE_IMPORT_DESCRIPTOR>();
            var descriptors = new List<Tuple<ulong, IMAGE_IMPORT_DESCRIPTOR>>();

            try
            {
                ulong offset = 0;

                while (true)
                {
                    var descriptor = await stream.ReadStructAsync<IMAGE_IMPORT_DESCRIPTOR>(size).ConfigureAwait(false);

                    if (descriptor.OriginalFirstThunk == 0 && descriptor.FirstThunk == 0)
                        break;

                    var tuple = new Tuple<ulong, IMAGE_IMPORT_DESCRIPTOR>(offset, descriptor);

                    offset += size.ToUInt32();

                    descriptors.Add(tuple);
                }
            }
            catch (Exception ex)
            {
                throw new PortableExecutableImageException(image, "Could not read import descriptor from stream.", ex);
            }

            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
            var totalSize = (descriptors.Count + 1) * size;
            var location = new Location(fileOffset, dataDirectory.VirtualAddress, imageBase + dataDirectory.VirtualAddress, totalSize.ToUInt32(), totalSize.ToUInt32(), section);
            var entries = new ImportDirectoryEntry[descriptors.Count];

            for (var i = 0; i < descriptors.Count; i++)
            {
                try
                {
                    var entryOffset = fileOffset + descriptors[i].Item1;
                    var entryRVA = calc.OffsetToRVA(entryOffset);
                    var entryVA = imageBase + entryRVA;
                    var entryLocation = new Location(calc, entryOffset, entryRVA, entryVA, size.ToUInt32(), size.ToUInt32());
                    var name = await GetNameAsync(calc, stream, descriptors[i].Item2.Name).ConfigureAwait(false);

                    entries[i] = new ImportDirectoryEntry(image, entryLocation, descriptors[i].Item2, name);
                }
                catch (Exception ex)
                {
                    throw new PortableExecutableImageException(image, "Could not read import library name from stream.", ex);
                }
            }
            
            var result = new ImportDirectory(image, dataDirectory, location, entries);

            return result;
        }

        #endregion
    }
}
