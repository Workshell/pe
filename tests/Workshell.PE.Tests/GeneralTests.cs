using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Workshell.PE.Tests
{
    [TestFixture]
    public sealed class GeneralTests
    {
        [Test]
        public void IsValid_With_Text_File_Returns_False()
        {
            var file = TestingUtils.GetFileFromResources("license.txt");
            var isValid = PortableExecutableImage.IsValid(file);

            isValid.Should().BeFalse();
        }

        [Test]
        public void IsValid_With_Executable_File_Returns_True()
        {
            var file = TestingUtils.GetFileFromResources("clrtest.any.dll");
            var isValid = PortableExecutableImage.IsValid(file);

            isValid.Should().BeTrue();
        }

        [Test]
        public void GetStream_Returns_Successfully()
        {
            var file = TestingUtils.GetFileFromResources("clrtest.any.dll");

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var stream = image.GetStream();

                stream.Should().NotBeNull();
            }
        }

        [Test]
        public void GetCalculator_Returns_Successfully()
        {
            var file = TestingUtils.GetFileFromResources("clrtest.any.dll");

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var calc = image.GetCalculator();

                calc.Should().NotBeNull();
            }
        }

        [Test]
        public void Is32Bit_With_32Bit_Image_Returns_True()
        {
            var file = TestingUtils.GetFileFromResources("clrtest.x86.dll");

            using (var image = PortableExecutableImage.FromStream(file))
            {
                image.Is32Bit.Should().BeTrue();
            }
        }

        [Test]
        public void Is32Bit_With_64Bit_Image_Returns_False()
        {
            var file = TestingUtils.GetFileFromResources("clrtest.x64.dll");

            using (var image = PortableExecutableImage.FromStream(file))
            {
                image.Is32Bit.Should().BeFalse();
            }
        }

        [Test]
        public void Is64Bit_With_64Bit_Image_Returns_True()
        {
            var file = TestingUtils.GetFileFromResources("clrtest.x64.dll");

            using (var image = PortableExecutableImage.FromStream(file))
            {
                image.Is64Bit.Should().BeTrue();
            }
        }

        [Test]
        public void Is64Bit_With_32Bit_Image_Returns_False()
        {
            var file = TestingUtils.GetFileFromResources("clrtest.x86.dll");

            using (var image = PortableExecutableImage.FromStream(file))
            {
                image.Is64Bit.Should().BeFalse();
            }
        }

        [Test]
        public void IsCLR_With_CLR_Image_Returns_True()
        {
            var file = TestingUtils.GetFileFromResources("clrtest.any.dll");

            using (var image = PortableExecutableImage.FromStream(file))
            {
                image.IsCLR.Should().BeTrue();
            }
        }

        [Test]
        public void IsCLR_With_Native_Image_Returns_False()
        {
            var file = TestingUtils.GetFileFromResources("nativetest.x86.dll");

            using (var image = PortableExecutableImage.FromStream(file))
            {
                image.IsCLR.Should().BeFalse();
            }
        }

        [Test]
        public void IsSigned_Returns_False()
        {
            var file = TestingUtils.GetFileFromResources(TestingUtils.GetRandomTestFilename());

            using (var image = PortableExecutableImage.FromStream(file))
            {
                image.IsSigned.Should().BeFalse();
            }
        }

        [Test]
        public void DOSStub_Is_Not_Null()
        {
            var file = TestingUtils.GetFileFromResources(TestingUtils.GetRandomTestFilename());

            using (var image = PortableExecutableImage.FromStream(file))
            {
                image.DOSStub.Should().NotBeNull();
            }
        }
    }
}
