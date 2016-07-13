using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Workshell.PE.Dump
{

    public enum NumberStyle : int
    {
        Decimal,
        Hexadecimal
    }

    internal class Utils
    {

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
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Byte: return IntToHex(value, 2);
                case TypeCode.SByte: return IntToHex(value, 2);
                case TypeCode.UInt16: return IntToHex(value, 4);
                case TypeCode.Int16: return IntToHex(value, 4);
                case TypeCode.UInt32: return IntToHex(value, 8);
                case TypeCode.Int32: return IntToHex(value, 8);
                case TypeCode.UInt64: return IntToHex(value, 16);
                case TypeCode.Int64: return IntToHex(value, 16);
                default:
                    throw new FormatException("Unknown integer value type.");
            }
        }

        public static string IntToHex(object value, int size)
        {
            string result = String.Empty;

            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Byte:
                    result = "0x" + ((byte)(value)).ToString("X" + size);
                    break;
                case TypeCode.SByte:
                    result = "0x" + ((sbyte)(value)).ToString("X" + size);
                    break;
                case TypeCode.UInt16:
                    result = "0x" + ((ushort)(value)).ToString("X" + size);
                    break;
                case TypeCode.Int16:
                    result = "0x" + ((short)(value)).ToString("X" + size);
                    break;
                case TypeCode.UInt32:
                    result = "0x" + ((uint)(value)).ToString("X" + size);
                    break;
                case TypeCode.Int32:
                    result = "0x" + ((int)(value)).ToString("X" + size);
                    break;
                case TypeCode.UInt64:
                    result = "0x" + ((ulong)(value)).ToString("X" + size);
                    break;
                case TypeCode.Int64:
                    result = "0x" + ((long)(value)).ToString("X" + size);
                    break;
                default:
                    throw new FormatException("Unknown integer value type.");
            }

            return result;
        }

        public static string FormatNumber(object value, NumberStyle numberStyle)
        {
            if (!IsNumeric(value))
                throw new FormatException("Unknown numeric value type.");

            if (numberStyle == NumberStyle.Decimal)
            {
                return value.ToString();
            }
            else
            {
                return IntToHex(value);
            }
        }

        public static string FormatNumber(object value, NumberStyle numberStyle, int size)
        {
            if (!IsNumeric(value))
                throw new FormatException("Unknown numeric value type.");

            string result = String.Empty;

            if (numberStyle == NumberStyle.Decimal)
            {
                switch (Type.GetTypeCode(value.GetType()))
                {
                    case TypeCode.Byte:
                        result = ((byte)(value)).ToString("D" + size);
                        break;
                    case TypeCode.SByte:
                        result = ((sbyte)(value)).ToString("D" + size);
                        break;
                    case TypeCode.UInt16:
                        result = ((ushort)(value)).ToString("D" + size);
                        break;
                    case TypeCode.Int16:
                        result = ((short)(value)).ToString("D" + size);
                        break;
                    case TypeCode.UInt32:
                        result = ((uint)(value)).ToString("D" + size);
                        break;
                    case TypeCode.Int32:
                        result = ((int)(value)).ToString("D" + size);
                        break;
                    case TypeCode.UInt64:
                        result = ((ulong)(value)).ToString("D" + size);
                        break;
                    case TypeCode.Int64:
                        result = ((long)(value)).ToString("D" + size);
                        break;
                    default:
                        throw new FormatException("Unknown integer value type.");
                }
            }
            else
            {
                result = IntToHex(value, size);
            }

            return result;
        }

        public static object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                return null;
            }
        }

        private static readonly string[] suf = { "Bytes", "KB", "MB", "GB", "TB", "PB", "EB" };

        public static string FormatBytes(long byteCount)
        {
            if (byteCount == 0)
                return "0 B";

            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);

            return String.Format("{0} {1}", (Math.Sign(byteCount) * num), suf[place]);
        }

    }

}
