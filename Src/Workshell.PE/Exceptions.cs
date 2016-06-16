﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    [Serializable]
    public class ImageReaderException : Exception
    {

        public ImageReaderException() : base()
        {
        }

        public ImageReaderException(string message) : base(message)
        {
        }

        public ImageReaderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ImageReaderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

    }

    [Serializable]
    public class DataDirectoryException : Exception
    {

        public DataDirectoryException() : base()
        {
        }

        public DataDirectoryException(string message) : base(message)
        {
        }

        public DataDirectoryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DataDirectoryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

    }

}
