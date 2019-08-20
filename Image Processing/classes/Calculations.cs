using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AL140_07
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

		public static byte ClampToByte(int value)
		{
			if (value < 0) value = 0;
			else if (value > 255) value = 255;
			return Convert.ToByte(value);
		}

		public static byte ClampToByte(float value)
		{
			if (value < 0f) value = 0f;
			else if (value > 255f) value = 255f;
			return Convert.ToByte(value);
		}

		public static byte ClampToByte(double value)
		{
			if (value < 0d) value = 0d;
			else if (value > 255d) value = 255d;
			return Convert.ToByte(value);
		}
	}
}
