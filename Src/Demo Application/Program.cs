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
            string error_message;

            if (!ExecutableImage.IsValid(file_name,out error_message))
            {
                Console.WriteLine("Invalid executable image: " + error_message);

                return;
            }

            ExecutableImage image = ExecutableImage.FromFile(file_name);
            Resources resources = Resources.Get(image);
            ResourceType cursor_groups = resources.FirstOrDefault(type => type.Id == ResourceType.RT_GROUP_CURSOR);
            Resource group_resource = cursor_groups.FirstOrDefault(res => res.Id == 1001);
            CursorGroupResource cursor_group = CursorGroupResource.FromResource(group_resource, Resource.DEFAULT_LANGUAGE);
            CursorGroupResourceEntry cursor_entry = cursor_group.FirstOrDefault();
            ResourceType cursors = resources.FirstOrDefault(type => type.Id == ResourceType.RT_CURSOR);
            Resource resource = cursors.FirstOrDefault(res => res.Id == cursor_entry.CursorId);
            CursorResource cursor_resource = CursorResource.FromResource(resource, Resource.DEFAULT_LANGUAGE);
            Cursor cursor = cursor_resource.ToCursor();
            Bitmap bitmap = cursor_resource.ToBitmap();

            bitmap.Save(@"d:\test.bmp");

            Console.ReadKey();
        }

    }

}
