using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    [Serializable]
    public class ExeReaderException : Exception
    {

        public ExeReaderException() : base()
        {
        }

        public ExeReaderException(string message) : base(message)
        {
        }

        public ExeReaderException(string message, Exception innerException) : base(message,innerException)
        {
        }

        protected ExeReaderException(SerializationInfo info, StreamingContext context) : base(info,context)
        {
        }

    }

}
