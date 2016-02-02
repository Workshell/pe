using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public class DataDirectory
    {

        private DataDirectoryType dir_type;
        private IMAGE_DATA_DIRECTORY data_dir;

        internal DataDirectory(DataDirectoryType dirType, IMAGE_DATA_DIRECTORY dataDirectory)
        {
            dir_type = dirType;
            data_dir = dataDirectory;
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
                return String.Format("0x{0:X8}+{1}",data_dir.VirtualAddress,data_dir.Size);
            }
        }

        #endregion

        #region Properties

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

    public class DataDirectories : IEnumerable<DataDirectory>
    {

        private OptionalHeader header;
        private Location location;
        private Dictionary<DataDirectoryType,DataDirectory> dirs;

        internal DataDirectories(OptionalHeader optHeader, IMAGE_DATA_DIRECTORY[] dataDirs)
        {
            uint size = Convert.ToUInt32(Utils.SizeOf<IMAGE_DATA_DIRECTORY>() * dataDirs.Length);
            ulong file_offset = optHeader.Location.FileOffset + optHeader.Location.FileSize;
            uint rva = optHeader.Location.RelativeVirtualAddress + optHeader.Location.VirtualSize;
            ulong va = optHeader.Location.VirtualAddress + optHeader.Location.VirtualSize;

            header = optHeader;
            location = new Location(file_offset,rva,va,size,size);
            dirs = new Dictionary<DataDirectoryType,DataDirectory>();
            
            for(int i = 0; i < dataDirs.Length; i++)
            {
                DataDirectoryType dir_type = DataDirectoryType.Unknown;

                if (i >= 0 && i <= 14)
                    dir_type = (DataDirectoryType)i;

                switch (dir_type)
                {
                    default:
                        {
                            DataDirectory data_dir = new DataDirectory(dir_type,dataDirs[i]);

                            dirs.Add(dir_type,data_dir);

                            break;
                        }
                }
            }
        }

        #region Methods

        public IEnumerator<DataDirectory> GetEnumerator()
        {
            return dirs.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public byte[] GetBytes()
        {
            return null;
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

        public Location Location
        {
            get
            {
                return location;
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
