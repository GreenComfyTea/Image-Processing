using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ImageProcessing
{
	class ColorConverter
	{
		private ColorConverter() { }

		public static byte RgbToGrayscale(Color color)
		{
			return RgbToGrayscale(color.R, color.G, color.B);
		}

		public static byte RgbToGrayscale(byte red, byte green, byte blue)
		{
			return Convert.ToByte(Math.Round((0.2126d * red) + (0.7152d * green) + (0.0722d * blue)));
		}

		public static HsvColor RgbToHsv(Color color)
		{
			return RgbToHsv(color.R, color.G, color.B);
		}

		public static HsvColor RgbToHsv(byte red, byte green, byte blue)
		{
			double delta, min;
			double hue = 0d;
			double saturation;
			double value;

			min = Math.Min(Math.Min(red, green), blue);
			value = Math.Max(Math.Max(red, green), blue);
			delta = value - min;

			if (value == 0d)
			{
				saturation = 0d;
			}
			else
			{
				saturation = delta / value;
			}

			if (saturation == 0d)
			{
				hue = 0d;
			}
			else if (red == value)
			{
				hue = (green - blue) / delta;
			}
			else if (green == value)
			{
				hue = 2d + (blue - red) / delta;
			}
			else if (blue == value)
			{
				hue = 4d + (red - green) / delta;
			}

			hue *= 60d;

			if (hue < 0d)
			{
				hue = hue + 360d;
			}

			return new HsvColor(hue, saturation, value / 255d);
		}

		public static Color HsvToRgb(HsvColor hsvColor)
		{
			return HsvToRgb(hsvColor.Hue, hsvColor.Saturation, hsvColor.Value);
		}

		public static Color HsvToRgb(double hue, double saturation, double value)
		{
			byte tempValue = Convert.ToByte(value * 255d);

			while (hue < 0d)
			{
				hue += 360d;
			}

			while (hue >= 360d)
			{
				hue -= 360d;
			}

			if (value <= 0d)
			{
				return Color.FromRgb(0, 0, 0);
			}
			else if (saturation <= 0d)
			{
				return Color.FromRgb(tempValue, tempValue, tempValue);
			}
			else
			{
				double hueInterval = hue / 60d;
				int hueIntervalIndex = (int) Math.Floor(hueInterval);
				double hueIntervalDelta = hueInterval - hueIntervalIndex;

				byte minValue = Convert.ToByte(tempValue * (1d - saturation));
				byte decValue = Convert.ToByte(tempValue * (1d - saturation * hueIntervalDelta));
				byte incValue = Convert.ToByte(tempValue * (1d - saturation * (1d - hueIntervalDelta)));

				switch (hueIntervalIndex)
				{
					case 0: return Color.FromRgb(tempValue, incValue, minValue);
					case 1: return Color.FromRgb(decValue, tempValue, minValue);
					case 2: return Color.FromRgb(minValue, tempValue, incValue);
					case 3: return Color.FromRgb(minValue, decValue, tempValue);
					case 4: return Color.FromRgb(incValue, minValue, tempValue);
					case 5: return Color.FromRgb(tempValue, minValue, decValue);
					case 6: return Color.FromRgb(tempValue, incValue, minValue);
					case -1: return Color.FromRgb(tempValue, minValue, decValue);
					default: return Color.FromRgb(tempValue, tempValue, tempValue);
				}
			}
		}

		public static Color SaturationToRgb(double saturation)
		{
			byte intensity = Convert.ToByte(255d * saturation);
			return Color.FromRgb(intensity, intensity, intensity);
		}

		public static CmykColor RgbToCmyk(Color color)
		{
			return RgbToCmyk(color.R, color.G, color.B);
		}

		public static CmykColor RgbToCmyk(byte red, byte green, byte blue)
		{
			double redDouble = red / 255d;
			double greenDouble = green / 255d;
			double blueDouble = blue / 255d;

			double black = 1d - Math.Max(Math.Max(redDouble, greenDouble), blueDouble);

			if(black == 1d)
			{
				return new CmykColor(0d, 0d, 0d, black);
			}

			double cyan = (1d - redDouble - black) / (1d - black);
			double magenta = (1d - greenDouble - black) / (1d - black);
			double yellow = (1d - blueDouble - black) / (1d - black);

			return new CmykColor(cyan, magenta, yellow, black);
		}

		public static Color CmykToRgb(CmykColor cmykColor)
		{
			return CmykToRgb(cmykColor.Cyan, cmykColor.Magenta, cmykColor.Yellow, cmykColor.Black);
		}

		public static Color CmykToRgb(double cyan, double magenta, double yellow, double black)
		{
			byte red = Convert.ToByte(255d * (1d - cyan) * (1d - black));
			byte green = Convert.ToByte(255d * (1d - magenta) * (1d - black));
			byte blue = Convert.ToByte(255d * (1d - yellow) * (1d - black));

			return Color.FromRgb(red, green, blue);
		}

		public static YuvColor RgbToYuv(Color color)
		{
			return RgbToYuv(color.R, color.G, color.B);
		}

		public static YuvColor RgbToYuv(byte red, byte green, byte blue)
		{
			double redDouble = red / 255d;
			double greenDouble = green / 255d;
			double blueDouble = blue / 255d;

			double luma = 0.2126d * redDouble + 0.7152d * greenDouble + 0.0722d * blueDouble;
			double colorDifferenceU = -0.09991d * redDouble - 0.33609d * greenDouble + 0.436d * blueDouble;
			double colorDifferenceV = 0.615d * redDouble - 0.55861d * greenDouble - 0.05639d * blueDouble;

			return new YuvColor(luma, colorDifferenceU, colorDifferenceV);
		}

		public static Color YuvToRgb(YuvColor yuvColor)
		{
			return YuvToRgb(yuvColor.Luma, yuvColor.ColorDifferenceU, yuvColor.ColorDifferenceV);
		}

		public static Color YuvToRgb(double luma, double colorDifferenceU, double colorDifferenceV)
		{
			double redDouble = 255d * (luma + 1.28033d * colorDifferenceV);
			double greenDouble = 255d * (luma - 0.21482d * colorDifferenceU - 0.38059d * colorDifferenceV);
			double blueDouble = 255d * (luma + 2.12798d * colorDifferenceU);

			redDouble = Clamp(redDouble, 0d, 255d);
			greenDouble = Clamp(greenDouble, 0d, 255d);
			blueDouble = Clamp(blueDouble, 0d, 255d);

			byte red = Convert.ToByte(redDouble);
			byte green = Convert.ToByte(greenDouble);
			byte blue = Convert.ToByte(blueDouble);

			return Color.FromRgb(red, green, blue);
		}

		private static double Clamp(double value, double min, double max)
		{
			return (value < min) ? min : (value > max) ? max : value;
		}
	}
}
