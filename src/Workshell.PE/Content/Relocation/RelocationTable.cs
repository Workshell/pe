using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE.Content
{
    public sealed class RelocationTable : DataContent, IEnumerable<RelocationBlock>
    {
        private readonly RelocationBlock[] _blocks;

        private RelocationTable(PortableExecutableImage image, DataDirectory dataDirectory, Location location, RelocationBlock[] blocks) : base(image, dataDirectory, location)
        {
            _blocks = blocks;

            Count = _blocks.Length;
        }

        #region Static Methods

        public static RelocationTable Get(PortableExecutableImage image)
        {
            return GetAsync(image).GetAwaiter().GetResult();
        }

        public static async Task<RelocationTable> GetAsync(PortableExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.BaseRelocationTable))
                return null;

            var dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.BaseRelocationTable];

            if (DataDirectory.IsNullOrEmpty(dataDirectory))
                return null;

            var calc = image.GetCalculator();
            var section = calc.RVAToSection(dataDirectory.VirtualAddress);
            var fileOffset = calc.RVAToOffset(section, dataDirectory.VirtualAddress);
            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
            var location = new Location(fileOffset, dataDirectory.VirtualAddress, imageBase + dataDirectory.VirtualAddress, dataDirectory.Size, dataDirectory.Size, section);
            var stream = image.GetStream();

            stream.Seek(fileOffset.ToInt64(), SeekOrigin.Begin);

            var blockOffset = fileOffset;
            var blockSize = 0u;
            var blocks = new List<RelocationBlock>();

            while (true)
            {
                IMAGE_BASE_RELOCATION baseRelocation;

                try
                {
                    baseRelocation = await stream.ReadStructAsync<IMAGE_BASE_RELOCATION>().ConfigureAwait(false);
                    blockSize += sizeof(ulong);
                }
                catch (Exception ex)
                {
                    throw new PortableExecutableImageException(image, "Could not read base relocation block from stream.", ex);
                }

                var count = (baseRelocation.SizeOfBlock - sizeof(ulong)) / sizeof(ushort);
                var baseRelocations = new ushort[count];

                for (var i = 0; i < count; i++)
                {
                    try
                    {
                        baseRelocations[i]  = await stream.ReadUInt16Async().ConfigureAwait(false);
                        blockSize += sizeof(ushort);
                    }
                    catch (Exception ex)
                    {
                        throw new PortableExecutableImageException(image, "Could not read base relocation from stream.", ex);
                    }
                }

                var blockRVA = calc.OffsetToRVA(blockOffset);
                var blockLocation = new Location(calc, blockOffset, blockRVA, imageBase + blockRVA, blockSize, blockSize);
                var block = new RelocationBlock(image, blockLocation, baseRelocation.VirtualAddress, baseRelocation.SizeOfBlock, baseRelocations);

                blocks.Add(block);

                if (blockSize >= dataDirectory.Size)
                    break;

                blockOffset += sizeof(ulong);
                blockOffset += sizeof(ushort) * count;
            }

            return new RelocationTable(image, dataDirectory, location, blocks.ToArray());
        }

        #endregion

        #region Methods

        public IEnumerator<RelocationBlock> GetEnumerator()
        {
            foreach (var block in _blocks)
                yield return block;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public int Count { get; }
        public RelocationBlock this[int index] => _blocks[index];

        #endregion
    }
}
