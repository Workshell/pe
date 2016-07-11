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
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.CLR
{

    public sealed class CLRMetaData : ISupportsLocation, ISupportsBytes
    {

        internal CLRMetaData(CLRContent clr, Location location, CLRHeader header)
        {
            CLR = clr;
            Location = location;
            Header = new CLRMetaDataHeader(this);
            StreamTable = new CLRMetaDataStreamTable(this);
            Streams = new CLRMetaDataStreams(this);
        }

        #region Static Methods

        public static CLRMetaData Get(CLRHeader header)
        {
            LocationCalculator calc = header.CLR.DataDirectory.Directories.Image.GetCalculator();
            ulong image_base = header.CLR.DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            uint rva = header.MetaDataAddress;
            ulong va = image_base + rva;
            ulong offset = calc.RVAToOffset(rva);
            uint size = header.MetaDataSize;
            Section section = calc.RVAToSection(rva);
            Location location = new Location(offset, rva, va, size, size, section);
            CLRMetaData meta_data = new CLRMetaData(header.CLR, location, header);

            return meta_data;
        }

        #endregion

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = CLR.DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,Location);

            return buffer;
        }

        #endregion

        #region Properties

        public CLRContent CLR
        {
            get;
            private set;
        }

        public Location Location
        {
            get;
            private set;
        }

        public CLRMetaDataHeader Header
        {
            get;
            private set;
        }

        public CLRMetaDataStreamTable StreamTable
        {
            get;
            private set;
        }

        public CLRMetaDataStreams Streams
        {
            get;
            private set;
        }

        #endregion

    }

}
