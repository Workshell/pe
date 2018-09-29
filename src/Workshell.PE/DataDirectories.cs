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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE
{
    public sealed class DataDirectories : IEnumerable<DataDirectory>, ISupportsLocation, ISupportsBytes
    {
        private readonly PortableExecutableImage _image;
        private readonly Dictionary<DataDirectoryType,DataDirectory> _directories;

        internal DataDirectories(PortableExecutableImage image, OptionalHeader optHeader, IMAGE_DATA_DIRECTORY[] dataDirs)
        {
            _image = image;

            var size = (Utils.SizeOf<IMAGE_DATA_DIRECTORY>() * dataDirs.Length).ToUInt32();
            var fileOffset = optHeader.Location.FileOffset + optHeader.Location.FileSize;
            var rva = optHeader.Location.RelativeVirtualAddress + optHeader.Location.VirtualSize.ToUInt32();
            var va = optHeader.Location.VirtualAddress + optHeader.Location.VirtualSize;

            Location = new Location(image.GetCalculator(), fileOffset, rva, va, size, size);

            _directories = new Dictionary<DataDirectoryType,DataDirectory>();

            for (var i = 0; i < dataDirs.Length; i++)
            {
                var type = DataDirectoryType.Unknown;

                if (i >= 0 && i <= 14)
                    type = (DataDirectoryType)i;

                var dir = new DataDirectory(image,this,type,dataDirs[i],optHeader.ImageBase);

                _directories.Add(type,dir);
            }
        }

        #region Methods

        public IEnumerator<DataDirectory> GetEnumerator()
        {
            foreach(var kvp in _directories)
            {
                yield return kvp.Value;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
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

        public bool Exists(DataDirectoryType directoryType)
        {
            return _directories.ContainsKey(directoryType);
        }

        public bool Exists(int directoryType)
        {
            return Exists((DataDirectoryType)directoryType);
        }

        #endregion

        #region Properties

        public Location Location { get; }
        public int Count => _directories.Count;
        public DataDirectory this[DataDirectoryType directoryType] => (_directories.ContainsKey(directoryType) ? _directories[directoryType] : null);
        public DataDirectory this[int directoryType] => this[(DataDirectoryType)directoryType];

        #endregion 
    }
}
