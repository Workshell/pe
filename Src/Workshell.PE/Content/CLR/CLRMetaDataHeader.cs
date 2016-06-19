using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Extensions;

namespace Workshell.PE
{

    public sealed class CLRMetaDataHeader : ISupportsLocation, ISupportsBytes
    {

        private const uint CLR_METADATA_SIGNATURE = 0x424A5342;

        internal CLRMetaDataHeader(CLRMetaData metaData)
        {
            LocationCalculator calc = metaData.CLR.DataDirectory.Directories.Image.GetCalculator();
            Stream stream = metaData.CLR.DataDirectory.Directories.Image.GetStream();
            ulong image_base = metaData.CLR.DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            uint rva = metaData.Location.RelativeVirtualAddress;
            ulong va = metaData.Location.VirtualAddress;  
            ulong offset = metaData.Location.FileOffset;
            uint size = 0;
            Section section = metaData.Location.Section;

            stream.Seek(offset.ToInt64(), SeekOrigin.Begin);

            MetaData = metaData;
            Signature = Utils.ReadUInt32(stream);

            if (Signature != CLR_METADATA_SIGNATURE)
                throw new ExecutableImageException("Incorrect signature found in CLR meta-data header.");

            MajorVersion = Utils.ReadUInt16(stream);
            MinorVersion = Utils.ReadUInt16(stream);
            Reserved = Utils.ReadUInt32(stream);
            VersionLength = Utils.ReadUInt32(stream);
            Version = Utils.ReadString(stream, VersionLength);
            Flags = Utils.ReadUInt16(stream);
            Streams = Utils.ReadUInt16(stream);

            size = sizeof(uint) + sizeof(ushort) + sizeof(ushort) + sizeof(uint) + sizeof(uint) + VersionLength + sizeof(ushort) + sizeof(ushort);

            Location = new Location(offset, rva, va, size, size, section);
        }

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = MetaData.CLR.DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,Location);

            return buffer;
        }

        #endregion

        #region Properties

        public CLRMetaData MetaData
        {
            get;
            private set;
        }

        public Location Location
        {
            get;
            private set;
        }

        [FieldAnnotation("Signature")]
        public uint Signature
        {
            get;
            private set;
        }

        [FieldAnnotation("Major Version")]
        public ushort MajorVersion
        {
            get;
            private set;
        }

        [FieldAnnotation("Minor Version")]
        public ushort MinorVersion
        {
            get;
            private set;
        }

        [FieldAnnotation("Reserved")]
        public uint Reserved
        {
            get;
            private set;
        }

        [FieldAnnotation("Version String Length")]
        public uint VersionLength
        {
            get;
            private set;
        }

        [FieldAnnotation("Version String")]
        public string Version
        {
            get;
            private set;
        }

        [FieldAnnotation("Flags")]
        public ushort Flags
        {
            get;
            private set;
        }

        [FieldAnnotation("Number of Streams")]
        public ushort Streams
        {
            get;
            private set;
        }

        #endregion

    }

}
