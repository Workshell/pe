using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Resources.Version
{
    internal sealed class FileInfo
    {
        public ushort Length { get; set; }
        public ushort ValueLength { get; set; }
        public ushort Type { get; set; }
        public string Key { get; set; }
    }
}
