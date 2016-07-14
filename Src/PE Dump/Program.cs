using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Options;
using Workshell.PE;
using Workshell.PE.Annotations;

namespace Workshell.PE.Dump
{

    class Program
    {

        private OptionSet options;
        private string offset_type;
        private bool show_all;
        private bool show_dos_header;
        private bool show_dos_stub;
        private bool show_file_header;
        private bool show_opt_header;
        private bool show_data_directories;
        private bool show_section_table;

        public Program()
        {
            options = new OptionSet();

            options.Add("offset=", "", v => offset_type = v);
            options.Add("all", "", v => show_all = v != null);
            options.Add("dos-header", "", v => show_dos_header = v != null);
            options.Add("dos-stub", "", v => show_dos_stub = v != null);
            options.Add("file-header", "", v => show_file_header = v != null);
            options.Add("optional-header", "", v => show_opt_header = v != null);
            options.Add("data-directories", "", v => show_data_directories = v != null);
            options.Add("sections", "", v => show_section_table = v != null);

            offset_type = "fo";
            show_all = false;
            show_dos_header = false;
            show_dos_stub = false;
            show_file_header = false;
            show_opt_header = false;
            show_data_directories = false;
            show_section_table = false;
        }

        #region Static Methods

        static int Main(string[] args)
        {
            return (new Program()).Run(args);
        }

        #endregion

        #region Methods

        public int Run(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Usage();

                    return 1;
                }

                List<string> extra = options.Parse(args);

                if (extra.Count == 0)
                {
                    Console.WriteLine("Error: No file specified.");

                    return 2;
                }

                string[] address_types = { "fo", "vo", "va", "rva" };

                if (!address_types.Contains(offset_type, StringComparer.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Error: Unknown address type - " + offset_type);

                    return 3;
                }

                string file_name = extra[0];
                string valid_message = null;

                if (!ExecutableImage.IsValid(file_name, out valid_message))
                {
                    Console.WriteLine("Error: {0}", valid_message);

                    return 4;
                }

                ExecutableImage image = ExecutableImage.FromFile(file_name);

                try
                {
                    if (show_all)
                    {
                        show_dos_header = true;
                        show_dos_stub = true;
                        show_file_header = true;
                        show_opt_header = true;
                        show_data_directories = true;
                        show_section_table = true;
                    }

                    int result = ShowBasicDetails(image, file_name);

                    if (result == 0 && show_dos_header)
                        result = ShowDOSHeader(image);

                    if (result == 0 && show_dos_stub)
                        result = ShowDOSStub(image);

                    if (result == 0 && show_file_header)
                        result = ShowFileHeader(image);

                    if (result == 0 && show_opt_header)
                        result = ShowOptionalHeader(image);

                    if (result == 0 && show_data_directories)
                        result = ShowDataDirectories(image);

                    if (result == 0 && show_section_table)
                        result = ShowSectionTable(image);

                    return result;
                }
                finally
                {
                    image.Dispose();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                throw;
#else
                Console.WriteLine("Error: {0}", ex.Message);

                return 666;
#endif
            }
        }

        private void Usage()
        {

        }

        private int ShowBasicDetails(ExecutableImage image, string fileName)
        {
            Stream stream = image.GetStream();
            string image_type = "Unknown";
            CharacteristicsType characteristics = image.NTHeaders.FileHeader.GetCharacteristics();

            if ((characteristics & CharacteristicsType.IsDLL) == CharacteristicsType.IsDLL)
            {
                image_type = "Dynamic Link Library";
            }
            else
            {
                SubSystemType sub_system = image.NTHeaders.OptionalHeader.GetSubsystem();

                if (sub_system == SubSystemType.WindowsCUI)
                {
                    image_type = "Console Application";
                }
                else if (sub_system == SubSystemType.WindowsGUI)
                {
                    image_type = "Windows Application";
                }
            }

            Console.WriteLine("[ Basic Details ]");
            Console.WriteLine();

            Console.WriteLine("    Filename:      {0}", Path.GetFileName(fileName));
            Console.WriteLine("    Path:          {0}", Path.GetDirectoryName(fileName));
            Console.WriteLine("    Size:          {1} ({0})", Utils.FormatBytes(stream.Length), stream.Length);
            Console.WriteLine("    Architecture:  {0}", (image.Is64Bit ? "64-Bit" : "32-Bit"));
            Console.WriteLine("    Image Type:    {0}", image_type);
            Console.WriteLine("    Is .NET:       {0}", (image.IsCLR ? "Yes" : "No"));
            Console.WriteLine("    Is Signed:     {0}", (image.IsSigned ? "Yes" : "No"));

            Console.WriteLine();
            Console.WriteLine();

            return 0;
        }

