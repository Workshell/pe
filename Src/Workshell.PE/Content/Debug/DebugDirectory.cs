using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public enum DebugDirectoryEntryType
    {
        [EnumAnnotation("IMAGE_DEBUG_TYPE_UNKNOWN")]
        Unknown = 0,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_COFF")]
        COFF = 1,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_CODEVIEW")]
        CodeView = 2,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_FPO")]
        FPO = 3,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_MISC")]
        Misc = 4,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_EXCEPTION")]
        Exception = 5,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_FIXUP")]
        Fixup = 6,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_OMAP_TO_SRC")]
        OMAPToSrc = 7,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_OMAP_FROM_SRC")]
        OMAPFromSrc = 8,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_BORLAND")]
        Bolrand = 9,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_RESERVED10")]
        Reserved = 10,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_CLSID")]
        CLSID = 11,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_VC_FEATURE")]
        VCFeature = 12,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_POGO")]
        POGO = 13,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_ILTCG")]
        ILTCG = 14,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_MPX")]
        MPX = 15
    }

    public sealed class DebugDirectoryEntry : ISupportsLocation, ISupportsBytes
    {

        public static readonly int size = Utils.SizeOf<IMAGE_DEBUG_DIRECTORY>();

        private DebugDirectory directories;
        private Location location;
        private IMAGE_DEBUG_DIRECTORY directory;
        private DebugData data;

        internal DebugDirectoryEntry(DebugDirectory debugDirs, Location dirLocation, IMAGE_DEBUG_DIRECTORY dir)
        {
            directories = debugDirs;
            location = dirLocation;
            directory = dir;
            data = null;
        }

        #region Methods

        public override string ToString()
        {
            return String.Format("Debug Type: {0}", GetEntryType());
        }

        public byte[] GetBytes()
        {
            Stream stream = directories.DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream, location);

            return buffer;
        }

        public DateTime GetTimeDateStamp()
        {
            return Utils.ConvertTimeDateStamp(directory.TimeDateStamp);
        }

        public DebugDirectoryEntryType GetEntryType()
        {
            return (DebugDirectoryEntryType)directory.Type;
        }

        public DebugData GetData()
        {
            if (data == null)
                data = DebugData.Get(this);

            return data;
        }

        #endregion

        #region Properties

        public DebugDirectory Directory
        {
            get
            {
                return directories;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        [FieldAnnotation("Characteristics")]
        public uint Characteristics
        {
            get
            {
                return directory.Characteristics;
            }
        }

        [FieldAnnotation("Date/Time Stamp")]
        public uint TimeDateStamp
        {
            get
            {
                return directory.TimeDateStamp;
            }
        }

        [FieldAnnotation("Major Version")]
        public ushort MajorVersion
        {
            get
            {
                return directory.MajorVersion;
            }
        }

        [FieldAnnotation("Minor Version")]
        public ushort MinorVersion
        {
            get
            {
                return directory.MinorVersion;
            }
        }

        [FieldAnnotation("Type")]
        public uint Type
        {
            get
            {
                return directory.Type;
            }
        }

        [FieldAnnotation("Size of Data")]
        public uint SizeOfData
        {
            get
            {
                return directory.SizeOfData;
            }
        }

        [FieldAnnotation("Address of Raw Data")]
        public uint AddressOfRawData
        {
            get
            {
                return directory.AddressOfRawData;
            }
        }

        [FieldAnnotation("Pointer to Raw Data")]
        public uint PointerToRawData
        {
            get
            {
                return directory.PointerToRawData;
            }
        }

        #endregion

    }

    public sealed class DebugDirectory : ExecutableImageContent, IEnumerable<DebugDirectoryEntry>, ISupportsBytes
    {

        private DebugDirectoryEntry[] entries;

        internal DebugDirectory(DataDirectory dataDirectory, Location dirLocation, Tuple<ulong,IMAGE_DEBUG_DIRECTORY>[] dirs) : base(dataDirectory,dirLocation)
        {
            entries = LoadEntries(dirs);
        }

        #region Static Methods

        public static DebugDirectory Get(ExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.Debug))
                return null;

            DataDirectory directory = image.NTHeaders.DataDirectories[DataDirectoryType.Debug];

            if (DataDirectory.IsNullOrEmpty(directory))
                return null;

            LocationCalculator calc = directory.Directories.Reader.GetCalculator();
            Section section = calc.RVAToSection(directory.VirtualAddress);
            ulong file_offset = calc.RVAToOffset(section, directory.VirtualAddress);
            ulong image_base = directory.Directories.Reader.NTHeaders.OptionalHeader.ImageBase;
            Location location = new Location(file_offset, directory.VirtualAddress, image_base + directory.VirtualAddress, directory.Size, directory.Size, section);
            Stream stream = directory.Directories.Reader.GetStream();

            stream.Seek(file_offset.ToInt64(), SeekOrigin.Begin);

            int size = Utils.SizeOf<IMAGE_DEBUG_DIRECTORY>();
            long count = directory.Size / size;
            Tuple<ulong, IMAGE_DEBUG_DIRECTORY>[] directories = new Tuple<ulong, IMAGE_DEBUG_DIRECTORY>[count];

            for (var i = 0; i < count; i++)
            {
                IMAGE_DEBUG_DIRECTORY entry = Utils.Read<IMAGE_DEBUG_DIRECTORY>(stream, size);

                directories[i] = new Tuple<ulong, IMAGE_DEBUG_DIRECTORY>(file_offset, entry);
            }

            DebugDirectory debug_dir = new DebugDirectory(directory, location, directories);

            return debug_dir;
        }

        #endregion

        #region Methods

        public IEnumerator<DebugDirectoryEntry> GetEnumerator()
        {
            for(var i = 0; i < entries.Length; i++)
            {
                yield return entries[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("Debug Entry Count: {0}", entries.Length);
        }

        public byte[] GetBytes()
        {
            Stream stream = DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream, Location);

            return buffer;
        }

        private DebugDirectoryEntry[] LoadEntries(Tuple<ulong, IMAGE_DEBUG_DIRECTORY>[] directoryEntries)
        {
            LocationCalculator calc = DataDirectory.Directories.Reader.GetCalculator();
            ulong image_base = DataDirectory.Directories.Reader.NTHeaders.OptionalHeader.ImageBase;
            uint size = Utils.SizeOf<IMAGE_DEBUG_DIRECTORY>().ToUInt32();
            DebugDirectoryEntry[] results = new DebugDirectoryEntry[directoryEntries.Length];

            for(var i = 0; i < directoryEntries.Length; i++)
            {
                Tuple<ulong, IMAGE_DEBUG_DIRECTORY> tuple = directoryEntries[i];
                uint rva = calc.OffsetToRVA(tuple.Item1);
                Section section = calc.RVAToSection(rva);
                ulong va = image_base + rva;
                Location dir_location = new Location(tuple.Item1, rva, va, size, size, section);
                DebugDirectoryEntry entry = new DebugDirectoryEntry(this, dir_location, tuple.Item2);

                results[i] = entry;
            }

            return results;
        }

        #endregion

        #region Properties

        public int Count
        {
            get
            {
                return entries.Length;
            }
        }

        public DebugDirectoryEntry this[int index]
        {
            get
            {
                return this[index];
            }
        }

        #endregion

    }

}
