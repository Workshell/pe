using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public enum DataDirectoryType
    {
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
        CLRRuntimeHeader = 14,
        None = 999
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

        public static bool IsNullOrEmpty(DataDirectory dataDir)
        {
            if (dataDir == null)
                return true;

            if (dataDir.VirtualAddress == 0)
                return true;

            if (dataDir.Size == 0)
                return true;

            return false;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            if (dir_type != DataDirectoryType.None)
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

    public class DataDirectories : IEnumerable<DataDirectory>, ILocatable
    {

        public static readonly int EntrySize = Utils.SizeOf<IMAGE_DATA_DIRECTORY>();

        private Dictionary<DataDirectoryType,DataDirectory> dirs;
        private StreamLocation location;

        public DataDirectories(Dictionary<DataDirectoryType,DataDirectory> dataDirs, long dirsOffset, long dirsSize)
        {
            dirs = dataDirs;
            location = new StreamLocation(dirsOffset,dirsSize);
        }

        #region Methods

        public bool Has(DataDirectoryType directoryType)
        {
            return dirs.ContainsKey(directoryType);
        }

        public bool Has(int directoryType)
        {
            return Has((DataDirectoryType)directoryType);
        }

        public IEnumerator<DataDirectory> GetEnumerator()
        {
            return dirs.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public StreamLocation Location
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
