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
using Workshell.PE.Annotations;
using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE.Content
{
    public sealed class ExportDirectory : DataContent
    {
        private readonly string _name;
        private readonly uint[] _functionAddresses;
        private readonly uint[] _functionNameAddresses;
        private readonly ushort[] _functionOrdinals;

        private ExportDirectory(PortableExecutableImage image, DataDirectory dataDirectory, Location location, IMAGE_EXPORT_DIRECTORY directory,
            string name, uint[] functionAddresses, uint[] functionNameAddresses, ushort[] functionOrdinals) : base(image, dataDirectory, location)
        {
            _name = name;
            _functionAddresses = functionAddresses;
            _functionNameAddresses = functionNameAddresses;
            _functionOrdinals = functionOrdinals;

            Characteristics = directory.Characteristics;
            TimeDateStamp = directory.TimeDateStamp;
            MajorVersion = directory.MajorVersion;
            MinorVersion = directory.MinorVersion;
            Name = directory.Name;
            Base = directory.Base;
            NumberOfFunctions = directory.NumberOfFunctions;
            NumberOfNames = directory.NumberOfNames;
            AddressOfFunctions = directory.AddressOfFunctions;
            AddressOfNames = directory.AddressOfNames;
            AddressOfNameOrdinals = directory.AddressOfNameOrdinals;
        }

        #region Static Methods

        public static ExportDirectory Get(PortableExecutableImage image)
        {
            return GetAsync(image).GetAwaiter().GetResult();
        }

        public static async Task<ExportDirectory> GetAsync(PortableExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.ExportTable))
            {
                return null;
            }

            var dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.ExportTable];

            if (DataDirectory.IsNullOrEmpty(dataDirectory))
            {
                return null;
            }

            try
            {
                var calc = image.GetCalculator();
                var rva = dataDirectory.VirtualAddress;
                var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
                var va = imageBase + rva;
                var section = calc.RVAToSection(rva);
                var offset = calc.RVAToOffset(section, rva);
                var size = Utils.SizeOf<IMAGE_EXPORT_DIRECTORY>();
                var location = new Location(image, offset, rva, va, size.ToUInt32(), size.ToUInt32(), section);
                var stream = image.GetStream();

                stream.Seek(offset, SeekOrigin.Begin);

                var exportDirectory = await stream.ReadStructAsync<IMAGE_EXPORT_DIRECTORY>(size).ConfigureAwait(false);
                var name = await BuildNameAsync(calc, stream, exportDirectory).ConfigureAwait(false);
                var functionAddresses = await BuildFunctionAddressesAsync(calc, stream, exportDirectory).ConfigureAwait(false);
                var functionNameAddresses = await BuildFunctionNameAddressesAsync(calc, stream, exportDirectory).ConfigureAwait(false);
                var functionOrdinals = await BuildFunctionOrdinalsAsync(calc, stream, exportDirectory).ConfigureAwait(false);

                return new ExportDirectory(image, dataDirectory, location, exportDirectory, name, functionAddresses, functionNameAddresses, functionOrdinals);
            }
            catch (Exception ex)
            {
                throw new PortableExecutableImageException(image, "Could not read export directory from stream.", ex);
            }
        }

        private static async Task<string> BuildNameAsync(LocationCalculator calc, Stream stream, IMAGE_EXPORT_DIRECTORY directory)
        {
            var builder = new StringBuilder(256);
            var offset = calc.RVAToOffset(directory.Name);

            stream.Seek(offset, SeekOrigin.Begin);

            while (true)
            {
                var value = await stream.ReadByteAsync().ConfigureAwait(false);

                if (value == 0)
                {
                    break;
                }

                var c = (char)value;

                builder.Append(c);
            }

            return builder.ToString();
        }

        private static async Task<uint[]> BuildFunctionAddressesAsync(LocationCalculator calc, Stream stream, IMAGE_EXPORT_DIRECTORY directory)
        {
            var offset = calc.RVAToOffset(directory.AddressOfFunctions);

            stream.Seek(offset, SeekOrigin.Begin);

            var results = new uint[directory.NumberOfFunctions];

            for (var i = 0; i < directory.NumberOfFunctions; i++)
            {
                results[i] = await stream.ReadUInt32Async().ConfigureAwait(false);
            }

            return results;
        }

        private static async Task<uint[]> BuildFunctionNameAddressesAsync(LocationCalculator calc, Stream stream, IMAGE_EXPORT_DIRECTORY directory)
        {
            var offset = calc.RVAToOffset(directory.AddressOfNames);

            stream.Seek(offset, SeekOrigin.Begin);

            var results = new uint[directory.NumberOfNames];

            for (var i = 0; i < directory.NumberOfNames; i++)
            {
                results[i] = await stream.ReadUInt32Async().ConfigureAwait(false);
            }

            return results;
        }

        private static async Task<ushort[]> BuildFunctionOrdinalsAsync(LocationCalculator calc, Stream stream, IMAGE_EXPORT_DIRECTORY directory)
        {
            var offset = calc.RVAToOffset(directory.AddressOfNameOrdinals);

            stream.Seek(offset, SeekOrigin.Begin);

            var results = new ushort[directory.NumberOfNames];

            for (var i = 0; i < directory.NumberOfNames; i++)
            {
                results[i] = await stream.ReadUInt16Async().ConfigureAwait(false);
            }

            return results;
        }

        #endregion

        #region Methods

        public DateTime GetTimeDateStamp()
        {
            return Utils.ConvertTimeDateStamp(TimeDateStamp);
        }

        public Version GetVersion()
        {
            return new Version(MajorVersion, MinorVersion);
        }

        public string GetName()
        {
            return _name;
        }

        public uint[] GetFunctionAddresses()
        {
            return _functionAddresses;
        }

        public uint[] GetFunctionNameAddresses()
        {
            return _functionNameAddresses;
        }

        public ushort[] GetFunctionOrdinals()
        {
            return _functionOrdinals;
        }

        #endregion

        #region Properties

        [FieldAnnotation("Characteristics", Order = 1)]
        public uint Characteristics { get; private set; }

        [FieldAnnotation("Date/Time Stamp", Order = 2)]
        public uint TimeDateStamp { get; private set; }

        [FieldAnnotation("Major Version", Order = 3)]
        public ushort MajorVersion { get; private set; }

        [FieldAnnotation("Minor Version", Order = 4)]
        public ushort MinorVersion { get; private set; }

        [FieldAnnotation("Name", Order = 5)]
        public uint Name { get; private set; }

        [FieldAnnotation("Base", Order = 6)]
        public uint Base { get; private set; }

        [FieldAnnotation("Number of Functions", Order = 7)]
        public uint NumberOfFunctions { get; private set; }

        [FieldAnnotation("Number of Names", Order = 8)]
        public uint NumberOfNames { get; private set; }

        [FieldAnnotation("Address of Functions", Order = 9)]
        public uint AddressOfFunctions { get; private set; }

        [FieldAnnotation("Address of Names", Order = 10)]
        public uint AddressOfNames { get; private set; }

        [FieldAnnotation("Address of Name Ordinals", Order = 11)]
        public uint AddressOfNameOrdinals { get; private set; }

        #endregion
    }
}
