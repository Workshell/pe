using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FluentAssertions;
using NUnit.Framework;

namespace Workshell.PE.Tests
{
    [TestFixture]
    public sealed class SectionTableTests
    {
        [Test]
        public void SectionTable_Is_Not_Null()
        {
            var file = TestingUtils.GetFileFromResources(TestingUtils.GetRandomTestFilename());

            using (var image = PortableExecutableImage.FromStream(file))
            {
                image.SectionTable.Should().NotBeNull();
            }
        }

        [TestCase("clrtest.any.dll", new [] { ".text", ".rsrc", ".reloc" })]
        [TestCase("clrtest.x86.dll", new [] { ".text", ".rsrc", ".reloc" })]
        [TestCase("clrtest.x64.dll", new [] { ".text", ".rsrc" })]
        [TestCase("nativetest.x86.dll", new [] { ".text", ".itext", ".data", ".bss", ".idata", ".didata", ".edata", ".rdata", ".reloc", ".rsrc", ".debug" })]
        [TestCase("nativetest.x64.dll", new[] { ".text", ".data", ".bss", ".idata", ".didata", ".edata", ".rdata", ".reloc", ".pdata", ".rsrc", ".debug" })]
        public void SectionTable_Is_Has_Correct_Sections_Count(string fileName, string[] expectedSections)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                image.SectionTable.Count.Should().Be(expectedSections.Length);
            }
        }

        [TestCase("clrtest.any.dll", new[] { ".text", ".rsrc", ".reloc" })]
        [TestCase("clrtest.x86.dll", new[] { ".text", ".rsrc", ".reloc" })]
        [TestCase("clrtest.x64.dll", new[] { ".text", ".rsrc" })]
        [TestCase("nativetest.x86.dll", new[] { ".text", ".itext", ".data", ".bss", ".idata", ".didata", ".edata", ".rdata", ".reloc", ".rsrc", ".debug" })]
        [TestCase("nativetest.x64.dll", new[] { ".text", ".data", ".bss", ".idata", ".didata", ".edata", ".rdata", ".reloc", ".pdata", ".rsrc", ".debug" })]
        public void SectionTable_Is_Has_Expected_Sections(string fileName, string[] expectedSections)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var sections = image.SectionTable.Select(s => s.Name)
                    .OrderBy(s => s)
                    .ToArray();
                var sortedExpectedSections = expectedSections.OrderBy(s => s)
                    .ToArray();

                sections.Should().BeEquivalentTo(sortedExpectedSections);
            }
        }

        [TestCase(".text", 0x00007DC0U, 0x00001000U, 0x00007E00U, 0x00000400U, 0x60000020U)]
        [TestCase(".itext", 0x00000100U, 0x00009000U, 0x00000200U, 0x00008200U, 0x60000020U)]
        [TestCase(".data", 0x0000083CU, 0x0000A000U, 0x00000A00U, 0x00008400U, 0xC0000040U)]
        [TestCase(".bss", 0x0000358CU, 0x0000B000U, 0x00000000U, 0x00000000U, 0xC0000000U)]
        [TestCase(".idata", 0x000003F0U, 0x0000F000U, 0x00000400U, 0x00008E00U, 0xC0000040U)]
        [TestCase(".didata", 0x00000124U, 0x00010000U, 0x00000200U, 0x00009200U, 0xC0000040U)]
        [TestCase(".edata", 0x0000008FU, 0x00011000U, 0x00000200U, 0x00009400U, 0x40000040U)]
        [TestCase(".rdata", 0x00000045U, 0x00012000U, 0x00000200U, 0x00009600U, 0x40000040U)]
        [TestCase(".reloc", 0x00000988U, 0x00013000U, 0x00000A00U, 0x00009800U, 0x42000040U)]
        [TestCase(".rsrc", 0x00000400U, 0x00014000U, 0x00000400U, 0x0000A200U, 0x40000040U)]
        [TestCase(".debug", 0x00030DB0U, 0x00015000U, 0x0030DB0U, 0x0000A600U, 0x40000040U)]
        public void SectionTable_Entry_Has_Expected_Values(string sectionName, uint virtualSize, uint rva, uint sizeRaw, uint pointerRaw, uint characteristics)
        {
            var file = TestingUtils.GetFileFromResources("nativetest.x86.dll");

            using (var image = PortableExecutableImage.FromStream(file))
            {
                var section = image.SectionTable.FirstOrDefault(s => string.Compare(sectionName, s.Name, StringComparison.OrdinalIgnoreCase) == 0);

                section.Should().NotBeNull();
                section.VirtualSizeOrPhysicalAddress.Should().Be(virtualSize);
                section.VirtualAddress.Should().Be(rva);
                section.SizeOfRawData.Should().Be(sizeRaw);
                section.PointerToRawData.Should().Be(pointerRaw);
                section.Characteristics.Should().Be(characteristics);
            }
        }
    }
}
