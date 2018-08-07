using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE.Content
{
    public sealed class DebugDirectory : DataContent, IEnumerable<DebugDirectoryEntry>
    {
        private readonly DebugDirectoryEntry[] _entries;

        private DebugDirectory(PortableExecutableImage image, DataDirectory directory, Location location, DebugDirectoryEntry[] entries) : base(image, directory, location)
        {
            _entries = entries;

            Count = _entries.Length;
        }

        #region Static Methods

        public DebugDirectory Get(PortableExecutableImage image)
        {
            return GetAsync(image).GetAwaiter().GetResult();
        }

        public static async Task<DebugDirectory> GetAsync(PortableExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.Debug))
                return null;

            var dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.Debug];

            if (DataDirectory.IsNullOrEmpty(dataDirectory))
                return null;

            try
            {
                var calc = image.GetCalculator();
                var section = calc.RVAToSection(dataDirectory.VirtualAddress);
                var fileOffset = calc.RVAToOffset(section, dataDirectory.VirtualAddress);
                var imageBase = image.NTHeaders.OptionalHeader.ImageBase;         
                var location = new Location(fileOffset, dataDirectory.VirtualAddress, imageBase + dataDirectory.VirtualAddress, dataDirectory.Size, dataDirectory.Size, section);
                var stream = image.GetStream();

                stream.Seek(fileOffset.ToInt32(), SeekOrigin.Begin);

                var entrySize = Marshal.SizeOf<IMAGE_DEBUG_DIRECTORY>();
                var entryCount = dataDirectory.Size / entrySize;
                var entries = new Tuple<ulong, IMAGE_DEBUG_DIRECTORY>[entryCount];

                for (var i = 0; i < entryCount; i++)
                {
                    var entry = await stream.ReadStructAsync<IMAGE_DEBUG_DIRECTORY>(entrySize).ConfigureAwait(false);

                    entries[i] = new Tuple<ulong, IMAGE_DEBUG_DIRECTORY>(fileOffset, entry);
                }

                var directoryEntries = LoadEntries(image, entrySize, entries);
                var directory = new DebugDirectory(image, dataDirectory, location, directoryEntries);

                return directory;
            }
            catch (Exception ex)
            {
                throw new PortableExecutableImageException(image, "Could not read debug directory from stream.", ex);
            }
        }

        private static DebugDirectoryEntry[] LoadEntries(PortableExecutableImage image, int entrySize, Tuple<ulong, IMAGE_DEBUG_DIRECTORY>[] entries)
        {
            var calc = image.GetCalculator();
            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
            var results = new DebugDirectoryEntry[entries.Length];

            for(var i = 0; i < entries.Length; i++)
            {
                var tuple = entries[i];
                var rva = calc.OffsetToRVA(tuple.Item1);
                var va = imageBase + rva;
                var location = new Location(calc, tuple.Item1, rva, va, entrySize.ToUInt32(), entrySize.ToUInt32());
                var entry = new DebugDirectoryEntry(image, location, tuple.Item2);

                results[i] = entry;
            }

            return results;
        }

        #endregion

        #region Methods

        public IEnumerator<DebugDirectoryEntry> GetEnumerator()
        {
            foreach (var entry in _entries)
                yield return entry;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public int Count { get; }
        public DebugDirectoryEntry this[int index] => _entries[index];

        #endregion
    }
}
