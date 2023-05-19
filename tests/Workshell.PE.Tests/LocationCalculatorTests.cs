using System;
using System.Collections.Generic;
using System.Text;

using FluentAssertions;
using NUnit.Framework;

namespace Workshell.PE.Tests
{
    [TestFixture]
    public sealed class LocationCalculatorTests
    {
        private PortableExecutableImage _image;
        private LocationCalculator _calc;

        [OneTimeSetUp]
        public void SetUp()
        {
            _image = PortableExecutableImage.FromStream(TestingUtils.GetFileFromResources("nativetest.x64.dll"));
            _calc = _image.GetCalculator();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _calc = null;
            _image.Dispose();
        }

        [Test]
        public void OffsetToLocation_Returns_Location()
        {
            var output = _calc.OffsetToLocation(0x000000000000F250, 14);

            output.Should().NotBeNull();
        }

        [Test]
        public void OffsetToLocation_Returns_Correct_Location()
        {
            var output = _calc.OffsetToLocation(0x000000000000F250, 14);

            output.FileOffset.Should().Be(0x000000000000F250);
            output.FileSize.Should().Be(14);
            output.VirtualAddress.Should().Be(0x0000000000419050);
            output.VirtualSize.Should().Be(14);
            output.RelativeVirtualAddress.Should().Be(0x00019050);
            output.Section.Should().NotBeNull();
            output.Section.Name.Should().Be(".edata");
        }

        [Test]
        public void VAToSection_Outside_Of_Section_Returns_Null()
        {
            var output = _calc.VAToSection(0x00000050);

            output.Should().BeNull();
        }

        [Test]
        public void VAToSection_Returns_Correct_Section()
        {
            var output = _calc.VAToSection(0x0000000000419050);

            output.Should().NotBeNull();
            output.Name.Should().Be(".edata");
        }

        [Test]
        public void VAToSectionTableEntry_Outside_Of_Section_Returns_Null()
        {
            var output = _calc.VAToSectionTableEntry(0x00000050);

            output.Should().BeNull();
        }

        [Test]
        public void VAToSectionTableEntry_Returns_Correct_Section()
        {
            var output = _calc.VAToSectionTableEntry(0x0000000000419050);

            output.Should().NotBeNull();
            output.Name.Should().Be(".edata");
        }

        [Test]
        public void VAToOffset_Returns_Correct_Value()
        {
            var output = _calc.VAToOffset(0x0000000000419050);

            output.Should().Be(0x000000000000F250);
        }

        [Test]
        public void OffsetToVA_Returns_Correct_Value()
        {
            var output = _calc.OffsetToVA(0x000000000000F250);

            output.Should().Be(0x0000000000419050);
        }

        [Test]
        public void RVAToSection_Outside_Of_Section_Returns_Null()
        {
            var output = _calc.RVAToSection(0x00000050);

            output.Should().BeNull();
        }

        [Test]
        public void RVAToSection_Returns_Correct_Section()
        {
            var output = _calc.RVAToSection(0x00019050);

            output.Should().NotBeNull();
            output.Name.Should().Be(".edata");
        }

        [Test]
        public void RVAToSectionTableEntry_Outside_Of_Section_Returns_Null()
        {
            var output = _calc.RVAToSectionTableEntry(0x00000050);

            output.Should().BeNull();
        }

        [Test]
        public void RVAToSectionTableEntry_Returns_Correct_Section()
        {
            var output = _calc.RVAToSectionTableEntry(0x00019050);

            output.Should().NotBeNull();
            output.Name.Should().Be(".edata");
        }

        [Test]
        public void RVAToOffset_Returns_Correct_Value()
        {
            var output = _calc.RVAToOffset(0x00019050);

            output.Should().Be(0x000000000000F250);
        }

        [Test]
        public void OffsetToRVA_Returns_Correct_Value()
        {
            var output = _calc.OffsetToRVA(0x000000000000F250);

            output.Should().Be(0x00019050);
        }
    }
}
