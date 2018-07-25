using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Extensions
{
    internal static class ConversionExtensions
    {
        #region int16

        public static short ToInt16(this int value)
        {
            return Convert.ToInt16(value);
        }

        public static short ToInt16(this long value)
        {
            return Convert.ToInt16(value);
        }

        public static short ToInt16(this ushort value)
        {
            return Convert.ToInt16(value);
        }

        public static short ToInt16(this uint value)
        {
            return Convert.ToInt16(value);
        }

        public static short ToInt16(this ulong value)
        {
            return Convert.ToInt16(value);
        }

        #endregion

        #region int32

        public static int ToInt32(this short value)
        {
            return value;
        }

        public static int ToInt32(this long value)
        {
            return Convert.ToInt32(value);
        }

        public static int ToInt32(this ushort value)
        {
            return Convert.ToInt32(value);
        }

        public static int ToInt32(this uint value)
        {
            return Convert.ToInt32(value);
        }

        public static int ToInt32(this ulong value)
        {
            return Convert.ToInt32(value);
        }

        #endregion

        #region int64

        public static long ToInt64(this short value)
        {
            return value;
        }

        public static long ToInt64(this int value)
        {
            return value;
        }

        public static long ToInt64(this ushort value)
        {
            return Convert.ToInt64(value);
        }

        public static long ToInt64(this uint value)
        {
            return Convert.ToInt64(value);
        }

        public static long ToInt64(this ulong value)
        {
            return Convert.ToInt32(value);
        }

        #endregion

        #region uint16

        public static ushort ToUInt16(this short value)
        {
            return Convert.ToUInt16(value);
        }

        public static ushort ToUInt16(this int value)
        {
            return Convert.ToUInt16(value);
        }

        public static ushort ToUInt16(this long value)
        {
            return Convert.ToUInt16(value);
        }

        public static ushort ToUInt16(this uint value)
        {
            return Convert.ToUInt16(value);
        }

        public static ushort ToUInt16(this ulong value)
        {
            return Convert.ToUInt16(value);
        }

        #endregion

        #region uint32

        public static uint ToUInt32(this short value)
        {
            return Convert.ToUInt32(value);
        }

        public static uint ToUInt32(this int value)
        {
            return Convert.ToUInt32(value);
        }

        public static uint ToUInt32(this long value)
        {
            return Convert.ToUInt32(value);
        }

        public static uint ToUInt32(this ushort value)
        {
            return value;
        }

        public static uint ToUInt32(this ulong value)
        {
            return Convert.ToUInt32(value);
        }

        #endregion

        #region uint64

        public static ulong ToUInt64(this short value)
        {
            return Convert.ToUInt64(value);
        }

        public static ulong ToUInt64(this int value)
        {
            return Convert.ToUInt64(value);
        }

        public static ulong ToUInt64(this long value)
        {
            return Convert.ToUInt64(value);
        }

        public static ulong ToUInt64(this ushort value)
        {
            return value;
        }

        public static ulong ToUInt64(this uint value)
        {
            return value;
        }

        #endregion
    }
}
