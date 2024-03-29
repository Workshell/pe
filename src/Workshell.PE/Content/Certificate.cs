﻿#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE.Content
{
    public enum CertificateType : ushort
    {
        X509Certificate = 0x0001,
        PKCSSignedData = 0x0002,
        Reserved = 0x0003,
        PKCS1ModuleSign = 0x0009
    }

    public sealed class Certificate : DataContent
    {
        private Certificate(PortableExecutableImage image, DataDirectory dataDirectory, Location location, WIN_CERTIFICATE cert) : base(image, dataDirectory, location)
        {
            Length = cert.dwLength;
            Revision = cert.wRevision;
            CertificateType = cert.wCertificateType;
        }

        #region Static Methods

        public static Certificate Get(PortableExecutableImage image)
        {
            return GetAsync(image).GetAwaiter().GetResult();
        }

        public static async Task<Certificate> GetAsync(PortableExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.CertificateTable))
                return null;

            var dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.CertificateTable];

            if (DataDirectory.IsNullOrEmpty(dataDirectory))
                return null;

            var stream = image.GetStream();
            var fileOffset = dataDirectory.VirtualAddress.ToInt64();

            stream.Seek(fileOffset, SeekOrigin.Begin);

            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
            var location = new Location(image, dataDirectory.VirtualAddress, dataDirectory.VirtualAddress, imageBase + dataDirectory.VirtualAddress, dataDirectory.Size, dataDirectory.Size);
            WIN_CERTIFICATE cert;

            try
            {
                cert = await stream.ReadStructAsync<WIN_CERTIFICATE>().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new PortableExecutableImageException(image, "Could not load certificate information from stream.", ex);
            }

            return new Certificate(image, dataDirectory, location, cert);
        }

        #endregion

        #region Methods

        public CertificateType GetCertificateType()
        {
            return (CertificateType)CertificateType;
        }

        public byte[] GetCertificateData()
        {
            return GetCertificateDataAsync().GetAwaiter().GetResult();
        }

        public async Task<byte[]> GetCertificateDataAsync()
        {
            var offset = Location.FileOffset + Utils.SizeOf<WIN_CERTIFICATE>().ToUInt32();
            var stream = Image.GetStream();

            stream.Seek(offset, SeekOrigin.Begin);

            var buffer = new byte[Length];         
            var numRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            if (numRead == 0)
            {
                throw new PortableExecutableImageException(Image, "Could not read certificate data from stream.");
            }

            return buffer;
        }

        public X509Certificate GetCertificate()
        {
            return GetCertificateAsync().GetAwaiter().GetResult();
        }

        public async Task<X509Certificate> GetCertificateAsync()
        {
            if (CertificateType == 1 || CertificateType == 2) // X.509 or PKCS#7
            {
                var data = await GetCertificateDataAsync().ConfigureAwait(false);
                X509Certificate2 cert = null;

                try
                {
                    cert = new X509Certificate2(data);
                }
                catch (Exception ex)
                {
                    throw new PortableExecutableImageException(Image, "Could not load certificate from data.", ex);
                }

                return cert;
            }

            return null;
        }

        #endregion

        #region Properties

        public uint Length { get; }
        public ushort Revision { get; }
        public ushort CertificateType { get; }

        #endregion
    }
}
