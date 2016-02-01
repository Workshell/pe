﻿using System;
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
        None = -1
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

    public class DataDirectories : IEnumerable<DataDirectory>, ILocationSupport, IRawDataSupport
    {

        public static readonly int EntrySize = Utils.SizeOf<IMAGE_DATA_DIRECTORY>();

        private OptionalHeader header;
        private StreamLocation location;
        private Dictionary<DataDirectoryType,DataDirectory> dirs;
        

        public DataDirectories(OptionalHeader optHeader, StreamLocation streamLoc, Dictionary<DataDirectoryType,DataDirectory> dataDirs)
        {
            header = optHeader;
            location = streamLoc;
            dirs = dataDirs;
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
            Stream stream = header.Reader.GetStream();

            return Utils.ReadBytes(stream,location);
        }

        public bool Has(DataDirectoryType directoryType)
        {
            return dirs.ContainsKey(directoryType);
        }

        public bool Has(int directoryType)
        {
            return Has((DataDirectoryType)directoryType);
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
