using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class RelocationTable : ExecutableImageContent, IEnumerable<RelocationBlock>, ISupportsBytes
    {

        private RelocationBlock[] blocks;

        internal RelocationTable(DataDirectory dataDirectory, Location relocLocation, Tuple<ulong,uint,uint,ushort[]>[] relocBlocks) : base(dataDirectory,relocLocation)
        {
            blocks = Load(relocBlocks);
        }

        #region Static Methods

        public static RelocationTable Get(ExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.Debug))
                return null;

            DataDirectory directory = image.NTHeaders.DataDirectories[DataDirectoryType.Debug];

            if (DataDirectory.IsNullOrEmpty(directory))
                return null;

            LocationCalculator calc = directory.Directories.Reader.GetCalculator();
            Section section = calc.RVAToSection(directory.VirtualAddress);
            ulong file_offset = calc.RVAToOffset(section, directory.VirtualAddress);
            ulong image_base = directory.Directories.Reader.NTHeaders.OptionalHeader.ImageBase;
            Location location = new Location(file_offset, directory.VirtualAddress, image_base + directory.VirtualAddress, directory.Size, directory.Size, section);
            Stream stream = directory.Directories.Reader.GetStream();

            stream.Seek(file_offset.ToInt64(), SeekOrigin.Begin);

            ulong block_offset = file_offset;
            uint block_size = 0;
            List<Tuple<ulong, uint, uint, ushort[]>> block_relocs = new List<Tuple<ulong, uint, uint, ushort[]>>();

            while (true)
            {
                IMAGE_BASE_RELOCATION base_reloc = Utils.Read<IMAGE_BASE_RELOCATION>(stream);

                block_size += sizeof(ulong);

                long count = (base_reloc.SizeOfBlock - sizeof(ulong)) / sizeof(ushort);
                ushort[] relocs = new ushort[count];

                for (var i = 0; i < count; i++)
                {
                    ushort reloc = Utils.ReadUInt16(stream);

                    relocs[i] = reloc;
                    block_size += sizeof(ushort);
                }

                Tuple<ulong, uint, uint, ushort[]> block = new Tuple<ulong, uint, uint, ushort[]>(block_offset, base_reloc.SizeOfBlock, base_reloc.VirtualAddress, relocs);

                block_relocs.Add(block);

                if (block_size >= directory.Size)
                    break;

                block_offset += sizeof(ulong);
                block_offset += sizeof(ushort) * (uint)count;
            }

            RelocationTable relocations = new RelocationTable(directory, location, block_relocs.ToArray());

            return relocations;
        }

        #endregion

        #region Methods

        public IEnumerator<RelocationBlock> GetEnumerator()
        {
            for(var i = 0; i < blocks.Length; i++)
            {
                yield return blocks[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public byte[] GetBytes()
        {
            Stream stream = DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream, Location);

            return buffer;
        }

        private RelocationBlock[] Load(Tuple<ulong,uint,uint,ushort[]>[] relocBlocks)
        {
            LocationCalculator calc = DataDirectory.Directories.Reader.GetCalculator();
            RelocationBlock[] results = new RelocationBlock[relocBlocks.Length];

            for(var i = 0; i < relocBlocks.Length; i++)
            {
                Tuple<ulong, uint, uint, ushort[]> tuple = relocBlocks[i];
                ulong offset = tuple.Item1;
                uint rva = calc.OffsetToRVA(offset);
                Section section = calc.RVAToSection(rva);
                ulong va = DataDirectory.Directories.Reader.NTHeaders.OptionalHeader.ImageBase + rva;
                uint size = tuple.Item2;
                uint page_rva = tuple.Item3;
                ushort[] relocs = tuple.Item4;
                Location block_location = new Location(offset, rva, va, size, size, section);
                RelocationBlock block = new RelocationBlock(this, block_location, page_rva, size, relocs);

                results[i] = block;
            }

            return results;
        }

        #endregion

        #region Properties

        public int Count
        {
            get
            {
                return blocks.Length;
            }
        }

        public RelocationBlock this[int index]
        {
            get
            {
                return blocks[index];
            }
        }

        #endregion

    }

}
