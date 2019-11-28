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
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Content;
using Workshell.PE.Native;
using Workshell.PE.Resources;

namespace Workshell.PE
{
    public enum DataDirectoryType : int
    {
        Unknown = -1,
        ExportTable = 0,
        ImportTable = 1,
        ResourceTable = 2,
        ExceptionTable = 3,
        CertificateTable = 4,
        BaseRelocationTable = 5,
        Debug = 6,
        Architecture = 7,
        GlobalPtr = 8,
        TLSTable = 9,
        LoadConfigTable = 10,
        BoundImport = 11,
        ImportAddressTable = 12,
        DelayImportDescriptor = 13,
        CLRRuntimeHeader = 14
    }

    public sealed class DataDirectory
    {
        private readonly PortableExecutableImage _image;
        private readonly DataDirectoryType _type;
        private readonly IMAGE_DATA_DIRECTORY _header;
        private readonly ulong _imageBase;
        private readonly Lazy<string> _sectionName;
        private readonly Lazy<Section> _section;

        internal DataDirectory(PortableExecutableImage image, DataDirectories dataDirs, DataDirectoryType dirType, IMAGE_DATA_DIRECTORY dataDirectory, ulong imageBase)
        {
            _image = image;
            _type = dirType;
            _header = dataDirectory;
            _imageBase = imageBase;
            _sectionName = new Lazy<string>(DoGetSectionName);
            _section = new Lazy<Section>(DoGetSection);

            Directories = dataDirs;
        }

        #region Static Methods

        public static bool IsNullOrEmpty(DataDirectory dataDirectory)
        {
            if (dataDirectory == null)
                return true;

            if (dataDirectory.VirtualAddress == 0)
                return true;

            if (dataDirectory.Size == 0)
                return true;

            return false;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return (_type != DataDirectoryType.Unknown ? _type.ToString() : $"0x{_header.VirtualAddress:X8}:{_header.Size}");
        }

        public DataContent GetContent()
        {
            return GetContentAsync().GetAwaiter().GetResult();
        }

        public async Task<DataContent> GetContentAsync()
        {
            if (VirtualAddress == 0 || Size == 0)
            {
                return null;
            }

            switch (DirectoryType)
            {
                case DataDirectoryType.LoadConfigTable:
                    return await LoadConfigurationDirectory.GetAsync(_image).ConfigureAwait(false);
                case DataDirectoryType.TLSTable:
                    return await TLSDirectory.GetAsync(_image).ConfigureAwait(false);
                case DataDirectoryType.CertificateTable:
                    return await Certificate.GetAsync(_image).ConfigureAwait(false);
                case DataDirectoryType.CLRRuntimeHeader:
                    return await CLR.GetAsync(_image).ConfigureAwait(false);
                case DataDirectoryType.Debug:
                    return await DebugDirectory.GetAsync(_image).ConfigureAwait(false);
                case DataDirectoryType.BaseRelocationTable:
                    return await RelocationTable.GetAsync(_image).ConfigureAwait(false);
                case DataDirectoryType.ExportTable:
                    return await ExportDirectory.GetAsync(_image).ConfigureAwait(false);
                case DataDirectoryType.ImportTable:
                    return await ImportDirectory.GetAsync(_image).ConfigureAwait(false);
                case DataDirectoryType.DelayImportDescriptor:
                    return await DelayedImportDirectory.GetAsync(_image).ConfigureAwait(false);
                case DataDirectoryType.ExceptionTable:
                    return await ExceptionTable.GetAsync(_image).ConfigureAwait(false);
                case DataDirectoryType.ResourceTable:
                    return await ResourceDirectory.GetAsync(_image).ConfigureAwait(false);
                default:
                {
                    var calc = _image.GetCalculator();
                    var fileOffset = calc.RVAToOffset(VirtualAddress);
                    var va = _imageBase + VirtualAddress;
                    var location = new Location(_image, fileOffset, VirtualAddress, va, Size, Size);

                    return new DataContent(_image, this, location);
                }
            }
        }

        public string GetSectionName()
        {
            return _sectionName.Value;
        }

        public Section GetSection()
        {
            return _section.Value;
        }

        private string DoGetSectionName()
        {
            if (_header.VirtualAddress == 0 || _type == DataDirectoryType.CertificateTable)
                return string.Empty;

            foreach (var entry in _image.SectionTable)
            {
                if (_header.VirtualAddress >= entry.VirtualAddress && _header.VirtualAddress < (entry.VirtualAddress + entry.SizeOfRawData))
                    return entry.Name;
            }

            return string.Empty;
        }

        private Section DoGetSection()
        {
            if (_header.VirtualAddress == 0 || _type == DataDirectoryType.CertificateTable)
                return null;

            foreach (var section in _image.Sections)
            {
                if (_header.VirtualAddress >= section.TableEntry.VirtualAddress && _header.VirtualAddress < (section.TableEntry.VirtualAddress + section.TableEntry.SizeOfRawData))
                    return section;
            }

            return null;
        }

        #endregion

        #region Properties

        public DataDirectories Directories { get; }
        public DataDirectoryType DirectoryType => _type;
        public uint VirtualAddress => _header.VirtualAddress;
        public uint Size => _header.Size;
        public bool IsEmpty => (_header.Size == 0);

        #endregion
    }
}
