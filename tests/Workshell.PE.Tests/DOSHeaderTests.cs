using System;
using System.Collections.Generic;
using System.Text;

using FluentAssertions;
using NUnit.Framework;

namespace Workshell.PE.Tests
{
    [TestFixture]
    public sealed class DOSHeaderTests
    {
        [Test]
        public void DOSHeader_Is_Not_Null()
        {
            var file = TestingUtils.GetFileFromResources(TestingUtils.GetRandomTestFilename());

            using (var image = PortableExecutableImage.FromStream(file))
            {
                image.DOSHeader.Should().NotBeNull();
            }
        }

        [TestCase("clrtest.any.dll", 0x0090)]
        [TestCase("clrtest.x86.dll", 0x0090)]
        [TestCase("clrtest.x64.dll", 0x0090)]
        [TestCase("nativetest.x86.dll", 0x0050)]
        [TestCase("nativetest.x64.dll", 0x0050)]
        public void Bytes_On_Last_Page_Of_File_Is_Correct(string fileName, int expectedValue)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.DOSHeader.BytesOnLastPage;

                value.Should().Be(Convert.ToUInt16(expectedValue));
            }
        }

        [TestCase("clrtest.any.dll", 0x0040)]
        [TestCase("clrtest.x86.dll", 0x0040)]
        [TestCase("clrtest.x64.dll", 0x0040)]
        [TestCase("nativetest.x86.dll", 0x0040)]
        [TestCase("nativetest.x64.dll", 0x0040)]
        public void File_Address_Relocation_Table_Is_Correct(string fileName, int expectedValue)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.DOSHeader.FileAddressRelocationTable;

                value.Should().Be(Convert.ToUInt16(expectedValue));
            }
        }

        [TestCase("clrtest.any.dll", 0x00000080U)]
        [TestCase("clrtest.x86.dll", 0x00000080U)]
        [TestCase("clrtest.x64.dll", 0x00000080U)]
        [TestCase("nativetest.x86.dll", 0x00000100U)]
        [TestCase("nativetest.x64.dll", 0x00000100U)]
        public void File_Address_New_Header_Is_Correct(string fileName, uint expectedValue)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.DOSHeader.FileAddressNewHeader;

                value.Should().Be(expectedValue);
            }
        }
    }
}
