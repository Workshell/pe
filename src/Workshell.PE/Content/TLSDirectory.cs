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
                return null;

            var dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.TLSTable];

            if (DataDirectory.IsNullOrEmpty(dataDirectory))
                return null;

            var calc = image.GetCalculator();
            var section = calc.RVAToSection(dataDirectory.VirtualAddress);
            var fileOffset = calc.RVAToOffset(section, dataDirectory.VirtualAddress);
            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;           
            var location = new Location(fileOffset, dataDirectory.VirtualAddress, imageBase + dataDirectory.VirtualAddress, dataDirectory.Size, dataDirectory.Size, section);
            var stream = image.GetStream();

            stream.Seek(fileOffset.ToInt64(), SeekOrigin.Begin);

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

        #region Properties

        [FieldAnnotation("Start Address of Raw Data")]
        public ulong StartAddress { get; }

        [FieldAnnotation("End Address of Raw Data")]
        public ulong EndAddress { get; }

        [FieldAnnotation("Address of Index")]
        public ulong AddressOfIndex { get; }

        [FieldAnnotation("Address of Callbacks")]
        public ulong AddressOfCallbacks { get; }

        [FieldAnnotation("Size of Zero Fill")]
        public uint SizeOfZeroFill { get; }

        [FieldAnnotation("Characteristics")]
        public uint Characteristics { get; }

        #endregion
    }
}

