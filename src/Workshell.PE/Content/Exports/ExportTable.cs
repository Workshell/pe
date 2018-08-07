using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public abstract class ExportTable : DataContent
    {
        protected ExportTable(PortableExecutableImage image, DataDirectory directory, Location location) : base(image, directory, location)
        {
        }

        #region Static Methods

        public static async Task<ExportTable<uint>> GetFunctionAddressTableAsync(PortableExecutableImage image, ExportDirectory directory = null)
        {
            if (directory == null)
                directory = await ExportDirectory.GetAsync(image).ConfigureAwait(false);

            var calc = image.GetCalculator();
            var section = calc.RVAToSection(directory.AddressOfFunctions);
            var fileOffset = calc.RVAToOffset(section, directory.AddressOfFunctions);
            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
            var size = directory.NumberOfFunctions * sizeof(uint);
            var location = new Location(fileOffset, directory.AddressOfFunctions, imageBase + directory.AddressOfFunctions, size, size, section);
            var stream = image.GetStream();

            stream.Seek(fileOffset.ToInt64(), SeekOrigin.Begin);

            var addresses = new uint[directory.NumberOfFunctions];

            try
            {
                for (var i = 0; i < directory.NumberOfFunctions; i++)
                    addresses[i] = await stream.ReadUInt32Async().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new PortableExecutableImageException(image, "Could not read address of function from stream.", ex);
            }


            var dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.ExportTable];
            var table = new ExportTable<uint>(image, dataDirectory, location, directory, addresses);

            return table;
        }

        public static async Task<ExportTable<uint>> GetNameAddressTableAsync(PortableExecutableImage image, ExportDirectory directory = null)
        {
            if (directory == null)
                directory = await ExportDirectory.GetAsync(image).ConfigureAwait(false);

            var calc = image.GetCalculator();
            var section = calc.RVAToSection(directory.AddressOfNames);
            var fileOffset = calc.RVAToOffset(section, directory.AddressOfNames);
            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
            var size = directory.NumberOfNames * sizeof(uint);
            var location = new Location(fileOffset, directory.AddressOfNames, imageBase + directory.AddressOfNames, size, size, section);
            var stream = image.GetStream();

            stream.Seek(fileOffset.ToInt64(), SeekOrigin.Begin);

            var addresses = new uint[directory.NumberOfNames];

            try
            {
                for (var i = 0; i < directory.NumberOfNames; i++)
                    addresses[i] = await stream.ReadUInt32Async().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new PortableExecutableImageException(image, "Could not read address of name from stream.", ex);
            }


            var dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.ExportTable];
            var table = new ExportTable<uint>(image, dataDirectory, location, directory, addresses);

            return table;
        }

        public static async Task<ExportTable<ushort>> GetOrdinalTableAsync(PortableExecutableImage image, ExportDirectory directory = null)
        {
            if (directory == null)
                directory = await ExportDirectory.GetAsync(image).ConfigureAwait(false);

            var calc = image.GetCalculator();
            var section = calc.RVAToSection(directory.AddressOfNameOrdinals);
            var fileOffset = calc.RVAToOffset(section, directory.AddressOfNameOrdinals);
            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
            var size = directory.NumberOfNames * sizeof(ushort);
            var location = new Location(fileOffset, directory.AddressOfNameOrdinals, imageBase + directory.AddressOfNameOrdinals, size, size, section);
            var stream = image.GetStream();

            stream.Seek(fileOffset.ToInt64(), SeekOrigin.Begin);

            var ordinals = new ushort[directory.NumberOfNames];

            try
            {
                for (var i = 0; i < directory.NumberOfNames; i++)
                    ordinals[i] = await stream.ReadUInt16Async().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new PortableExecutableImageException(image, "Could not read ordinal of name from stream.", ex);
            }


            var dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.ExportTable];
            var table = new ExportTable<ushort>(image, dataDirectory, location, directory, ordinals);

            return table;
        }

        #endregion
    }

    public sealed class ExportTable<T> : ExportTable, IEnumerable<T>
    {
        private readonly ExportDirectory _directory;
        private readonly T[] _table;

        internal ExportTable(PortableExecutableImage image, DataDirectory dataDirectory, Location location, ExportDirectory directory, T[] table) : base(image, dataDirectory, location)
        {
            _directory = directory;
            _table = table;

            Count = _table.Length;
        }

        #region Methods

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var entry in _table)
                yield return entry;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public int Count { get; }
        public T this[int index] => _table[index];

        #endregion
    }
}
