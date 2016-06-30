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
            //string file_name = @"C:\Windows\System32\kernel32.dll";
            string file_name = @"C:\Windows\SysWOW64\shell32.dll";
            //string file_name = @"C:\Windows\System32\shell32.dll";
            //string file_name = @"C:\Windows\SysWOW64\xpsservices.dll";
            //string file_name = @"c:\windows\system32\advapi32.dll";
            //string file_name = @"P:\Workshell\dotNET Dependency Walker\Bin\Debug\netdepends.exe";
            //string file_name = @"C:\Users\Lloyd\Desktop\PE Related\Tools\PeInternals\x64\PeInternals.exe";
            //string file_name = @"D:\Lloyd\Downloads\Win32DiskImager-0.9.5-install.exe";
            string error_message;

            if (!ExecutableImage.IsValid(file_name,out error_message))
            {
                Console.WriteLine("Invalid executable image: " + error_message);

                return;
            }

            ExecutableImage image = ExecutableImage.FromFile(file_name);
            Resources results = Resources.Get(image);

            Console.ReadKey();
        }

    }

}
