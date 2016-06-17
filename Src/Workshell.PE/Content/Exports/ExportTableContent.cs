using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    /*

    public class ExportTableContent : DataDirectoryContent
    {

        class ExportItem
        {
        
            public int Index;
            public uint FunctionAddress;
            public int NameIndex;
            public uint NameAddress;
            public int Ordinal;

        }

        private ulong image_base;
        private Section section;
        private ExportDirectory dir;
        private ExportTable<uint> address_table;
        private ExportTable<uint> name_pointer_table;
        private ExportTable<ushort> ordinal_table;
        private Exports exports;

        internal ExportTableContent(DataDirectory dataDirectory, ulong imageBase) : base(dataDirectory,imageBase)
        {
            LocationCalculator calc = dataDirectory.Directories.Reader.GetCalculator();
            Stream stream = dataDirectory.Directories.Reader.GetStream();

            image_base = imageBase;
            section = calc.RVAToSection(dataDirectory.VirtualAddress);

            LoadDirectory(calc,stream);
            LoadAddressTable(calc,stream);
            LoadNamePointerTable(calc,stream);
            LoadOrdinalTable(calc,stream);
            LoadExports(calc,stream);
        }

        #region Methods

        private void LoadNamePointerTable(LocationCalculator calc, Stream stream)
        {
            ulong offset = calc.RVAToOffset(section,dir.AddressOfNames);
            uint size = dir.NumberOfFunctions * sizeof(uint);
            Location location = new Location(offset,dir.AddressOfNames,image_base + dir.AddressOfNames,size,size);
            List<uint> addresses = new List<uint>();

            stream.Seek(Convert.ToInt64(offset),SeekOrigin.Begin);

            for(int i = 0; i < Directory.NumberOfFunctions; i++)
            {
                uint address = Utils.ReadUInt32(stream);

                addresses.Add(address);
            }

            name_pointer_table = new ExportTable<uint>(this,location,addresses);
        }

        private void LoadOrdinalTable(LocationCalculator calc, Stream stream)
        {
            ulong offset = calc.RVAToOffset(section,dir.AddressOfNameOrdinals);
            uint size = dir.NumberOfFunctions * sizeof(ushort);
            Location location = new Location(offset,dir.AddressOfNameOrdinals,image_base + dir.AddressOfNameOrdinals,size,size);
            List<ushort> ordinals = new List<ushort>();

            stream.Seek(Convert.ToInt64(offset),SeekOrigin.Begin);

            for(int i = 0; i < Directory.NumberOfFunctions; i++)
            {
                ushort ord = Utils.ReadUInt16(stream);

                ordinals.Add(ord);
            }

            ordinal_table = new ExportTable<ushort>(this,location,ordinals);
        }

        private void LoadExports(LocationCalculator calc, Stream stream)
        {
            List<Export> list = new List<Export>();
            uint[] function_addresses = dir.GetFunctionAddresses();
            uint[] name_addresses = dir.GetFunctionNameAddresses();
            ushort[] ordinals = dir.GetFunctionOrdinals();
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
                    item.Ordinal = Convert.ToInt32(ord + dir.Base);
                }
            }

            for(int i = 0; i < items.Count; i++)
            {
                if (items[i].NameIndex == -1)
                    items[i].Ordinal = Convert.ToInt32(i + dir.Base);
            }

            foreach(ExportItem item in items)
            {
                string name = String.Empty;

                if (item.NameIndex > -1)
                {
                    Section section = calc.RVAToSection(item.NameAddress);
                    long offset = Convert.ToInt64(calc.RVAToOffset(section,item.NameAddress));

                    name = GetString(stream,offset);
                }

                string fwd_name = String.Empty;

                if (item.FunctionAddress >= DataDirectory.VirtualAddress && item.FunctionAddress <= (DataDirectory.VirtualAddress + DataDirectory.Size))
                {
                    Section section = calc.RVAToSection(item.FunctionAddress);
                    long offset = Convert.ToInt64(calc.RVAToOffset(section,item.FunctionAddress));

                    fwd_name = GetString(stream,offset);
                }

                Export export = new Export(item.Index,item.NameIndex,item.FunctionAddress,name,item.Ordinal,fwd_name);
                
                list.Add(export);
            }

            exports = new Exports(this,list.OrderBy(e => e.Ordinal).ToList());
        }

        private string GetString(Stream stream, long offset)
        {
            stream.Seek(offset,SeekOrigin.Begin);

            return Utils.ReadString(stream);
        }

        #endregion

        #region Properties

        public Section Section
        {
            get
            {
                return section;
            }
        }

        public ExportDirectory Directory
        {
            get
            {
                return dir;
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

        public Exports Exports
        {
            get
            {
                return exports;
            }
        }

        #endregion

    }

    */

}
