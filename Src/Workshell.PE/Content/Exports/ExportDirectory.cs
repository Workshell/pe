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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE.Exports
{

    public class ExportDirectory : ExecutableImageContent, ISupportsBytes
    {

        private static readonly int size = Utils.SizeOf<IMAGE_EXPORT_DIRECTORY>();

        private IMAGE_EXPORT_DIRECTORY directory;
        private Lazy<string> name;
        private Lazy<uint[]> function_addresses;
        private Lazy<uint[]> function_name_addresses;
        private Lazy<ushort[]> function_ordinals;

        internal ExportDirectory(DataDirectory dataDirectory, Location dirLocation, IMAGE_EXPORT_DIRECTORY exportDirectory) : base(dataDirectory,dirLocation)
        {
            directory = exportDirectory;
            name = new Lazy<string>(DoGetName);
            function_addresses = new Lazy<uint[]>(DoGetFunctionAddresses);
            function_name_addresses = new Lazy<uint[]>(DoGetFunctionNameAddresses);
            function_ordinals = new Lazy<ushort[]>(DoGetFunctionOrdinals);
        }

        #region Static Methods

        public static ExportDirectory Get(ExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.ExportTable))
                return null;

            DataDirectory directory = image.NTHeaders.DataDirectories[DataDirectoryType.ExportTable];

            if (DataDirectory.IsNullOrEmpty(directory))
                return null;

            LocationCalculator calc = directory.Directories.Image.GetCalculator();
            uint rva = directory.VirtualAddress;
            ulong image_base = directory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            ulong va = image_base + rva;
            Section section = calc.RVAToSection(rva);
            ulong offset = calc.RVAToOffset(section, rva);
            uint size = Utils.SizeOf<IMAGE_EXPORT_DIRECTORY>().ToUInt32();
            Location location = new Location(offset, rva, va, size, size, section);
            Stream stream = directory.Directories.Image.GetStream();

            stream.Seek(offset.ToInt64(), SeekOrigin.Begin);

            IMAGE_EXPORT_DIRECTORY export_directory = Utils.Read<IMAGE_EXPORT_DIRECTORY>(stream);
            ExportDirectory result = new ExportDirectory(directory, location, export_directory);

            return result;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return "Export Directory";
        }

        public byte[] GetBytes()
        {
            byte[] buffer = new byte[size];

            Utils.Write<IMAGE_EXPORT_DIRECTORY>(directory,buffer,0,buffer.Length);

            return buffer;
        }

        public DateTime GetTimeDateStamp()
        {
            return Utils.ConvertTimeDateStamp(directory.TimeDateStamp);
        }

        public string GetName()
        {
            return name.Value;
        }

        public uint[] GetFunctionAddresses()
        {
            return function_addresses.Value;
        }

        public uint[] GetFunctionNameAddresses()
        {
            return function_name_addresses.Value;
        }

        public ushort[] GetFunctionOrdinals()
        {
            return function_ordinals.Value;
        }

        private string DoGetName()
        {
            LocationCalculator calc = DataDirectory.Directories.Image.GetCalculator();
            StringBuilder builder = new StringBuilder(256);
            long offset = calc.RVAToOffset(directory.Name).ToInt64();
            Stream stream = DataDirectory.Directories.Image.GetStream();

            stream.Seek(offset, SeekOrigin.Begin);

            while (true)
            {
                int value = stream.ReadByte();

                if (value <= 0)
                    break;

                char c = (char)value;

                builder.Append(c);
            }

            return builder.ToString();
        }

        private uint[] DoGetFunctionAddresses()
        {
            LocationCalculator calc = DataDirectory.Directories.Image.GetCalculator();
            long offset = calc.RVAToOffset(directory.AddressOfFunctions).ToInt64();
            Stream stream = DataDirectory.Directories.Image.GetStream();

            stream.Seek(offset, SeekOrigin.Begin);

            uint[] results = new uint[directory.NumberOfFunctions];

            for (int i = 0; i < directory.NumberOfFunctions; i++)
            {
                uint address = Utils.ReadUInt32(stream);

                results[i] = address;
            }

            return results;
        }

        private uint[] DoGetFunctionNameAddresses()
        {
            LocationCalculator calc = DataDirectory.Directories.Image.GetCalculator();
            long offset = calc.RVAToOffset(directory.AddressOfNames).ToInt64();
            Stream stream = DataDirectory.Directories.Image.GetStream();

            stream.Seek(offset, SeekOrigin.Begin);

            uint[] results = new uint[directory.NumberOfNames];

            for (int i = 0; i < directory.NumberOfNames; i++)
            {
                uint address = Utils.ReadUInt32(stream);

                results[i] = address;
            }

            return results;
        }

        private ushort[] DoGetFunctionOrdinals()
        {
            LocationCalculator calc = DataDirectory.Directories.Image.GetCalculator();
            long offset = calc.RVAToOffset(directory.AddressOfNameOrdinals).ToInt64();
            Stream stream = DataDirectory.Directories.Image.GetStream();

            stream.Seek(offset, SeekOrigin.Begin);

            ushort[] results = new ushort[directory.NumberOfNames];

            for (int i = 0; i < directory.NumberOfNames; i++)
            {
                ushort ord = Utils.ReadUInt16(stream);

                results[i] = ord;
            }

            return results;
        }

        #endregion

        #region Properties

        [FieldAnnotation("Characteristics")]
        public uint Characteristics
        {
            get
            {
                return directory.Characteristics;
            }
        }

        [FieldAnnotation("Date/Time Stamp")]
        public uint TimeDateStamp
        {
            get
            {
                return directory.TimeDateStamp;
            }
        }

        [FieldAnnotation("Major Version")]
        public ushort MajorVersion
        {
            get
            {
                return directory.MajorVersion;
            }
        }

        [FieldAnnotation("Minor Version")]
        public ushort MinorVersion
        {
            get
            {
                return directory.MinorVersion;
            }
        }

        [FieldAnnotation("Name")]
        public uint Name
        {
            get
            {
                return directory.Name;
            }
        }

        [FieldAnnotation("Base")]
        public uint Base
        {
            get
            {
                return directory.Base;
            }
        }

        [FieldAnnotation("Number of Functions")]
        public uint NumberOfFunctions
        {
            get
            {
                return directory.NumberOfFunctions;
            }
        }

        [FieldAnnotation("Number of Names")]
        public uint NumberOfNames
        {
            get
            {
                return directory.NumberOfNames;
            }
        }

        [FieldAnnotation("Address of Functions")]
        public uint AddressOfFunctions
        {
            get
            {
                return directory.AddressOfFunctions;
            }
        }

        [FieldAnnotation("Address of Names")]
        public uint AddressOfNames
        {
            get
            {
                return directory.AddressOfNames;
            }
        }

        [FieldAnnotation("Address of Name Ordinals")]
        public uint AddressOfNameOrdinals
        {
            get
            {
                return directory.AddressOfNameOrdinals;
            }
        }

        #endregion

    }

}
