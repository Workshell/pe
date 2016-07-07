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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class DebugData : ISupportsLocation, ISupportsBytes
    {

        private DebugDirectoryEntry entry;
        private Location location;

        internal DebugData(DebugDirectoryEntry directorEntry, Location dataLocation)
        {
            entry = directorEntry;
            location = dataLocation;
        }

        #region Static Methods

        public static DebugData Get(DebugDirectoryEntry entry)
        {
            if (entry.PointerToRawData == 0 && entry.SizeOfData == 0)
                return null;

            LocationCalculator calc = entry.Directory.DataDirectory.Directories.Image.GetCalculator();
            uint rva = entry.AddressOfRawData;
            Section section = calc.RVAToSection(rva);
            ulong image_base = entry.Directory.DataDirectory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            Location location = new Location(entry.PointerToRawData, rva, image_base + rva, entry.SizeOfData, entry.SizeOfData, section);
            DebugData data = new DebugData(entry, location);

            return data;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            string type = entry.GetEntryType().ToString();

            return String.Format("Debug Type: {0}, File Offset: 0x{1:X8}, Size: 0x{2:X8}", type, location.FileOffset, location.FileSize);
        }

        public byte[] GetBytes()
        {
            Stream stream = entry.Directory.DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream, location);

            return buffer;
        }

        #endregion

        #region Properties

        public DebugDirectoryEntry Entry
        {
            get
            {
                return entry;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        #endregion

    }

}
