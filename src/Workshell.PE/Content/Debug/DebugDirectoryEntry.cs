using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Annotations;
using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE.Content
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
        private readonly PortableExecutableImage _image;
        private DebugData _data;

        internal DebugDirectoryEntry(PortableExecutableImage image, Location location, IMAGE_DEBUG_DIRECTORY directory)
        {
            _image = image;

            Location = location;
            Characteristics = directory.Characteristics;
            TimeDateStamp = directory.TimeDateStamp;
            MajorVersion = directory.MajorVersion;
            MinorVersion = directory.MinorVersion;
            Type = directory.Type;
            SizeOfData = directory.SizeOfData;
            AddressOfRawData = directory.AddressOfRawData;
            PointerToRawData = directory.PointerToRawData;
        }

        #region Methods

        public override string ToString()
        {
            return $"Debug Type: {GetEntryType()}";
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

        public DateTime GetTimeDateStamp()
        {
            return Utils.ConvertTimeDateStamp(TimeDateStamp);
        }

        public Version GetVersion()
        {
            return new Version(MajorVersion, MinorVersion);
        }

        public DebugDirectoryEntryType GetEntryType()
        {
            return (DebugDirectoryEntryType)Type;
        }

        public DebugData GetData()
        {
            if (_data == null)
            {
                if (PointerToRawData == 0 && SizeOfData == 0)
                    return null;

                var calc = _image.GetCalculator();
                var rva = AddressOfRawData;
                var imageBase = _image.NTHeaders.OptionalHeader.ImageBase;
                var location = new Location(calc, PointerToRawData, rva, imageBase + rva, SizeOfData, SizeOfData);

                _data = new DebugData(_image, location, this);
            }

            return _data;
        }

        #endregion
        
        #region Properties

        public Location Location { get; }

        [FieldAnnotation("Characteristics")]
        public uint Characteristics { get; }

        [FieldAnnotation("Date/Time Stamp")]
        public uint TimeDateStamp { get; }

        [FieldAnnotation("Major Version")]
        public ushort MajorVersion { get; }

        [FieldAnnotation("Minor Version")]
        public ushort MinorVersion { get; }

        [FieldAnnotation("Type")]
        public uint Type { get; }

        [FieldAnnotation("Size of Data")]
        public uint SizeOfData { get; }

        [FieldAnnotation("Address of Raw Data")]
        public uint AddressOfRawData { get; }

        [FieldAnnotation("Pointer to Raw Data")]
        public uint PointerToRawData { get; }

        #endregion
    }
}