        private int ShowDOSHeader(ExecutableImage image)
        {
            Console.WriteLine("[ MS-DOS Header ]");
            Console.WriteLine();

            Console.WriteLine("    File Offset:      {0}", Utils.IntToHex(image.DOSHeader.Location.FileOffset));
            Console.WriteLine("    Virtual Address:  {0}", Utils.IntToHex(image.DOSHeader.Location.VirtualAddress));
            Console.WriteLine("    RVA:              {0}", Utils.IntToHex(image.DOSHeader.Location.RelativeVirtualAddress));
            Console.WriteLine("    Size:             {1} ({0})", Utils.FormatBytes(Convert.ToInt64(image.DOSHeader.Location.FileSize)), image.DOSHeader.Location.FileSize);

            Console.WriteLine();

            List<Tuple<string, string, string>> tuples = new List<Tuple<string, string, string>>();
            ulong offset;

            if (offset_type == "fo")
            {
                offset = image.DOSHeader.Location.FileOffset;
            }
            else if (offset_type == "vo")
            {
                offset = 0;
            }
            else if (offset_type == "va")
            {
                offset = image.DOSHeader.Location.VirtualAddress;
            }
            else
            {
                offset = image.DOSHeader.Location.RelativeVirtualAddress;
            }

            int address_size = (image.Is64Bit ? 16 : 8);

            FieldAnnotations annotations = FieldAnnotations.Get(image.DOSHeader);

            foreach (FieldAnnotation annotation in annotations)
            {
                int size = annotation.Size;
                int array_size = (annotation.IsArray ? annotation.ArraySize : annotation.Size);
                object value = (annotation.IsArray ? Utils.GetDefaultValue(annotation.Type.GetElementType()) : annotation.Value);

                Tuple<string, string, string> tuple = new Tuple<string, string, string>(Utils.IntToHex(offset, address_size), Utils.IntToHex(value, size * 2), annotation.Description);

                tuples.Add(tuple);

                offset += Convert.ToUInt32(array_size);
            }

            int max_value_len = 0;
            int max_desc_len = 0;

            foreach(var tuple in tuples)
            {
                if (tuple.Item2.Length > max_value_len)
                    max_value_len = tuple.Item2.Length;

                if (tuple.Item3.Length > max_desc_len)
                    max_desc_len = tuple.Item3.Length;
            }

            string header = String.Format("{0}  {1}  {2}", "Address".PadRight(address_size + 2), "Value".PadRight(max_value_len), "Description".PadRight(max_desc_len));

            Console.WriteLine("    " + header);
            Console.WriteLine("    " + String.Empty.PadRight(header.Length, '-'));

            foreach (var tuple in tuples)
                Console.WriteLine("    {0}  {1}  {2}", tuple.Item1.PadRight(address_size + 2), tuple.Item2.PadRight(max_value_len), tuple.Item3.PadRight(max_desc_len));

            Console.WriteLine();
            Console.WriteLine();

            return 0;
        }

