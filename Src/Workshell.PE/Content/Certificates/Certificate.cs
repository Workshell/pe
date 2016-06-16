using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public enum CertificateType : ushort
    {
        X509Certificate = 0x0001,
        PKCSSignedData = 0x0002,
        Reserved = 0x0003,
        PKCS1ModuleSign = 0x0009
    }

    public sealed class Certificate : DataDirectoryContent, ISupportsBytes
    {

        private WIN_CERTIFICATE cert;

        internal Certificate(DataDirectory dataDirectory, Location certLocation, WIN_CERTIFICATE winCert) : base(dataDirectory,certLocation)
        {
            cert = winCert;
        }

        #region Static Methods

        public static Certificate Get(DataDirectory directory)
        {
            if (directory == null)
                throw new ArgumentNullException("directory", "No data directory was specified.");

            if (directory.DirectoryType != DataDirectoryType.CertificateTable)
                throw new DataDirectoryException("Cannot create instance, directory is not the Certificate Table.");

            if (directory.VirtualAddress == 0 && directory.Size == 0)
                throw new DataDirectoryException("Certificate Table address and size are 0.");

            Stream stream = directory.Directories.Reader.GetStream();
            long file_offset = directory.VirtualAddress.ToInt64();

            if (file_offset > stream.Length)
                throw new DataDirectoryException("Certificate Table offset is beyond end of stream.");

            ulong image_base = directory.Directories.Reader.NTHeaders.OptionalHeader.ImageBase;
            Location location = new Location(directory.VirtualAddress, directory.VirtualAddress, image_base + directory.VirtualAddress, directory.Size, directory.Size);

            stream.Seek(file_offset, SeekOrigin.Begin);

            WIN_CERTIFICATE win_cert = Utils.Read<WIN_CERTIFICATE>(stream);
            Certificate cert = new Certificate(directory, location, win_cert);

            return cert;
        }

        #endregion

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,Location);

            return buffer;
        }

        public CertificateType GetCertificateType()
        {
            return (CertificateType)cert.wCertificateType;
        }

        public byte[] GetData()
        {
            Stream stream = DataDirectory.Directories.Reader.GetStream();
            ulong offset = Location.FileOffset + Utils.SizeOf<WIN_CERTIFICATE>().ToUInt32();
            byte[] buffer = Utils.ReadBytes(stream, offset.ToInt64(), cert.dwLength);

            return buffer;
        }

        #endregion

        #region Properties

        public uint Length
        {
            get
            {
                return cert.dwLength;
            }
        }

        public ushort Revision
        {
            get
            {
                return cert.wRevision;
            }
        }

        public ushort CertificateType
        {
            get
            {
                return cert.wCertificateType;
            }
        }

        #endregion

    }

}
