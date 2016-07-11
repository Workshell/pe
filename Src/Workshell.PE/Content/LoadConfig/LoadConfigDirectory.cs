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
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE.LoadConfiguration
{

    public abstract class LoadConfigDirectory : ExecutableImageContent, ISupportsBytes
    {

        internal LoadConfigDirectory(DataDirectory dataDirectory, Location configLocation) : base(dataDirectory,configLocation)
        {
        }

        #region Static Methods

        public static LoadConfigDirectory Get(ExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.LoadConfigTable))
                return null;

            DataDirectory directory = image.NTHeaders.DataDirectories[DataDirectoryType.LoadConfigTable];

            if (DataDirectory.IsNullOrEmpty(directory))
                return null;

            LocationCalculator calc = directory.Directories.Image.GetCalculator();
            Section section = calc.RVAToSection(directory.VirtualAddress);
            ulong file_offset = calc.RVAToOffset(section, directory.VirtualAddress);
            ulong image_base = directory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;           
            Location location = new Location(file_offset, directory.VirtualAddress, image_base + directory.VirtualAddress, directory.Size, directory.Size, section);
            Stream stream = directory.Directories.Image.GetStream();

            stream.Seek(file_offset.ToInt64(), SeekOrigin.Begin);

            bool is_64bit = directory.Directories.Image.Is64Bit;
            LoadConfigDirectory load_config_dir = null;

            if (!is_64bit)
            {
                IMAGE_LOAD_CONFIG_DIRECTORY32 config_dir = Utils.Read<IMAGE_LOAD_CONFIG_DIRECTORY32>(stream);

                load_config_dir = new LoadConfigDirectory32(directory, location, config_dir);
            }
            else
            {
                IMAGE_LOAD_CONFIG_DIRECTORY64 config_dir = Utils.Read<IMAGE_LOAD_CONFIG_DIRECTORY64>(stream);

                load_config_dir = new LoadConfigDirectory64(directory, location, config_dir);
            }

            return load_config_dir;
        }

        #endregion

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream, Location);

            return buffer;
        }

        public DateTime GetTimeDateStamp()
        {
            return Utils.ConvertTimeDateStamp(TimeDateStamp);
        }

        #endregion

        #region Properties

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

        [FieldAnnotation("Reserved")]
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

        internal LoadConfigDirectory32(DataDirectory dataDirectory, Location configLocation, IMAGE_LOAD_CONFIG_DIRECTORY32 configDirectory) : base(dataDirectory,configLocation)
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

        internal LoadConfigDirectory64(DataDirectory dataDirectory, Location configLocation, IMAGE_LOAD_CONFIG_DIRECTORY64 configDirectory) : base(dataDirectory,configLocation)
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
