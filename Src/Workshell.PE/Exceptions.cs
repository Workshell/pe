using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    [Serializable]
    public class ExecutableImageException : Exception
    {

        public ExecutableImageException() : base()
        {
        }

        public ExecutableImageException(string message) : base(message)
        {
        }

        public ExecutableImageException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ExecutableImageException(SerializationInfo info, StreamingContext context) : base(info, context)
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
