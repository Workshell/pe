using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Workshell.PE.Annotations;
using Workshell.PE.Extensions;
using Workshell.PE.Resources.Native;

namespace Workshell.PE.Resources.Version
{
    using FileVersion = System.Version;

    [Flags]
    public enum FileFlags : int
    {
        None = 0,
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

    public enum FontFileSubType : int
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
        private readonly uint _subType;

        internal FixedFileInfo(VS_FIXEDFILEINFO info)
        {
            FileVersion = GetFileVersion(info);
            ProductVersion = GetProductVersion(info);
            FileDate = GetFileDate(info);
            FileOS = (FileOS)info.dwFileOS;
            FileType = (FileType)info.dwFileType;
            DriverSubType = GetDriverFileSubType(info);
            FontSubType = GetFontFileSubType(info);
            Flags = GetFileFlags(info);
        }

        #region Static Methods

        private static FileVersion GetFileVersion(VS_FIXEDFILEINFO info)
        {
            var major = Utils.HiWord(info.dwFileVersionMS);
            var minor = Utils.LoWord(info.dwFileVersionMS);
            var build = Utils.HiWord(info.dwFileVersionLS);
            var priv = Utils.LoWord(info.dwFileVersionLS);

            return new FileVersion(major, minor, build, priv);
        }

        private static FileVersion GetProductVersion(VS_FIXEDFILEINFO info)
        {
            var major = Utils.HiWord(info.dwProductVersionMS);
            var minor = Utils.LoWord(info.dwProductVersionMS);
            var build = Utils.HiWord(info.dwProductVersionLS);
            var priv = Utils.LoWord(info.dwProductVersionLS);

            return new FileVersion(major, minor, build, priv);
        }

        private static DateTime GetFileDate(VS_FIXEDFILEINFO info)
        {
            var value = Utils.MakeUInt64(info.dwFileDateMS, info.dwFileDateLS);
            var result = DateTime.FromFileTime(value.ToInt64());

            return result;
        }

        private static DriverFileSubType GetDriverFileSubType(VS_FIXEDFILEINFO info)
        {
            var fileType = (FileType)info.dwFileType;

            if (fileType != FileType.Driver)
                return DriverFileSubType.Unknown;

            return (DriverFileSubType)info.dwFileSubtype;
        }

        private static FontFileSubType GetFontFileSubType(VS_FIXEDFILEINFO info)
        {
            var fileType = (FileType)info.dwFileType;

            if (fileType != FileType.Font)
                return FontFileSubType.Unknown;

            return (FontFileSubType)info.dwFileSubtype;
        }

        private static FileFlags GetFileFlags(VS_FIXEDFILEINFO info)
        {
            var flags = FileFlags.None;

            if ((info.dwFileFlagsMask & info.dwFileFlags & (uint)FileFlags.Debug) == (uint)FileFlags.Debug)
                flags &= FileFlags.Debug;

            if ((info.dwFileFlagsMask & info.dwFileFlags & (uint)FileFlags.InfoInferred) == (uint)FileFlags.InfoInferred)
                flags &= FileFlags.InfoInferred;

            if ((info.dwFileFlagsMask & info.dwFileFlags & (uint)FileFlags.Patched) == (uint)FileFlags.Patched)
                flags &= FileFlags.Patched;

            if ((info.dwFileFlagsMask & info.dwFileFlags & (uint)FileFlags.PreRelease) == (uint)FileFlags.PreRelease)
                flags &= FileFlags.PreRelease;

            if ((info.dwFileFlagsMask & info.dwFileFlags & (uint)FileFlags.PrivateBuild) == (uint)FileFlags.PrivateBuild)
                flags &= FileFlags.PrivateBuild;

            if ((info.dwFileFlagsMask & info.dwFileFlags & (uint)FileFlags.SpecialBuild) == (uint)FileFlags.SpecialBuild)
                flags &= FileFlags.SpecialBuild;

            return flags;
        }

        #endregion

        #region Properties

        public FileVersion FileVersion { get; }
        public FileVersion ProductVersion { get; }
        public DateTime FileDate { get; }
        public FileOS FileOS { get; }
        public FileType FileType { get; }
        public DriverFileSubType DriverSubType { get; }
        public FontFileSubType FontSubType { get; }
        public FileFlags Flags { get; }

        #endregion
    }
}
