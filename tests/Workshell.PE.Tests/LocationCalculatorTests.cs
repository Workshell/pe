using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
