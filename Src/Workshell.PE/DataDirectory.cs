using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;
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

        private DataDirectoryCollection dirs;
        private DataDirectoryType dir_type;
        private IMAGE_DATA_DIRECTORY data_dir;
        private ulong image_base;
        private Lazy<string> section_name;
        private Lazy<Section> section;

        internal DataDirectory(DataDirectoryCollection dataDirs, DataDirectoryType dirType, IMAGE_DATA_DIRECTORY dataDirectory, ulong imageBase)
        {
            dirs = dataDirs;
            dir_type = dirType;
            data_dir = dataDirectory;
            image_base = imageBase;
            section_name = new Lazy<string>(DoGetSectionName);
            section = new Lazy<Section>(DoGetSection);
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
            if (dir_type != DataDirectoryType.Unknown)
            {
                return dir_type.ToString();
            }
            else
            {
                return String.Format("0x{0:X8}:{1}",data_dir.VirtualAddress,data_dir.Size);
            }
        }

        public ExecutableImageContent GetContent()
        {
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

            return null;
        }

        public string GetSectionName()
        {
            return section_name.Value;
        }

        public Section GetSection()
        {
            return section.Value;
        }

        private string DoGetSectionName()
        {
            if (data_dir.VirtualAddress == 0 || dir_type == DataDirectoryType.CertificateTable)
                return String.Empty;

            foreach (SectionTableEntry entry in dirs.Reader.SectionTable)
            {
                if (data_dir.VirtualAddress >= entry.VirtualAddress && data_dir.VirtualAddress < (entry.VirtualAddress + entry.SizeOfRawData))
                    return entry.Name;
            }

            return String.Empty;
        }

        private Section DoGetSection()
        {
            if (data_dir.VirtualAddress == 0 || dir_type == DataDirectoryType.CertificateTable)
                return null;

            foreach (Section section in dirs.Reader.Sections)
            {
                if (data_dir.VirtualAddress >= section.TableEntry.VirtualAddress && data_dir.VirtualAddress < (section.TableEntry.VirtualAddress + section.TableEntry.SizeOfRawData))
                    return section;
            }

            return null;
        }

        #endregion

        #region Properties

        public DataDirectoryCollection Directories
        {
            get
            {
                return dirs;
            }
        }

        public DataDirectoryType DirectoryType
        {
            get
            {
                return dir_type;
            }
        }

        public uint VirtualAddress
        {
            get
            {
                return data_dir.VirtualAddress;
            }
        }

        public uint Size
        {
            get
            {
                return data_dir.Size;
            }
        }

        #endregion

    }

    public sealed class DataDirectoryCollection : IEnumerable<DataDirectory>, ISupportsLocation, ISupportsBytes
    {

        private ExecutableImage reader;
        private Location location;
        private Dictionary<DataDirectoryType,DataDirectory> dirs;

        internal DataDirectoryCollection(OptionalHeader optHeader, IMAGE_DATA_DIRECTORY[] dataDirs)
        {
            uint size = (Utils.SizeOf<IMAGE_DATA_DIRECTORY>() * dataDirs.Length).ToUInt32();
            ulong file_offset = optHeader.Location.FileOffset + optHeader.Location.FileSize;
            uint rva = optHeader.Location.RelativeVirtualAddress + optHeader.Location.VirtualSize.ToUInt32();
            ulong va = optHeader.Location.VirtualAddress + optHeader.Location.VirtualSize;

            reader = optHeader.Reader;
            location = new Location(file_offset,rva,va,size,size);
            dirs = new Dictionary<DataDirectoryType,DataDirectory>();

            for(int i = 0; i < dataDirs.Length; i++)
            {
                DataDirectoryType type = DataDirectoryType.Unknown;

                if (i >= 0 && i <= 14)
                    type = (DataDirectoryType)i;

                DataDirectory dir = new DataDirectory(this,type,dataDirs[i],optHeader.ImageBase);

                dirs.Add(type,dir);
            }
        }

        #region Methods

        public IEnumerator<DataDirectory> GetEnumerator()
        {
            foreach(KeyValuePair<DataDirectoryType,DataDirectory> kvp in dirs)
            {
                yield return kvp.Value;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public byte[] GetBytes()
        {
            Stream stream = reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        public bool Exists(DataDirectoryType directoryType)
        {
            return dirs.ContainsKey(directoryType);
        }

        public bool Exists(int directoryType)
        {
            return Exists((DataDirectoryType)directoryType);
        }

        #endregion

        #region Properties

        public ExecutableImage Reader
        {
            get
            {
                return reader;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        public int Count
        {
            get
            {
                return dirs.Count;
            }
        }

        public DataDirectory this[DataDirectoryType directoryType]
        {
            get
            {
                if (dirs.ContainsKey(directoryType))
                {
                    return dirs[directoryType];
                }
                else
                {
                    return null;
                }
            }
        }

        public DataDirectory this[int directoryType]
        {
            get
            {
                DataDirectoryType dir_type = (DataDirectoryType)directoryType;

                return this[dir_type];
            }
        }

        #endregion
   
    }

}
