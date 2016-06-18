using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE;

namespace Workshell.PE.Demo
{

    class Program
    {

        static void Main(string[] args)
        {
            //string file_name = Environment.GetCommandLineArgs()[0];
            //string file_name = Assembly.GetEntryAssembly().Location;
            //string file_name = @"C:\Windows\SysWOW64\kernel32.dll";
            string file_name = @"C:\Windows\System32\kernel32.dll";
            //string file_name = @"C:\Windows\SysWOW64\shell32.dll";
            //string file_name = @"C:\Windows\System32\shell32.dll";
            //string file_name = @"C:\Windows\SysWOW64\xpsservices.dll";
            //string file_name = @"c:\windows\system32\advapi32.dll";
            //string file_name = @"P:\Workshell\dotNET Dependency Walker\Bin\Release\netdepends.exe";
            //string file_name = @"C:\Users\Lloyd\Desktop\PE Related\Tools\PeInternals\x64\PeInternals.exe";
            //string file_name = @"D:\Lloyd\Downloads\Win32DiskImager-0.9.5-install.exe";
            string error_message;

            if (!ExecutableImage.IsValid(file_name,out error_message))
            {
                Console.WriteLine("Invalid executable image: " + error_message);

                return;
            }

            ExecutableImage image = ExecutableImage.FromFile(file_name);

            LoadConfigDirectory load_config = LoadConfigDirectory.Get(image);
            Certificate cert = Certificate.Get(image);
            DebugDirectory debug_dir = DebugDirectory.Get(image);
            TLSDirectory tls_dir = TLSDirectory.Get(image);

            ExportDirectory export_dir = ExportDirectory.Get(image);
            ExportTable<uint> function_addresses = ExportTable<uint>.GetFunctionAddressTable(export_dir);
            ExportTable<uint> name_addresses = ExportTable<uint>.GetNameAddressTable(export_dir);
            ExportTable<ushort> ordinals = ExportTable<ushort>.GetOrdinalTable(export_dir);
            Exports exports = Exports.Get(export_dir, function_addresses, name_addresses, ordinals);

            ImportDirectory import_dir = ImportDirectory.Get(image);
            ImportAddressTables ilt = ImportAddressTables.GetLookupTable(import_dir);
            ImportAddressTables iat = ImportAddressTables.GetAddressTable(import_dir);
            ImportHintNameTable hint_name_table = ImportHintNameTable.Get(import_dir);
            Imports imports = Imports.Get(ilt,hint_name_table);

            Console.ReadKey();
        }

    }

}
