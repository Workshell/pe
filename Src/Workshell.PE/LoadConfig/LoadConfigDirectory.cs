using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Attributes;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public abstract class LoadConfigDirectory : ILocationSupport, IRawDataSupport
    {

        public static readonly int Size32 = Utils.SizeOf<IMAGE_LOAD_CONFIG_DIRECTORY32>();
        public static readonly int Size64 = Utils.SizeOf<IMAGE_LOAD_CONFIG_DIRECTORY64>();

        private LoadConfigContent content;

        internal LoadConfigDirectory(LoadConfigContent lcContent)
        {
            content = lcContent;
        }

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = Content.Section.Sections.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,Location);

            return buffer;
        }

        public DateTime GetTimeDateStamp()
        {
            return Utils.ConvertTimeDateStamp(TimeDateStamp);
        }

        #endregion

        #region Properties

        public LoadConfigContent Content
        {
            get
            {
                return content;
            }
        }

        public abstract StreamLocation Location
        {
            get;
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

    public class LoadConfigDirectory32 : LoadConfigDirectory
    {

        private StreamLocation location;
        private IMAGE_LOAD_CONFIG_DIRECTORY32 directory;

        internal LoadConfigDirectory32(LoadConfigContent lcContent, long directoryOffset, IMAGE_LOAD_CONFIG_DIRECTORY32 lcDirectory) : base(lcContent)
        {
            location = new StreamLocation(directoryOffset,LoadConfigDirectory.Size32);
            directory = lcDirectory;
        }

        #region Properties

        public override StreamLocation Location
        {
            get
            {
                return location;
            }
        }

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

    public class LoadConfigDirectory64 : LoadConfigDirectory
    {

        private StreamLocation location;
        private IMAGE_LOAD_CONFIG_DIRECTORY64 directory;

        internal LoadConfigDirectory64(LoadConfigContent lcContent, long directoryOffset, IMAGE_LOAD_CONFIG_DIRECTORY64 lcDirectory) : base(lcContent)
        {
            location = new StreamLocation(directoryOffset,LoadConfigDirectory.Size64);
            directory = lcDirectory;
        }

        #region Properties

        public override StreamLocation Location
        {
            get
            {
                return location;
            }
        }

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
