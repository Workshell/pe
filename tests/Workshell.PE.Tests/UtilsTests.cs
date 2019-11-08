using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using FluentAssertions;
using NUnit.Framework;

namespace Workshell.PE.Tests
{
    [TestFixture]
    public sealed class UtilsTests
    {
        [Test]
        public void SizeOf_Returns_Correct_Size()
        {
            var output = Utils.SizeOf<uint>();

            output.Should().Be(sizeof(uint));
        }

        [Test]
        public void Read_Returns_Correct_Result()
        {
            var input = 0x1000U;
            var bytes = BitConverter.GetBytes(input);
            var output = Utils.Read<uint>(bytes);

            output.Should().Be(input);
        }

        [Test]
        public void ReadByte_Returns_Correct_Result()
        {
            byte input = 0x21;
            var output = Utils.ReadByte(new byte[] {0x00, 0x01, 0x03, 0x07, 0x21, 0x15}, 4);

            output.Should().Be(input);
        }

        [Test]
        public void ReadInt16_Returns_Correct_Result()
        {
            short input = 32000;
            var bytes = BitConverter.GetBytes(input);
            var output = Utils.ReadInt16(bytes);

            output.Should().Be(input);
        }

        [Test]
        public void ReadInt32_Returns_Correct_Result()
        {
            int input = 32000;
            var bytes = BitConverter.GetBytes(input);
            var output = Utils.ReadInt32(bytes);

            output.Should().Be(input);
        }

        [Test]
        public void ReadInt64_Returns_Correct_Result()
        {
            long input = 32000;
            var bytes = BitConverter.GetBytes(input);
            var output = Utils.ReadInt64(bytes);

            output.Should().Be(input);
        }

        [Test]
        public void ReadUInt16_Returns_Correct_Result()
        {
            ushort input = 32000;
            var bytes = BitConverter.GetBytes(input);
            var output = Utils.ReadUInt16(bytes);

            output.Should().Be(input);
        }

        [Test]
        public void ReadUInt32_Returns_Correct_Result()
        {
            uint input = 32000;
            var bytes = BitConverter.GetBytes(input);
            var output = Utils.ReadUInt32(bytes);

            output.Should().Be(input);
        }

        [Test]
        public void ReadUInt64_Returns_Correct_Result()
        {
            ulong input = 32000;
            var bytes = BitConverter.GetBytes(input);
            var output = Utils.ReadUInt64(bytes);

            output.Should().Be(input);
        }

        [Test]
        public void Write_To_Stream_Succeeds()
        {
            using (var mem = new MemoryStream())
            {
                var buffer = new byte[ushort.MaxValue];

                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(buffer);
                }

                mem.Write(buffer, 0, buffer.Length);

                mem.Length.Should().Be(ushort.MaxValue);
            }
        }

        [Test]
        public void Write_Structure_To_Buffer_Succeeds()
        {
            uint input = 0x1000;
            var buffer = new byte[sizeof(uint)];

            Utils.Write<uint>(input, buffer, 0, sizeof(uint));

            var output = BitConverter.ToUInt32(buffer, 0);

            output.Should().Be(input);
        }

        [Test]
        public void Write_Structure_To_Stream_Succeeds()
        {
            using (var mem = new MemoryStream())
            {
                uint input = 0x1000;

                Utils.Write<uint>(input, mem, sizeof(uint));

                var output = BitConverter.ToUInt32(mem.ToArray(), 0);

                output.Should().Be(input);
            }
        }

        [TestCase(0U, "1970-01-01T00:00:00")]
        [TestCase(1573219373U, "2019-11-08T13:22:53")]
        public void ConvertTimeDateStamp_Returns_Correct_DateTime(uint input, string expectedValue)
        {
            var parsedExpectedValue = DateTime.ParseExact(expectedValue, "yyyy-MM-ddTHH:mm:ss", null);
            var output = Utils.ConvertTimeDateStamp(input);

            output.Should().Be(parsedExpectedValue);
        }

        [TestCase((byte)7, true)]
        [TestCase((sbyte)-21, true)]
        [TestCase((ushort)7, true)]
        [TestCase((short)-21, true)]
        [TestCase((uint)7, true)]
        [TestCase((int)-21, true)]
        [TestCase((ulong)7, true)]
        [TestCase((long)-21, true)]
        [TestCase((float)7.21, true)]
        [TestCase((double)-21.7, true)]
        [TestCase("Not numeric", false)]
        [TestCase(true, false)]
        public void IsNumeric_Returns_Correct_For_Given_Value(object input, bool expectedOutput)
        {
            var output = Utils.IsNumeric(input);

            output.Should().Be(expectedOutput);
        }

        [TestCase((byte)7, "0x07")]
        [TestCase((sbyte)-21, "0xEB")]
        [TestCase((ushort)7, "0x0007")]
        [TestCase((short)-21, "0xFFEB")]
        [TestCase((uint)7, "0x00000007")]
        [TestCase((int)-21, "0xFFFFFFEB")]
        [TestCase((ulong)7, "0x0000000000000007")]
        [TestCase((long)-21, "0xFFFFFFFFFFFFFFEB")]
        public void IntToHex_Returns_Correct_For_Given_Value(object input, string expectedOutput)
        {
            var output = Utils.IntToHex(input);

            output.Should().Be(expectedOutput);
        }

        [TestCase((float)7.21)]
        [TestCase((double)-21.7)]
        [TestCase("Not numeric")]
        [TestCase(true)]
        public void IntToHex_Throws_Exception_For_Invalid_Value(object input)
        {
            Assert.Throws<FormatException>(() => Utils.IntToHex(input));
        }

        [Test]
        public void HiByte_Returns_Correct_Value()
        {
            ushort input = 0x8664;
            var output = Utils.HiByte(input);

            output.Should().Be(0x86);
        }

        [Test]
        public void LoByte_Returns_Correct_Value()
        {
            ushort input = 0x8664;
            var output = Utils.LoByte(input);

            output.Should().Be(0x64);
        }

        [Test]
        public void HiWord_Returns_Correct_Value()
        {
            uint input = 0x12344321;
            var output = Utils.HiWord(input);

            output.Should().Be(0x1234);
        }

        [Test]
        public void LoWord_Returns_Correct_Value()
        {
            uint input = 0x12344321;
            var output = Utils.LoWord(input);

            output.Should().Be(0x4321);
        }

        [Test]
        public void HiDWord_Returns_Correct_Value()
        {
            ulong input = 0x1234432156788765;
            var output = Utils.HiDWord(input);

            output.Should().Be(0x12344321);
        }

        [Test]
        public void LoDWord_Returns_Correct_Value()
        {
            ulong input = 0x1234432156788765;
            var output = Utils.LoDWord(input);

            output.Should().Be(0x56788765);
        }

        [Test]
        public void MakeUInt64_Returns_Correct_Value()
        {
            var inputMS = 0x12344321U;
            var inputLS = 0x56788765U;
            var output = Utils.MakeUInt64(inputMS, inputLS);

            output.Should().Be(0x1234432156788765);
        }
    }
}
