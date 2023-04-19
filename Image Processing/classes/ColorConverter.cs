using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		public static HsvColor RgbToHsv(Color color)
		{
			return RgbToHsv(color.R, color.G, color.B);
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

			if (black == 1d)
			{
				return new CmykColor(0d, 0d, 0d, black);
			}

			double cyan = (1d - redDouble - black) / (1d - black);
			double magenta = (1d - greenDouble - black) / (1d - black);
			double yellow = (1d - blueDouble - black) / (1d - black);

			return new CmykColor(cyan, magenta, yellow, black);
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

		public static XyzColor RgbToXyz(Color color, double gamma, double[,] matrix)
		{
			return RgbToXyz(color.R, color.G, color.B, gamma, matrix);
		}

		public static XyzColor RgbToXyz(byte red, byte green, byte blue, double gamma, double[,] matrix)
		{
			double normalizedRed = Math.Pow(red / 255d, gamma);
			double normalizedGreen = Math.Pow(green / 255d, gamma);
			double normalizedBlue = Math.Pow(blue / 255d, gamma);

			double x = Clamp(matrix[0, 0] * normalizedRed + matrix[0, 1] * normalizedGreen + matrix[0, 2] * normalizedBlue, 0d, 1d);
			double y = Clamp(matrix[1, 0] * normalizedRed + matrix[1, 1] * normalizedGreen + matrix[1, 2] * normalizedBlue, 0d, 1d);
			double z = Clamp(matrix[2, 0] * normalizedRed + matrix[2, 1] * normalizedGreen + matrix[2, 2] * normalizedBlue, 0d, 1d);

			return new XyzColor(x, y, z);
		}

		public static XyzColor RgbToSRgb(Color color)
		{
			return RgbToSRgb(color.R, color.G, color.B);
		}

		public static XyzColor RgbToSRgb(byte red, byte green, byte blue)
		{
			double gamma = 2.2d;

			double normalizedRed = Math.Pow(red / 255d, gamma);
			double normalizedGreen = Math.Pow(green / 255d, gamma);
			double normalizedBlue = Math.Pow(blue / 255d, gamma);

			double x = Clamp(0.4124564d * normalizedRed + 0.3575761d * normalizedGreen + 0.1804375d * normalizedBlue, 0d, 1d);
			double y = Clamp(0.2126729d * normalizedRed + 0.7151522d * normalizedGreen + 0.072175d * normalizedBlue, 0d, 1d);
			double z = Clamp(0.0193339d * normalizedRed + 0.119192d * normalizedGreen + 0.9503041d * normalizedBlue, 0d, 1d);

			return new XyzColor(x, y, z);
		}

		public static XyzColor RgbToAdobeRgb(Color color)
		{
			return RgbToAdobeRgb(color.R, color.G, color.B);
		}

		public static XyzColor RgbToAdobeRgb(byte red, byte green, byte blue)
		{
			double gamma = 2.2d;

			double normalizedRed = Math.Pow(red / 255d, gamma);
			double normalizedGreen = Math.Pow(green / 255d, gamma);
			double normalizedBlue = Math.Pow(blue / 255d, gamma);

			double x = Clamp(0.5767309d * normalizedRed + 0.185554d * normalizedGreen + 0.1881852d * normalizedBlue, 0d, 1d);
			double y = Clamp(0.2973769d * normalizedRed + 0.6273491d * normalizedGreen + 0.0752741d * normalizedBlue, 0d, 1d);
			double z = Clamp(0.0270343d * normalizedRed + 0.0706872d * normalizedGreen + 0.9911085d * normalizedBlue, 0d, 1d);

			return new XyzColor(x, y, z);
		}

		public static XyzColor RgbToAppleRgb(Color color)
		{
			return RgbToAppleRgb(color.R, color.G, color.B);
		}

		public static XyzColor RgbToAppleRgb(byte red, byte green, byte blue)
		{
			double gamma = 2.2d;

			double normalizedRed = Math.Pow(red / 255d, gamma);
			double normalizedGreen = Math.Pow(green / 255d, gamma);
			double normalizedBlue = Math.Pow(blue / 255d, gamma);

			double x = Clamp(0.4497288d * normalizedRed + 0.3162486d * normalizedGreen + 0.1844926d * normalizedBlue, 0d, 1d);
			double y = Clamp(0.2446525d * normalizedRed + 0.6720283d * normalizedGreen + 0.0833192d * normalizedBlue, 0d, 1d);
			double z = Clamp(0.0251848d * normalizedRed + 0.1411824d * normalizedGreen + 0.9224628d * normalizedBlue, 0d, 1d);

			return new XyzColor(x, y, z);
		}

		public static XyzColor RgbToColorMatchRgb(Color color)
		{
			return RgbToColorMatchRgb(color.R, color.G, color.B);
		}

		public static XyzColor RgbToColorMatchRgb(byte red, byte green, byte blue)
		{
			double gamma = 2.2d;

			double normalizedRed = Math.Pow(red / 255d, gamma);
			double normalizedGreen = Math.Pow(green / 255d, gamma);
			double normalizedBlue = Math.Pow(blue / 255d, gamma);

			double x = Clamp(0.5093439d * normalizedRed + 0.3209071d * normalizedGreen + 0.1339691d * normalizedBlue, 0d, 1d);
			double y = Clamp(0.274884d * normalizedRed + 0.6581315d * normalizedGreen + 0.0669845d * normalizedBlue, 0d, 1d);
			double z = Clamp(0.0242545d * normalizedRed + 0.1087821d * normalizedGreen + 0.6921735d * normalizedBlue, 0d, 1d);

			return new XyzColor(x, y, z);
		}

		public static XyzColor RgbToProPhotoRgb(Color color)
		{
			return RgbToProPhotoRgb(color.R, color.G, color.B);
		}

		public static XyzColor RgbToProPhotoRgb(byte red, byte green, byte blue)
		{
			double gamma = 2.2d;

			double normalizedRed = Math.Pow(red / 255d, gamma);
			double normalizedGreen = Math.Pow(green / 255d, gamma);
			double normalizedBlue = Math.Pow(blue / 255d, gamma);

			double x = Clamp(0.7976749d * normalizedRed + 0.1351917d * normalizedGreen + 0.0313534d * normalizedBlue, 0d, 1d);
			double y = Clamp(0.2880402d * normalizedRed + 0.7118741d * normalizedGreen + 0.0000857d * normalizedBlue, 0d, 1d);
			double z = Clamp(0.82521d * normalizedBlue, 0d, 1d);

			return new XyzColor(x, y, z);
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

		public static Color XyzToRgb(XyzColor color, double gamma, double[,] matrix)
		{
			return XyzToRgb(color.X, color.Y, color.Z, gamma, matrix);
		}

		public static Color XyzToRgb(double x, double y, double z, double gamma, double[,] matrix)
		{
			double normalizedRed = Clamp(matrix[0, 0] * x - matrix[0, 1] * y - matrix[0, 2] * z, 0d, 1d);
			double normalizedGreen = Clamp(matrix[1, 0] * x + matrix[1, 1] * y + matrix[1, 2] * z, 0d, 1d);
			double normalizedBlue = Clamp(matrix[2, 0] * x - matrix[2, 1] * y + matrix[2, 2] * z, 0d, 1d);

			byte red = Calculations.ClampToByte(255d * Math.Pow(normalizedRed, 1d / gamma));
			byte green = Calculations.ClampToByte(255d * Math.Pow(normalizedGreen, 1d / gamma));
			byte blue = Calculations.ClampToByte(255d * Math.Pow(normalizedBlue, 1d / gamma));

			return Color.FromRgb(red, green, blue);
		}

		public static Color SRgbToRgb(XyzColor color)
		{
			return SRgbToRgb(color.X, color.Y, color.Z);
		}

		public static Color SRgbToRgb(double x, double y, double z)
		{
			double normalizedRed = Clamp(3.2404542d * x - 1.5371385d * y - 0.4985314d * z, 0d, 1d);
			double normalizedGreen = Clamp(-0.969266d * x + 1.8760108d * y + 0.041556d * z, 0d, 1d);
			double normalizedBlue = Clamp(0.0556434d * x - 0.2040259d * y + 1.0572252d * z, 0d, 1d);

			double gamma = 2.2d;

			byte red = Calculations.ClampToByte(255d * Math.Pow(normalizedRed, 1d / gamma));
			byte green = Calculations.ClampToByte(255d * Math.Pow(normalizedGreen, 1d / gamma));
			byte blue = Calculations.ClampToByte(255d * Math.Pow(normalizedBlue, 1d / gamma));

			return Color.FromRgb(red, green, blue);
		}

		public static Color AdobeRgbToRgb(XyzColor color)
		{
			return AdobeRgbToRgb(color.X, color.Y, color.Z);
		}

		public static Color AdobeRgbToRgb(double x, double y, double z)
		{
			double normalizedRed = Clamp(2.041369d * x - 0.5649464d * y - 0.3446944d * z, 0d, 1d);
			double normalizedGreen = Clamp(-0.969266d * x + 1.8760108d * y + 0.041556d * z, 0d, 1d);
			double normalizedBlue = Clamp(0.0134474d * x - 0.1183897d * y + 1.0154096d * z, 0d, 1d);

			double gamma = 2.2d;

			byte red = Calculations.ClampToByte(255d * Math.Pow(normalizedRed, 1d / gamma));
			byte green = Calculations.ClampToByte(255d * Math.Pow(normalizedGreen, 1d / gamma));
			byte blue = Calculations.ClampToByte(255d * Math.Pow(normalizedBlue, 1d / gamma));

			return Color.FromRgb(red, green, blue);
		}

		public static Color AppleRgbToRgb(XyzColor color)
		{
			return AppleRgbToRgb(color.X, color.Y, color.Z);
		}

		public static Color AppleRgbToRgb(double x, double y, double z)
		{
			double normalizedRed = Clamp(2.9515373d * x - 1.2894116d * y - 0.4738445d * z, 0d, 1d);
			double normalizedGreen = Clamp(-1.0851093d * x + 1.9908566d * y + 0.0372026d * z, 0d, 1d);
			double normalizedBlue = Clamp(0.0854934d * x - 0.2694964d * y + 1.0912975d * z, 0d, 1d);

			double gamma = 1.8d;

			byte red = Calculations.ClampToByte(255d * Math.Pow(normalizedRed, 1d / gamma));
			byte green = Calculations.ClampToByte(255d * Math.Pow(normalizedGreen, 1d / gamma));
			byte blue = Calculations.ClampToByte(255d * Math.Pow(normalizedBlue, 1d / gamma));

			return Color.FromRgb(red, green, blue);
		}

		public static Color ColorMatchRgbToRgb(XyzColor color)
		{
			return ColorMatchRgbToRgb(color.X, color.Y, color.Z);
		}

		public static Color ColorMatchRgbToRgb(double x, double y, double z)
		{
			double normalizedRed = Clamp(2.6422874d * x - 1.223427d * y - 0.3930143d * z, 0d, 1d);
			double normalizedGreen = Clamp(-1.1119763d * x + 2.0590183d * y + 0.0159614d * z, 0d, 1d);
			double normalizedBlue = Clamp(0.0821699d * x - 0.2807254d * y + 1.4559877d * z, 0d, 1d);

			double gamma = 1.8d;

			byte red = Calculations.ClampToByte(255d * Math.Pow(normalizedRed, 1d / gamma));
			byte green = Calculations.ClampToByte(255d * Math.Pow(normalizedGreen, 1d / gamma));
			byte blue = Calculations.ClampToByte(255d * Math.Pow(normalizedBlue, 1d / gamma));

			return Color.FromRgb(red, green, blue);
		}

		public static Color ProPhotoRgbToRgb(XyzColor color)
		{
			return ProPhotoRgbToRgb(color.X, color.Y, color.Z);
		}

		public static Color ProPhotoRgbToRgb(double x, double y, double z)
		{
			double normalizedRed = Clamp(1.3459433d * x - 0.2556075d * y - 0.0511118d * z, 0d, 1d);
			double normalizedGreen = Clamp(-0.5445989d * x + 1.5081673d * y + 0.0205351d * z, 0d, 1d);
			double normalizedBlue = Clamp(1.2118128d * z, 0d, 1d);

			double gamma = 1.8d;

			byte red = Calculations.ClampToByte(255d * Math.Pow(normalizedRed, 1d / gamma));
			byte green = Calculations.ClampToByte(255d * Math.Pow(normalizedGreen, 1d / gamma));
			byte blue = Calculations.ClampToByte(255d * Math.Pow(normalizedBlue, 1d / gamma));

			return Color.FromRgb(red, green, blue);
		}

		public static LabColor XyzToLab(XyzColor color,double whiteX, double whiteY, double whiteZ)
		{
			return XyzToLab(color.X, color.Y, color.Z, whiteX, whiteY, whiteZ);
		}

		public static LabColor XyzToLab(double x, double y, double z, double whiteX, double whiteY, double whiteZ)
		{
			double xw = x * whiteX;
			double yw = y * whiteY;
			double zw = z * whiteZ;

			double fx = xw > 0.008856d
				? Math.Pow(xw, 1d / 3d)
				: (903.3d * xw + 16d) / 116d;

			double fy = yw > 0.008856d
				? Math.Pow(yw, 1d / 3d)
				: (903.3d * yw + 16d) / 116d;

			double fz = zw > 0.008856d
				? Math.Pow(zw, 1d / 3d)
				: (903.3d * zw + 16d) / 116d;

			byte lightness = Calculations.ClampToByte(116d * fy - 16d);
			sbyte chromaA = Calculations.ClampToSByte(500d * (fx - fy));
			sbyte chromaB = Calculations.ClampToSByte(200d * (fy - fz));

			return new LabColor(lightness, chromaA, chromaB);
		}

		public static LabColor SRgbToLab(XyzColor color)
		{
			return SRgbToLab(color.X, color.Y, color.Z);
		}

		public static LabColor SRgbToLab(double x, double y, double z)
		{
			double xw = x / 0.95047d;
			double yw = y;
			double zw = z / 1.08883d;

			double fx = xw > 0.008856d
				? Math.Pow(xw, 1d / 3d)
				: (903.3d * xw + 16d) / 116d;

			double fy = yw > 0.008856d
				? Math.Pow(yw, 1d / 3d)
				: (903.3d * yw + 16d) / 116d;

			double fz = zw > 0.008856d
				? Math.Pow(zw, 1d / 3d)
				: (903.3d * zw + 16d) / 116d;

			byte lightness = Calculations.ClampToByte(116d * fy - 16d);
			sbyte chromaA = Calculations.ClampToSByte(500d * (fx - fy));
			sbyte chromaB = Calculations.ClampToSByte(200d * (fy - fz));

			return new LabColor(lightness, chromaA, chromaB);
		}

		public static LabColor AdobeRgbToLab(XyzColor color)
		{
			return AdobeRgbToLab(color.X, color.Y, color.Z);
		}

		public static LabColor AdobeRgbToLab(double x, double y, double z)
		{
			double xw = x / 0.95047d;
			double yw = y;
			double zw = z / 1.08883d;

			double fx = xw > 0.008856d
				? Math.Pow(xw, 1d / 3d)
				: (903.3d * xw + 16d) / 116d;

			double fy = yw > 0.008856d
				? Math.Pow(yw, 1d / 3d)
				: (903.3d * yw + 16d) / 116d;

			double fz = zw > 0.008856d
				? Math.Pow(zw, 1d / 3d)
				: (903.3d * zw + 16d) / 116d;

			byte lightness = Calculations.ClampToByte(116d * fy - 16d);
			sbyte chromaA = Calculations.ClampToSByte(500d * (fx - fy));
			sbyte chromaB = Calculations.ClampToSByte(200d * (fy - fz));

			return new LabColor(lightness, chromaA, chromaB);
		}

		public static LabColor AppleRgbToLab(XyzColor color)
		{
			return AppleRgbToLab(color.X, color.Y, color.Z);
		}

		public static LabColor AppleRgbToLab(double x, double y, double z)
		{
			double xw = x / 0.95047d;
			double yw = y;
			double zw = z / 1.08883d;

			double fx = xw > 0.008856d
				? Math.Pow(xw, 1d / 3d)
				: (903.3d * xw + 16d) / 116d;

			double fy = yw > 0.008856d
				? Math.Pow(yw, 1d / 3d)
				: (903.3d * yw + 16d) / 116d;

			double fz = zw > 0.008856d
				? Math.Pow(zw, 1d / 3d)
				: (903.3d * zw + 16d) / 116d;

			byte lightness = Calculations.ClampToByte(116d * fy - 16d);
			sbyte chromaA = Calculations.ClampToSByte(500d * (fx - fy));
			sbyte chromaB = Calculations.ClampToSByte(200d * (fy - fz));

			return new LabColor(lightness, chromaA, chromaB);
		}

		public static LabColor ColorMatchRgbToLab(XyzColor color)
		{
			return ColorMatchRgbToLab(color.X, color.Y, color.Z);
		}

		public static LabColor ColorMatchRgbToLab(double x, double y, double z)
		{
			double xw = x / 0.96422d;
			double yw = y;
			double zw = z / 0.82521d;

			double fx = xw > 0.008856d
				? Math.Pow(xw, 1d / 3d)
				: (903.3d * xw + 16d) / 116d;

			double fy = yw > 0.008856d
				? Math.Pow(yw, 1d / 3d)
				: (903.3d * yw + 16d) / 116d;

			double fz = zw > 0.008856d
				? Math.Pow(zw, 1d / 3d)
				: (903.3d * zw + 16d) / 116d;

			byte lightness = Calculations.ClampToByte(116d * fy - 16d);
			sbyte chromaA = Calculations.ClampToSByte(500d * (fx - fy));
			sbyte chromaB = Calculations.ClampToSByte(200d * (fy - fz));

			return new LabColor(lightness, chromaA, chromaB);
		}

		public static LabColor ProPhotoRgbToLab(XyzColor color)
		{
			return ProPhotoRgbToLab(color.X, color.Y, color.Z);
		}

		public static LabColor ProPhotoRgbToLab(double x, double y, double z)
		{
			double xw = x / 0.96422d;
			double yw = y;
			double zw = z / 0.82521d;

			double fx = xw > 0.008856d
				? Math.Pow(xw, 1d / 3d)
				: (903.3d * xw + 16d) / 116d;

			double fy = yw > 0.008856d
				? Math.Pow(yw, 1d / 3d)
				: (903.3d * yw + 16d) / 116d;

			double fz = zw > 0.008856d
				? Math.Pow(zw, 1d / 3d)
				: (903.3d * zw + 16d) / 116d;

			byte lightness = Calculations.ClampToByte(116d * fy - 16d);
			sbyte chromaA = Calculations.ClampToSByte(500d * (fx - fy));
			sbyte chromaB = Calculations.ClampToSByte(200d * (fy - fz));

			return new LabColor(lightness, chromaA, chromaB);
		}

		public static XyzColor LabToXyz(LabColor color, double whiteX, double whiteY, double whiteZ)
		{
			return LabToXyz(color.Lightness, color.ChromaA, color.ChromaB, whiteX, whiteY, whiteZ);
		}

		public static XyzColor LabToXyz(byte lightness, sbyte chromaA, sbyte chromaB, double whiteX, double whiteY, double whiteZ)
		{
			double fy = (lightness + 16d) / 116d;
			double fx = chromaA / 500d + fy;
			double fz = fy - chromaB / 200d;

			double fx3 = Math.Pow(fx, 3d);
			double fz3 = Math.Pow(fz, 3d);

			double xw = fx3 > 0.008856d
				? fx3
				: (116d * fx - 16d) / 903.3d;

			double yw = lightness >= 8
				? Math.Pow(fy, 3d)
				: lightness / 903.3d;

			double zw = fz3 > 0.008856d
				? fz3
				: (116d * fz - 16d) / 903.3d;

			double x = xw * whiteX;
			double y = yw * whiteY;
			double z = zw * whiteZ;

			return new XyzColor(x, y, z);
		}

		public static XyzColor LabToSRgb(LabColor color)
		{
			return LabToSRgb(color.Lightness, color.ChromaA, color.ChromaB);
		}

		public static XyzColor LabToSRgb(byte lightness, sbyte chromaA, sbyte chromaB)
		{
			double fy = (lightness + 16d) / 116d;
			double fx = chromaA / 500d + fy;
			double fz = fy - chromaB / 200d;

			double fx3 = Math.Pow(fx, 3d);
			double fz3 = Math.Pow(fz, 3d);

			double xw = fx3 > 0.008856d
				? fx3
				: (116d * fx - 16d) / 903.3d;

			double yw = lightness >= 8
				? Math.Pow(fy, 3d)
				: lightness / 903.3d;

			double zw = fz3 > 0.008856d
				? fz3
				: (116d * fz - 16d) / 903.3d;

			double x = xw * 0.95047d;
			double y = yw;
			double z = zw * 1.08883d;

			return new XyzColor(x, y, z);
		}

		public static XyzColor LabToAdobeRgb(LabColor color)
		{
			return LabToAdobeRgb(color.Lightness, color.ChromaA, color.ChromaB);
		}

		public static XyzColor LabToAdobeRgb(byte lightness, sbyte chromaA, sbyte chromaB)
		{
			double fy = (lightness + 16d) / 116d;
			double fx = chromaA / 500d + fy;
			double fz = fy - chromaB / 200d;

			double fx3 = Math.Pow(fx, 3d);
			double fz3 = Math.Pow(fz, 3d);

			double xw = fx3 > 0.008856d
				? fx3
				: (116d * fx - 16d) / 903.3d;

			double yw = lightness >= 8
				? Math.Pow(fy, 3d)
				: lightness / 903.3d;

			double zw = fz3 > 0.008856d
				? fz3
				: (116d * fz - 16d) / 903.3d;

			double x = xw * 0.95047d;
			double y = yw;
			double z = zw * 1.08883d;

			return new XyzColor(x, y, z);
		}

		public static XyzColor LabToAppleRgb(LabColor color)
		{
			return LabToAppleRgb(color.Lightness, color.ChromaA, color.ChromaB);
		}

		public static XyzColor LabToAppleRgb(byte lightness, sbyte chromaA, sbyte chromaB)
		{
			double fy = (lightness + 16d) / 116d;
			double fx = chromaA / 500d + fy;
			double fz = fy - chromaB / 200d;

			double fx3 = Math.Pow(fx, 3d);
			double fz3 = Math.Pow(fz, 3d);

			double xw = fx3 > 0.008856d
				? fx3
				: (116d * fx - 16d) / 903.3d;

			double yw = lightness >= 8
				? Math.Pow(fy, 3d)
				: lightness / 903.3d;

			double zw = fz3 > 0.008856d
				? fz3
				: (116d * fz - 16d) / 903.3d;

			double x = xw * 0.95047d;
			double y = yw;
			double z = zw * 1.08883d;

			return new XyzColor(x, y, z);
		}

		public static XyzColor LabToColorMatchRgb(LabColor color)
		{
			return LabToColorMatchRgb(color.Lightness, color.ChromaA, color.ChromaB);
		}

		public static XyzColor LabToColorMatchRgb(byte lightness, sbyte chromaA, sbyte chromaB)
		{
			double fy = (lightness + 16d) / 116d;
			double fx = chromaA / 500d + fy;
			double fz = fy - chromaB / 200d;

			double fx3 = Math.Pow(fx, 3d);
			double fz3 = Math.Pow(fz, 3d);

			double xw = fx3 > 0.008856d
				? fx3
				: (116d * fx - 16d) / 903.3d;

			double yw = lightness >=8
				? Math.Pow(fy, 3d)
				: lightness / 903.3d;

			double zw = fz3 > 0.008856d
				? fz3
				: (116d * fz - 16d) / 903.3d;

			double x = xw * 0.96422d;
			double y = yw;
			double z = zw * 0.82521d;

			return new XyzColor(x, y, z);
		}

		public static XyzColor LabToProPhotoRgb(LabColor color)
		{
			return LabToProPhotoRgb(color.Lightness, color.ChromaA, color.ChromaB);
		}

		public static XyzColor LabToProPhotoRgb(byte lightness, sbyte chromaA, sbyte chromaB)
		{
			double fy = (lightness + 16d) / 116d;
			double fx = chromaA / 500d + fy;
			double fz = fy - chromaB / 200d;

			double fx3 = Math.Pow(fx, 3d);
			double fz3 = Math.Pow(fz, 3d);

			double xw = fx3 > 0.008856d
				? fx3
				: (116d * fx - 16d) / 903.3d;

			double yw = lightness >= 8
				? Math.Pow(fy, 3d)
				: lightness / 903.3d;

			double zw = fz3 > 0.008856d
				? fz3
				: (116d * fz - 16d) / 903.3d;

			double x = xw * 0.96422d;
			double y = yw;
			double z = zw * 0.82521d;

			return new XyzColor(x, y, z);
		}

		private static double Clamp(double value, double min, double max)
		{
			return (value < min) ? min : (value > max) ? max : value;
		}
	}
}