        private int ShowDOSStub(ExecutableImage image)
        {
            Console.WriteLine("[ MS-DOS Stub ]");
            Console.WriteLine();

            Console.WriteLine("    File Offset:      {0}", Utils.IntToHex(image.DOSStub.Location.FileOffset));
            Console.WriteLine("    Virtual Address:  {0}", Utils.IntToHex(image.DOSStub.Location.VirtualAddress));
            Console.WriteLine("    RVA:              {0}", Utils.IntToHex(image.DOSStub.Location.RelativeVirtualAddress));
            Console.WriteLine("    Size:             {1} ({0})", Utils.FormatBytes(Convert.ToInt64(image.DOSStub.Location.FileSize)), image.DOSStub.Location.FileSize);

            Console.WriteLine();
            Console.WriteLine();

            return 0;
        }

        private int ShowFileHeader(ExecutableImage image)
        {
            Console.WriteLine("[ File Header ]");
            Console.WriteLine();

            Console.WriteLine("    File Offset:      {0}", Utils.IntToHex(image.NTHeaders.FileHeader.Location.FileOffset));
            Console.WriteLine("    Virtual Address:  {0}", Utils.IntToHex(image.NTHeaders.FileHeader.Location.VirtualAddress));
            Console.WriteLine("    RVA:              {0}", Utils.IntToHex(image.NTHeaders.FileHeader.Location.RelativeVirtualAddress));
            Console.WriteLine("    Size:             {1} ({0})", Utils.FormatBytes(Convert.ToInt64(image.NTHeaders.FileHeader.Location.FileSize)), image.NTHeaders.FileHeader.Location.FileSize);

            Console.WriteLine();

            List<Tuple<string, string, string>> tuples = new List<Tuple<string, string, string>>();
            ulong offset;

            if (offset_type == "fo")
            {
                offset = image.NTHeaders.FileHeader.Location.FileOffset;
            }
            else if (offset_type == "vo")
            {
                offset = 0;
            }
            else if (offset_type == "va")
            {
                offset = image.NTHeaders.FileHeader.Location.VirtualAddress;
            }
            else
            {
                offset = image.NTHeaders.FileHeader.Location.RelativeVirtualAddress;
            }

            int address_size = (image.Is64Bit ? 16 : 8);

            FieldAnnotations annotations = FieldAnnotations.Get(image.NTHeaders.FileHeader);

            foreach (FieldAnnotation annotation in annotations)
            {
                int size = annotation.Size;
                int array_size = (annotation.IsArray ? annotation.ArraySize : annotation.Size);
                object value = (annotation.IsArray ? Utils.GetDefaultValue(annotation.Type.GetElementType()) : annotation.Value);

                Tuple<string, string, string> tuple = new Tuple<string, string, string>(Utils.IntToHex(offset, address_size), Utils.IntToHex(value, size * 2), annotation.Description);

                tuples.Add(tuple);

                offset += Convert.ToUInt32(array_size);
            }

            int max_value_len = 0;
            int max_desc_len = 0;

            foreach (var tuple in tuples)
            {
                if (tuple.Item2.Length > max_value_len)
                    max_value_len = tuple.Item2.Length;

                if (tuple.Item3.Length > max_desc_len)
                    max_desc_len = tuple.Item3.Length;
            }

            string header = String.Format("{0}  {1}  {2}", "Address".PadRight(address_size + 2), "Value".PadRight(max_value_len), "Description".PadRight(max_desc_len));

            Console.WriteLine("    " + header);
            Console.WriteLine("    " + String.Empty.PadRight(header.Length, '-'));

            foreach (var tuple in tuples)
                Console.WriteLine("    {0}  {1}  {2}", tuple.Item1.PadRight(address_size + 2), tuple.Item2.PadRight(max_value_len), tuple.Item3.PadRight(max_desc_len));

            Console.WriteLine();

            ShowFileHeader_Characteristics(image);

            Console.WriteLine();

            return 0;
        }

