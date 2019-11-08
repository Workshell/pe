using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Workshell.PE.Tests
{
    internal static class Utils
    {
        private static readonly string[] _testFilenames = new string[]
        {
            "clrtest.any.dll",
            "clrtest.x86.dll",
            "clrtest.x64.dll",
            "nativetest.x86.dll",
            "nativetest.x64.dll"
        };
        private static readonly Random _random = new Random();

        #region Methods

        public static Stream GetFileFromResources(string fileName)
        {
            var path = $"Workshell.PE.Tests.Files.{fileName}";
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(path);

            return stream;
        }

        public static string GetRandomTestFilename()
        {
            var idx = _random.Next(0, _testFilenames.Length - 1);

            return _testFilenames[idx];
        }

        #endregion
    }
}
