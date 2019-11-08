using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FluentAssertions;
using NUnit.Framework;

namespace Workshell.PE.Tests
{
    [TestFixture]
    public sealed class SectionTests
    {
        [TestCase("clrtest.any.dll")]
        [TestCase("clrtest.x86.dll")]
        [TestCase("clrtest.x64.dll")]
        [TestCase("nativetest.x86.dll")]
        [TestCase("nativetest.x64.dll")]
        public void Sections_Is_Not_Null(string fileName)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                image.Sections.Should().NotBeNull();
            }
        }

        [TestCase("clrtest.any.dll")]
        [TestCase("clrtest.x86.dll")]
        [TestCase("clrtest.x64.dll")]
        [TestCase("nativetest.x86.dll")]
        [TestCase("nativetest.x64.dll")]
        public void Sections_Has_Expected_Count(string fileName)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                image.Sections.Count.Should().Be(image.SectionTable.Count);
            }
        }

        [TestCase("clrtest.any.dll")]
        [TestCase("clrtest.x86.dll")]
        [TestCase("clrtest.x64.dll")]
        [TestCase("nativetest.x86.dll")]
        [TestCase("nativetest.x64.dll")]
        public void Section_Has_Expected_Offset(string fileName)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                foreach (var section in image.Sections)
                {
                    section.Location.FileOffset.Should().Be(section.TableEntry.PointerToRawData);
                }
            }
        }

        [TestCase("clrtest.any.dll")]
        [TestCase("clrtest.x86.dll")]
        [TestCase("clrtest.x64.dll")]
        [TestCase("nativetest.x86.dll")]
        [TestCase("nativetest.x64.dll")]
        public void Section_Has_Expected_Size(string fileName)
        {
            var file = TestingUtils.GetFileFromResources(fileName);

            using (var image = PortableExecutableImage.FromStream(file))
            {
                foreach (var section in image.Sections)
                {
                    var buffer = section.GetBytes();

                    buffer.LongLength.Should().Be(Convert.ToInt64(section.TableEntry.SizeOfRawData));
                }
            }
        }
    }
}
