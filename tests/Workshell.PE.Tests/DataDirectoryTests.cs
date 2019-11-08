using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FluentAssertions;
using NUnit.Framework;

namespace Workshell.PE.Tests
{
    [TestFixture]
    public sealed class DataDirectoryTests
    {
        [Test]
        public void DataDirectory_Is_Not_Null()
        {
            var file = Utils.GetFileFromResources(Utils.GetRandomTestFilename());

            using (var image = PortableExecutableImage.FromStream(file))
            {
                image.NTHeaders.DataDirectories.Should().NotBeNull();
            }
        }

        [Test]
        public void DataDirectory_Is_Not_Empty()
        {
            var file = Utils.GetFileFromResources(Utils.GetRandomTestFilename());

            using (var image = PortableExecutableImage.FromStream(file))
            {
                image.NTHeaders.DataDirectories.Count.Should().BeGreaterThan(0);
            }
        }

        [TestCase("clrtest.any.dll", false)]
        [TestCase("clrtest.x86.dll", false)]
        [TestCase("clrtest.x64.dll", false)]
        [TestCase("nativetest.x86.dll", true)]
        [TestCase("nativetest.x64.dll", true)]
        public void DataDirectory_ExportTable_Exists_Or_Not(string fileName, bool expectedValue)
        {
            DataDirectory_Exists(fileName, DataDirectoryType.ExportTable, expectedValue);
        }

        [TestCase("clrtest.any.dll", true)]
        [TestCase("clrtest.x86.dll", true)]
        [TestCase("clrtest.x64.dll", false)]
        [TestCase("nativetest.x86.dll", true)]
        [TestCase("nativetest.x64.dll", true)]
        public void DataDirectory_ImportTable_Exists_Or_Not(string fileName, bool expectedValue)
        {
            DataDirectory_Exists(fileName, DataDirectoryType.ImportTable, expectedValue);
        }

        [TestCase("clrtest.any.dll", true)]
        [TestCase("clrtest.x86.dll", true)]
        [TestCase("clrtest.x64.dll", true)]
        [TestCase("nativetest.x86.dll", true)]
        [TestCase("nativetest.x64.dll", true)]
        public void DataDirectory_ResourceTable_Exists_Or_Not(string fileName, bool expectedValue)
        {
            DataDirectory_Exists(fileName, DataDirectoryType.ResourceTable, expectedValue);
        }

        [TestCase("clrtest.any.dll", false)]
        [TestCase("clrtest.x86.dll", false)]
        [TestCase("clrtest.x64.dll", false)]
        [TestCase("nativetest.x86.dll", false)]
        [TestCase("nativetest.x64.dll", true)]
        public void DataDirectory_ExceptionTable_Exists_Or_Not(string fileName, bool expectedValue)
        {
            DataDirectory_Exists(fileName, DataDirectoryType.ExceptionTable, expectedValue);
        }

        [TestCase("clrtest.any.dll", false)]
        [TestCase("clrtest.x86.dll", false)]
        [TestCase("clrtest.x64.dll", false)]
        [TestCase("nativetest.x86.dll", false)]
        [TestCase("nativetest.x64.dll", false)]
        public void DataDirectory_CertificateTable_Exists_Or_Not(string fileName, bool expectedValue)
        {
            DataDirectory_Exists(fileName, DataDirectoryType.CertificateTable, expectedValue);
        }

        [TestCase("clrtest.any.dll", true)]
        [TestCase("clrtest.x86.dll", true)]
        [TestCase("clrtest.x64.dll", false)]
        [TestCase("nativetest.x86.dll", true)]
        [TestCase("nativetest.x64.dll", true)]
        public void DataDirectory_RelocationTable_Exists_Or_Not(string fileName, bool expectedValue)
        {
            DataDirectory_Exists(fileName, DataDirectoryType.BaseRelocationTable, expectedValue);
        }

        [TestCase("clrtest.any.dll", true)]
        [TestCase("clrtest.x86.dll", true)]
        [TestCase("clrtest.x64.dll", true)]
        [TestCase("nativetest.x86.dll", true)]
        [TestCase("nativetest.x64.dll", true)]
        public void DataDirectory_DebugDirectory_Exists_Or_Not(string fileName, bool expectedValue)
        {
            DataDirectory_Exists(fileName, DataDirectoryType.Debug, expectedValue);
        }

