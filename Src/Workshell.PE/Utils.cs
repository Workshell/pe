#region License
//  Copyright(c) 2016, Workshell Ltd
//  All rights reserved.
//  
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Workshell Ltd nor the names of its contributors
//  may be used to endorse or promote products
//  derived from this software without specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED.IN NO EVENT SHALL WORKSHELL BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;

namespace Workshell.PE
{

    internal static class Utils
    {

        private static readonly DateTime UNIX_EPOCH = new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc);

        #region Methods

        public static int SizeOf<T>() where T : struct
        {
            int result = Marshal.SizeOf(typeof(T));

            return result;
        }

        public static T Read<T>(byte[] bytes) where T : struct
        {
            IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);

            try
            {
                Marshal.Copy(bytes,0,ptr,bytes.Length);

                T result = (T)Marshal.PtrToStructure(ptr,typeof(T));

                return result;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public static T Read<T>(Stream stream) where T : struct
        {
            int size = SizeOf<T>();

            return Read<T>(stream,size);
        }

        public static T Read<T>(Stream stream, int size) where T : struct
        {
            return Read<T>(stream,size,false);
        }

        public static T Read<T>(Stream stream, int size, bool allowSmaller) where T : struct
        {
            byte[] buffer = new byte[size];           
            int num_read = stream.Read(buffer,0,buffer.Length);

            if (!allowSmaller && num_read < size)
                throw new IOException("Could not read all of structure from stream.");

            if (num_read < size)
                return default(T);

            return Read<T>(buffer);
        }

        public static short ReadInt16(byte[] bytes)
        {
            return BitConverter.ToInt16(bytes,0);
        }

        public static short ReadInt16(Stream stream)
        {
            byte[] buffer = new byte[sizeof(short)];

            stream.Read(buffer,0,buffer.Length);

            return ReadInt16(buffer);
        }

        public static int ReadInt32(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes,0);
        }

        public static int ReadInt32(Stream stream)
        {
            byte[] buffer = new byte[sizeof(int)];

            stream.Read(buffer,0,buffer.Length);

            return ReadInt32(buffer);
        }

        public static long ReadInt64(byte[] bytes)
        {
            return BitConverter.ToInt64(bytes,0);
        }

        public static long ReadInt64(Stream stream)
        {
            byte[] buffer = new byte[sizeof(long)];

            stream.Read(buffer,0,buffer.Length);

            return ReadInt64(buffer);
        }

        public static ushort ReadUInt16(byte[] bytes)
        {
            return BitConverter.ToUInt16(bytes,0);
        }

        public static ushort ReadUInt16(Stream stream)
        {
            byte[] buffer = new byte[sizeof(ushort)];

            stream.Read(buffer,0,buffer.Length);

            return ReadUInt16(buffer);
        }

        public static uint ReadUInt32(byte[] bytes)
        {
            return BitConverter.ToUInt32(bytes,0);
        }

        public static uint ReadUInt32(Stream stream)
        {
            byte[] buffer = new byte[sizeof(uint)];

            stream.Read(buffer,0,buffer.Length);

            return ReadUInt32(buffer);
        }

        public static ulong ReadUInt64(byte[] bytes)
        {
            return BitConverter.ToUInt64(bytes,0);
        }

        public static ulong ReadUInt64(Stream stream)
        {
            byte[] buffer = new byte[sizeof(ulong)];

            stream.Read(buffer,0,buffer.Length);

            return ReadUInt64(buffer);
        }

        public static string ReadString(Stream stream)
        {
            StringBuilder builder = new StringBuilder(256);

            while (true)
            {
                int b = stream.ReadByte();

                if (b <= 0)
                    break;

                builder.Append((char)b);
            }

            return builder.ToString();
        }

        public static string ReadString(Stream stream, long size)
        {
            byte[] buffer = new byte[size];

            stream.Read(buffer,0,buffer.Length);

            StringBuilder builder = new StringBuilder(256);

            foreach(byte b in buffer)
            {
                if (b == 0)
                    break;

                builder.Append((char)b);
            }

            return builder.ToString();
        }

        public static byte[] ReadBytes(Stream stream, long offset, long size)
        {
            byte[] buffer = new byte[size];

            stream.Seek(offset, SeekOrigin.Begin);
            stream.Read(buffer, 0, buffer.Length);

            return buffer;
        }

        public static byte[] ReadBytes(Stream stream, Location location)
        {
            return ReadBytes(stream, location.FileOffset.ToInt64(), location.FileSize.ToInt64());
        }

        public static void Write(sbyte value, Stream stream)
        {
            byte[] buffer = BitConverter.GetBytes(value);

            stream.WriteByte((byte)value);
        }

        public static void Write(byte value, Stream stream)
        {
            byte[] buffer = BitConverter.GetBytes(value);

            stream.WriteByte(value);
        }

        public static void Write(short value, Stream stream)
        {
            byte[] buffer = BitConverter.GetBytes(value);

            Write(buffer, stream);
        }

        public static void Write(ushort value, Stream stream)
        {
            byte[] buffer = BitConverter.GetBytes(value);

            Write(buffer, stream);
        }

        public static void Write(int value, Stream stream)
        {
            byte[] buffer = BitConverter.GetBytes(value);

            Write(buffer, stream);
        }

        public static void Write(uint value, Stream stream)
        {
            byte[] buffer = BitConverter.GetBytes(value);

            Write(buffer, stream);
        }

        public static void Write(long value, Stream stream)
        {
            byte[] buffer = BitConverter.GetBytes(value);

            Write(buffer, stream);
        }

        public static void Write(ulong value, Stream stream)
        {
            byte[] buffer = BitConverter.GetBytes(value);

            Write(buffer, stream);
        }

        public static void Write(byte[] bytes, Stream stream)
        {
            stream.Write(bytes, 0, bytes.Length);
        }

        public static void Write<T>(T structure, byte[] buffer, int startIndex, int count) where T : struct
        {
            IntPtr ptr = Marshal.AllocHGlobal(count);

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
            int size = SizeOf<T>();
            
            Write<T>(structure,stream,size);
        }

        public static void Write<T>(T structure, Stream stream, int size) where T : struct
        {
            byte[] buffer = new byte[size];

            Write<T>(structure,buffer,0,buffer.Length);
            stream.Write(buffer,0,buffer.Length);
        }

        public static int SkipBytes(Stream stream, int count)
        {
            byte[] buffer = new byte[count];

            return stream.Read(buffer,0,buffer.Length);
        }

        public static DateTime ConvertTimeDateStamp(uint timeDateStamp)
        {
            DateTime result = UNIX_EPOCH.AddSeconds(timeDateStamp);

            result += TimeZone.CurrentTimeZone.GetUtcOffset(result);

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
            string result = String.Empty;

            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Byte:
                    result = "0x" + ((byte)(value)).ToString("X2");
                    break;
                case TypeCode.SByte:
                    result = "0x" + ((sbyte)(value)).ToString("X2");
                    break;
                case TypeCode.UInt16:
                    result = "0x" + ((ushort)(value)).ToString("X4");
                    break;
                case TypeCode.Int16:
                    result = "0x" + ((short)(value)).ToString("X4");
                    break;
                case TypeCode.UInt32:
                    result = "0x" + ((uint)(value)).ToString("X8");
                    break;
                case TypeCode.Int32:
                    result = "0x" + ((int)(value)).ToString("X8");
                    break;
                case TypeCode.UInt64:
                    result = "0x" + ((ulong)(value)).ToString("X16");
                    break;
                case TypeCode.Int64:
                    result = "0x" + ((long)(value)).ToString("X16");
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

        #endregion

    }

}
