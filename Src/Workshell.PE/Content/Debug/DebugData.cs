using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class DebugData : ISupportsLocation, ISupportsBytes
    {

        private DebugDirectoryEntry entry;
        private Location location;

        internal DebugData(DebugDirectoryEntry directorEntry, Location dataLocation)
        {
            entry = directorEntry;
            location = dataLocation;
        }

        #region Static Methods

        public static DebugData Get(DebugDirectoryEntry entry)
        {
            if (entry.PointerToRawData == 0 && entry.SizeOfData == 0)
                return null;

            LocationCalculator calc = entry.Directory.DataDirectory.Directories.Image.GetCalculator();
            uint rva = entry.AddressOfRawData;
            Section section = calc.RVAToSection(rva);
            ulong image_base = entry.Directory.DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            Location location = new Location(entry.PointerToRawData, rva, image_base + rva, entry.SizeOfData, entry.SizeOfData, section);
            DebugData data = new DebugData(entry, location);

            return data;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            string type = entry.GetEntryType().ToString();

            return String.Format("Debug Type: {0}, File Offset: 0x{1:X8}, Size: 0x{2:X8}", type, location.FileOffset, location.FileSize);
        }

        public byte[] GetBytes()
        {
            Stream stream = entry.Directory.DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream, location);

            return buffer;
        }

        #endregion

        #region Properties

        public DebugDirectoryEntry Entry
        {
            get
            {
                return entry;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        #endregion

    }

}
