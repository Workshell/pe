using System;
using System.Collections.Generic;
using System.Text;

using FluentAssertions;
using NUnit.Framework;

namespace Workshell.PE.Tests
{
    [TestFixture]
    public sealed class FileHeaderTests
    {
        [Test]
        public void FileHeader_Is_Not_Null()
        {
            var file = Utils.GetFileFromResources(Utils.GetRandomTestFilename());

            using (var image = PortableExecutableImage.FromStream(file))
            {
                image.NTHeaders.FileHeader.Should().NotBeNull();
            }
        }

        [TestCase("clrtest.any.dll", 0x014C)]
        [TestCase("clrtest.x86.dll", 0x014C)]
        [TestCase("clrtest.x64.dll", 0x8664)]
        [TestCase("nativetest.x86.dll", 0x014C)]
        [TestCase("nativetest.x64.dll", 0x8664)]
        public void Machine_Is_Correct(string fileName, int expectedValue)
        {
            var file = Utils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.FileHeader.Machine;

                value.Should().Be(Convert.ToUInt16(expectedValue));
            }
        }

        [TestCase("clrtest.any.dll", 0x0003)]
        [TestCase("clrtest.x86.dll", 0x0003)]
        [TestCase("clrtest.x64.dll", 0x0002)]
        [TestCase("nativetest.x86.dll", 0x000B)]
        [TestCase("nativetest.x64.dll", 0x000B)]
        public void NumberOfSections_Is_Correct(string fileName, int expectedValue)
        {
            var file = Utils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.FileHeader.NumberOfSections;

                value.Should().Be(Convert.ToUInt16(expectedValue));
            }
        }

        [TestCase("clrtest.any.dll", 0xD8FCD8BF)]
        [TestCase("clrtest.x86.dll", 0xC404A333)]
        [TestCase("clrtest.x64.dll", 0xE88B9424)]
        [TestCase("nativetest.x86.dll", 0x5DC4ACFF)]
        [TestCase("nativetest.x64.dll", 0x5DC4AD3D)]
        public void TimeDateStamp_Is_Correct(string fileName, long expectedValue)
        {
            var file = Utils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.FileHeader.TimeDateStamp;

                value.Should().Be(Convert.ToUInt32(expectedValue));
            }
        }

        [TestCase("clrtest.any.dll", 0x00E0)]
        [TestCase("clrtest.x86.dll", 0x00E0)]
        [TestCase("clrtest.x64.dll", 0x00F0)]
        [TestCase("nativetest.x86.dll", 0x00E0)]
        [TestCase("nativetest.x64.dll", 0x00F0)]
        public void SizeOfOptionalHeader_Is_Correct(string fileName, long expectedValue)
        {
            var file = Utils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.FileHeader.SizeOfOptionalHeader;

                value.Should().Be(Convert.ToUInt16(expectedValue));
            }
        }

        [TestCase("clrtest.any.dll", 0x2022)]
        [TestCase("clrtest.x86.dll", 0x2102)]
        [TestCase("clrtest.x64.dll", 0x2022)]
        [TestCase("nativetest.x86.dll", 0xA18E)]
        [TestCase("nativetest.x64.dll", 0x2022)]
        public void Characteristics_Is_Correct(string fileName, long expectedValue)
        {
            var file = Utils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.FileHeader.Characteristics;

                value.Should().Be(Convert.ToUInt16(expectedValue));
            }
        }
    }
}
