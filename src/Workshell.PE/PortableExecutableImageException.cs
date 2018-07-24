using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE
{
    public sealed class PortableExecutableImageException : Exception
    {
        public PortableExecutableImageException(PortableExecutableImage image)
        {
            Image = image;
        }

        public PortableExecutableImageException(PortableExecutableImage image, string message) : base(message)
        {
            Image = image;
        }

        public PortableExecutableImageException(PortableExecutableImage image, string message, Exception innerException) : base(message, innerException)
        {
            Image = image;
        }

        #region Properties

        public PortableExecutableImage Image { get; }

        #endregion
    }
}