        [TestCase("clrtest.any.dll", false)]
        [TestCase("clrtest.x86.dll", false)]
        [TestCase("clrtest.x64.dll", false)]
        [TestCase("nativetest.x86.dll", false)]
        [TestCase("nativetest.x64.dll", false)]
        public void DataDirectory_Architecture_Exists_Or_Not(string fileName, bool expectedValue)
        {
            DataDirectory_Exists(fileName, DataDirectoryType.Architecture, expectedValue);
        }

        [TestCase("clrtest.any.dll", false)]
        [TestCase("clrtest.x86.dll", false)]
        [TestCase("clrtest.x64.dll", false)]
        [TestCase("nativetest.x86.dll", false)]
        [TestCase("nativetest.x64.dll", false)]
        public void DataDirectory_GlobalPtr_Exists_Or_Not(string fileName, bool expectedValue)
        {
            DataDirectory_Exists(fileName, DataDirectoryType.GlobalPtr, expectedValue);
        }

        [TestCase("clrtest.any.dll", false)]
        [TestCase("clrtest.x86.dll", false)]
        [TestCase("clrtest.x64.dll", false)]
        [TestCase("nativetest.x86.dll", false)]
        [TestCase("nativetest.x64.dll", false)]
        public void DataDirectory_TLSTable_Exists_Or_Not(string fileName, bool expectedValue)
        {
            DataDirectory_Exists(fileName, DataDirectoryType.TLSTable, expectedValue);
        }

        [TestCase("clrtest.any.dll", false)]
        [TestCase("clrtest.x86.dll", false)]
        [TestCase("clrtest.x64.dll", false)]
        [TestCase("nativetest.x86.dll", false)]
        [TestCase("nativetest.x64.dll", false)]
        public void DataDirectory_LoadConfigTable_Exists_Or_Not(string fileName, bool expectedValue)
        {
            DataDirectory_Exists(fileName, DataDirectoryType.LoadConfigTable, expectedValue);
        }

        [TestCase("clrtest.any.dll", false)]
        [TestCase("clrtest.x86.dll", false)]
        [TestCase("clrtest.x64.dll", false)]
        [TestCase("nativetest.x86.dll", false)]
        [TestCase("nativetest.x64.dll", false)]
        public void DataDirectory_BoundImportTable_Exists_Or_Not(string fileName, bool expectedValue)
        {
            DataDirectory_Exists(fileName, DataDirectoryType.BoundImport, expectedValue);
        }

        [TestCase("clrtest.any.dll", true)]
        [TestCase("clrtest.x86.dll", true)]
        [TestCase("clrtest.x64.dll", false)]
        [TestCase("nativetest.x86.dll", true)]
        [TestCase("nativetest.x64.dll", true)]
        public void DataDirectory_ImportAddressTable_Exists_Or_Not(string fileName, bool expectedValue)
        {
            DataDirectory_Exists(fileName, DataDirectoryType.ImportAddressTable, expectedValue);
        }

        [TestCase("clrtest.any.dll", false)]
        [TestCase("clrtest.x86.dll", false)]
        [TestCase("clrtest.x64.dll", false)]
        [TestCase("nativetest.x86.dll", true)]
        [TestCase("nativetest.x64.dll", true)]
        public void DataDirectory_DelayImportDescriptors_Exists_Or_Not(string fileName, bool expectedValue)
        {
            DataDirectory_Exists(fileName, DataDirectoryType.DelayImportDescriptor, expectedValue);
        }

        [TestCase("clrtest.any.dll", true)]
        [TestCase("clrtest.x86.dll", true)]
        [TestCase("clrtest.x64.dll", true)]
        [TestCase("nativetest.x86.dll", false)]
        [TestCase("nativetest.x64.dll", false)]
        public void DataDirectory_CLR_Exists_Or_Not(string fileName, bool expectedValue)
        {
            DataDirectory_Exists(fileName, DataDirectoryType.CLRRuntimeHeader, expectedValue);
        }

