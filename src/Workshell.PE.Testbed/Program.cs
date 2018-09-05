﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Workshell.PE.Content;
using Workshell.PE.Resources;
using Workshell.PE.Resources.Dialogs;
using Workshell.PE.Resources.Dialogs.Styles;
using Workshell.PE.Resources.Menus;
using Workshell.PE.Resources.Messages;
using Workshell.PE.Resources.Strings;

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
            var culture = CultureInfo.CurrentCulture;

            DialogResource.Register();
            MenuResource.Register();
            StringTableResource.Register();
            MessageTableResource.Register();

            //var image = await PortableExecutableImage.FromFileAsync(@"C:\Users\lkinsella\Downloads\IISCrypto.exe");
            //var image = await PortableExecutableImage.FromFileAsync(@"C:\Windows\System32\shell32.dll");
            var image = await PortableExecutableImage.FromFileAsync(@"C:\Windows\System32\en-GB\user32.dll.mui");
            var dataDirectory = image.NTHeaders.DataDirectories[DataDirectoryType.ResourceTable];
            var content = await dataDirectory.GetContentAsync().ConfigureAwait(false);
            var resources = await ResourceCollection.GetAsync(image);
            var messageResources = resources.FirstOrDefault(res => res.Id == ResourceType.MessageTable);
            var messageResource = (MessageTableResource)messageResources.FirstOrDefault();
            var table = messageResource.GetTable(ResourceLanguage.English.UnitedKingdom);
        }
    }
}
