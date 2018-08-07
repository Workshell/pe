using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Workshell.PE.Content;

namespace Workshell.PE.Testbed
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync(args).GetAwaiter().GetResult();
        }

        static async Task RunAsync(string[] args)
        {
            //var image = await PortableExecutableImage.FromFileAsync(@"C:\Users\lkinsella\Downloads\IISCrypto.exe");
            var image = await PortableExecutableImage.FromFileAsync(@"C:\Windows\System32\shell32.dll");
            var dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.DelayImportDescriptor];
            var content = await dataDirectory.GetContentAsync().ConfigureAwait(false);

            var table = await ExceptionTable.GetAsync(image);
            var entries = table.Cast<ExceptionTableEntry64>().ToArray();
            var entry = entries[41];
            //var entry = entries[6506];
            var info = await entry.GetUnwindInfoAsync();

            //var table = await ImportHintNameTable.GetAsync(image).ConfigureAwait(false);
            //var entries = table.ToArray();
        }
    }
}
