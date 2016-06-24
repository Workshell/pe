using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;

namespace Workshell.PE
{

    public sealed class ExportTable<T> : ExecutableImageContent, IEnumerable<T>, ISupportsBytes
    {

        private ExportDirectory directory;
        private T[] table;

        internal ExportTable(ExportDirectory exportDirectory, Location tableLocation, T[] tableContent) : base(exportDirectory.DataDirectory,tableLocation)
        {
            directory = exportDirectory;
            table = tableContent;
        }

        #region Static Methods

        public static ExportTable<uint> GetFunctionAddressTable(ExportDirectory directory)
        {
            if (directory == null)
                throw new ArgumentNullException("directory", "No export directory was specified.");

            LocationCalculator calc = directory.DataDirectory.Directories.Image.GetCalculator();
            Section section = calc.RVAToSection(directory.AddressOfFunctions);
            ulong file_offset = calc.RVAToOffset(section, directory.AddressOfFunctions);
            ulong image_base = directory.DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            uint size = directory.NumberOfFunctions * sizeof(uint);
            Location location = new Location(file_offset, directory.AddressOfFunctions, image_base + directory.AddressOfFunctions, size, size, section);
            Stream stream = directory.DataDirectory.Directories.Image.GetStream();

            stream.Seek(file_offset.ToInt64(), SeekOrigin.Begin);

            uint[] addresses = new uint[directory.NumberOfFunctions];

            for (int i = 0; i < directory.NumberOfFunctions; i++)
            {
                uint address = Utils.ReadUInt32(stream);

                addresses[i] = address;
            }

            ExportTable<uint> address_table = new ExportTable<uint>(directory, location, addresses);

            return address_table;
        }

        public static ExportTable<uint> GetNameAddressTable(ExportDirectory directory)
        {
            if (directory == null)
                throw new ArgumentNullException("directory", "No export directory was specified.");

            LocationCalculator calc = directory.DataDirectory.Directories.Image.GetCalculator();
            Section section = calc.RVAToSection(directory.AddressOfNames);
            ulong file_offset = calc.RVAToOffset(section, directory.AddressOfNames);
            ulong image_base = directory.DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            uint size = directory.NumberOfNames * sizeof(uint);
            Location location = new Location(file_offset, directory.AddressOfNames, image_base + directory.AddressOfNames, size, size, section);
            Stream stream = directory.DataDirectory.Directories.Image.GetStream();

            stream.Seek(file_offset.ToInt64(), SeekOrigin.Begin);

            uint[] addresses = new uint[directory.NumberOfNames];

            for (int i = 0; i < directory.NumberOfNames; i++)
            {
                uint address = Utils.ReadUInt32(stream);

                addresses[i] = address;
            }

            ExportTable<uint> address_table = new ExportTable<uint>(directory, location, addresses);

            return address_table;
        }

        public static ExportTable<ushort> GetOrdinalTable(ExportDirectory directory)
        {
            if (directory == null)
                throw new ArgumentNullException("directory", "No export directory was specified.");

            LocationCalculator calc = directory.DataDirectory.Directories.Image.GetCalculator();
            Section section = calc.RVAToSection(directory.AddressOfNameOrdinals);
            ulong file_offset = calc.RVAToOffset(section, directory.AddressOfNameOrdinals);
            ulong image_base = directory.DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            uint size = directory.NumberOfNames * sizeof(ushort);
            Location location = new Location(file_offset, directory.AddressOfNameOrdinals, image_base + directory.AddressOfNames, size, size, section);
            Stream stream = directory.DataDirectory.Directories.Image.GetStream();

            stream.Seek(file_offset.ToInt64(), SeekOrigin.Begin);

            ushort[] ordinals = new ushort[directory.NumberOfNames];

            for (var i = 0; i < directory.NumberOfNames; i++)
            {
                ushort ord = Utils.ReadUInt16(stream);

                ordinals[i] = ord;
            }

            ExportTable<ushort> ordinals_table = new ExportTable<ushort>(directory, location, ordinals);

            return ordinals_table;
        }

        #endregion

        #region Methods

        public IEnumerator<T> GetEnumerator()
        {
            for(var i = 0; i < table.Length; i++)
            {
                yield return table[i];
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

        #endregion

        #region Properties

        public int Count
        {
            get
            {
                return table.Length;
            }
        }

        public T this[int index]
        {
            get
            {
                return table[index];
            }
        }

        #endregion

    }

}
