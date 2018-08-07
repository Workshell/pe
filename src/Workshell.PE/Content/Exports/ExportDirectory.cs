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
                return null;

            var dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.ExportTable];

            if (DataDirectory.IsNullOrEmpty(dataDirectory))
                return null;

            try
            {
                var calc = image.GetCalculator();
                var rva = dataDirectory.VirtualAddress;
                var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
                var va = imageBase + rva;
                var section = calc.RVAToSection(rva);
                var offset = calc.RVAToOffset(section, rva);
                var size = Marshal.SizeOf<IMAGE_EXPORT_DIRECTORY>();
                var location = new Location(offset, rva, va, size.ToUInt32(), size.ToUInt32(), section);
                var stream = image.GetStream();

                stream.Seek(offset.ToInt64(), SeekOrigin.Begin);

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
            var offset = calc.RVAToOffset(directory.Name).ToInt64();

            stream.Seek(offset, SeekOrigin.Begin);

            while (true)
            {
                int value = await stream.ReadByteAsync().ConfigureAwait(false);

                if (value <= 0)
                    break;

                var c = (char)value;

                builder.Append(c);
            }

            return builder.ToString();
        }

        private static async Task<uint[]> BuildFunctionAddressesAsync(LocationCalculator calc, Stream stream, IMAGE_EXPORT_DIRECTORY directory)
        {
            var offset = calc.RVAToOffset(directory.AddressOfFunctions).ToInt64();

            stream.Seek(offset, SeekOrigin.Begin);

            var results = new uint[directory.NumberOfFunctions];

            for (var i = 0; i < directory.NumberOfFunctions; i++)
                results[i] = await stream.ReadUInt32Async().ConfigureAwait(false);

            return results;
        }

        private static async Task<uint[]> BuildFunctionNameAddressesAsync(LocationCalculator calc, Stream stream, IMAGE_EXPORT_DIRECTORY directory)
        {
            var offset = calc.RVAToOffset(directory.AddressOfNames).ToInt64();

            stream.Seek(offset, SeekOrigin.Begin);

            var results = new uint[directory.NumberOfNames];

            for (var i = 0; i < directory.NumberOfNames; i++)
                results[i] = await stream.ReadUInt32Async().ConfigureAwait(false);

            return results;
        }

        private static async Task<ushort[]> BuildFunctionOrdinalsAsync(LocationCalculator calc, Stream stream, IMAGE_EXPORT_DIRECTORY directory)
        {
            var offset = calc.RVAToOffset(directory.AddressOfNameOrdinals).ToInt64();

            stream.Seek(offset, SeekOrigin.Begin);

            var results = new ushort[directory.NumberOfNames];

            for (var i = 0; i < directory.NumberOfNames; i++)
                results[i] = await stream.ReadUInt16Async().ConfigureAwait(false);

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

        [FieldAnnotation("Characteristics")]
        public uint Characteristics { get; private set; }

        [FieldAnnotation("Date/Time Stamp")]
        public uint TimeDateStamp { get; private set; }

        [FieldAnnotation("Major Version")]
        public ushort MajorVersion { get; private set; }

        [FieldAnnotation("Minor Version")]
        public ushort MinorVersion { get; private set; }

        [FieldAnnotation("Name")]
        public uint Name { get; private set; }

        [FieldAnnotation("Base")]
        public uint Base { get; private set; }

        [FieldAnnotation("Number of Functions")]
        public uint NumberOfFunctions { get; private set; }

        [FieldAnnotation("Number of Names")]
        public uint NumberOfNames { get; private set; }

        [FieldAnnotation("Address of Functions")]
        public uint AddressOfFunctions { get; private set; }

        [FieldAnnotation("Address of Names")]
        public uint AddressOfNames { get; private set; }

        [FieldAnnotation("Address of Name Ordinals")]
        public uint AddressOfNameOrdinals { get; private set; }

        #endregion
    }
}
