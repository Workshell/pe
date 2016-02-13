using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;

namespace Workshell.PE
{

    public sealed class CLRMetaDataHeader : ISupportsLocation, ISupportsBytes
    {

        private const uint CLR_METADATA_SIGNATURE = 0x424A5342;

        internal CLRMetaDataHeader(CLRMetaData metaData, ulong imageBase)
        {
            LocationCalculator calc = metaData.Content.DataDirectory.Directories.Reader.GetCalculator();
            Stream stream = metaData.Content.DataDirectory.Directories.Reader.GetStream();

            uint rva = metaData.Content.Header.MetaDataAddress;
            ulong va = imageBase + rva;
            uint size = 0;
            ulong offset = calc.RVAToOffset(rva);

            stream.Seek(Convert.ToInt64(offset),SeekOrigin.Begin);

            MetaData = metaData;
            Signature = Utils.ReadUInt32(stream);

            if (Signature != CLR_METADATA_SIGNATURE)
                throw new ImageReaderException("Incorrect signature found in CLR meta-data header.");

            MajorVersion = Utils.ReadUInt16(stream);
            MinorVersion = Utils.ReadUInt16(stream);
            Reserved = Utils.ReadUInt32(stream);
            VersionLength = Utils.ReadUInt32(stream);
            Version = Utils.ReadString(stream,VersionLength);
            Flags = Utils.ReadUInt16(stream);
            Streams = Utils.ReadUInt16(stream);

            size = sizeof(uint) + sizeof(ushort) + sizeof(ushort) + sizeof(uint) + sizeof(uint) + VersionLength + sizeof(ushort) + sizeof(ushort);

            Location = new Location(offset,rva,va,size,size);
        }

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = MetaData.Content.DataDirectory.Directories.Reader.GetStream();
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
