using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class ImportTableContent : DataDirectoryContent
    {

        private ulong image_base;
        private ImportDirectory dir;
        private ImportAddressTableCollection ilt;
        private ImportAddressTableCollection iat;
        private ImportHintNameTable hint_name_table;
        private Imports imports;

        internal ImportTableContent(DataDirectory dataDirectory, ulong imageBase) : base(dataDirectory,imageBase)
        {
            image_base = imageBase;

            LocationCalculator calc = DataDirectory.Directories.Reader.GetCalculator();
            Stream stream = DataDirectory.Directories.Reader.GetStream();

            LoadDirectory(calc,stream);
            LoadILT(calc,stream);
            LoadIAT(calc,stream);
            LoadHintNameTable(calc,stream);
            LoadImports(calc,stream);
        }

        #region Methods

        private void LoadDirectory(LocationCalculator calc, Stream stream)
        {
            List<IMAGE_IMPORT_DESCRIPTOR> descriptors = new List<IMAGE_IMPORT_DESCRIPTOR>();
            Section section = calc.RVAToSection(DataDirectory.VirtualAddress);
            ulong offset = calc.RVAToOffset(section,DataDirectory.VirtualAddress);
            int size = Utils.SizeOf<IMAGE_IMPORT_DESCRIPTOR>();

            stream.Seek(Convert.ToInt64(offset),SeekOrigin.Begin);

            while (true)
            {
                IMAGE_IMPORT_DESCRIPTOR descriptor = Utils.Read<IMAGE_IMPORT_DESCRIPTOR>(stream,size);

                if (descriptor.OriginalFirstThunk == 0 && descriptor.FirstThunk == 0)
                    break;

                descriptors.Add(descriptor);
            }

            uint total_size = Convert.ToUInt32((descriptors.Count + 1) * size);
            Location location = new Location(offset,DataDirectory.VirtualAddress,image_base + DataDirectory.VirtualAddress,total_size,total_size);

            dir = new ImportDirectory(this,location,section,descriptors,image_base);
        }

        private void LoadILT(LocationCalculator calc, Stream stream)
        {
            bool is_64bit = DataDirectory.Directories.Reader.Is64Bit;
            Section section = calc.RVAToSection(DataDirectory.VirtualAddress);
            List<Tuple<ulong,ImportDirectoryEntry>> tables = new List<Tuple<ulong,ImportDirectoryEntry>>();

            foreach(ImportDirectoryEntry dir_entry in dir)
            {
                if (dir_entry.OriginalFirstThunk == 0)
                    continue;

                ulong offset = calc.RVAToOffset(section,dir_entry.OriginalFirstThunk);

                tables.Add(new Tuple<ulong,ImportDirectoryEntry>(offset,dir_entry));
            }

            ilt = new ImportAddressTableCollection(this,section,tables);
        }

        private void LoadIAT(LocationCalculator calc, Stream stream)
        {
            bool is_64bit = DataDirectory.Directories.Reader.Is64Bit;
            Section section = calc.RVAToSection(DataDirectory.VirtualAddress);
            List<Tuple<ulong,ImportDirectoryEntry>> tables = new List<Tuple<ulong,ImportDirectoryEntry>>();

            foreach(ImportDirectoryEntry dir_entry in dir)
            {
                if (dir_entry.FirstThunk == 0)
                    continue;

                ulong offset = calc.RVAToOffset(section,dir_entry.FirstThunk);

                tables.Add(new Tuple<ulong,ImportDirectoryEntry>(offset,dir_entry));
            }

            iat = new ImportAddressTableCollection(this,section,tables);
        }

        private void LoadHintNameTable(LocationCalculator calc, Stream stream)
        {
            Dictionary<uint, Tuple<ulong, uint, ushort, string, bool>> entries = new Dictionary<uint, Tuple<ulong, uint, ushort, string, bool>>();

            foreach (ImportAddressTable table in ilt)
            {
                foreach (ImportAddressTableEntry entry in table)
                {
                    if (entry.Address == 0)
                        continue;

                    if (entries.ContainsKey(entry.Address))
                        continue;

                    if (!entry.IsOrdinal)
                    {
                        ulong offset = calc.RVAToOffset(entry.Address);
                        uint size = 0;
                        bool is_padded = false;
                        ushort hint = 0;
                        StringBuilder name = new StringBuilder();

                        stream.Seek(Convert.ToInt64(offset), SeekOrigin.Begin);

                        hint = Utils.ReadUInt16(stream);
                        size += sizeof(ushort);

                        while (true)
                        {
                            int b = stream.ReadByte();

                            size++;

                            if (b <= 0)
                                break;

                            name.Append((char)b);
                        }

                        if ((size % 2) != 0)
                        {
                            is_padded = true;
                            size++;
                        }

                        Tuple<ulong, uint, ushort, string, bool> tuple = new Tuple<ulong, uint, ushort, string, bool>(offset, size, hint, name.ToString(), is_padded);

                        entries.Add(entry.Address, tuple);
                    }
                }
            }

            hint_name_table = new ImportHintNameTable(this, entries.Values);
        }

        private void LoadImports(LocationCalculator calc, Stream stream)
        {
            List<Tuple<string,ImportAddressTable,ImportHintNameTable>> libraries = new List<Tuple<string,ImportAddressTable,ImportHintNameTable>>();

            foreach(ImportAddressTable table in ilt)
            {
                StringBuilder builder = new StringBuilder();
                ulong offset = calc.RVAToOffset(table.DirectoryEntry.Name);

                stream.Seek(Convert.ToInt64(offset),SeekOrigin.Begin);

                while (true)
                {
                    int b = stream.ReadByte();

                    if (b <= 0)
                        break;

                    builder.Append((char)b);
                }

                Tuple<string,ImportAddressTable,ImportHintNameTable> tuple = new Tuple<string,ImportAddressTable,ImportHintNameTable>(builder.ToString(),table,hint_name_table);

                libraries.Add(tuple);
            }

            imports = new Imports(this,libraries);
        }

        #endregion

        #region Properties

        public ImportDirectory Directory
        {
            get
            {
                return dir;
            }
        }

        public ImportAddressTableCollection ILT
        {
            get
            {
                return ilt;
            }
        }

        public ImportAddressTableCollection IAT
        {
            get
            {
                return iat;
            }
        }

        public ImportHintNameTable HintNameTable
        {
            get
            {
                return hint_name_table;
            }
        }

        public Imports Imports
        {
            get
            {
                return imports;
            }
        }

        #endregion

    }

}
