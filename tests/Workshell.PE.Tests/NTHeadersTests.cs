using System;
using System.Collections.Generic;
using System.Text;

using FluentAssertions;
using NUnit.Framework;

namespace Workshell.PE.Tests
{
    [TestFixture]
    public sealed class NTHeadersTests
    {
        [Test]
        public void NTHeaders_Is_Not_Null()
        {
            var file = Utils.GetFileFromResources(Utils.GetRandomTestFilename());

            using (var image = PortableExecutableImage.FromStream(file))
            {
                image.NTHeaders.Should().NotBeNull();
            }
        }
    }
}
