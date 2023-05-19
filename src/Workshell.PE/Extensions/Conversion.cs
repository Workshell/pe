#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Extensions
{
    internal static class ConversionExtensions
    {
        #region Int16

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

        #region Int32

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

        #region Int64

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

        #region UInt16

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

        #region UInt32

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

        #region UInt64

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
