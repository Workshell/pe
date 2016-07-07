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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public enum MachineType : int
    {
        Native = 0,
        [EnumAnnotation("IMAGE_FILE_MACHINE_I386")]
        x86 = 0x014c,
        [EnumAnnotation("IMAGE_FILE_MACHINE_IA64")]
        Itanium = 0x0200,
        [EnumAnnotation("IMAGE_FILE_MACHINE_AMD64")]
        x64 = 0x8664
    }

    [Flags]
    public enum CharacteristicsType : int
    {
        [EnumAnnotation("IMAGE_FILE_RELOCS_STRIPPED")]
        RelocationInformationStrippedFromFile = 0x1,
        [EnumAnnotation("IMAGE_FILE_EXECUTABLE_IMAGE")]
        Executable = 0x2,
        [EnumAnnotation("IMAGE_FILE_LINE_NUMS_STRIPPED")]
        LineNumbersStripped = 0x4,
        [EnumAnnotation("IMAGE_FILE_LOCAL_SYMS_STRIPPED")]
        SymbolTableStripped = 0x8,
        [EnumAnnotation("IMAGE_FILE_AGGRESIVE_WS_TRIM")]
        AggresiveTrimWorkingSet = 0x10,
        [EnumAnnotation("IMAGE_FILE_LARGE_ADDRESS_AWARE")]
        LargeAddressAware = 0x20,
        [EnumAnnotation("IMAGE_FILE_BYTES_REVERSED_LO")]
        Supports16Bit = 0x40,
        [EnumAnnotation("IMAGE_FILE_16BIT_MACHINE")]
        ReservedBytesWo = 0x80,
        [EnumAnnotation("IMAGE_FILE_32BIT_MACHINE")]
        Supports32Bit = 0x100,
        [EnumAnnotation("IMAGE_FILE_DEBUG_STRIPPED")]
        DebugInfoStripped = 0x200,
        [EnumAnnotation("IMAGE_FILE_REMOVABLE_RUN_FROM_SWAP")]
        RunFromSwapIfInRemovableMedia = 0x400,
        [EnumAnnotation("IMAGE_FILE_NET_RUN_FROM_SWAP")]
        RunFromSwapIfInNetworkMedia = 0x800,
        [EnumAnnotation("IMAGE_FILE_SYSTEM")]
        IsSytemFile = 0x1000,
        [EnumAnnotation("IMAGE_FILE_DLL")]
        IsDLL = 0x2000,
        [EnumAnnotation("IMAGE_FILE_UP_SYSTEM_ONLY")]
        IsOnlyForSingleCoreProcessor = 0x4000,
        [EnumAnnotation("IMAGE_FILE_BYTES_REVERSED_HI")]
        BytesOfWordReserved = 0x8000
    }

    public sealed class FileHeader : ISupportsLocation, ISupportsBytes
    {

        internal static readonly int Size = Utils.SizeOf<IMAGE_FILE_HEADER>();

        private ExecutableImage image;
        private IMAGE_FILE_HEADER header;
        private Location location;

        internal FileHeader(ExecutableImage exeImage, IMAGE_FILE_HEADER fileHeader, ulong headerOffset, ulong imageBase)
        {
            image = exeImage;
            header = fileHeader;

            uint size = Convert.ToUInt32(Utils.SizeOf<IMAGE_FILE_HEADER>());

            location = new Location(headerOffset,Convert.ToUInt32(headerOffset),imageBase + headerOffset,size,size);
        }

        #region Methods

        public override string ToString()
        {
            return "File Header";
        }

        public byte[] GetBytes()
        {
            Stream stream = image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        public MachineType GetMachineType()
        {
            return (MachineType)header.Machine;
        }

        public DateTime GetTimeDateStamp()
        {
            return Utils.ConvertTimeDateStamp(header.TimeDateStamp);
        }

        public CharacteristicsType GetCharacteristics()
        {
            return (CharacteristicsType)header.Characteristics;
        }

        #endregion

        #region Properties

        public Location Location
        {
            get
            {
                return location;
            }
        }

        [FieldAnnotation("Machine Type",Flags = true,FlagType = typeof(MachineType))]
        public ushort Machine
        {
            get
            {
                return header.Machine;
            }
        }

        [FieldAnnotation("Number of Sections")]
        public ushort NumberOfSections
        {
            get
            {
                return header.NumberOfSections;
            }
        }

        [FieldAnnotation("Date/Time Stamp")]
        public uint TimeDateStamp
        {
            get
            {
                return header.TimeDateStamp;
            }
        }

        [FieldAnnotation("Pointer to Symbol Table")]
        public uint PointerToSymbolTable
        {
            get
            {
                return header.PointerToSymbolTable;
            }
        }

        [FieldAnnotation("Number of Symbols")]
        public uint NumberOfSymbols
        {
            get
            {
                return header.NumberOfSymbols;
            }
        }

        [FieldAnnotation("Size of Optional Header")]
        public ushort SizeOfOptionalHeader
        {
            get
            {
                return header.SizeOfOptionalHeader;
            }
        }

        [FieldAnnotation("Characteristics",Flags = true,FlagType = typeof(CharacteristicsType))]
        public ushort Characteristics
        {
            get
            {
                return header.Characteristics;
            }
        }

        #endregion

    }

}
