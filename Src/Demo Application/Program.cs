using System;
using System.Collections.Generic;
using System.Linq;
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
            //string file_name = @"C:\Windows\SysWOW64\kernel32.dll";
            //string file_name = @"C:\Windows\SysWOW64\shell32.dll";
            //string file_name = @"C:\Windows\SysWOW64\xpsservices.dll";
            //string file_name = @"c:\windows\system32\advapi32.dll";
            //string file_name = @"P:\Workshell\dotNET Dependency Walker\Bin\Release\netdepends.exe";
            string file_name = @"C:\Windows\System32\shell32.dll";
            ExeReader reader = ExeReader.FromFile(file_name);
            Section[] sections = reader.Sections.ToArray();

            foreach(Section section in sections)
            {
                TLSContent content = (TLSContent)section[DataDirectoryType.TLSTable];

                if (content != null)
                {

                }
            }

            Console.ReadKey();
        }

    }

}
