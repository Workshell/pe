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

namespace Workshell.PE.Content
{
    [Flags]
    public enum COMImageFlags : int
    {
        [EnumAnnotationAttribute("COMIMAGE_FLAGS_ILONLY")]
        ILOnly = 0x00001,
        [EnumAnnotationAttribute("COMIMAGE_FLAGS_32BITREQUIRED")]
        Requires32Bit = 0x00002,
        [EnumAnnotationAttribute("COMIMAGE_FLAGS_IL_LIBRARY")]
        ILLibrary = 0x00004,
        [EnumAnnotationAttribute("COMIMAGE_FLAGS_STRONGNAMESIGNED")]
        StrongNameSigned = 0x00008,
        [EnumAnnotationAttribute("COMIMAGE_FLAGS_NATIVE_ENTRYPOINT")]
        NativeEntryPoint = 0x00010,
        [EnumAnnotationAttribute("COMIMAGE_FLAGS_TRACKDEBUGDATA")]
        TrackDebugData = 0x10000,
        [EnumAnnotationAttribute("COMIMAGE_FLAGS_32BITPREFERRED")]
        Prefer32Bit = 0x20000
    }

    public sealed class CLRHeader
    {
        private Version _runtimeVersion;
        private CLRDataDirectory _metaData;
        private CLRDataDirectory _resources;
        private CLRDataDirectory _snSig;
        private CLRDataDirectory _codeManTable;
        private CLRDataDirectory _vtableFixups;
        private CLRDataDirectory _exportAddressTable;
        private CLRDataDirectory _nativeHeader;

        internal CLRHeader(Location location, IMAGE_COR20_HEADER header)
        {
            _runtimeVersion = null;
            _metaData = null;
            _resources = null;
            _snSig = null;
            _codeManTable = null;
            _vtableFixups = null;
            _exportAddressTable = null;
            _nativeHeader = null;

            Location = location;
            HeaderSize = header.cb;
            MajorRuntimeVersion = header.MajorRuntimeVersion;
            MinorRuntimeVersion = header.MinorRuntimeVersion;
            MetaDataAddress = header.MetaData.VirtualAddress;
            MetaDataSize = header.MetaData.Size;
            Flags = header.Flags;
            EntryPointTokenOrVirtualAddress = header.EntryPointTokenOrRVA;
            ResourcesAddress = header.Resources.VirtualAddress;
            ResourcesSize = header.Resources.Size;
            StrongNameSignatureAddress = header.StrongNameSignature.VirtualAddress;
            StrongNameSignatureSize = header.StrongNameSignature.Size;
            CodeManagerTableAddress = header.CodeManagerTable.VirtualAddress;
            CodeManagerTableSize = header.CodeManagerTable.Size;
            VTableFixupsAddress = header.VTableFixups.VirtualAddress;
            VTableFixupsSize = header.VTableFixups.Size;
            ExportAddressTableJumpsAddress = header.ExportAddressTableJumps.VirtualAddress;
            ExportAddressTableJumpsSize = header.ExportAddressTableJumps.Size;
            ManagedNativeHeaderAddress = header.ManagedNativeHeader.VirtualAddress;
            ManagedNativeHeaderSize = header.ManagedNativeHeader.Size;
        }

        #region Static Methods

        internal static async Task<CLRHeader> GetAsync(PortableExecutableImage image, Location clrLocation)
        {
            var size = Utils.SizeOf<IMAGE_COR20_HEADER>();
            var location = new Location(image, clrLocation.FileOffset, clrLocation.RelativeVirtualAddress, clrLocation.VirtualAddress, size.ToUInt32(), size.ToUInt32(), clrLocation.Section);
            var stream = image.GetStream();

            stream.Seek(clrLocation.FileOffset, SeekOrigin.Begin);

            IMAGE_COR20_HEADER header;

            try
            {
                header = await stream.ReadStructAsync<IMAGE_COR20_HEADER>(size).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new PortableExecutableImageException(image, "Could not read CLR header from stream.", ex);
            }

            return new CLRHeader(location, header);
        }

        #endregion

        #region Methods

        public Version GetRuntimeVersion()
        {
            if (_runtimeVersion == null)
                _runtimeVersion = new Version(MajorRuntimeVersion, MinorRuntimeVersion);

            return _runtimeVersion;
        }

