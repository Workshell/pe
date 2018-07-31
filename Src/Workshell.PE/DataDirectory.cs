using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Content;
using Workshell.PE.Native;

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
            if (VirtualAddress == 0)
                return null;

            switch (DirectoryType)
            {
                case DataDirectoryType.LoadConfigTable:
                    return await LoadConfigurationDirectory.LoadAsync(_image).ConfigureAwait(false);
                case DataDirectoryType.TLSTable:
                    return await TLSDirectory.LoadAsync(_image).ConfigureAwait(false);
                case DataDirectoryType.CertificateTable:
                    return await Certificate.LoadAsync(_image).ConfigureAwait(false);
                case DataDirectoryType.CLRRuntimeHeader:
                    return await CLR.LoadAsync(_image).ConfigureAwait(false);
                case DataDirectoryType.Debug:
                    return await DebugDirectory.LoadAsync(_image).ConfigureAwait(false);
                case DataDirectoryType.BaseRelocationTable:
                    return await RelocationTable.LoadAsync(_image).ConfigureAwait(false);
                case DataDirectoryType.ExportTable:
                    return await ExportDirectory.LoadAsync(_image).ConfigureAwait(false);
                case DataDirectoryType.ImportTable:
                    return await ImportDirectory.LoadAsync(_image).ConfigureAwait(false);
                case DataDirectoryType.DelayImportDescriptor:
                    return await DelayedImportDirectory.LoadAsync(_image).ConfigureAwait(false);
                default:
                {
                    var calc = _image.GetCalculator();
                    var fileOffset = calc.VAToOffset(VirtualAddress);
                    var location = new Location(calc, fileOffset, calc.OffsetToRVA(fileOffset), VirtualAddress, Size, Size);

                    return new DataContent(_image, this, location);
                }
            }

            /*
            if (data_dir.VirtualAddress == 0) // No content so no point...
                return null;

            if (dir_content == null)
            {
                switch (dir_type)
                {
                    case DataDirectoryType.ExportTable:
                        dir_content = new ExportTableContent(this,image_base);
                        break;
                    case DataDirectoryType.ImportTable:
                        dir_content = new ImportTableContent(this,image_base);
                        break;
                    case DataDirectoryType.Debug:
                        dir_content = new DebugContent(this,image_base);
                        break;
                    case DataDirectoryType.LoadConfigTable:
                        dir_content = new LoadConfigTableContent(this,image_base);
                        break;
                    case DataDirectoryType.TLSTable:
                        dir_content = new TLSTableContent(this,image_base);
                        break;
                    case DataDirectoryType.BaseRelocationTable:
                        dir_content = new RelocationTableContent(this,image_base);
                        break;
                    case DataDirectoryType.CLRRuntimeHeader:
                        dir_content = new CLRContent(this,image_base);
                        break;
                    case DataDirectoryType.ResourceTable:
                        dir_content = new ResourceTableContent(this,image_base);
                        break;
                    case DataDirectoryType.CertificateTable:
                        dir_content = new CertificateTableContent(this,image_base);
                        break;
                    default:
                        dir_content = null;
                        break;
                }
            }

            return dir_content;
            */
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