        private void ShowFileHeader_Characteristics(ExecutableImage image)
        {
            List<string> lines = new List<string>();

            CharacteristicsType characteristics = image.NTHeaders.FileHeader.GetCharacteristics();
            long enum_value = Convert.ToInt64(characteristics);           
            EnumAnnotations<CharacteristicsType> enum_annotations = new EnumAnnotations<CharacteristicsType>();

            foreach (EnumAnnotation<CharacteristicsType> annotation in enum_annotations)
            {
                long value = Convert.ToInt64(annotation.Value);
                bool selected = ((enum_value & value) == value);

                if (!selected)
                    continue;

                string line = String.Format("{0}  {1}", Utils.IntToHex(value, 4), annotation.HeaderName);

                lines.Add(line);
            }

            int max_len = 0;

            foreach (var line in lines)
            {
                if (line.Length > max_len)
                    max_len = line.Length;
            }

            max_len = max_len + 2;

            Console.WriteLine("    Characteristics".PadRight(max_len));
            Console.WriteLine("    " + String.Empty.PadRight(max_len, '-'));

            foreach(var line in lines)
                Console.WriteLine("    " + line.PadRight(max_len));

            Console.WriteLine();
        }

        private int ShowOptionalHeader(ExecutableImage image)
        {
            Console.WriteLine("[ Optional Header ]");
            Console.WriteLine();

            Console.WriteLine("    File Offset:      {0}", Utils.IntToHex(image.NTHeaders.OptionalHeader.Location.FileOffset));
            Console.WriteLine("    Virtual Address:  {0}", Utils.IntToHex(image.NTHeaders.OptionalHeader.Location.VirtualAddress));
            Console.WriteLine("    RVA:              {0}", Utils.IntToHex(image.NTHeaders.OptionalHeader.Location.RelativeVirtualAddress));
            Console.WriteLine("    Size:             {1} ({0})", Utils.FormatBytes(Convert.ToInt64(image.NTHeaders.OptionalHeader.Location.FileSize)), image.NTHeaders.OptionalHeader.Location.FileSize);
            Console.WriteLine();

            List<Tuple<string, string, string>> tuples = new List<Tuple<string, string, string>>();
            ulong offset;

            if (offset_type == "fo")
            {
                offset = image.NTHeaders.OptionalHeader.Location.FileOffset;
            }
            else if (offset_type == "vo")
            {
                offset = 0;
            }
            else if (offset_type == "va")
            {
                offset = image.NTHeaders.OptionalHeader.Location.VirtualAddress;
            }
            else
            {
                offset = image.NTHeaders.OptionalHeader.Location.RelativeVirtualAddress;
            }

            int address_size = (image.Is64Bit ? 16 : 8);

            string[] VARY_FIELDS = { "ImageBase", "SizeOfStackReserve", "SizeOfStackCommit", "SizeOfHeapReserve", "SizeOfHeapCommit" };
            FieldAnnotations annotations = FieldAnnotations.Get(image.NTHeaders.OptionalHeader);

            foreach (FieldAnnotation annotation in annotations)
            {
                int size = annotation.Size * 2;

                if (VARY_FIELDS.Contains(annotation.Name, StringComparer.OrdinalIgnoreCase))
                    size = (image.Is64Bit ? 16 : 8);

                int array_size = (annotation.IsArray ? annotation.ArraySize : annotation.Size);
                object value = (annotation.IsArray ? Utils.GetDefaultValue(annotation.Type.GetElementType()) : annotation.Value);

                Tuple<string, string, string> tuple = new Tuple<string, string, string>(Utils.IntToHex(offset, address_size), Utils.IntToHex(value, size), annotation.Description);

                tuples.Add(tuple);

                offset += Convert.ToUInt32(array_size);
            }

            int max_value_len = 0;
            int max_desc_len = 0;

            foreach (var tuple in tuples)
            {
                if (tuple.Item2.Length > max_value_len)
                    max_value_len = tuple.Item2.Length;

                if (tuple.Item3.Length > max_desc_len)
                    max_desc_len = tuple.Item3.Length;
            }

            string header = String.Format("{0}  {1}  {2}", "Address".PadRight(address_size + 2), "Value".PadRight(max_value_len), "Description".PadRight(max_desc_len));

            Console.WriteLine("    " + header);
            Console.WriteLine("    " + String.Empty.PadRight(header.Length, '-'));

            foreach (var tuple in tuples)
                Console.WriteLine("    {0}  {1}  {2}", tuple.Item1.PadRight(address_size + 2), tuple.Item2.PadRight(max_value_len), tuple.Item3.PadRight(max_desc_len));

            Console.WriteLine();

            ShowOptionalHeader_SubSystem(image);
            ShowOptionalHeader_DllCharacteristics(image);

            Console.WriteLine();

            return 0;
        }

