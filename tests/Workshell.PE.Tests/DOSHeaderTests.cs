﻿using System;
using System.Collections.Generic;
using System.Text;

using FluentAssertions;
using NUnit.Framework;

namespace Workshell.PE.Tests
{
    [TestFixture]
    public sealed class DOSHeaderTests
    {
        [TestCase("clrtest.any.dll", 0x0090)]
        [TestCase("clrtest.x86.dll", 0x0090)]
        [TestCase("clrtest.x64.dll", 0x0090)]
        [TestCase("nativetest.x86.dll", 0x0050)]
        [TestCase("nativetest.x64.dll", 0x0050)]
        public void Bytes_On_Last_Page_Of_File_Is_Correct(string fileName, int expectedValue)
        {
            var file = Utils.GetFileFromResources(fileName);

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
            var file = Utils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.DOSHeader.FileAddressRelocationTable;

                value.Should().Be(Convert.ToUInt16(expectedValue));
            }
        }

        [TestCase("clrtest.any.dll", 0x00000080)]
        [TestCase("clrtest.x86.dll", 0x00000080)]
        [TestCase("clrtest.x64.dll", 0x00000080)]
        [TestCase("nativetest.x86.dll", 0x00000100)]
        [TestCase("nativetest.x64.dll", 0x00000100)]
        public void File_Address_New_Header_Is_Correct(string fileName, int expectedValue)
        {
            var file = Utils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.DOSHeader.FileAddressNewHeader;

                value.Should().Be(expectedValue);
            }
        }
    }
}
