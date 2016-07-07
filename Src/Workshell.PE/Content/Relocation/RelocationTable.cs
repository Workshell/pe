#region License
//  Copyright(c) 2016, Workshell Ltd
//  All rights reserved.
//  
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Workshell Ltd nor the names of its contributors
//  may be used to endorse or promote products
//  derived from this software without specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED.IN NO EVENT SHALL WORKSHELL BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

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

            DataDirectory directory = image.NTHeaders.DataDirectories[DataDirectoryType.BaseRelocationTable];

            if (DataDirectory.IsNullOrEmpty(directory))
                return null;

            LocationCalculator calc = directory.Directories.Image.GetCalculator();
            Section section = calc.RVAToSection(directory.VirtualAddress);
            ulong file_offset = calc.RVAToOffset(section, directory.VirtualAddress);
            ulong image_base = directory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            Location location = new Location(file_offset, directory.VirtualAddress, image_base + directory.VirtualAddress, directory.Size, directory.Size, section);
            Stream stream = directory.Directories.Image.GetStream();

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
            Stream stream = DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream, Location);

            return buffer;
        }

        private RelocationBlock[] Load(Tuple<ulong,uint,uint,ushort[]>[] relocBlocks)
        {
            LocationCalculator calc = DataDirectory.Directories.Image.GetCalculator();
            RelocationBlock[] results = new RelocationBlock[relocBlocks.Length];

            for(var i = 0; i < relocBlocks.Length; i++)
            {
                Tuple<ulong, uint, uint, ushort[]> tuple = relocBlocks[i];
                ulong offset = tuple.Item1;
                uint rva = calc.OffsetToRVA(offset);
                Section section = calc.RVAToSection(rva);
                ulong va = DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase + rva;
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
