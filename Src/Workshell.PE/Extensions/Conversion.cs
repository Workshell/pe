using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Extensions
{

    internal static class ConversionExtensions
    {

        public static int ToInt32(this uint value)
        {
            return Convert.ToInt32(value);
        }

        public static int ToInt32(this ulong value)
        {
            return Convert.ToInt32(value);
        }

        public static long ToInt64(this uint value)
        {
            return Convert.ToInt64(value);
        }

        public static long ToInt64(this ulong value)
        {
            return Convert.ToInt64(value);
        }

        public static uint ToUInt32(this int value)
        {
            return Convert.ToUInt32(value);
        }

        public static uint ToUInt32(this ulong value)
        {
            return Convert.ToUInt32(value);
        }

    }

}