        [Test]
        public void DataDirectory_ExportTable_RVA_And_Size_Is_Correct()
        {
            var file = Utils.GetFileFromResources("nativetest.x64.dll");

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var directory = image.NTHeaders.DataDirectories.FirstOrDefault(e => e.DirectoryType == DataDirectoryType.ExportTable);

                directory.Should().NotBeNull();
                directory.VirtualAddress.Should().Be(0x00019000U);
                directory.Size.Should().Be(0x0000008FU);
            }
        }

        [Test]
        public void DataDirectory_ImportTable_RVA_And_Size_Is_Correct()
        {
            var file = Utils.GetFileFromResources("nativetest.x64.dll");

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var directory = image.NTHeaders.DataDirectories.FirstOrDefault(e => e.DirectoryType == DataDirectoryType.ImportTable);

                directory.Should().NotBeNull();
                directory.VirtualAddress.Should().Be(0x00017000U);
                directory.Size.Should().Be(0x0000054AU);
            }
        }

        [Test]
        public void DataDirectory_ResourceTable_RVA_And_Size_Is_Correct()
        {
            var file = Utils.GetFileFromResources("nativetest.x64.dll");

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var directory = image.NTHeaders.DataDirectories.FirstOrDefault(e => e.DirectoryType == DataDirectoryType.ResourceTable);

                directory.Should().NotBeNull();
                directory.VirtualAddress.Should().Be(0x0001D000U);
                directory.Size.Should().Be(0x00000400U);
            }
        }

        [Test]
        public void DataDirectory_ExceptionTable_RVA_And_Size_Is_Correct()
        {
            var file = Utils.GetFileFromResources("nativetest.x64.dll");

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var directory = image.NTHeaders.DataDirectories.FirstOrDefault(e => e.DirectoryType == DataDirectoryType.ExceptionTable);

                directory.Should().NotBeNull();
                directory.VirtualAddress.Should().Be(0x0001C000U);
                directory.Size.Should().Be(0x00000E10U);
            }
        }

        [Test]
        public void DataDirectory_RelocationTable_RVA_And_Size_Is_Correct()
        {
            var file = Utils.GetFileFromResources("nativetest.x64.dll");

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var directory = image.NTHeaders.DataDirectories.FirstOrDefault(e => e.DirectoryType == DataDirectoryType.BaseRelocationTable);

                directory.Should().NotBeNull();
                directory.VirtualAddress.Should().Be(0x0001B000U);
                directory.Size.Should().Be(0x00000590U);
            }
        }

        [Test]
        public void DataDirectory_DebugDirectory_RVA_And_Size_Is_Correct()
        {
            var file = Utils.GetFileFromResources("nativetest.x64.dll");

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var directory = image.NTHeaders.DataDirectories.FirstOrDefault(e => e.DirectoryType == DataDirectoryType.Debug);

                directory.Should().NotBeNull();
                directory.VirtualAddress.Should().Be(0x0001E000U);
                directory.Size.Should().Be(0x00000001U);
            }
        }

        [Test]
        public void DataDirectory_ImportAddressTable_RVA_And_Size_Is_Correct()
        {
            var file = Utils.GetFileFromResources("nativetest.x64.dll");

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var directory = image.NTHeaders.DataDirectories.FirstOrDefault(e => e.DirectoryType == DataDirectoryType.ImportAddressTable);

                directory.Should().NotBeNull();
                directory.VirtualAddress.Should().Be(0x00017180U);
                directory.Size.Should().Be(0x00000140U);
            }
        }

        [Test]
        public void DataDirectory_DelayImportDescriptors_RVA_And_Size_Is_Correct()
        {
            var file = Utils.GetFileFromResources("nativetest.x64.dll");

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var directory = image.NTHeaders.DataDirectories.FirstOrDefault(e => e.DirectoryType == DataDirectoryType.DelayImportDescriptor);

                directory.Should().NotBeNull();
                directory.VirtualAddress.Should().Be(0x00018000U);
                directory.Size.Should().Be(0x00000180U);
            }
        }


        private static void DataDirectory_Exists(string fileName, DataDirectoryType type, bool expectedValue)
        {
            var file = Utils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.DataDirectories.ExistsAndNotEmpty(type);

                value.Should().Be(expectedValue);
            }
        }
    }
}
