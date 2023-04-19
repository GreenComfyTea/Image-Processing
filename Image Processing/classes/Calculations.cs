using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
	class Calculations
	{
		public static byte AffineTransformationByte(byte value, byte oldMin, byte oldMax, byte newMin, byte newMax)
		{
			return Convert.ToByte(AffineTransformation(value, oldMin, oldMax, newMin, newMax));
		}

		public static double AffineTransformation(double value, double oldMin, double oldMax, double newMin, double newMax)
		{
			if (value < oldMin || value > oldMax)
			{
				throw new Exception("Incorrect arguments");
			}

			return ((value - oldMin) * ((newMax - newMin) / (oldMax - oldMin))) + newMin;
		}

		public static byte ClampToByte(sbyte value)
		{
			if (value < byte.MinValue) return byte.MinValue;
			return Convert.ToByte(value);
		}

		public static byte ClampToByte(short value)
		{
			if (value < byte.MinValue) return byte.MinValue;
			if (value > byte.MaxValue) return byte.MaxValue;
			return Convert.ToByte(value);
		}

		public static byte ClampToByte(ushort value)
		{
			if (value > byte.MaxValue) return byte.MaxValue;
			return Convert.ToByte(value);
		}

		public static byte ClampToByte(int value)
		{
			if (value < byte.MinValue) return byte.MinValue;
			if (value > byte.MaxValue) return byte.MaxValue;
			return Convert.ToByte(value);
		}

		public static byte ClampToByte(uint value)
		{
			if (value > byte.MaxValue) return byte.MaxValue;
			return Convert.ToByte(value);
		}

		public static byte ClampToByte(long value)
		{
			if (value < byte.MinValue) return byte.MinValue;
			if (value > byte.MaxValue) return byte.MaxValue;
			return Convert.ToByte(value);
		}

		public static byte ClampToByte(ulong value)
		{
			if (value > byte.MaxValue) return byte.MaxValue;
			return Convert.ToByte(value);
		}

		public static byte ClampToByte(float value)
		{
			if (value < byte.MinValue) return byte.MinValue;
			if (value > byte.MaxValue) return byte.MaxValue;
			return Convert.ToByte(value);
		}

		public static byte ClampToByte(double value)
		{
			if (value < byte.MinValue) return byte.MinValue;
			if (value > byte.MaxValue) return byte.MaxValue;
			return Convert.ToByte(value);
		}

		public static byte ClampToByte(decimal value)
		{
			if (value < byte.MinValue) return byte.MinValue;
			if (value > byte.MaxValue) return byte.MaxValue;
			return Convert.ToByte(value);
		}

		public static sbyte ClampToSByte(byte value)
		{
			if (value > sbyte.MaxValue) return sbyte.MaxValue;
			return Convert.ToSByte(value);
		}

		public static sbyte ClampToSByte(short value)
		{
			if (value < sbyte.MinValue) return sbyte.MinValue;
			if (value > sbyte.MaxValue) return sbyte.MaxValue;
			return Convert.ToSByte(value);
		}

		public static sbyte ClampToSByte(ushort value)
		{
			if (value > sbyte.MaxValue) return sbyte.MaxValue;
			return Convert.ToSByte(value);
		}

		public static sbyte ClampToSByte(int value)
		{
			if (value < sbyte.MinValue) return sbyte.MinValue;
			if (value > sbyte.MaxValue) return sbyte.MaxValue;
			return Convert.ToSByte(value);
		}

		public static sbyte ClampToSByte(uint value)
		{
			if (value > sbyte.MaxValue) return sbyte.MaxValue;
			return Convert.ToSByte(value);
		}

		public static sbyte ClampToSByte(long value)
		{
			if (value < sbyte.MinValue) return sbyte.MinValue;
			if (value > sbyte.MaxValue) return sbyte.MaxValue;
			return Convert.ToSByte(value);
		}

		public static sbyte ClampToSByte(ulong value)
		{
			if (value > (ulong)sbyte.MaxValue) return sbyte.MaxValue;
			return Convert.ToSByte(value);
		}

		public static sbyte ClampToSByte(float value)
		{
			if (value < sbyte.MinValue) return sbyte.MinValue;
			if (value > sbyte.MaxValue) return sbyte.MaxValue;
			return Convert.ToSByte(value);
		}

		public static sbyte ClampToSByte(double value)
		{
			if (value < sbyte.MinValue) return sbyte.MinValue;
			if (value > sbyte.MaxValue) return sbyte.MaxValue;
			return Convert.ToSByte(value);
		}

		public static short ClampToShort(byte value)
		{
			return Convert.ToInt16(value);
		}

		public static short ClampToShort(sbyte value)
		{
			return Convert.ToInt16(value);
		}

		public static short ClampToShort(ushort value)
		{
			if (value > short.MaxValue) return short.MaxValue;
			return Convert.ToInt16(value);
		}

		public static short ClampToShort(int value)
		{
			if (value < short.MinValue) return short.MinValue;
			if (value > short.MaxValue) return short.MaxValue;
			return Convert.ToInt16(value);
		}

		public static short ClampToShort(uint value)
		{
			if (value > short.MaxValue) return short.MaxValue;
			return Convert.ToInt16(value);
		}

		public static short ClampToShort(long value)
		{
			if (value < short.MinValue) return short.MinValue;
			if (value > short.MaxValue) return short.MaxValue;
			return Convert.ToInt16(value);
		}

		public static short ClampToShort(ulong value)
		{
			if (value > (ulong)short.MaxValue) return short.MaxValue;
			return Convert.ToInt16(value);
		}

		public static short ClampToShort(float value)
		{
			if (value < short.MinValue) return short.MinValue;
			if (value > short.MaxValue) return short.MaxValue;
			return Convert.ToInt16(value);
		}

		public static short ClampToShort(double value)
		{
			if (value < short.MinValue) return short.MinValue;
			if (value > short.MaxValue) return short.MaxValue;
			return Convert.ToInt16(value);
		}

		public static short ClampToShort(decimal value)
		{
			if (value < short.MinValue) return short.MinValue;
			if (value > short.MaxValue) return short.MaxValue;
			return Convert.ToInt16(value);
		}

		public static ushort ClampToUShort(byte value)
		{
			return Convert.ToUInt16(value);
		}

		public static ushort ClampToUShort(sbyte value)
		{
			if (value < ushort.MinValue) return ushort.MinValue;
			return Convert.ToUInt16(value);
		}

		public static ushort ClampToUShort(short value)
		{
			if (value < ushort.MinValue) return ushort.MinValue;
			return Convert.ToUInt16(value);
		}

		public static ushort ClampToUShort(int value)
		{
			if (value < ushort.MinValue) return ushort.MinValue;
			if (value > ushort.MaxValue) return ushort.MaxValue;
			return Convert.ToUInt16(value);
		}

		public static ushort ClampToUShort(uint value)
		{
			if (value > ushort.MaxValue) return ushort.MaxValue;
			return Convert.ToUInt16(value);
		}

		public static ushort ClampToUShort(long value)
		{
			if (value < ushort.MinValue) return ushort.MinValue;
			if (value > ushort.MaxValue) return ushort.MaxValue;
			return Convert.ToUInt16(value);
		}

		public static ushort ClampToUShort(ulong value)
		{
			if (value > ushort.MaxValue) return ushort.MaxValue;
			return Convert.ToUInt16(value);
		}

		public static ushort ClampToUShort(float value)
		{
			if (value < ushort.MinValue) return ushort.MinValue;
			if (value > ushort.MaxValue) return ushort.MaxValue;
			return Convert.ToUInt16(value);
		}

		public static ushort ClampToUShort(double value)
		{
			if (value < ushort.MinValue) return ushort.MinValue;
			if (value > ushort.MaxValue) return ushort.MaxValue;
			return Convert.ToUInt16(value);
		}

		public static ushort ClampToUShort(decimal value)
		{
			if (value < ushort.MinValue) return ushort.MinValue;
			if (value > ushort.MaxValue) return ushort.MaxValue;
			return Convert.ToUInt16(value);
		}

		public static int ClampToInt(byte value)
		{
			return Convert.ToInt32(value);
		}

		public static int ClampToInt(sbyte value)
		{
			return Convert.ToInt32(value);
		}

		public static int ClampToInt(short value)
		{
			return Convert.ToInt32(value);
		}

		public static int ClampToInt(ushort value)
		{
			return Convert.ToInt32(value);
		}

		public static int ClampToInt(uint value)
		{
			if (value > int.MaxValue) return int.MaxValue;
			return Convert.ToInt32(value);
		}

		public static int ClampToInt(long value)
		{
			if (value < int.MinValue) return int.MinValue;
			if (value > int.MaxValue) return int.MaxValue;
			return Convert.ToInt32(value);
		}

		public static int ClampToInt(ulong value)
		{
			if (value > int.MaxValue) return int.MaxValue;
			return Convert.ToInt32(value);
		}

		public static int ClampToInt(float value)
		{
			if (value < int.MinValue) return int.MinValue;
			if (value > int.MaxValue) return int.MaxValue;
			return Convert.ToInt32(value);
		}

		public static int ClampToInt(double value)
		{
			if (value < int.MinValue) return int.MinValue;
			if (value > int.MaxValue) return int.MaxValue;
			return Convert.ToInt32(value);
		}

		public static int ClampToInt(decimal value)
		{
			if (value < int.MinValue) return int.MinValue;
			if (value > int.MaxValue) return int.MaxValue;
			return Convert.ToInt32(value);
		}

		public static uint ClampToUInt(byte value)
		{
			return Convert.ToUInt32(value);
		}

		public static uint ClampToUInt(sbyte value)
		{
			if (value < uint.MinValue) return uint.MinValue;
			return Convert.ToUInt32(value);
		}

		public static uint ClampToUInt(short value)
		{
			if (value < uint.MinValue) return uint.MinValue;
			return Convert.ToUInt32(value);
		}

		public static uint ClampToUInt(ushort value)
		{
			return Convert.ToUInt32(value);
		}

		public static uint ClampToUInt(int value)
		{
			if (value < uint.MinValue) return uint.MinValue;
			return Convert.ToUInt32(value);
		}

		public static uint ClampToUInt(long value)
		{
			if (value < uint.MinValue) return uint.MinValue;
			if (value > uint.MaxValue) return uint.MaxValue;
			return Convert.ToUInt32(value);
		}

		public static uint ClampToUInt(ulong value)
		{
			if (value > uint.MaxValue) return uint.MaxValue;
			return Convert.ToUInt32(value);
		}

		public static uint ClampToUInt(float value)
		{
			if (value < uint.MinValue) return uint.MinValue;
			if (value > uint.MaxValue) return uint.MaxValue;
			return Convert.ToUInt32(value);
		}

		public static uint ClampToUInt(double value)
		{
			if (value < uint.MinValue) return uint.MinValue;
			if (value > uint.MaxValue) return uint.MaxValue;
			return Convert.ToUInt32(value);
		}

		public static uint ClampToUInt(decimal value)
		{
			if (value < uint.MinValue) return uint.MinValue;
			if (value > uint.MaxValue) return uint.MaxValue;
			return Convert.ToUInt32(value);
		}

		public static long ClampToLong(byte value)
		{
			return Convert.ToInt64(value);
		}

		public static long ClampToLong(sbyte value)
		{
			return Convert.ToInt64(value);
		}

		public static long ClampToLong(short value)
		{
			return Convert.ToInt64(value);
		}

		public static long ClampToLong(ushort value)
		{
			return Convert.ToInt64(value);
		}

		public static long ClampToLong(int value)
		{
			return Convert.ToInt64(value);
		}

		public static long ClampToLong(uint value)
		{
			return Convert.ToInt64(value);
		}

		public static long ClampToLong(ulong value)
		{
			if (value > long.MaxValue) return long.MaxValue;
			return Convert.ToInt64(value);
		}

		public static long ClampToLong(float value)
		{
			if (value < long.MinValue) return long.MinValue;
			if (value > long.MaxValue) return long.MaxValue;
			return Convert.ToInt64(value);
		}

		public static long ClampToLong(double value)
		{
			if (value < long.MinValue) return long.MinValue;
			if (value > long.MaxValue) return long.MaxValue;
			return Convert.ToInt64(value);
		}

		public static long ClampToLong(decimal value)
		{
			if (value < long.MinValue) return long.MinValue;
			if (value > long.MaxValue) return long.MaxValue;
			return Convert.ToInt64(value);
		}

		public static ulong ClampToULong(byte value)
		{
			return Convert.ToUInt64(value);
		}

		public static ulong ClampToULong(sbyte value)
		{
			if (value < (sbyte)ulong.MinValue) return ulong.MinValue;
			return Convert.ToUInt64(value);
		}

		public static ulong ClampToULong(short value)
		{
			if (value < (short)ulong.MinValue) return ulong.MinValue;
			return Convert.ToUInt64(value);
		}

		public static ulong ClampToULong(ushort value)
		{
			return Convert.ToUInt64(value);
		}

		public static ulong ClampToULong(int value)
		{
			if (value < (int)ulong.MinValue) return ulong.MinValue;
			return Convert.ToUInt64(value);
		}

		public static ulong ClampToULong(uint value)
		{
			return Convert.ToUInt64(value);
		}

		public static ulong ClampToULong(long value)
		{
			if (value < (long)ulong.MinValue) return ulong.MinValue;
			return Convert.ToUInt64(value);
		}

		public static ulong ClampToULong(float value)
		{
			if (value < ulong.MinValue) return ulong.MinValue;
			if (value > ulong.MaxValue) return ulong.MaxValue;
			return Convert.ToUInt64(value);
		}

		public static ulong ClampToULong(double value)
		{
			if (value < ulong.MinValue) return ulong.MinValue;
			if (value > ulong.MaxValue) return ulong.MaxValue;
			return Convert.ToUInt64(value);
		}

		public static ulong ClampToULong(decimal value)
		{
			if (value < ulong.MinValue) return ulong.MinValue;
			if (value > ulong.MaxValue) return ulong.MaxValue;
			return Convert.ToUInt64(value);
		}

		public static float ClampToFloat(byte value)
		{
			return Convert.ToSingle(value);
		}

		public static float ClampToFloat(sbyte value)
		{
			return Convert.ToSingle(value);
		}

		public static float ClampToFloat(short value)
		{
			return Convert.ToSingle(value);
		}

		public static float ClampToFloat(ushort value)
		{
			return Convert.ToSingle(value);
		}

		public static float ClampToFloat(int value)
		{
			return Convert.ToSingle(value);
		}

		public static float ClampToFloat(uint value)
		{
			return Convert.ToSingle(value);
		}

		public static float ClampToFloat(long value)
		{
			return Convert.ToSingle(value);
		}

		public static float ClampToFloat(ulong value)
		{
			return Convert.ToSingle(value);
		}

		public static float ClampToFloat(double value)
		{
			if (value < float.MinValue) return float.MinValue;
			if (value > float.MaxValue) return float.MaxValue;
			return Convert.ToSingle(value);
		}

		public static float ClampToFloat(decimal value)
		{
			return Convert.ToSingle(value);
		}

		public static double ClampToDouble(byte value)
		{
			return Convert.ToDouble(value);
		}

		public static double ClampToDouble(sbyte value)
		{
			return Convert.ToDouble(value);
		}

		public static double ClampToDouble(short value)
		{
			return Convert.ToDouble(value);
		}

		public static double ClampToDouble(ushort value)
		{
			return Convert.ToDouble(value);
		}

		public static double ClampToDouble(int value)
		{
			return Convert.ToDouble(value);
		}

		public static double ClampToDouble(uint value)
		{
			return Convert.ToDouble(value);
		}

		public static double ClampToDouble(long value)
		{
			return Convert.ToDouble(value);
		}

		public static double ClampToDouble(ulong value)
		{
			return Convert.ToDouble(value);
		}

		public static double ClampToDouble(float value)
		{
			return Convert.ToDouble(value);
		}

		public static double ClampToDouble(decimal value)
		{
			return Convert.ToDouble(value);
		}

		public static decimal ClampToDecimal(byte value)
		{
			return Convert.ToDecimal(value);
		}

		public static decimal ClampToDecimal(sbyte value)
		{
			return Convert.ToDecimal(value);
		}

		public static decimal ClampToDecimal(short value)
		{
			return Convert.ToDecimal(value);
		}

		public static decimal ClampToDecimal(ushort value)
		{
			return Convert.ToDecimal(value);
		}

		public static decimal ClampToDecimal(int value)
		{
			return Convert.ToDecimal(value);
		}

		public static decimal ClampToDecimal(uint value)
		{
			return Convert.ToDecimal(value);
		}

		public static decimal ClampToDecimal(long value)
		{
			return Convert.ToDecimal(value);
		}

		public static decimal ClampToDecimal(ulong value)
		{
			return Convert.ToDecimal(value);
		}

		public static decimal ClampToDecimal(float value)
		{
			if (value < (float)decimal.MinValue) return decimal.MinValue;
			if (value > (float)decimal.MaxValue) return decimal.MaxValue;
			return Convert.ToDecimal(value);
		}

		public static decimal ClampToDecimal(double value)
		{
			if (value < (double)decimal.MinValue) return decimal.MinValue;
			if (value > (double)decimal.MaxValue) return decimal.MaxValue;
			return Convert.ToDecimal(value);
		}
	}
}
