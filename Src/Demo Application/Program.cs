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
            string file_name = @"C:\Windows\SysWOW64\kernel32.dll";
            ExeReader reader = ExeReader.FromFile(file_name);
            Section[] sections = reader.Sections.ToArray();

            foreach(Section section in sections)
            {
                ExportContent exports = (ExportContent)section[DataDirectoryType.ExportTable];

                if (exports == null)
                    continue;

                string name = exports.Directory.GetName();
                uint[] function_addresses = exports.Directory.GetFunctionAddresses();
            }
        }

    }

}
