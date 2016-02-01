using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Attributes;

namespace Workshell.PE
{

    public class CLRMetaDataHeader : ILocationSupport, IRawDataSupport
    {

        private const uint CLR_METADATA_SIGNATURE = 0x424A5342;

        internal CLRMetaDataHeader(CLRMetaData metaData)
        {
            long offset = metaData.Location.Offset;
            Stream stream = metaData.Content.Section.Sections.Reader.GetStream();

            stream.Seek(offset,SeekOrigin.Begin);

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

            long size = stream.Position - offset;

            Location = new StreamLocation(offset,size);
        }

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = MetaData.Content.Section.Sections.Reader.GetStream();

            return Utils.ReadBytes(stream,Location);
        }

        #endregion

        #region Properties

        public CLRMetaData MetaData
        {
            get;
            private set;
        }

        public StreamLocation Location
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
