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
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE.Content
{
    [Flags]
    public enum GuardFlagsType : uint
    {
        [EnumAnnotation("IMAGE_GUARD_CF_INSTRUMENTED")]
        Instrumented = 0x00000100,
        [EnumAnnotation("IMAGE_GUARD_CFW_INSTRUMENTED")]
        WriteInstrumented = 0x00000200,
        [EnumAnnotation("IMAGE_GUARD_CF_FUNCTION_TABLE_PRESENT")]
        FunctionTablePresent = 0x00000400,
        [EnumAnnotation("IMAGE_GUARD_SECURITY_COOKIE_UNUSED")]
        SecurityCookieUnused = 0x00000800,
        [EnumAnnotation("IMAGE_GUARD_PROTECT_DELAYLOAD_IAT")]
        ProtectDelayLoadIAT = 0x00001000,
        [EnumAnnotation("IMAGE_GUARD_DELAYLOAD_IAT_IN_ITS_OWN_SECTION")]
        DelayLoadIATInItsOwnSection = 0x00002000,
        [EnumAnnotation("IMAGE_GUARD_CF_EXPORT_SUPPRESSION_INFO_PRESENT")]
        ExportSuppressionInfoPresent = 0x00004000,
        [EnumAnnotation("IMAGE_GUARD_CF_ENABLE_EXPORT_SUPPRESSION")]
        EnableExportSuppression = 0x00008000,
        [EnumAnnotation("IMAGE_GUARD_CF_LONGJUMP_TABLE_PRESENT")]
        LongJumpTablePresent = 0x00010000,
    }

    public sealed class LoadConfigurationDirectory : DataContent
    {
        private readonly LoadConfigurationCodeIntegrity _codeIntegrity;

        private LoadConfigurationDirectory(PortableExecutableImage image, DataDirectory dataDirectory, Location location, IMAGE_LOAD_CONFIG_DIRECTORY32 directory) : base(image, dataDirectory, location)
        {
            Size = directory.Size;
            TimeDateStamp = directory.TimeDateStamp;
            MajorVersion = directory.MajorVersion;
            MinorVersion = directory.MinorVersion;
            GlobalFlagsClear = directory.GlobalFlagsClear;
            GlobalFlagsSet = directory.GlobalFlagsSet;
            CriticalSectionDefaultTimeout = directory.CriticalSectionDefaultTimeout;
            DeCommitFreeBlockThreshold = directory.DeCommitFreeBlockThreshold;
            DeCommitTotalFreeThreshold = directory.DeCommitFreeBlockThreshold;
            LockPrefixTable = directory.LockPrefixTable;
            MaximumAllocationSize = directory.MaximumAllocationSize;
            VirtualMemoryThreshold = directory.VirtualMemoryThreshold;
            ProcessAffinityMask = directory.ProcessAffinityMask;
            ProcessHeapFlags = directory.ProcessHeapFlags;
            CSDVersion = directory.CSDVersion;
            DependentLoadFlags = directory.DependentLoadFlags;
            EditList = directory.EditList;
            SecurityCookie = directory.SecurityCookie;
            SEHandlerTable = directory.SEHandlerTable;
            SEHandlerCount = directory.SEHandlerCount;
            GuardCFCheckFunctionPointer = directory.GuardCFCheckFunctionPointer;
            GuardCFDispatchFunctionPointer = directory.GuardCFDispatchFunctionPointer;
            GuardCFFunctionTable = directory.GuardCFFunctionTable;
            GuardCFFunctionCount = directory.GuardCFFunctionCount;
            GuardFlags = directory.GuardFlags;
            //CodeIntegrity;
            GuardAddressTakenIatEntryTable = directory.GuardAddressTakenIatEntryTable;
            GuardAddressTakenIatEntryCount = directory.GuardAddressTakenIatEntryCount;
            GuardLongJumpTargetTable = directory.GuardLongJumpTargetTable;
            GuardLongJumpTargetCount = directory.GuardLongJumpTargetCount;
            DynamicValueRelocTable = directory.DynamicValueRelocTable;
            CHPEMetadataPointer = directory.CHPEMetadataPointer;
            GuardRFFailureRoutine = directory.GuardRFFailureRoutine;
            GuardRFFailureRoutineFunctionPointer = directory.GuardRFFailureRoutineFunctionPointer;
            DynamicValueRelocTableOffset = directory.DynamicValueRelocTableOffset;
            DynamicValueRelocTableSection = directory.DynamicValueRelocTableSection;
            Reserved2 = directory.Reserved2;
            GuardRFVerifyStackPointerFunctionPointer = directory.GuardRFVerifyStackPointerFunctionPointer;
            HotPatchTableOffset = directory.HotPatchTableOffset;
            Reserved3 = directory.Reserved3;
            EnclaveConfigurationPointer = directory.EnclaveConfigurationPointer;
            VolatileMetadataPointer = directory.VolatileMetadataPointer;

            _codeIntegrity = new LoadConfigurationCodeIntegrity(this, directory.CodeIntegrity);
        }

        private LoadConfigurationDirectory(PortableExecutableImage image, DataDirectory dataDirectory, Location location, IMAGE_LOAD_CONFIG_DIRECTORY64 directory) : base(image, dataDirectory, location)
        {
            Size = directory.Size;
            TimeDateStamp = directory.TimeDateStamp;
            MajorVersion = directory.MajorVersion;
            MinorVersion = directory.MinorVersion;
            GlobalFlagsClear = directory.GlobalFlagsClear;
            GlobalFlagsSet = directory.GlobalFlagsSet;
            CriticalSectionDefaultTimeout = directory.CriticalSectionDefaultTimeout;
            DeCommitFreeBlockThreshold = directory.DeCommitFreeBlockThreshold;
            DeCommitTotalFreeThreshold = directory.DeCommitFreeBlockThreshold;
            LockPrefixTable = directory.LockPrefixTable;
            MaximumAllocationSize = directory.MaximumAllocationSize;
            VirtualMemoryThreshold = directory.VirtualMemoryThreshold;
            ProcessAffinityMask = directory.ProcessAffinityMask;
            ProcessHeapFlags = directory.ProcessHeapFlags;
            CSDVersion = directory.CSDVersion;
            DependentLoadFlags = directory.DependentLoadFlags;
            EditList = directory.EditList;
            SecurityCookie = directory.SecurityCookie;
            SEHandlerTable = directory.SEHandlerTable;
            SEHandlerCount = directory.SEHandlerCount;
            GuardCFCheckFunctionPointer = directory.GuardCFCheckFunctionPointer;
            GuardCFDispatchFunctionPointer = directory.GuardCFDispatchFunctionPointer;
            GuardCFFunctionTable = directory.GuardCFFunctionTable;
            GuardCFFunctionCount = directory.GuardCFFunctionCount;
            GuardFlags = directory.GuardFlags;
            //CodeIntegrity;
            GuardAddressTakenIatEntryTable = directory.GuardAddressTakenIatEntryTable;
            GuardAddressTakenIatEntryCount = directory.GuardAddressTakenIatEntryCount;
            GuardLongJumpTargetTable = directory.GuardLongJumpTargetTable;
            GuardLongJumpTargetCount = directory.GuardLongJumpTargetCount;
            DynamicValueRelocTable = directory.DynamicValueRelocTable;
            CHPEMetadataPointer = directory.CHPEMetadataPointer;
            GuardRFFailureRoutine = directory.GuardRFFailureRoutine;
            GuardRFFailureRoutineFunctionPointer = directory.GuardRFFailureRoutineFunctionPointer;
            DynamicValueRelocTableOffset = directory.DynamicValueRelocTableOffset;
            DynamicValueRelocTableSection = directory.DynamicValueRelocTableSection;
            Reserved2 = directory.Reserved2;
            GuardRFVerifyStackPointerFunctionPointer = directory.GuardRFVerifyStackPointerFunctionPointer;
            HotPatchTableOffset = directory.HotPatchTableOffset;
            Reserved3 = directory.Reserved3;
            EnclaveConfigurationPointer = directory.EnclaveConfigurationPointer;
            VolatileMetadataPointer = directory.VolatileMetadataPointer;

            _codeIntegrity = new LoadConfigurationCodeIntegrity(this, directory.CodeIntegrity);
        }

        #region Static Methods

        public static LoadConfigurationDirectory Get(PortableExecutableImage image)
        {
            return GetAsync(image).GetAwaiter().GetResult();
        }

        public static async Task<LoadConfigurationDirectory> GetAsync(PortableExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.LoadConfigTable))
                return null;

            var dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.LoadConfigTable];

            if (DataDirectory.IsNullOrEmpty(dataDirectory))
                return null;

            var calc = image.GetCalculator();
            var section = calc.RVAToSection(dataDirectory.VirtualAddress);
            var fileOffset = calc.RVAToOffset(section, dataDirectory.VirtualAddress);
            var imageBase = image.NTHeaders.OptionalHeader.ImageBase;           
            var location = new Location(image, fileOffset, dataDirectory.VirtualAddress, imageBase + dataDirectory.VirtualAddress, dataDirectory.Size, dataDirectory.Size, section);
            var stream = image.GetStream();

            stream.Seek(fileOffset.ToInt64(), SeekOrigin.Begin);

            LoadConfigurationDirectory directory = null;

            if (image.Is32Bit)
            {
                IMAGE_LOAD_CONFIG_DIRECTORY32 config;

                try
                {
                    config = await stream.ReadStructAsync<IMAGE_LOAD_CONFIG_DIRECTORY32>().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new PortableExecutableImageException(image, "Could not load Load Configration Directory from stream.", ex);
                }

                directory = new LoadConfigurationDirectory(image, dataDirectory, location, config);
            }
            else
            {
                IMAGE_LOAD_CONFIG_DIRECTORY64 config;

                try
                {
                    config = await stream.ReadStructAsync<IMAGE_LOAD_CONFIG_DIRECTORY64>().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new PortableExecutableImageException(image, "Could not load Load Configration Directory from stream.", ex);
                }

                directory = new LoadConfigurationDirectory(image, dataDirectory, location, config);
            }

            return directory;
        }

        #endregion

        #region Methods

        public DateTime GetTimeDateStamp()
        {
            return Utils.ConvertTimeDateStamp(TimeDateStamp);
        }

        public GuardFlagsType GetGuardFlags()
        {
            return (GuardFlagsType)GuardFlags;
        }

        public LoadConfigurationCodeIntegrity GetCodeIntegrity()
        {
            return _codeIntegrity;
        }

        #endregion

        #region Properties

        [FieldAnnotation("Size", Order = 1)]
        public uint Size { get; }

        [FieldAnnotation("Date/Time Stamp", Order = 2)]
        public uint TimeDateStamp { get; }

        [FieldAnnotation("Major Version", Order = 3)]
        public ushort MajorVersion { get; }

        [FieldAnnotation("Minor Version", Order = 4)]
        public ushort MinorVersion { get; }

        [FieldAnnotation("Global Flags Clear", Order = 5)]
        public uint GlobalFlagsClear { get; }

        [FieldAnnotation("Global Flags Set", Order = 6)]
        public uint GlobalFlagsSet { get; }

        [FieldAnnotation("Critical Section Default Timeout", Order = 7)]
        public uint CriticalSectionDefaultTimeout { get; }

        [FieldAnnotation("De-commit Free Block Threshold", Order = 8)]
        public ulong DeCommitFreeBlockThreshold { get; }

        [FieldAnnotation("De-commit Total Free Threshold", Order = 9)]
        public ulong DeCommitTotalFreeThreshold { get; }

        [FieldAnnotation("Lock Prefix Table", Order = 10)]
        public ulong LockPrefixTable { get; }

        [FieldAnnotation("Maximum Allocation Size", Order = 11)]
        public ulong MaximumAllocationSize { get; }

        [FieldAnnotation("Virtual Memory Threshold", Order = 12)]
        public ulong VirtualMemoryThreshold { get; }

        [FieldAnnotation("Process Affinity Mask", Order = 13)]
        public ulong ProcessAffinityMask { get; }

        [FieldAnnotation("Process Heap Flags", Order = 14)]
        public uint ProcessHeapFlags { get; }

        [FieldAnnotation("CSD Version", Order = 15)]
        public ushort CSDVersion { get; }

        [FieldAnnotation("Dependent Load Flags", Order = 16)]
        public ushort DependentLoadFlags { get; }

        [FieldAnnotation("Edit List", Order = 17)]
        public ulong EditList { get; }

        [FieldAnnotation("Security Cookie", Order = 18)]
        public ulong SecurityCookie { get; }

        [FieldAnnotation("SEHandler Table", Order = 19)]
        public ulong SEHandlerTable { get; }

        [FieldAnnotation("SEHandler Count", Order = 20)]
        public ulong SEHandlerCount { get; }

        [FieldAnnotation("Guard CF Check Function Pointer", Order = 21)]
        public ulong GuardCFCheckFunctionPointer { get; }

        [FieldAnnotation("Guard CF Dispatch Function Pointer", Order = 21)]
        public ulong GuardCFDispatchFunctionPointer { get; }

        [FieldAnnotation("Guard CF Function Table", Order = 22)]
        public ulong GuardCFFunctionTable { get; }

        [FieldAnnotation("Guard CF Function Count", Order = 23)]
        public ulong GuardCFFunctionCount { get; }

        [FieldAnnotation("Guard Flags", Order = 24)]
        public uint GuardFlags { get; }

        [FieldAnnotation("Code Integrity", ArrayLength = 12, Order = 25)]
        public byte[] CodeIntegrity { get; }

        [FieldAnnotation("Guard Address Taken IAT Entry Table", Order = 26)]
        public ulong GuardAddressTakenIatEntryTable { get; }

        [FieldAnnotation("Guard Address Taken IAT Entry Count", Order = 27)]
        public ulong GuardAddressTakenIatEntryCount { get; }

        [FieldAnnotation("Guard Long Jump Target Table", Order = 28)]
        public ulong GuardLongJumpTargetTable { get; }

        [FieldAnnotation("Guard Long Jump Target Count", Order = 29)]
        public ulong GuardLongJumpTargetCount { get; }

        [FieldAnnotation("Dynamic Value Reloc Table", Order = 30)]
        public ulong DynamicValueRelocTable { get; }

        [FieldAnnotation("CHPE Metadata Pointer", Order = 31)]
        public ulong CHPEMetadataPointer { get; }

        [FieldAnnotation("Guard RF Failure Routine", Order = 32)]
        public ulong GuardRFFailureRoutine { get; }

        [FieldAnnotation("Guard RF Failure Routine Function Pointer", Order = 33)]
        public ulong GuardRFFailureRoutineFunctionPointer { get; }

        [FieldAnnotation("Dynamic Value Reloc Table Offset", Order = 34)]
        public uint DynamicValueRelocTableOffset { get; }

        [FieldAnnotation("Dynamic Value Reloc Table Section", Order = 35)]
        public ushort DynamicValueRelocTableSection { get; }

        [FieldAnnotation("Reserved 2", Order = 36)]
        public ushort Reserved2 { get; }

        [FieldAnnotation("Guard RF Verify Stack Pointer Function Pointer", Order = 37)]
        public ulong GuardRFVerifyStackPointerFunctionPointer { get; }

        [FieldAnnotation("Hot Patch Table Offset", Order = 38)]
        public uint HotPatchTableOffset { get; }

        [FieldAnnotation("Reserved 3", Order = 39)]
        public uint Reserved3 { get; }

        [FieldAnnotation("Enclave Configuration Pointer", Order = 40)]
        public ulong EnclaveConfigurationPointer { get; }

        [FieldAnnotation("Volatile Metadata Pointer", Order = 41)]
        public ulong VolatileMetadataPointer { get; }

        #endregion
    }
}

