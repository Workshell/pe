using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    [Serializable]
    public class PortableExecutableException : Exception
    {

        public PortableExecutableException() : base()
        {
        }

        public PortableExecutableException(string message) : base(message)
        {
        }

        public PortableExecutableException(string message, Exception innerException) : base(message,innerException)
        {
        }

        protected PortableExecutableException(SerializationInfo info, StreamingContext context) : base(info,context)
        {
        }

    }

}