        public CLRDataDirectory GetMetaData()
        {
            if (_metaData == null)
                _metaData = new CLRDataDirectory(MetaDataAddress, MetaDataSize);

            return _metaData;
        }

        public COMImageFlags GetFlags()
        {
            return (COMImageFlags)Flags;
        }

        public CLRDataDirectory GetResources()
        {
            if (_resources == null)
                _resources = new CLRDataDirectory(ResourcesAddress, ResourcesSize);

            return _resources;
        }

        public CLRDataDirectory GetStrongNameSignature()
        {
            if (_snSig == null)
                _snSig = new CLRDataDirectory(StrongNameSignatureAddress, StrongNameSignatureSize);

            return _snSig;
        }

        public CLRDataDirectory GetCodeManagerTable()
        {
            if (_codeManTable == null)
                _codeManTable = new CLRDataDirectory(CodeManagerTableAddress, CodeManagerTableSize);

            return _codeManTable;
        }

        public CLRDataDirectory GetVTableFixups()
        {
            if (_vtableFixups == null)
                _vtableFixups = new CLRDataDirectory(VTableFixupsAddress, VTableFixupsSize);

            return _vtableFixups;
        }

        public CLRDataDirectory GetExportAddressTableJumps()
        {
            if (_exportAddressTable == null)
                _exportAddressTable = new CLRDataDirectory(ExportAddressTableJumpsAddress, ExportAddressTableJumpsSize);

            return _exportAddressTable;
        }

        public CLRDataDirectory GetManagedNativeHeader()
        {
            if (_nativeHeader == null)
                _nativeHeader = new CLRDataDirectory(ManagedNativeHeaderAddress, ManagedNativeHeaderSize);

            return _nativeHeader;
        }

        #endregion

        #region Properties

        public Location Location { get; }

        [FieldAnnotation("Header Size", Order = 1)]
        public uint HeaderSize { get; }

        [FieldAnnotation("Major Runtime Version", Order = 2)]
        public ushort MajorRuntimeVersion { get; }

        [FieldAnnotation("Minor Runtime Version", Order = 3)]
        public ushort MinorRuntimeVersion { get; }

        [FieldAnnotation("MetaData Virtual Address", Order = 4)]
        public uint MetaDataAddress { get; }

        [FieldAnnotation("MetaData Size", Order = 5)]
        public uint MetaDataSize { get; }

        [FieldAnnotation("Flags", Order = 6)]
        public uint Flags { get; }

        [FieldAnnotation("EntryPoint Token/Virtual Address", Order = 7)]
        public uint EntryPointTokenOrVirtualAddress { get; }

        [FieldAnnotation("Resources Virtual Address", Order = 8)]
        public uint ResourcesAddress { get; }

        [FieldAnnotation("Resources Size", Order = 9)]
        public uint ResourcesSize { get; }

        [FieldAnnotation("Strongname Signature Virtual Address", Order = 10)]
        public uint StrongNameSignatureAddress { get; }

        [FieldAnnotation("Strongname Signature Size", Order = 11)]
        public uint StrongNameSignatureSize { get; }

        [FieldAnnotation("Code Manager Table Virtual Address", Order = 12)]
        public uint CodeManagerTableAddress { get; }

        [FieldAnnotation("Code Manager Table Size", Order = 13)]
        public uint CodeManagerTableSize { get; }

        [FieldAnnotation("VTable Fixups Virtual Address", Order = 14)]
        public uint VTableFixupsAddress { get; }

        [FieldAnnotation("VTable Fixups Size", Order = 15)]
        public uint VTableFixupsSize { get; }

        [FieldAnnotation("Export Address Table Jumps Virtual Address", Order = 16)]
        public uint ExportAddressTableJumpsAddress { get; }

        [FieldAnnotation("Export Address Table Jumps Size", Order = 17)]
        public uint ExportAddressTableJumpsSize { get; }

        [FieldAnnotation("Managed Native Header Virtual Address", Order = 18)]
        public uint ManagedNativeHeaderAddress { get; }

        [FieldAnnotation("Managed Native Header Size", Order = 19)]
        public uint ManagedNativeHeaderSize { get; }

        #endregion
    }
}

