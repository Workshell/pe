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

namespace Workshell.PE.Native
{

    [Flags]
    public enum FileFlags : int
    {
        [EnumAnnotation("VS_FF_DEBUG")]
        Debug = 0x00000001,
        [EnumAnnotation("VS_FF_INFOINFERRED")]
        InfoInferred = 0x00000010,
        [EnumAnnotation("VS_FF_PATCHED")]
        Patched = 0x00000004,
        [EnumAnnotation("VS_FF_PRERELEASE")]
        PreRelease = 0x00000002,
        [EnumAnnotation("VS_FF_PRIVATEBUILD")]
        PrivateBuild = 0x00000008,
        [EnumAnnotation("VS_FF_SPECIALBUILD")]
        SpecialBuild = 0x00000020
    }

    [Flags]
    public enum FileOS : int
    {
        [EnumAnnotation("VOS_DOS")]
        DOS = 0x00010000,
        [EnumAnnotation("VOS_NT")]
        NT = 0x00040000,
        [EnumAnnotation("VOS__WINDOWS16")]
        Win16 = 0x00000001,
        [EnumAnnotation("VOS__WINDOWS32")]
        Win32 = 0x00000004,
        [EnumAnnotation("VOS_OS216")]
        OS2_16 = 0x00020000,
        [EnumAnnotation("VOS_OS232")]
        OS2_32 = 0x00030000,
        [EnumAnnotation("VOS__PM16")]
        PM16 = 0x00000002,
        [EnumAnnotation("VOS__PM32")]
        PM32 = 0x00000003,
        [EnumAnnotation("VOS_UNKNOWN")]
        Unknown = 0x00000000,

        [EnumAnnotation("VOS_DOS_WINDOWS16")]
        DOS_Win16 = 0x00010001,
        [EnumAnnotation("VOS_DOS_WINDOWS32")]
        DOS_Win32 = 0x00010004,
        [EnumAnnotation("VOS_NT_WINDOWS32")]
        NT_Win32 = 0x00040004,
        [EnumAnnotation("VOS_OS216_PM16")]
        OS2_PM16 = 0x00020002,
        [EnumAnnotation("VOS_OS232_PM32")]
        OS2_PM32 = 0x00030003
    }

    public enum FileType : int
    {
        [EnumAnnotation("VFT_APP")]
        Application = 0x00000001,
        [EnumAnnotation("VFT_DLL")]
        Library = 0x00000002,
        [EnumAnnotation("VFT_DRV")]
        Driver = 0x00000003,
        [EnumAnnotation("VFT_FONT")]
        Font = 0x00000004,
        [EnumAnnotation("VFT_STATIC_LIB")]
        StaticLibrary = 0x00000007,
        [EnumAnnotation("VFT_UNKNOWN")]
        Unknown = 0x00000000,
        [EnumAnnotation("VFT_VXD")]
        VxD = 0x00000005
    }

    public enum DriverFileSubType : int
    {
        [EnumAnnotation("VFT2_DRV_COMM")]
        Communications = 0x0000000A,
        [EnumAnnotation("VFT2_DRV_DISPLAY")]
        Display = 0x00000004,
        [EnumAnnotation("VFT2_DRV_INSTALLABLE")]
        Installable = 0x00000008,
        [EnumAnnotation("VFT2_DRV_KEYBOARD")]
        Keyboard = 0x00000002,
        [EnumAnnotation("VFT2_DRV_LANGUAGE")]
        Language = 0x00000003,
        [EnumAnnotation("VFT2_DRV_MOUSE")]
        Mouse = 0x00000005,
        [EnumAnnotation("VFT2_DRV_NETWORK")]
        Network = 0x00000006,
        [EnumAnnotation("VFT2_DRV_PRINTER")]
        Printer = 0x00000001,
        [EnumAnnotation("VFT2_DRV_SOUND")]
        Sound = 0x00000009,
        [EnumAnnotation("VFT2_DRV_SYSTEM")]
        System = 0x00000007,
        [EnumAnnotation("VFT2_DRV_VERSIONED_PRINTER")]
        VersionedPrinter = 0x0000000C,
        [EnumAnnotation("VFT2_UNKNOWN")]
        Unknown = 0x00000000
    }

    public enum FontSubType : int
    {
        [EnumAnnotation("VFT2_FONT_RASTER")]
        Raster = 0x00000001,
        [EnumAnnotation("VFT2_FONT_TRUETYPE")]
        TrueType = 0x00000003,
        [EnumAnnotation("VFT2_FONT_VECTOR")]
        Vector = 0x00000002,
        [EnumAnnotation("VFT2_UNKNOWN")]
        Unknown = 0x00000000
    }

    public sealed class FixedFileInfo
    {

        private VS_FIXEDFILEINFO fixed_file_info;
        private Lazy<Version> file_version;
        private Lazy<Version> product_version;
        private Lazy<DateTime> file_date;

        internal FixedFileInfo(Stream stream)
        {
            fixed_file_info = Utils.Read<VS_FIXEDFILEINFO>(stream);
            file_version = new Lazy<Version>(() => GetFileVersion());
            product_version = new Lazy<Version>(() => GetProductVersion());
            file_date = new Lazy<DateTime>(() => GetFileDate());
        }

        #region Methods

        private Version GetFileVersion()
        {
            ushort major = Utils.HiWord(fixed_file_info.dwFileVersionMS);
            ushort minor = Utils.LoWord(fixed_file_info.dwFileVersionMS);
            ushort build = Utils.HiWord(fixed_file_info.dwFileVersionLS);
            ushort priv = Utils.LoWord(fixed_file_info.dwFileVersionLS);

            return new Version(major, minor, build, priv);
        }

        private Version GetProductVersion()
        {
            ushort major = Utils.HiWord(fixed_file_info.dwProductVersionMS);
            ushort minor = Utils.LoWord(fixed_file_info.dwProductVersionMS);
            ushort build = Utils.HiWord(fixed_file_info.dwProductVersionLS);
            ushort priv = Utils.LoWord(fixed_file_info.dwProductVersionLS);

            return new Version(major, minor, build, priv);
        }

        private DateTime GetFileDate()
        {
            ulong value = Utils.MakeUInt64(fixed_file_info.dwFileDateMS, fixed_file_info.dwFileDateLS);
            DateTime result = DateTime.FromFileTime(Convert.ToInt64(value));

            return result;
        }

        #endregion

        #region Properties

        public Version FileVersion
        {
            get
            {
                return file_version.Value;
            }
        }

        public Version ProductVersion
        {
            get
            {
                return product_version.Value;
            }
        }

        public DateTime FileDate
        {
            get
            {
                return file_date.Value;
            }
        }

        public FileOS FileOS
        {
            get
            {
                return (FileOS)fixed_file_info.dwFileOS;
            }
        }

        public FileType FileType
        {
            get
            {
                return (FileType)fixed_file_info.dwFileType;
            }
        }

        #endregion

    }

}
