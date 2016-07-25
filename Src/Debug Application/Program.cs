using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Workshell.PE;
using Workshell.PE.Imports;
using Workshell.PE.Resources;

namespace Workshell.PE.Demo
{

    class Program
    {

        static void Main(string[] args)
        {
            //string file_name = Environment.GetCommandLineArgs()[0];
            //string file_name = Assembly.GetEntryAssembly().Location;
            //string file_name = @"C:\Windows\SysWOW64\kernel32.dll";
            //string file_name = @"C:\Windows\System32\kernel32.dll";
            //string file_name = @"C:\Windows\SysWOW64\shell32.dll";
            string file_name = @"C:\Windows\System32\shell32.dll";
            //string file_name = @"C:\Windows\System32\user32.dll";
            //string file_name = @"C:\Windows\explorer.exe";
            //string file_name = @"C:\Windows\SysWOW64\xpsservices.dll";
            //string file_name = @"c:\windows\system32\advapi32.dll";
            //string file_name = @"C:\Program Files (x86)\Notepad++\notepad++.exe";
            //string file_name = @"C:\Windows\WinSxS\x86_microsoft-windows-notepad.resources_31bf3856ad364e35_6.3.9600.16384_en-gb_aafce17aef6b0109\notepad.exe.mui";
            string error_message;

            if (!ExecutableImage.IsValid(file_name,out error_message))
            {
                Console.WriteLine("Invalid executable image: " + error_message);

                return;
            }

            ExecutableImage image = ExecutableImage.FromFile(file_name);
            DelayImportDirectory directory = DelayImportDirectory.Get(image);
            DelayImportAddressTables tables = DelayImportAddressTables.GetAddressTable(directory);


        }

    }

}
