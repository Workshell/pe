using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public sealed class RelocationBlock : IEnumerable<Relocation>, ISupportsLocation, ISupportsBytes
    {
        private readonly PortableExecutableImage _image;
        private readonly Relocation[] _relocations;

        internal RelocationBlock(PortableExecutableImage image, Location location, uint pageRVA, uint blockSize, ushort[] relocations)
        {
            _image = image;
            _relocations = new Relocation[relocations.Length];

            Location = location;
            PageRVA = pageRVA;
            BlockSize = blockSize;
            Section = GetSection(pageRVA);
            Count = relocations.Length;

            for (var i = 0; i < relocations.Length; i++)
                _relocations[i] = new Relocation(this, relocations[i]);
        }

        #region Methods

        public override string ToString()
        {
            return $"Page RVA: 0x{PageRVA:X8}, Block Size: {BlockSize}, Relocations: {_relocations.Length}";
        }

        public IEnumerator<Relocation> GetEnumerator()
        {
            foreach (var relocation in _relocations)
                yield return relocation;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public byte[] GetBytes()
        {
            return GetBytesAsync().GetAwaiter().GetResult();
        }

        public async Task<byte[]> GetBytesAsync()
        {
            var stream = _image.GetStream();
            var buffer = await stream.ReadBytesAsync(Location).ConfigureAwait(false);

            return buffer;
        }

        private Section GetSection(uint pageRVA)
        {
            var calc = _image.GetCalculator();
            var section = calc.RVAToSection(pageRVA);

            return section;
        }

        #endregion

        #region Properties

        public Location Location { get; }
        public uint PageRVA { get; }
        public uint BlockSize { get; }
        public Section Section { get; }
        public int Count { get; }
        public Relocation this[int index] => _relocations[index];

        #endregion
    }
}
