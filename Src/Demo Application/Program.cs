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
            string file_name = Environment.GetCommandLineArgs()[0];
            ExeReader reader = ExeReader.FromFile(file_name);
            Section[] sections = reader.Sections.ToArray();

        }

    }

}
