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
using Workshell.PE.Relocations;

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
            //string file_name = @"C:\Windows\System32\shell32.dll";
            //string file_name = @"C:\Windows\System32\user32.dll";
            //string file_name = @"C:\Windows\explorer.exe";
            //string file_name = @"C:\Windows\SysWOW64\xpsservices.dll";
            //string file_name = @"c:\windows\system32\advapi32.dll";
            //string file_name = @"C:\Program Files (x86)\Notepad++\notepad++.exe";
            string file_name = @"C:\Windows\System32\en-GB\shell32.dll.mui";
            string error_message;

            if (!ExecutableImage.IsValid(file_name,out error_message))
            {
                Console.WriteLine("Invalid executable image: " + error_message);

                return;
            }

            MessageTableResource.Register();

            ExecutableImage image = ExecutableImage.FromFile(file_name);
            LocationCalculator calc = image.GetCalculator();

            ResourceCollection resources = ResourceCollection.Get(image);
            ResourceType types = resources.FirstOrDefault(t => t.Id == ResourceType.RT_MESSAGETABLE);
            Resource resource = types.FirstOrDefault(r => r.Id == 1);
            MessageTableResource message_table_resource = resource as MessageTableResource;
            MessageTable message_table = message_table_resource.ToTable(2057);

        }

    }

}
