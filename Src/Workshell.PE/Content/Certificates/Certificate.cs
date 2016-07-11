#region License
//  Copyright(c) 2016, Workshell Ltd
//  All rights reserved.
//  
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Workshell Ltd nor the names of its contributors
//  may be used to endorse or promote products
//  derived from this software without specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED.IN NO EVENT SHALL WORKSHELL BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE.Certificates
{

    public enum CertificateType : ushort
    {
        X509Certificate = 0x0001,
        PKCSSignedData = 0x0002,
        Reserved = 0x0003,
        PKCS1ModuleSign = 0x0009
    }

    public sealed class Certificate : ExecutableImageContent, ISupportsBytes
    {

        private WIN_CERTIFICATE cert;

        internal Certificate(DataDirectory dataDirectory, Location certLocation, WIN_CERTIFICATE winCert) : base(dataDirectory,certLocation)
        {
            cert = winCert;
        }

        #region Static Methods

        public static Certificate Get(ExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.CertificateTable))
                return null;

            DataDirectory directory = image.NTHeaders.DataDirectories[DataDirectoryType.CertificateTable];

            if (DataDirectory.IsNullOrEmpty(directory))
                return null;

            Stream stream = directory.Directories.Image.GetStream();
            long file_offset = directory.VirtualAddress.ToInt64();

            stream.Seek(file_offset, SeekOrigin.Begin);

            ulong image_base = directory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            Location location = new Location(directory.VirtualAddress, directory.VirtualAddress, image_base + directory.VirtualAddress, directory.Size, directory.Size);
            WIN_CERTIFICATE win_cert = Utils.Read<WIN_CERTIFICATE>(stream);
            Certificate cert = new Certificate(directory, location, win_cert);

            return cert;
        }

        #endregion

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,Location);

            return buffer;
        }

        public CertificateType GetCertificateType()
        {
            return (CertificateType)cert.wCertificateType;
        }

        public byte[] GetData()
        {
            Stream stream = DataDirectory.Directories.Image.GetStream();
            ulong offset = Location.FileOffset + Utils.SizeOf<WIN_CERTIFICATE>().ToUInt32();
            byte[] buffer = Utils.ReadBytes(stream, offset.ToInt64(), cert.dwLength);

            return buffer;
        }

        public X509Certificate GetCertificate()
        {
            if (cert.wCertificateType == 1 || cert.wCertificateType == 2) // X.509 or PKCS#7
            {
                byte[] data = GetData();
                X509Certificate2 certificate = new X509Certificate2(data);

                return certificate;
            }
            else
            {
                return null;
            }
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
