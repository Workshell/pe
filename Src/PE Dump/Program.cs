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
        private string address_type;
        private bool show_all;
        private bool show_dos_header;
        private bool show_dos_stub;

        public Program()
        {
            options = new OptionSet();

            options.Add("address=", "", v => address_type = v);
            options.Add("all", "", v => show_all = v != null);
            options.Add("dos-header", "", v => show_dos_header = v != null);
            options.Add("dos-stub", "", v => show_dos_stub = v != null);

            address_type = "fo";
            show_all = false;
            show_dos_header = false;
            show_dos_stub = false;
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

                if (!address_types.Contains(address_type, StringComparer.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Error: Unknown address type - " + address_type);

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
                    }

                    int result = ShowBasicDetails(image, file_name);

                    if (result == 0 && show_dos_header)
                        result = ShowDOSHeader(image);

                    if (result == 0 && show_dos_stub)
                        result = ShowDOSStub(image);

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

            Console.WriteLine("    File Offset:     {0}", Utils.IntToHex(image.DOSHeader.Location.FileOffset));
            Console.WriteLine("    Virtual Address: {0}", Utils.IntToHex(image.DOSHeader.Location.VirtualAddress));
            Console.WriteLine("    RVA:             {0}", Utils.IntToHex(image.DOSHeader.Location.RelativeVirtualAddress));
            Console.WriteLine("    Size:            {1} ({0})", Utils.FormatBytes(Convert.ToInt64(image.DOSHeader.Location.FileSize)), image.DOSHeader.Location.FileSize);

            Console.WriteLine();

            List<Tuple<string, string, string>> tuples = new List<Tuple<string, string, string>>();
            ulong offset;

            if (address_type == "fo")
            {
                offset = image.DOSHeader.Location.FileOffset;
            }
            else if (address_type == "vo")
            {
                offset = 0;
            }
            else if (address_type == "va")
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

            string header = String.Format("{0}    {1}    {2}", "Address".PadRight(address_size + 2), "Value".PadRight(max_value_len), "Description".PadRight(max_desc_len));

            Console.WriteLine("    " + header);
            Console.WriteLine("    " + String.Empty.PadRight(header.Length, '-'));

            foreach (var tuple in tuples)
                Console.WriteLine("    {0}    {1}    {2}", tuple.Item1.PadRight(address_size + 2), tuple.Item2.PadRight(max_value_len), tuple.Item3.PadRight(max_desc_len));

            Console.WriteLine();
            Console.WriteLine();

            return 0;
        }

        private int ShowDOSStub(ExecutableImage image)
        {
            Console.WriteLine("[ MS-DOS Stub ]");
            Console.WriteLine();

            Console.WriteLine("    File Offset: {0}", Utils.IntToHex(image.DOSStub.Location.FileOffset));
            Console.WriteLine("    Virtual Address: {0}", Utils.IntToHex(image.DOSStub.Location.VirtualAddress));
            Console.WriteLine("    RVA: {0}", Utils.IntToHex(image.DOSStub.Location.RelativeVirtualAddress));
            Console.WriteLine("    Size: {1} ({0})", Utils.FormatBytes(Convert.ToInt64(image.DOSStub.Location.FileSize)), image.DOSStub.Location.FileSize);

            Console.WriteLine();
            Console.WriteLine();

            return 0;
        }

        #endregion

    }

}
