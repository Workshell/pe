using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public abstract class LoadConfigDirectory : ISupportsLocation, ISupportsBytes
    {

        private LoadConfigTableContent content;
        private Location location;

        internal LoadConfigDirectory(LoadConfigTableContent configContent, Location configLocation)
        {
            content = configContent;
            location = configLocation;
        }

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = content.DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream, Location);

            return buffer;
        }

        public DateTime GetTimeDateStamp()
        {
            return Utils.ConvertTimeDateStamp(TimeDateStamp);
        }

        #endregion

        #region Properties

        public LoadConfigTableContent Content
        {
            get
            {
                return content;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        [FieldAnnotation("Size")]
        public abstract uint Size
        {
            get;
        }

        [FieldAnnotation("Date/Time Stamp")]
        public abstract uint TimeDateStamp
        {
            get;
        }

        [FieldAnnotation("Major Version")]
        public abstract ushort MajorVersion
        {
            get;
        }

        [FieldAnnotation("Minor Version")]
        public abstract ushort MinorVersion
        {
            get;
        }

        [FieldAnnotation("Global Flags Clear")]
        public abstract uint GlobalFlagsClear
        {
            get;
        }

        [FieldAnnotation("Global Flags Set")]
        public abstract uint GlobalFlagsSet
        {
            get;
        }

        [FieldAnnotation("Critical Section Default Timeout")]
        public abstract uint CriticalSectionDefaultTimeout
        {
            get;
        }

        [FieldAnnotation("De-commit Free Block Threshold")]
        public abstract ulong DeCommitFreeBlockThreshold
        {
            get;
        }

        [FieldAnnotation("De-commit Total Free Threshold")]
        public abstract ulong DeCommitTotalFreeThreshold
        {
            get;
        }

        [FieldAnnotation("Lock Prefix Table")]
        public abstract ulong LockPrefixTable
        {
            get;
        }

        [FieldAnnotation("Maximum Allocation Size")]
        public abstract ulong MaximumAllocationSize
        {
            get;
        }

        [FieldAnnotation("Virtual Memory Threshold")]
        public abstract ulong VirtualMemoryThreshold
        {
            get;
        }

        [FieldAnnotation("Process Affinity Mask")]
        public abstract ulong ProcessAffinityMask
        {
            get;
        }

        [FieldAnnotation("Process Heap Flags")]
        public abstract uint ProcessHeapFlags
        {
            get;
        }

        [FieldAnnotation("CSD Version")]
        public abstract ushort CSDVersion
        {
            get;
        }

        [FieldAnnotation("Reserved")]
        public abstract ushort Reserved1
        {
            get;
        }

        [FieldAnnotation("Edit List")]
        public abstract ulong EditList
        {
            get;
        }

        [FieldAnnotation("Security Cookie")]
        public abstract ulong SecurityCookie
        {
            get;
        }

        [FieldAnnotation("SEHandler Table")]
        public abstract ulong SEHandlerTable
        {
            get;
        }

        [FieldAnnotation("SEHandler Count")]
        public abstract ulong SEHandlerCount
        {
            get;
        }

        [FieldAnnotation("Guard CF Check Function Pointer")]
        public abstract ulong GuardCFCheckFunctionPointer
        {
            get;
        }

        [FieldAnnotation("Reservws")]
        public abstract ulong Reserved2
        {
            get;
        }

        [FieldAnnotation("Guard CF Function Table")]
        public abstract ulong GuardCFFunctionTable
        {
            get;
        }

        [FieldAnnotation("Guard CF Function Count")]
        public abstract ulong GuardCFFunctionCount
        {
            get;
        }

        [FieldAnnotation("Guard Flags")]
        public abstract uint GuardFlags
        {
            get;
        }

        #endregion

    }

    public sealed class LoadConfigDirectory32 : LoadConfigDirectory
    {

        private IMAGE_LOAD_CONFIG_DIRECTORY32 directory;

        internal LoadConfigDirectory32(LoadConfigTableContent configContent, Location configLocation, IMAGE_LOAD_CONFIG_DIRECTORY32 configDirectory) : base(configContent,configLocation)
        {
            directory = configDirectory;
        }

        #region Properties

        public override uint Size
        {
            get
            {
                return directory.Size;
            }
        }

        public override uint TimeDateStamp
        {
            get
            {
                return directory.TimeDateStamp;
            }
        }

        public override ushort MajorVersion
        {
            get
            {
                return directory.MajorVersion;
            }
        }

        public override ushort MinorVersion
        {
            get
            {
                return directory.MinorVersion;
            }
        }

        public override uint GlobalFlagsClear
        {
            get
            {
                return directory.GlobalFlagsClear;
            }
        }

        public override uint GlobalFlagsSet
        {
            get
            {
                return directory.GlobalFlagsSet;
            }
        }

        public override uint CriticalSectionDefaultTimeout
        {
            get
            {
                return directory.CriticalSectionDefaultTimeout;
            }
        }

        public override ulong DeCommitFreeBlockThreshold
        {
            get
            {
                return directory.DeCommitFreeBlockThreshold;
            }
        }

        public override ulong DeCommitTotalFreeThreshold
        {
            get
            {
                return directory.DeCommitTotalFreeThreshold;
            }
        }

        public override ulong LockPrefixTable
        {
            get
            {
                return directory.LockPrefixTable;
            }
        }

        public override ulong MaximumAllocationSize
        {
            get
            {
                return directory.MaximumAllocationSize;
            }
        }

        public override ulong VirtualMemoryThreshold
        {
            get
            {
                return directory.VirtualMemoryThreshold;
            }
        }

        public override ulong ProcessAffinityMask
        {
            get
            {
                return directory.ProcessAffinityMask;
            }
        }

        public override uint ProcessHeapFlags
        {
            get
            {
                return directory.ProcessHeapFlags;
            }
        }

        public override ushort CSDVersion
        {
            get
            {
                return directory.CSDVersion;
            }
        }

        public override ushort Reserved1
        {
            get
            {
                return directory.Reserved1;
            }
        }

        public override ulong EditList
        {
            get
            {
                return directory.EditList;
            }
        }

        public override ulong SecurityCookie
        {
            get
            {
                return directory.SecurityCookie;
            }
        }

        public override ulong SEHandlerTable
        {
            get
            {
                return directory.SEHandlerTable;
            }
        }

        public override ulong SEHandlerCount
        {
            get
            {
                return directory.SEHandlerCount;
            }
        }

        public override ulong GuardCFCheckFunctionPointer
        {
            get
            {
                return directory.GuardCFCheckFunctionPointer;
            }
        }

        public override ulong Reserved2
        {
            get
            {
                return directory.Reserved2;
            }
        }

        public override ulong GuardCFFunctionTable
        {
            get
            {
                return directory.GuardCFFunctionTable;
            }
        }

        public override ulong GuardCFFunctionCount
        {
            get
            {
                return directory.GuardCFFunctionCount;
            }
        }

        public override uint GuardFlags
        {
            get
            {
                return directory.GuardFlags;
            }
        }

        #endregion

    }

    public sealed class LoadConfigDirectory64 : LoadConfigDirectory
    {

        private IMAGE_LOAD_CONFIG_DIRECTORY64 directory;

        internal LoadConfigDirectory64(LoadConfigTableContent configContent, Location configLocation, IMAGE_LOAD_CONFIG_DIRECTORY64 configDirectory) : base(configContent,configLocation)
        {
            directory = configDirectory;
        }

        #region Properties

        public override uint Size
        {
            get
            {
                return directory.Size;
            }
        }

        public override uint TimeDateStamp
        {
            get
            {
                return directory.TimeDateStamp;
            }
        }

        public override ushort MajorVersion
        {
            get
            {
                return directory.MajorVersion;
            }
        }

        public override ushort MinorVersion
        {
            get
            {
                return directory.MinorVersion;
            }
        }

        public override uint GlobalFlagsClear
        {
            get
            {
                return directory.GlobalFlagsClear;
            }
        }

        public override uint GlobalFlagsSet
        {
            get
            {
                return directory.GlobalFlagsSet;
            }
        }

        public override uint CriticalSectionDefaultTimeout
        {
            get
            {
                return directory.CriticalSectionDefaultTimeout;
            }
        }

        public override ulong DeCommitFreeBlockThreshold
        {
            get
            {
                return directory.DeCommitFreeBlockThreshold;
            }
        }

        public override ulong DeCommitTotalFreeThreshold
        {
            get
            {
                return directory.DeCommitTotalFreeThreshold;
            }
        }

        public override ulong LockPrefixTable
        {
            get
            {
                return directory.LockPrefixTable;
            }
        }

        public override ulong MaximumAllocationSize
        {
            get
            {
                return directory.MaximumAllocationSize;
            }
        }

        public override ulong VirtualMemoryThreshold
        {
            get
            {
                return directory.VirtualMemoryThreshold;
            }
        }

        public override ulong ProcessAffinityMask
        {
            get
            {
                return directory.ProcessAffinityMask;
            }
        }

        public override uint ProcessHeapFlags
        {
            get
            {
                return directory.ProcessHeapFlags;
            }
        }

        public override ushort CSDVersion
        {
            get
            {
                return directory.CSDVersion;
            }
        }

        public override ushort Reserved1
        {
            get
            {
                return directory.Reserved1;
            }
        }

        public override ulong EditList
        {
            get
            {
                return directory.EditList;
            }
        }

        public override ulong SecurityCookie
        {
            get
            {
                return directory.SecurityCookie;
            }
        }

        public override ulong SEHandlerTable
        {
            get
            {
                return directory.SEHandlerTable;
            }
        }

        public override ulong SEHandlerCount
        {
            get
            {
                return directory.SEHandlerCount;
            }
        }

        public override ulong GuardCFCheckFunctionPointer
        {
            get
            {
                return directory.GuardCFCheckFunctionPointer;
            }
        }

        public override ulong Reserved2
        {
            get
            {
                return directory.Reserved2;
            }
        }

        public override ulong GuardCFFunctionTable
        {
            get
            {
                return directory.GuardCFFunctionTable;
            }
        }

        public override ulong GuardCFFunctionCount
        {
            get
            {
                return directory.GuardCFFunctionCount;
            }
        }

        public override uint GuardFlags
        {
            get
            {
                return directory.GuardFlags;
            }
        }

        #endregion

    }

}