        private void ShowOptionalHeader_SubSystem(ExecutableImage image)
        {
            List<string> lines = new List<string>();

            SubSystemType sub_system = image.NTHeaders.OptionalHeader.GetSubsystem();
            long enum_value = Convert.ToInt64(sub_system);
            EnumAnnotations<SubSystemType> enum_annotations = new EnumAnnotations<SubSystemType>();

            foreach (EnumAnnotation<SubSystemType> annotation in enum_annotations)
            {
                long value = Convert.ToInt64(annotation.Value);

                if (value == 0)
                    continue;

                bool selected = ((enum_value & value) == value);

                if (!selected)
                    continue;

                string line = String.Format("{0}  {1}", Utils.IntToHex(value, 4), annotation.HeaderName);

                lines.Add(line);
            }

            int max_len = 0;

            foreach (var line in lines)
            {
                if (line.Length > max_len)
                    max_len = line.Length;
            }

            max_len = max_len + 2;

            Console.WriteLine("    Sub-System".PadRight(max_len));
            Console.WriteLine("    " + String.Empty.PadRight(max_len, '-'));

            foreach (var line in lines)
                Console.WriteLine("    " + line.PadRight(max_len));

            Console.WriteLine();
        }

        private void ShowOptionalHeader_DllCharacteristics(ExecutableImage image)
        {
            List<string> lines = new List<string>();

            DllCharacteristicsType dll_chars = image.NTHeaders.OptionalHeader.GetDllCharacteristics();
            long enum_value = Convert.ToInt64(dll_chars);
            EnumAnnotations<DllCharacteristicsType> enum_annotations = new EnumAnnotations<DllCharacteristicsType>();

            foreach (EnumAnnotation<DllCharacteristicsType> annotation in enum_annotations)
            {
                long value = Convert.ToInt64(annotation.Value);

                if (value == 0)
                    continue;

                bool selected = ((enum_value & value) == value);

                if (!selected)
                    continue;

                string line = String.Format("{0}  {1}", Utils.IntToHex(value, 4), annotation.HeaderName);

                lines.Add(line);
            }

            int max_len = 0;

            foreach (var line in lines)
            {
                if (line.Length > max_len)
                    max_len = line.Length;
            }

            max_len = max_len + 2;

            Console.WriteLine("    DLL Characteristics".PadRight(max_len));
            Console.WriteLine("    " + String.Empty.PadRight(max_len, '-'));

            foreach (var line in lines)
                Console.WriteLine("    " + line.PadRight(max_len));

            Console.WriteLine();
        }

