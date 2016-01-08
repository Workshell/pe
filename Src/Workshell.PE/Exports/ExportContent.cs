using System;
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

        public Export(uint address, string name, int ord, string forwardName)
        {
            EntryPoint = address;
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

    public class ExportContent : SectionContent, IEnumerable<Export>
    {

        private List<Export> exports;
        private ExportDirectory directory;
        private ExportTable<uint> address_table;
        private ExportTable<uint> name_pointer_table;
        private ExportTable<ushort> ordinal_table;

        internal ExportContent(DataDirectory dataDirectory, Section section) : base(dataDirectory,section)
        {
            Stream stream = Section.Sections.Reader.Stream;

            exports = new List<Export>();

            LoadDirectory(stream);
            LoadAddressTable(stream);
            LoadNamePointerTable(stream);
            LoadOrdinalTable(stream);
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

            for(var i = 0; i < directory.NumberOfFunctions; i++)
            {
                ushort ord = Utils.ReadUInt16(stream);

                ordinals.Add(ord);
            }

            ordinal_table = new ExportTable<ushort>(this,offset,ordinals.Count * sizeof(ushort),ordinals);
        }

        private void LoadExports(Stream stream)
        {
            uint[] function_addresses = directory.GetFunctionAddresses();
            uint[] name_addresses = directory.GetFunctionNameAddresses();
            ushort[] ordinals = directory.GetFunctionOrdinals();

            for(int i = 0; i < ordinals.Length; i++)
            {
                ushort ord = ordinals[i];
                uint function_address = function_addresses[ord];
                uint name_address = name_addresses[i];
                long name_offset = Convert.ToInt64(Section.RVAToOffset(name_address));
                string name = GetString(stream,name_offset);
                string fwd_name = String.Empty;

                if (function_address >= DataDirectory.VirtualAddress && function_address <= (DataDirectory.VirtualAddress + DataDirectory.Size))
                {
                    long fwd_offset = Convert.ToInt64(Section.RVAToOffset(function_address));

                    fwd_name = GetString(stream,fwd_offset);
                }

                Export export = new Export(function_address,name,Convert.ToInt32(ord + directory.Base),fwd_name);

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
