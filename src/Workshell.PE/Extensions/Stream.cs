using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Extensions
{
    internal static class StreamExtensions
    {
        #region Methods

        public static async Task<T> ReadStructAsync<T>(this Stream stream) where T : struct
        {
            var size = Marshal.SizeOf<T>();

            return await ReadStructAsync<T>(stream, size).ConfigureAwait(false);
        }

        public static async Task<T> ReadStructAsync<T>(this Stream stream, int size, bool allowSmaller = false) where T : struct
        {
            var buffer = new byte[size];
            var numRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            if (!allowSmaller && numRead < size)
                throw new IOException("Could not read all of structure from stream.");

            if (numRead < size)
                return default(T);

            return Utils.Read<T>(buffer);
        }

        public static async Task<byte> ReadByteAsync(this Stream stream)
        {
            var buffer = new byte[1];
            var numRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            if (numRead < 1)
                throw new IOException("Could not read byte from stream.");

            return buffer[0];
        }

        public static async Task<short> ReadInt16Async(this Stream stream)
        {
            var buffer = new byte[sizeof(short)];
            var numRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            if (numRead < buffer.Length)
                throw new IOException("Could not read int16 from stream.");

            return Utils.ReadInt16(buffer);
        }

        public static async Task<int> ReadInt32Async(this Stream stream)
        {
            var buffer = new byte[sizeof(int)];
            var numRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            if (numRead < buffer.Length)
                throw new IOException("Could not read int32 from stream.");

            return Utils.ReadInt32(buffer);
        }

        public static async Task<long> ReadInt64Async(this Stream stream)
        {
            var buffer = new byte[sizeof(long)];
            var numRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            if (numRead < buffer.Length)
                throw new IOException("Could not read int64 from stream.");

            return Utils.ReadInt64(buffer);
        }
      
        public static async Task<ushort> ReadUInt16Async(this Stream stream)
        {
            var buffer = new byte[sizeof(ushort)];
            var numRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            if (numRead < buffer.Length)
                throw new IOException("Could not read uint16 from stream.");

            return Utils.ReadUInt16(buffer);
        }

        public static async Task<uint> ReadUInt32Async(this Stream stream)
        {
            var buffer = new byte[sizeof(uint)];
            var numRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            if (numRead < buffer.Length)
                throw new IOException("Could not read uint32 from stream.");

            return Utils.ReadUInt32(buffer);
        }

        public static async Task<ulong> ReadUInt64Async(this Stream stream)
        {
            var buffer = new byte[sizeof(ulong)];
            var numRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            if (numRead < buffer.Length)
                throw new IOException("Could not read uint64 from stream.");

            return Utils.ReadUInt64(buffer);
        }

        public static async Task<string> ReadStringAsync(this Stream stream)
        {
            var builder = new StringBuilder(256);
            var buffer = new byte[1];

            while (true)
            {
                var numRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (numRead < 1 || buffer[0] == 0)
                    break;

                builder.Append((char) buffer[0]);
            }

            return builder.ToString();
        }

        public static async Task<string> ReadStringAsync(this Stream stream, int size, bool allowSmaller = true)
        {
            var buffer = new byte[size];
            var numRead = await stream.ReadAsync(buffer, 0, buffer.Length);

            if (!allowSmaller && numRead < buffer.Length)
                throw new IOException("Could not read string from stream.");
            
            var builder = new StringBuilder(256);

            foreach(var b in buffer)
            {
                if (b == 0)
                    break;

                builder.Append((char)b);
            }

            return builder.ToString();
        }

        public static async Task<string> ReadUnicodeStringAsync(this Stream stream)
        {
            var builder = new StringBuilder();

            while (true)
            {
                var value = await ReadUInt16Async(stream).ConfigureAwait(false);

                if (value == 0)
                    break;

                builder.Append((char)value);
            }

            return builder.ToString();
        }

        public static async Task<string> ReadUnicodeStringAsync(this Stream stream, int charCount)
        {
            var builder = new StringBuilder();

            for(var i = 0; i < charCount; i++)
            {
                var value = await ReadUInt16Async(stream).ConfigureAwait(false);

                if (value == 0)
                    break;

                builder.Append((char)value);
            }

            return builder.ToString();
        }

        public static async Task<byte[]> ReadBytesAsync(this Stream stream, int count, long offset = -1)
        {
            if (offset >= 0)
                stream.Seek(offset, SeekOrigin.Begin);

            var buffer = new byte[count];
            var numRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
            var results = new byte[numRead];

            Array.Copy(buffer, 0, results, 0, numRead);

            return results;
        }

        public static async Task<byte[]> ReadBytesAsync(this Stream stream, Location location)
        {
            return await ReadBytesAsync(stream, location.FileSize.ToInt32(), location.FileOffset.ToInt64())
                .ConfigureAwait(false);
        }

        public static async Task<int> SkipBytesAsync(this Stream stream, int count)
        {
            var buffer = new byte[count];

            return await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
        }

        public static async Task WriteBytesAsync(this Stream stream, byte[] bytes)
        {
            await stream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
        }

        public static async Task WriteStructAsync<T>(this Stream stream, T structure) where T : struct
        {
            var size = Marshal.SizeOf<T>();

            await WriteStructAsync<T>(stream, structure, size).ConfigureAwait(false);
        }

        public static async Task WriteStructAsync<T>(this Stream stream, T structure, int size) where T : struct
        {
            var buffer = new byte[size];

            WriteStruct<T>(structure,buffer,0,buffer.Length);
            await stream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
        }

        private static void WriteStruct<T>(T structure, byte[] buffer, int startIndex, int count) where T : struct
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

        #endregion
    }
}