        private int ShowDataDirectories(ExecutableImage image)
        {
            Console.WriteLine("[ Data Directories ]");
            Console.WriteLine();

            Console.WriteLine("    File Offset:      {0}", Utils.IntToHex(image.NTHeaders.DataDirectories.Location.FileOffset));
            Console.WriteLine("    Virtual Address:  {0}", Utils.IntToHex(image.NTHeaders.DataDirectories.Location.VirtualAddress));
            Console.WriteLine("    RVA:              {0}", Utils.IntToHex(image.NTHeaders.DataDirectories.Location.RelativeVirtualAddress));
            Console.WriteLine("    Size:             {1} ({0})", Utils.FormatBytes(Convert.ToInt64(image.NTHeaders.DataDirectories.Location.FileSize)), image.NTHeaders.DataDirectories.Location.FileSize);
            Console.WriteLine();

            int address_size = (image.Is64Bit ? 16 : 8);
            ulong offset;

            if (offset_type == "fo")
            {
                offset = image.NTHeaders.DataDirectories.Location.FileOffset;
            }
            else if (offset_type == "vo")
            {
                offset = 0;
            }
            else if (offset_type == "va")
            {
                offset = image.NTHeaders.DataDirectories.Location.VirtualAddress;
            }
            else
            {
                offset = image.NTHeaders.DataDirectories.Location.RelativeVirtualAddress;
            }

            for(var i = 0; i < image.NTHeaders.DataDirectories.Count - 1; i++)
            {
                DataDirectory directory = image.NTHeaders.DataDirectories[i];
                string name;

                switch (directory.DirectoryType)
                {
                    case DataDirectoryType.Unknown:
                        name = String.Empty;
                        break;
                    case DataDirectoryType.ExportTable:
                        name = "Export Table";
                        break;
                    case DataDirectoryType.ImportTable:
                        name = "Import Table";
                        break;
                    case DataDirectoryType.ResourceTable:
                        name = "Resource Table";
                        break;
                    case DataDirectoryType.ExceptionTable:
                        name = "Exception Table";
                        break;
                    case DataDirectoryType.CertificateTable:
                        name = "Certificate Table";
                        break;
                    case DataDirectoryType.BaseRelocationTable:
                        name = "Base Relocation Table";
                        break;
                    case DataDirectoryType.Debug:
                        name = "Debug Directory";
                        break;
                    case DataDirectoryType.Architecture:
                        name = "Architecture";
                        break;
                    case DataDirectoryType.GlobalPtr:
                        name = "Global Pointer";
                        break;
                    case DataDirectoryType.TLSTable:
                        name = "TLS Table";
                        break;
                    case DataDirectoryType.LoadConfigTable:
                        name = "Load Configuration Table";
                        break;
                    case DataDirectoryType.BoundImport:
                        name = "Bound Import Table";
                        break;
                    case DataDirectoryType.ImportAddressTable:
                        name = "Import Address Table";
                        break;
                    case DataDirectoryType.DelayImportDescriptor:
                        name = "Delayed Import Descriptors";
                        break;
                    case DataDirectoryType.CLRRuntimeHeader:
                        name = ".NET Common Language Runtime Header";
                        break;
                    default:
                        name = "Unknown";
                        break;
                }

                List<string> lines = new List<string>();

                lines.Add(String.Format("Entry Offset:   {0}",Utils.IntToHex(offset, address_size)));
                lines.Add(String.Format("RVA:            {0}", Utils.IntToHex(directory.VirtualAddress)));
                lines.Add(String.Format("Size:           {0} ({1})", directory.Size, Utils.FormatBytes(directory.Size)));

                string section = directory.GetSectionName();

                if (!String.IsNullOrWhiteSpace(section))
                    lines.Add(String.Format("Section:        {0}", section));

                int max_len = name.Length;

                foreach(var line in lines)
                {
                    if (line.Length > max_len)
                        max_len = line.Length;
                }

                max_len = max_len + 2;

                Console.WriteLine("    " + name.PadRight(max_len));
                Console.WriteLine("    " + String.Empty.PadRight(max_len, '-'));

                foreach(var line in lines)
                    Console.WriteLine("    " + line.PadRight(max_len));

                Console.WriteLine();

                offset += sizeof(uint) * 2;
            }

            Console.WriteLine();

            return 0;
        }

