using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Content
{
    public abstract class ExceptionTable : DataContent, IEnumerable<ExceptionTableEntry>
    {
        private readonly ExceptionTableEntry[] _entries;

        protected internal ExceptionTable(PortableExecutableImage image, DataDirectory directory, Location location, ExceptionTableEntry[] entries) : base(image, directory, location)
        {
            _entries = entries;

            Count = _entries.Length;
        }

        #region Static Methods

        public static async Task<ExceptionTable> GetAsync(PortableExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.ExceptionTable))
                return null;

            var dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.ExceptionTable];

            if (DataDirectory.IsNullOrEmpty(dataDirectory))
                return null;

            var calc = image.GetCalculator();
            var section = calc.RVAToSection(dataDirectory.VirtualAddress);
            var fileOffset = calc.RVAToOffset(section, dataDirectory.VirtualAddress);
            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;           
            var location = new Location(fileOffset, dataDirectory.VirtualAddress, imageBase + dataDirectory.VirtualAddress, dataDirectory.Size, dataDirectory.Size, section);
            ExceptionTable table;

            if (image.Is64Bit)
            {
                table = await ExceptionTable64.GetAsync(image, dataDirectory, location).ConfigureAwait(false);
            }
            else
            {
                table = await ExceptionTable32.GetAsync(image, dataDirectory, location).ConfigureAwait(false);
            }

            return table;
        }

        #endregion

        #region Methods

        public IEnumerator<ExceptionTableEntry> GetEnumerator()
        {
            foreach (var function in _entries)
                yield return function;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
        
        #region Properties

        public int Count { get; }
        public ExceptionTableEntry this[int index] => _entries[index];

        #endregion
    }
}
