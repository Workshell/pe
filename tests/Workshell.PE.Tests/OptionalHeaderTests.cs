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
        [TestCase("clrtest.any.dll", typeof(OptionalHeader32))]
        [TestCase("clrtest.x86.dll", typeof(OptionalHeader32))]
        [TestCase("clrtest.x64.dll", typeof(OptionalHeader64))]
        [TestCase("nativetest.x86.dll", typeof(OptionalHeader32))]
        [TestCase("nativetest.x64.dll", typeof(OptionalHeader64))]
        public void Type_Is_Correct(string fileName, Type type)
        {
            var file = Utils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.OptionalHeader;

                value.Should().BeOfType(type);
            }
        }

        [TestCase("clrtest.any.dll", 0x0090)]
        [TestCase("clrtest.x86.dll", 0x0090)]
        [TestCase("clrtest.x64.dll", 0x0090)]
        [TestCase("nativetest.x86.dll", 0x0050)]
        [TestCase("nativetest.x64.dll", 0x0050)]
        public void Magic_Is_Correct(string fileName, int expectedValue)
        {
            var file = Utils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var value = image.NTHeaders.OptionalHeader.Magic;

                value.Should().Be(Convert.ToUInt16(expectedValue));
            }
        }
    }
}
