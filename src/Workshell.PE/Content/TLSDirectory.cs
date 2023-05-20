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
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE.Content
{
    public sealed class TLSDirectory : DataContent
    {
        private TLSDirectory(PortableExecutableImage image, DataDirectory dataDirectory, Location location, IMAGE_TLS_DIRECTORY32 directory) : base(image, dataDirectory, location)
        {
            StartAddress = directory.StartAddress;
            EndAddress = directory.EndAddress;
            AddressOfIndex = directory.AddressOfIndex;
            AddressOfCallbacks = directory.AddressOfCallbacks;
            SizeOfZeroFill = directory.SizeOfZeroFill;
            Characteristics = directory.Characteristics;
        }

        private TLSDirectory(PortableExecutableImage image, DataDirectory dataDirectory, Location location, IMAGE_TLS_DIRECTORY64 directory) : base(image, dataDirectory, location)
        {
            StartAddress = directory.StartAddress;
            EndAddress = directory.EndAddress;
            AddressOfIndex = directory.AddressOfIndex;
            AddressOfCallbacks = directory.AddressOfCallbacks;
            SizeOfZeroFill = directory.SizeOfZeroFill;
            Characteristics = directory.Characteristics;
        }

        #region Static Methods

        public static TLSDirectory Get(PortableExecutableImage image)
        {
            return GetAsync(image).GetAwaiter().GetResult();
        }

        public static async Task<TLSDirectory> GetAsync(PortableExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.TLSTable))
            {
                return null;
            }

            var dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.TLSTable];

            if (DataDirectory.IsNullOrEmpty(dataDirectory))
            {
                return null;
            }

            var calc = image.GetCalculator();
            var section = calc.RVAToSection(dataDirectory.VirtualAddress);
            var fileOffset = calc.RVAToOffset(section, dataDirectory.VirtualAddress);
            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;           
            var location = new Location(image, fileOffset, dataDirectory.VirtualAddress, imageBase + dataDirectory.VirtualAddress, dataDirectory.Size, dataDirectory.Size, section);
            var stream = image.GetStream();

            stream.Seek(fileOffset, SeekOrigin.Begin);

            TLSDirectory directory = null;

            if (image.Is32Bit)
            {
                IMAGE_TLS_DIRECTORY32 config;

                try
                {
                    config = await stream.ReadStructAsync<IMAGE_TLS_DIRECTORY32>().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new PortableExecutableImageException(image, "Could not load TLS Directory from stream.", ex);
                }

                directory = new TLSDirectory(image, dataDirectory, location, config);
            }
            else
            {
                IMAGE_TLS_DIRECTORY64 config;

                try
                {
                    config = await stream.ReadStructAsync<IMAGE_TLS_DIRECTORY64>().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new PortableExecutableImageException(image, "Could not load TLS Directory from stream.", ex);
                }

                directory = new TLSDirectory(image, dataDirectory, location, config);
            }

            return directory;
        }

        #endregion

        #region Methods

        public SectionCharacteristicsType GetCharacteristics()
        {
            return (SectionCharacteristicsType)Characteristics;
        }

        #endregion

        #region Properties

        [FieldAnnotation("Start Address of Raw Data", Order = 1)]
        public ulong StartAddress { get; }

        [FieldAnnotation("End Address of Raw Data", Order = 2)]
        public ulong EndAddress { get; }

        [FieldAnnotation("Address of Index", Order = 3)]
        public ulong AddressOfIndex { get; }

        [FieldAnnotation("Address of Callbacks", Order = 4)]
        public ulong AddressOfCallbacks { get; }

        [FieldAnnotation("Size of Zero Fill", Order = 5)]
        public uint SizeOfZeroFill { get; }

        [FieldAnnotation("Characteristics", Order = 6)]
        public uint Characteristics { get; }

        #endregion
    }
}

