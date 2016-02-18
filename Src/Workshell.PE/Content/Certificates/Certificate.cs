using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

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

    public sealed class Certificate : ISupportsLocation, ISupportsBytes
    {

        private CertificateTableContent content;
        private Location location;
        private WIN_CERTIFICATE cert;

        internal Certificate(CertificateTableContent certContent, Location certLocation, WIN_CERTIFICATE winCert)
        {
            content = certContent;
            location = certLocation;
            cert = winCert;
        }

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = content.DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        public CertificateType GetCertificateType()
        {
            return (CertificateType)cert.wCertificateType;
        }

        public byte[] GetCertificateData()
        {
            Stream stream = content.DataDirectory.Directories.Reader.GetStream();
            ulong offset = location.FileOffset + Convert.ToUInt32(Utils.SizeOf<WIN_CERTIFICATE>());
            byte[] buffer = new byte[cert.dwLength];

            stream.Seek(Convert.ToInt64(offset),SeekOrigin.Begin);
            stream.Read(buffer,0,buffer.Length);

            return buffer;
        }

        #endregion

        #region Properties

        public CertificateTableContent Content
        {
            get
            {
                return content;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

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
