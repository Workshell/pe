using System;
using System.Collections.Generic;
using System.Text;

using FluentAssertions;
using NUnit.Framework;

namespace Workshell.PE.Tests
{
    [TestFixture]
    public sealed class OptionalHeaderTests
    {
        [Test]
        public void OptionalHeader_Is_Not_Null()
        {
            var file = TestingUtils.GetFileFromResources(TestingUtils.GetRandomTestFilename());

            using (var image = PortableExecutableImage.FromStream(file))
            {
                image.NTHeaders.OptionalHeader.Should().NotBeNull();
            }
        }

        [TestCase("clrtest.any.dll", typeof(OptionalHeader32))]
        [TestCase("clrtest.x86.dll", typeof(OptionalHeader32))]
        [TestCase("clrtest.x64.dll", typeof(OptionalHeader64))]
        [TestCase("nativetest.x86.dll", typeof(OptionalHeader32))]
        [TestCase("nativetest.x64.dll", typeof(OptionalHeader64))]
        public void Type_Is_Correct(string fileName, Type type)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.OptionalHeader;

                value.Should().BeOfType(type);
            }
        }

        [TestCase("clrtest.any.dll", 0x010B)]
        [TestCase("clrtest.x86.dll", 0x010B)]
        [TestCase("clrtest.x64.dll", 0x020B)]
        [TestCase("nativetest.x86.dll", 0x010B)]
        [TestCase("nativetest.x64.dll", 0x020B)]
        public void Magic_Is_Correct(string fileName, int expectedValue)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.OptionalHeader.Magic;

                value.Should().Be(Convert.ToUInt16(expectedValue));
            }
        }

        [TestCase("clrtest.any.dll", 0x30)]
        [TestCase("clrtest.x86.dll", 0x30)]
        [TestCase("clrtest.x64.dll", 0x30)]
        [TestCase("nativetest.x86.dll", 0x02)]
        [TestCase("nativetest.x64.dll", 0x08)]
        public void MajorLinkerVersion_Is_Correct(string fileName, int expectedValue)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.OptionalHeader.MajorLinkerVersion;

                value.Should().Be(Convert.ToByte(expectedValue));
            }
        }

        [TestCase("clrtest.any.dll", 0x00)]
        [TestCase("clrtest.x86.dll", 0x00)]
        [TestCase("clrtest.x64.dll", 0x00)]
        [TestCase("nativetest.x86.dll", 0x19)]
        [TestCase("nativetest.x64.dll", 0x00)]
        public void MinorLinkerVersion_Is_Correct(string fileName, int expectedValue)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.OptionalHeader.MinorLinkerVersion;

                value.Should().Be(Convert.ToByte(expectedValue));
            }
        }

        [TestCase("clrtest.any.dll", 0x00000800)]
        [TestCase("clrtest.x86.dll", 0x00000800)]
        [TestCase("clrtest.x64.dll", 0x00000800)]
        [TestCase("nativetest.x86.dll", 0x00008000)]
        [TestCase("nativetest.x64.dll", 0x0000C800)]
        public void SizeOfCode_Is_Correct(string fileName, int expectedValue)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.OptionalHeader.SizeOfCode;

                value.Should().Be(Convert.ToUInt32(expectedValue));
            }
        }

        [TestCase("clrtest.any.dll", 0x0000268E)]
        [TestCase("clrtest.x86.dll", 0x0000268E)]
        [TestCase("clrtest.x64.dll", 0x00000000)]
        [TestCase("nativetest.x86.dll", 0x000090E8)]
        [TestCase("nativetest.x64.dll", 0x0000CE10)]
        public void AddressOfEntryPoint_Is_Correct(string fileName, int expectedValue)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.OptionalHeader.AddressOfEntryPoint;

                value.Should().Be(Convert.ToUInt32(expectedValue));
            }
        }

        [TestCase("clrtest.any.dll", 0x00002000)]
        [TestCase("clrtest.x86.dll", 0x00002000)]
        [TestCase("clrtest.x64.dll", 0x00002000)]
        [TestCase("nativetest.x86.dll", 0x00001000)]
        [TestCase("nativetest.x64.dll", 0x00001000)]
        public void BaseOfCode_Is_Correct(string fileName, int expectedValue)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.OptionalHeader.BaseOfCode;

                value.Should().Be(Convert.ToUInt32(expectedValue));
            }
        }

        [TestCase("clrtest.any.dll", 0x00004000)]
        [TestCase("clrtest.x86.dll", 0x00004000)]
        [TestCase("clrtest.x64.dll", 0x00000000)]
        [TestCase("nativetest.x86.dll", 0x0000A000)]
        [TestCase("nativetest.x64.dll", 0x00000000)]
        public void BaseOfData_Is_Correct(string fileName, int expectedValue)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.OptionalHeader.BaseOfData;

                value.Should().Be(Convert.ToUInt32(expectedValue));
            }
        }

        [TestCase("clrtest.any.dll", 0x10000000UL)]
        [TestCase("clrtest.x86.dll", 0x10000000UL)]
        [TestCase("clrtest.x64.dll", 0x0000000180000000UL)]
        [TestCase("nativetest.x86.dll", 0x00400000UL)]
        [TestCase("nativetest.x64.dll", 0x0000000000400000UL)]
        public void ImageBase_Is_Correct(string fileName, ulong expectedValue)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.OptionalHeader.ImageBase;

                value.Should().Be(expectedValue);
            }
        }

        [TestCase("clrtest.any.dll", 0x00002000U)]
        [TestCase("clrtest.x86.dll", 0x00002000U)]
        [TestCase("clrtest.x64.dll", 0x00002000U)]
        [TestCase("nativetest.x86.dll", 0x00001000U)]
        [TestCase("nativetest.x64.dll", 0x00001000U)]
        public void SectionAlignment_Is_Correct(string fileName, uint expectedValue)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.OptionalHeader.SectionAlignment;

                value.Should().Be(expectedValue);
            }
        }

        [TestCase("clrtest.any.dll", 0x00000200U)]
        [TestCase("clrtest.x86.dll", 0x00000200U)]
        [TestCase("clrtest.x64.dll", 0x00000200U)]
        [TestCase("nativetest.x86.dll", 0x00000200U)]
        [TestCase("nativetest.x64.dll", 0x00000200U)]
        public void FileAlignment_Is_Correct(string fileName, uint expectedValue)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.OptionalHeader.FileAlignment;

                value.Should().Be(expectedValue);
            }
        }

        [TestCase("clrtest.any.dll", 0x00008000U)]
        [TestCase("clrtest.x86.dll", 0x00008000U)]
        [TestCase("clrtest.x64.dll", 0x00006000U)]
        [TestCase("nativetest.x86.dll", 0x00046000U)]
        [TestCase("nativetest.x64.dll", 0x00054000U)]
        public void SizeOfImage_Is_Correct(string fileName, uint expectedValue)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.OptionalHeader.SizeOfImage;

                value.Should().Be(expectedValue);
            }
        }

        [TestCase("clrtest.any.dll", 0x00000200U)]
        [TestCase("clrtest.x86.dll", 0x00000200U)]
        [TestCase("clrtest.x64.dll", 0x00000200U)]
        [TestCase("nativetest.x86.dll", 0x00000400U)]
        [TestCase("nativetest.x64.dll", 0x00000400U)]
        public void SizeOfHeaders_Is_Correct(string fileName, uint expectedValue)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.OptionalHeader.SizeOfHeaders;

                value.Should().Be(expectedValue);
            }
        }

        [TestCase("clrtest.any.dll", 0x0003)]
        [TestCase("clrtest.x86.dll", 0x0003)]
        [TestCase("clrtest.x64.dll", 0x0003)]
        [TestCase("nativetest.x86.dll", 0x0002)]
        [TestCase("nativetest.x64.dll", 0x0002)]
        public void Subsystem_Is_Correct(string fileName, int expectedValue)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.OptionalHeader.Subsystem;

                value.Should().Be(Convert.ToUInt16(expectedValue));
            }
        }

        [TestCase("clrtest.any.dll", 0x8540)]
        [TestCase("clrtest.x86.dll", 0x8540)]
        [TestCase("clrtest.x64.dll", 0x8540)]
        [TestCase("nativetest.x86.dll", 0x0000)]
        [TestCase("nativetest.x64.dll", 0x0000)]
        public void DllCharacteristics_Is_Correct(string fileName, int expectedValue)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.OptionalHeader.DllCharacteristics;

                value.Should().Be(Convert.ToUInt16(expectedValue));
            }
        }

        [Test]
        public void NumberOfRVA_Is_Correct()
        {
            var fileName = TestingUtils.GetRandomTestFilename();
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.OptionalHeader.NumberOfRvaAndSizes;

                value.Should().Be(16);
            }
        }
    }
}
