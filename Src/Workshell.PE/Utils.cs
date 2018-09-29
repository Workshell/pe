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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;

namespace Workshell.PE
{
    internal static class Utils
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #region Methods

        public static int SizeOf<T>() where T : struct
        {
            var result = Marshal.SizeOf(typeof(T));

            return result;
        }

        public static T Read<T>(byte[] bytes) where T : struct
        {
            var ptr = Marshal.AllocHGlobal(bytes.Length);

            try
            {
                Marshal.Copy(bytes,0,ptr,bytes.Length);

                #pragma warning disable CS0618 // Type or member is obsolete
                T result = (T)Marshal.PtrToStructure(ptr,typeof(T));
                #pragma warning restore CS0618 // Type or member is obsolete

                return result;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public static byte ReadByte(byte[] bytes, int index)
        {
            return bytes[index];
        }

        public static short ReadInt16(byte[] bytes)
        {
            return BitConverter.ToInt16(bytes,0);
        }

        public static int ReadInt32(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes,0);
        }

        public static long ReadInt64(byte[] bytes)
        {
            return BitConverter.ToInt64(bytes,0);
        }

        public static ushort ReadUInt16(byte[] bytes)
        {
            return BitConverter.ToUInt16(bytes,0);
        }

        public static uint ReadUInt32(byte[] bytes)
        {
            return BitConverter.ToUInt32(bytes,0);
        }

        public static ulong ReadUInt64(byte[] bytes)
        {
            return BitConverter.ToUInt64(bytes,0);
        }

        public static void Write(sbyte value, Stream stream)
        {
            var buffer = BitConverter.GetBytes(value);

            stream.WriteByte((byte)value);
        }

        public static void Write(byte value, Stream stream)
        {
            var buffer = BitConverter.GetBytes(value);

            stream.WriteByte(value);
        }

        public static void Write(short value, Stream stream)
        {
            var buffer = BitConverter.GetBytes(value);

            Write(buffer, stream);
        }

        public static void Write(ushort value, Stream stream)
        {
            var buffer = BitConverter.GetBytes(value);

            Write(buffer, stream);
        }

        public static void Write(int value, Stream stream)
        {
            var buffer = BitConverter.GetBytes(value);

            Write(buffer, stream);
        }

        public static void Write(uint value, Stream stream)
        {
            var buffer = BitConverter.GetBytes(value);

            Write(buffer, stream);
        }

        public static void Write(long value, Stream stream)
        {
            var buffer = BitConverter.GetBytes(value);

            Write(buffer, stream);
        }

        public static void Write(ulong value, Stream stream)
        {
            var buffer = BitConverter.GetBytes(value);

            Write(buffer, stream);
        }

        public static void Write(byte[] bytes, Stream stream)
        {
            stream.Write(bytes, 0, bytes.Length);
        }

        public static void Write<T>(T structure, byte[] buffer, int startIndex, int count) where T : struct
        {
            var ptr = Marshal.AllocHGlobal(count);

            try
            {
                Marshal.StructureToPtr(structure,ptr,false);
                Marshal.Copy(ptr,buffer,startIndex,count);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public static void Write<T>(T structure, Stream stream) where T : struct
        {
            var size = Utils.SizeOf<T>();
            
            Write<T>(structure,stream,size);
        }

        public static void Write<T>(T structure, Stream stream, int size) where T : struct
        {
            var buffer = new byte[size];

            Write<T>(structure,buffer,0,buffer.Length);
            stream.Write(buffer,0,buffer.Length);
        }

        public static DateTime ConvertTimeDateStamp(uint timeDateStamp)
        {
            var result = UnixEpoch.AddSeconds(timeDateStamp).Add(DateTimeOffset.Now.Offset);

            return result;
        }

        public static bool IsNumeric(object value)
        {
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.Int16:
                case TypeCode.UInt32:
                case TypeCode.Int32:
                case TypeCode.UInt64:
                case TypeCode.Int64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return true;
                default:
                    return false;
            }
        }

        public static string IntToHex(object value)
        {
            var result = String.Empty;

            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Byte:
                    result = "0x" + ((byte)value).ToString("X2");
                    break;
                case TypeCode.SByte:
                    result = "0x" + ((sbyte)value).ToString("X2");
                    break;
                case TypeCode.UInt16:
                    result = "0x" + ((ushort)value).ToString("X4");
                    break;
                case TypeCode.Int16:
                    result = "0x" + ((short)value).ToString("X4");
                    break;
                case TypeCode.UInt32:
                    result = "0x" + ((uint)value).ToString("X8");
                    break;
                case TypeCode.Int32:
                    result = "0x" + ((int)value).ToString("X8");
                    break;
                case TypeCode.UInt64:
                    result = "0x" + ((ulong)value).ToString("X16");
                    break;
                case TypeCode.Int64:
                    result = "0x" + ((long)value).ToString("X16");
                    break;
                default:
                    throw new FormatException("Unknown integer value type.");
            }

            return result;
        }

        public static byte HiByte(ushort value)
        {
            return Convert.ToByte((value >> 8) & 0xFF);
        }

        public static byte LoByte(ushort value)
        {
            return Convert.ToByte(value & 0xFF);
        }

        public static ushort HiWord(uint value)
        {
            return Convert.ToUInt16((value >> 16) & 0xFFFF);
        }

        public static ushort LoWord(uint value)
        {
            return Convert.ToUInt16(value & 0xFFFF);
        }

        public static uint HiDWord(ulong value)
        {
            return Convert.ToUInt32((value >> 32) & 0xFFFFFFFF);
        }

        public static uint LoDWord(ulong value)
        {
            return Convert.ToUInt32(value & 0xFFFFFFFF);
        }

        public static ulong MakeUInt64(uint ms, uint ls)
        {
            var result = (((ulong)ms) << 32) | ls;

            return result;
        }

        public static async Task<long> CopyStreamAsync(Stream from, Stream to, int bufferSize = 4096)
        {
            if (bufferSize < 1)
                bufferSize = 4096;

            var count = 0L;
            var buffer = new byte[bufferSize];
            var numRead = await from.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            while (numRead > 0)
            {
                await to.WriteAsync(buffer, 0, numRead).ConfigureAwait(false);

                count += numRead;
                numRead = await from.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
            }

            return count;
        }

        #endregion
    }
}
