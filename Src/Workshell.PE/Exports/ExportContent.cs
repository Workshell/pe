﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    internal class ExportContentProvider : ISectionContentProvider
    {

        #region Methods

        public SectionContent Create(DataDirectory directory, Section section)
        {
            return new ExportContent(directory,section);
        }

        #endregion

        #region Properties

        public DataDirectoryType DirectoryType
        {
            get
            {
                return DataDirectoryType.ExportTable;
            }
        }

        #endregion

    }

    public class Export
    {

        internal Export(ExportContent exportContent, int index, int nameIndex, uint entryPoint, string name, int ord, string forwardName)
        {
            Content = exportContent;
            Index = index;
            NameIndex = nameIndex;
            EntryPoint = entryPoint;
            Name = name;
            Ordinal = ord;
            ForwardName = forwardName;
        }

        #region Methods

        public override string ToString()
        {
            if (String.IsNullOrWhiteSpace(ForwardName))
            {
                return String.Format("0x{0:X8} {1:D4} {2}",EntryPoint,Ordinal,Name);
            }
            else
            {
                return String.Format("0x{0:X8} {1:D4} {2} -> {3}",EntryPoint,Ordinal,Name,ForwardName);
            }
        }

        #endregion

        #region Properties

        public ExportContent Content
        {
            get;
            private set;
        }

        public int Index
        {
            get;
            private set;
        }

        public int NameIndex
        {
            get;
            private set;
        }

        public uint EntryPoint
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public int Ordinal
        {
            get;
            private set;
        }

        public string ForwardName
        {
            get;
            private set;
        }

        #endregion

    }

    public class ExportContent : SectionContent, ILocationSupport, IEnumerable<Export>
    {

        class ExportItem
        {
        
            public int Index
            {
                get;
                set;
            }

            public uint FunctionAddress
            {
                get;
                set;
            }

            public int NameIndex
            {
                get;
                set;
            }

            public uint NameAddress
            {
                get;
                set;
            }

            public int Ordinal
            {
                get;
                set;
            }

        }

        private StreamLocation location;
        private List<Export> exports;
        private ExportDirectory directory;
        private ExportTable<uint> address_table;
        private ExportTable<uint> name_pointer_table;
        private ExportTable<ushort> ordinal_table;
        private GenericLocationSupport name_table;

        internal ExportContent(DataDirectory dataDirectory, Section section) : base(dataDirectory,section)
        {
            long offset = Convert.ToInt64(section.RVAToOffset(dataDirectory.VirtualAddress));

            location = new StreamLocation(offset,dataDirectory.Size);

            Stream stream = Section.Sections.Reader.Stream;

            exports = new List<Export>();

            LoadDirectory(stream);
            LoadAddressTable(stream);
            LoadNamePointerTable(stream);
            LoadOrdinalTable(stream);
            LoadNameTable(stream);
            LoadExports(stream);
        }

        #region Methods

        public IEnumerator<Export> GetEnumerator()
        {
            return exports.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public byte[] GetBytes()
        {
            Stream stream = Section.Sections.Reader.Stream;
            byte[] buffer = new byte[location.Size];
            
            stream.Seek(location.Offset,SeekOrigin.Begin);
            stream.Read(buffer,0,buffer.Length);

            return buffer;
        }

        private void LoadDirectory(Stream stream)
        {
            long offset = Convert.ToInt64(Section.RVAToOffset(DataDirectory.VirtualAddress));

            stream.Seek(offset,SeekOrigin.Begin);

            IMAGE_EXPORT_DIRECTORY export_dir = Utils.Read<IMAGE_EXPORT_DIRECTORY>(stream,ExportDirectory.Size);
            StreamLocation location = new StreamLocation(offset,ExportDirectory.Size);

            directory = new ExportDirectory(this,export_dir,location);
        }

        private void LoadAddressTable(Stream stream)
        {
            List<uint> addresses = new List<uint>();
            long offset = Convert.ToInt64(Section.RVAToOffset(directory.AddressOfFunctions));
            
            stream.Seek(offset,SeekOrigin.Begin);

            for(var i = 0; i < directory.NumberOfFunctions; i++)
            {
                uint address = Utils.ReadUInt32(stream);

                addresses.Add(address);
            }

            address_table = new ExportTable<uint>(this,offset,addresses.Count * sizeof(uint),addresses);
        }

        private void LoadNamePointerTable(Stream stream)
        {
            List<uint> addresses = new List<uint>();
            long offset = Convert.ToInt64(Section.RVAToOffset(directory.AddressOfNames));
            
            stream.Seek(offset,SeekOrigin.Begin);

            for(var i = 0; i < directory.NumberOfNames; i++)
            {
                uint address = Utils.ReadUInt32(stream);

                addresses.Add(address);
            }

            name_pointer_table = new ExportTable<uint>(this,offset,addresses.Count * sizeof(uint),addresses);
        }

        private void LoadOrdinalTable(Stream stream)
        {
            List<ushort> ordinals = new List<ushort>();
            long offset = Convert.ToInt64(Section.RVAToOffset(directory.AddressOfNameOrdinals));
            
            stream.Seek(offset,SeekOrigin.Begin);

            for(var i = 0; i < directory.NumberOfNames; i++)
            {
                ushort ord = Utils.ReadUInt16(stream);

                ordinals.Add(ord);
            }

            ordinal_table = new ExportTable<ushort>(this,offset,ordinals.Count * sizeof(ushort),ordinals);
        }

        private void LoadNameTable(Stream stream)
        {
            long offset = Convert.ToInt64(Section.RVAToOffset(directory.Name));
            long size = (Directory.Location.Offset + DataDirectory.Size) - offset;
            
            name_table = new GenericLocationSupport(offset,size,this);
        }

        private void LoadExports(Stream stream)
        {
            uint[] function_addresses = directory.GetFunctionAddresses();
            uint[] name_addresses = directory.GetFunctionNameAddresses();
            ushort[] ordinals = directory.GetFunctionOrdinals();
            List<ExportItem> items = new List<ExportItem>();

            for(int i = 0; i < function_addresses.Length; i++)
            {
                ExportItem item = new ExportItem() {
                    Index = i,
                    FunctionAddress = function_addresses[i],
                    NameIndex = -1,
                    NameAddress = 0,
                    Ordinal = -1
                };

                items.Add(item);
            }
            
            for(int i = 0; i < name_addresses.Length; i++)
            {
                ushort ord = ordinals[i];
                ExportItem item = items.FirstOrDefault(ei => ei.Index == ord);

                if (item != null)
                {
                    item.NameIndex = i;
                    item.NameAddress = name_addresses[i];
                    item.Ordinal = Convert.ToInt32(ord + directory.Base);
                }
            }

            for(int i = 0; i < items.Count; i++)
            {
                if (items[i].NameIndex == -1)
                    items[i].Ordinal = Convert.ToInt32(i + directory.Base);
            }

            foreach(ExportItem item in items)
            {
                string name = String.Empty;

                if (item.NameIndex > -1)
                {
                    long name_offset = Convert.ToInt64(Section.RVAToOffset(item.NameAddress));

                    name = GetString(stream,name_offset);
                }

                string fwd_name = String.Empty;

                if (item.FunctionAddress >= DataDirectory.VirtualAddress && item.FunctionAddress <= (DataDirectory.VirtualAddress + DataDirectory.Size))
                {
                    long fwd_offset = Convert.ToInt64(Section.RVAToOffset(item.FunctionAddress));

                    fwd_name = GetString(stream,fwd_offset);
                }

                Export export = new Export(this,item.Index,item.NameIndex,item.FunctionAddress,name,item.Ordinal,fwd_name);
                
                exports.Add(export);
            }

            exports = exports.OrderBy(e => e.Ordinal).ToList();
        }

        private string GetString(Stream stream, long offset)
        {
            stream.Seek(offset,SeekOrigin.Begin);

            return Utils.ReadString(stream);
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

        public ExportDirectory Directory
        {
            get
            {
                return directory;
            }
        }

        public ExportTable<uint> AddressTable
        {
            get
            {
                return address_table;
            }
        }

        public ExportTable<uint> NamePointerTable
        {
            get
            {
                return name_pointer_table;
            }
        }

        public ExportTable<ushort> OrdinalTable
        {
            get
            {
                return ordinal_table;
            }
        }

        public GenericLocationSupport NameTable
        {
            get
            {
                return name_table;
            }
        }

        public int Count
        {
            get
            {
                return exports.Count;
            }
        }

        public Export this[int index]
        {
            get
            {
                return exports[index];
            }
        }

        public Export this[string name]
        {
            get
            {
                Export result = exports.FirstOrDefault(e => String.Compare(name,e.Name,StringComparison.OrdinalIgnoreCase) == 0);

                return result;
            }
        }

        public Export this[string name, string forwardName]
        {
            get
            {
                Export result = exports.FirstOrDefault(e => String.Compare(name,e.Name,StringComparison.OrdinalIgnoreCase) == 0 && String.Compare(forwardName,e.ForwardName,StringComparison.OrdinalIgnoreCase) == 0);

                return result;
            }
        }

        #endregion

    }

}
