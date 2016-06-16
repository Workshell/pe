using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public interface IMemoryStreamProvider
    {

        MemoryStream GetStream();
        MemoryStream GetStream(byte[] buffer);

    }

    public sealed class DefaultMemoryStreamProvider : IMemoryStreamProvider
    {

        public MemoryStream GetStream()
        {
            return new MemoryStream();
        }

        public MemoryStream GetStream(byte[] buffer)
        {
            return new MemoryStream(buffer);
        }

    }

}