        private int ShowSectionTable(ExecutableImage image)
        {
            Console.WriteLine("[ Section Table ]");
            Console.WriteLine();

            Console.WriteLine("    File Offset:      {0}", Utils.IntToHex(image.SectionTable.Location.FileOffset));
            Console.WriteLine("    Virtual Address:  {0}", Utils.IntToHex(image.SectionTable.Location.VirtualAddress));
            Console.WriteLine("    RVA:              {0}", Utils.IntToHex(image.SectionTable.Location.RelativeVirtualAddress));
            Console.WriteLine("    Size:             {1} ({0})", Utils.FormatBytes(Convert.ToInt64(image.SectionTable.Location.FileSize)), image.SectionTable.Location.FileSize);
            Console.WriteLine();

            int address_size = (image.Is64Bit ? 16 : 8);
            ulong offset;

            if (offset_type == "fo")
            {
                offset = image.SectionTable.Location.FileOffset;
            }
            else if (offset_type == "vo")
            {
                offset = 0;
            }
            else if (offset_type == "va")
            {
                offset = image.SectionTable.Location.VirtualAddress;
            }
            else
            {
                offset = image.SectionTable.Location.RelativeVirtualAddress;
            }

            foreach(SectionTableEntry section in image.SectionTable)
            {
                string name = section.Name;
                string[] chars = ShowSectionTable_Characteristics(section);
                List<string> lines = new List<string>();

                lines.Add(String.Format("Entry Offset:             {0}", Utils.IntToHex(offset, address_size)));
                lines.Add(String.Format("Virtual Size:             {0} ({1})", section.VirtualSizeOrPhysicalAddress, Utils.FormatBytes(section.VirtualSizeOrPhysicalAddress)));
                lines.Add(String.Format("RVA:                      {0}", Utils.IntToHex(section.VirtualAddress)));
                lines.Add(String.Format("Size of Raw Data:         {0} ({1})", section.SizeOfRawData, Utils.FormatBytes(section.SizeOfRawData)));
                lines.Add(String.Format("Pointer to Raw Data:      {0}", Utils.IntToHex(section.PointerToRawData)));
                lines.Add(String.Format("Pointer to Relocations:   {0}", Utils.IntToHex(section.PointerToRelocations)));
                lines.Add(String.Format("Pointer to Line Numbers:  {0}", Utils.IntToHex(section.PointerToLineNumbers)));
                lines.Add(String.Format("Number of Relocations:    {0}", section.NumberOfRelocations));
                lines.Add(String.Format("Number of Line Numbers:   {0}", section.NumberOfLineNumbers));

                if (chars.Length > 0)
                {
                    lines.Add(String.Format("Characteristics:          {0}", chars.First()));

                    if (chars.Length > 1)
                    {
                        foreach(string line in chars.Skip(1))
                            lines.Add(String.Format("                          {0}", line));
                    }
                }

                int max_len = name.Length;

                foreach (var line in lines)
                {
                    if (line.Length > max_len)
                        max_len = line.Length;
                }

                max_len = max_len + 2;

                Console.WriteLine("    " + name.PadRight(max_len));
                Console.WriteLine("    " + String.Empty.PadRight(max_len, '-'));

                foreach (var line in lines)
                    Console.WriteLine("    " + line.PadRight(max_len));

                Console.WriteLine();

                offset += section.Location.FileSize;
            }

            Console.WriteLine();

            return 0;
        }

        private string[] ShowSectionTable_Characteristics(SectionTableEntry section)
        {
            List<string> results = new List<string>();

            SectionCharacteristicsType chars = section.GetCharacteristics();
            long enum_value = Convert.ToInt64(chars);
            EnumAnnotations<SectionCharacteristicsType> enum_annotations = new EnumAnnotations<SectionCharacteristicsType>();

            foreach (EnumAnnotation<SectionCharacteristicsType> annotation in enum_annotations)
            {
                long value = Convert.ToInt64(annotation.Value);

                if (value == 0)
                    continue;

                bool selected = ((enum_value & value) == value);

                if (!selected)
                    continue;

                string line = String.Format("{0}  {1}", Utils.IntToHex(value, 8), annotation.HeaderName);

                results.Add(line);
            }

            int max_len = 0;

            foreach (var line in results)
            {
                if (line.Length > max_len)
                    max_len = line.Length;
            }

            return results.ToArray();
        }

        #endregion

    }

}
