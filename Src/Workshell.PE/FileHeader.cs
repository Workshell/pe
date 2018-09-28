#region License
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
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Annotations;
using Workshell.PE.Extensions;
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
        private readonly PortableExecutableImage _image;
        private readonly IMAGE_FILE_HEADER _header;

        internal FileHeader(PortableExecutableImage image, IMAGE_FILE_HEADER fileHeader, ulong headerOffset, ulong imageBase)
        {
            _image = image;
            _header = fileHeader;

            Location = new Location(image.GetCalculator(), headerOffset, headerOffset.ToUInt32(), imageBase + headerOffset, Size.ToUInt32(), Size.ToUInt32());
        }

        #region Methods

        public override string ToString()
        {
            return "File Header";
        }

        public byte[] GetBytes()
        {
            return GetBytesAsync().GetAwaiter().GetResult();
        }

        public async Task<byte[]> GetBytesAsync()
        {
            var stream = _image.GetStream();
            var buffer = await stream.ReadBytesAsync(Location).ConfigureAwait(false);

            return buffer;
        }

        public MachineType GetMachineType()
        {
            return (MachineType)_header.Machine;
        }

        public DateTime GetTimeDateStamp()
        {
            return Utils.ConvertTimeDateStamp(_header.TimeDateStamp);
        }

        public CharacteristicsType GetCharacteristics()
        {
            return (CharacteristicsType)_header.Characteristics;
        }

        #endregion


        #region Static Properties

        public static int Size { get; } = Marshal.SizeOf<IMAGE_FILE_HEADER>();

        #endregion

        #region Properties

        public Location Location { get; }

        [FieldAnnotation("Machine Type",Flags = true,FlagType = typeof(MachineType))]
        public ushort Machine => _header.Machine;

        [FieldAnnotation("Number of Sections")]
        public ushort NumberOfSections => _header.NumberOfSections;

        [FieldAnnotation("Date/Time Stamp")]
        public uint TimeDateStamp => _header.TimeDateStamp;

        [FieldAnnotation("Pointer to Symbol Table")]
        public uint PointerToSymbolTable => _header.PointerToSymbolTable;

        [FieldAnnotation("Number of Symbols")]
        public uint NumberOfSymbols => _header.NumberOfSymbols;

        [FieldAnnotation("Size of Optional Header")]
        public ushort SizeOfOptionalHeader => _header.SizeOfOptionalHeader;

        [FieldAnnotation("Characteristics",Flags = true,FlagType = typeof(CharacteristicsType))]
        public ushort Characteristics => _header.Characteristics;

        #endregion
    }
}
