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

        private DebugDirectory(PortableExecutableImage image, DataDirectory dataDirectory, Location location, DebugDirectoryEntry[] entries) : base(image, dataDirectory, location)
        {
            _entries = entries;

            Count = _entries.Length;
        }

        #region Static Methods

        public static DebugDirectory Get(PortableExecutableImage image)
        {
            return GetAsync(image).GetAwaiter().GetResult();
        }

        public static async Task<DebugDirectory> GetAsync(PortableExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.Debug))
            {
                return null;
            }

            var dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.Debug];

            if (DataDirectory.IsNullOrEmpty(dataDirectory))
            {
                return null;
            }

            try
            {
                var calc = image.GetCalculator();
                var section = calc.RVAToSection(dataDirectory.VirtualAddress);
                var fileOffset = calc.RVAToOffset(section, dataDirectory.VirtualAddress);
                var imageBase = image.NTHeaders.OptionalHeader.ImageBase;         
                var location = new Location(image, fileOffset, dataDirectory.VirtualAddress, imageBase + dataDirectory.VirtualAddress, dataDirectory.Size, dataDirectory.Size, section);
                var stream = image.GetStream();

                stream.Seek(fileOffset.ToInt32(), SeekOrigin.Begin);

                var entrySize = Utils.SizeOf<IMAGE_DEBUG_DIRECTORY>();
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
                var location = new Location(image, tuple.Item1, rva, va, entrySize.ToUInt32(), entrySize.ToUInt32());
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
            {
                yield return entry;
            }
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
