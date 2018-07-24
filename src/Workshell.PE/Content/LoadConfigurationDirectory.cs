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
    public sealed class LoadConfigurationDirectory : DataContent
    {
        internal LoadConfigurationDirectory(PortableExecutableImage image, DataDirectory dataDirectory, Location location, IMAGE_LOAD_CONFIG_DIRECTORY32 directory) : base(image, dataDirectory, location)
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
            EditList = directory.EditList;
            SecurityCookie = directory.SecurityCookie;
            SEHandlerTable = directory.SEHandlerTable;
            SEHandlerCount = directory.SEHandlerCount;
            GuardCFCheckFunctionPointer = directory.GuardCFCheckFunctionPointer;
            GuardCFFunctionTable = directory.GuardCFFunctionTable;
            GuardCFFunctionCount = directory.GuardCFFunctionCount;
            GuardFlags = directory.GuardFlags;
        }

        internal LoadConfigurationDirectory(PortableExecutableImage image, DataDirectory dataDirectory, Location location, IMAGE_LOAD_CONFIG_DIRECTORY64 directory) : base(image, dataDirectory, location)
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
            EditList = directory.EditList;
            SecurityCookie = directory.SecurityCookie;
            SEHandlerTable = directory.SEHandlerTable;
            SEHandlerCount = directory.SEHandlerCount;
            GuardCFCheckFunctionPointer = directory.GuardCFCheckFunctionPointer;
            GuardCFFunctionTable = directory.GuardCFFunctionTable;
            GuardCFFunctionCount = directory.GuardCFFunctionCount;
            GuardFlags = directory.GuardFlags;
        }

        #region Static Methods

        public static async Task<LoadConfigurationDirectory> LoadAsync(PortableExecutableImage image)
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
            var location = new Location(fileOffset, dataDirectory.VirtualAddress, imageBase + dataDirectory.VirtualAddress, dataDirectory.Size, dataDirectory.Size, section);
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
                    throw new PortableExecutableImageException(image, "Could not load Load Configration Directory from stream.");
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

        #region Properties

        [FieldAnnotation("Size")]
        public uint Size { get; }

        [FieldAnnotation("Date/Time Stamp")]
        public uint TimeDateStamp { get; }

        [FieldAnnotation("Major Version")]
        public ushort MajorVersion { get; }

        [FieldAnnotation("Minor Version")]
        public ushort MinorVersion { get; }

        [FieldAnnotation("Global Flags Clear")]
        public uint GlobalFlagsClear { get; }

        [FieldAnnotation("Global Flags Set")]
        public uint GlobalFlagsSet { get; }

        [FieldAnnotation("Critical Section Default Timeout")]
        public uint CriticalSectionDefaultTimeout { get; }

        [FieldAnnotation("De-commit Free Block Threshold")]
        public ulong DeCommitFreeBlockThreshold { get; }

        [FieldAnnotation("De-commit Total Free Threshold")]
        public ulong DeCommitTotalFreeThreshold { get; }

        [FieldAnnotation("Lock Prefix Table")]
        public ulong LockPrefixTable { get; }

        [FieldAnnotation("Maximum Allocation Size")]
        public ulong MaximumAllocationSize { get; }

        [FieldAnnotation("Virtual Memory Threshold")]
        public ulong VirtualMemoryThreshold { get; }

        [FieldAnnotation("Process Affinity Mask")]
        public ulong ProcessAffinityMask { get; }

        [FieldAnnotation("Process Heap Flags")]
        public uint ProcessHeapFlags { get; }

        [FieldAnnotation("CSD Version")]
        public ushort CSDVersion { get; }

        [FieldAnnotation("Edit List")]
        public ulong EditList { get; }

        [FieldAnnotation("Security Cookie")]
        public ulong SecurityCookie { get; }

        [FieldAnnotation("SEHandler Table")]
        public ulong SEHandlerTable { get; }

        [FieldAnnotation("SEHandler Count")]
        public ulong SEHandlerCount { get; }

        [FieldAnnotation("Guard CF Check Function Pointer")]
        public ulong GuardCFCheckFunctionPointer { get; }

        [FieldAnnotation("Guard CF Function Table")]
        public ulong GuardCFFunctionTable { get; }

        [FieldAnnotation("Guard CF Function Count")]
        public ulong GuardCFFunctionCount { get; }

        [FieldAnnotation("Guard Flags")]
        public uint GuardFlags { get; }

        #endregion
    }
}

