using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Extensions
{
    internal static class ConversionExtensions
    {
        #region Methods

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

        public static ulong ToUInt64(this int value)
        {
            return Convert.ToUInt32(value);
        }

        public static ulong ToUInt64(this long value)
        {
            return Convert.ToUInt32(value);
        }

        #endregion
    }
}
