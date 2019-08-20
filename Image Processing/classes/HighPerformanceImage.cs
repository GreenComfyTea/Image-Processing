#define RTU_HSV_COMPONENTS

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageProcessing
{
	class HighPerformanceImage : IImage
	{
		public WriteableBitmap WriteableBmp { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public ByteArray CurrentArray { get; set; }
		public ImageModes ImageMode { get; set; }
		public ImageModes OriginalImageMode { get; set; }
		public HsvColor[,] HsvArray { get; set; }
		public CmykColor[,] CmykArray { get; set; }
		public YuvColor[,] YuvArray { get; set; }
		public ByteArray GrayscaleArray { get; set; }
		public ByteArray RgbArray { get; set; }
		public ByteArray RgbaArray { get; set; }
		public ByteArray RedToRgbArray { get; set; }
		public ByteArray GreenToRgbArray { get; set; }
		public ByteArray BlueToRgbArray { get; set; }
		public ByteArray AlphaToGrayscaleArray { get; set; }
		public ByteArray HsvToRgbArray { get; set; }
		public ByteArray HueToRgbArray { get; set; }
		public ByteArray SaturationToRgbArray { get; set; }
		public ByteArray ValueToRgbArray { get; set; }
		public ByteArray CmykToRgbArray { get; set; }
		public ByteArray CyanToRgbArray { get; set; }
		public ByteArray MagentaToRgbArray { get; set; }
		public ByteArray YellowToRgbArray { get; set; }
		public ByteArray BlackToRgbArray { get; set; }
		public ByteArray YuvToRgbArray { get; set; }
		public ByteArray LumaToRgbArray { get; set; }
		public ByteArray ColorDifferenceUToRgbArray { get; set; }
		public ByteArray ColorDifferenceVToRgbArray { get; set; }
		public Histogram ImageHistogram { get; set; }

		public HighPerformanceImage(WriteableBitmap writeableBitmap)
		{
			WriteableBmp = writeableBitmap;
			Width = WriteableBmp.PixelWidth;
			Height = WriteableBmp.PixelHeight;
			int channels = ((WriteableBmp.Format.BitsPerPixel + 7) / 8);

			ByteArray byteArray = new ByteArray(Width, Height, channels);
			WriteableBmp.CopyPixels(byteArray.Array, byteArray.Stride, 0);

			switch (channels)
			{
				case 1:
					OriginalImageMode = ImageMode = ImageModes.Grayscale;
					CurrentArray = GrayscaleArray = byteArray;
					RgbArray = new ByteArray(Width, Height, 3);
					RgbaArray = new ByteArray(Width, Height, 4);
					break;
				case 3:
					OriginalImageMode = ImageMode = ImageModes.RGB;
					GrayscaleArray = new ByteArray(Width, Height, 1);
					CurrentArray = RgbArray = byteArray;
					RgbaArray = new ByteArray(Width, Height, 4);
					break;
				case 4:
					OriginalImageMode = ImageMode = ImageModes.RGBa;
					GrayscaleArray = new ByteArray(Width, Height, 1);
					RgbArray = new ByteArray(Width, Height, 3);
					CurrentArray = RgbaArray = byteArray;
					break;
			}

			HsvArray = new HsvColor[Width, Height];
			CmykArray = new CmykColor[Width, Height];
			YuvArray = new YuvColor[Width, Height];

			RedToRgbArray = new ByteArray(Width, Height, 3);
			GreenToRgbArray = new ByteArray(Width, Height, 3);
			BlueToRgbArray = new ByteArray(Width, Height, 3);
			AlphaToGrayscaleArray = new ByteArray(Width, Height, 1);
			HsvToRgbArray = new ByteArray(Width, Height, 3);
			HueToRgbArray = new ByteArray(Width, Height, 3);
			SaturationToRgbArray = new ByteArray(Width, Height, 3);
			ValueToRgbArray = new ByteArray(Width, Height, 3);
			CmykToRgbArray = new ByteArray(Width, Height, 3);
			CyanToRgbArray = new ByteArray(Width, Height, 3);
			MagentaToRgbArray = new ByteArray(Width, Height, 3);
			YellowToRgbArray = new ByteArray(Width, Height, 3);
			BlackToRgbArray = new ByteArray(Width, Height, 3);
			YuvToRgbArray = new ByteArray(Width, Height, 3);
			LumaToRgbArray = new ByteArray(Width, Height, 3);
			ColorDifferenceUToRgbArray = new ByteArray(Width, Height, 3);
			ColorDifferenceVToRgbArray = new ByteArray(Width, Height, 3);

			ImageHistogram = new Histogram(this);
		}

		public void CalculateColorspaces(ImageModes imageMode)
		{
			int[] nums;

			switch (imageMode)
			{
				case ImageModes.Grayscale:
					nums = Enumerable.Range(0, GrayscaleArray.Array.Length).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = i % Width;
						int y = i / Width;

						int index3 = y * RgbArray.Stride + x * RgbArray.Channels;
						int index4 = y * RgbaArray.Stride + x * RgbaArray.Channels;

						byte intensity = GrayscaleArray.Array[i];

						HsvColor hsv = ColorConverter.RgbToHsv(intensity, intensity, intensity);
						CmykColor cmyk = ColorConverter.RgbToCmyk(intensity, intensity, intensity);
						YuvColor yuv = ColorConverter.RgbToYuv(intensity, intensity, intensity);

						HsvArray[x, y] = hsv;
						CmykArray[x, y] = cmyk;
						YuvArray[x, y] = yuv;

						Color hsvToRgb = ColorConverter.HsvToRgb(hsv);
						Color cmykToRgb = ColorConverter.CmykToRgb(cmyk);
						Color yuvToRgb = ColorConverter.YuvToRgb(yuv);

#if (RTU_HSV_COMPONENTS)
						Color hueToRgb = ColorConverter.HsvToRgb(hsv.Hue, 1d, 1d);
						Color saturationToRgb = ColorConverter.SaturationToRgb(hsv.Saturation);
						Color valueToRgb = ColorConverter.HsvToRgb(0d, 0d, hsv.Value);
#else
						Color hueToRgb = ColorConverter.HsvToRgb(hsv.Hue, 1d, 1d);
						Color saturationToRgb = ColorConverter.HsvToRgb(0d, hsv.Saturation, 1d);
						Color valueToRgb = ColorConverter.HsvToRgb(0d, 1d, hsv.Value);
#endif

						Color cyanToRgb = ColorConverter.CmykToRgb(cmyk.Cyan, 0d, 0d, 0d);
						Color magentaToRgb = ColorConverter.CmykToRgb(0d, cmyk.Magenta, 0d, 0d);
						Color yellowToRgb = ColorConverter.CmykToRgb(0d, 0d, cmyk.Yellow, 0d);
						Color blackToRgb = ColorConverter.CmykToRgb(0d, 0d, 0d, cmyk.Black);

						Color lumaToRgb = ColorConverter.YuvToRgb(yuv.Luma, 0d, 0d);
						Color colorDifferenceUToRgb = ColorConverter.YuvToRgb(0d, yuv.ColorDifferenceU, 0d);
						Color colorDifferenceVToRgb = ColorConverter.YuvToRgb(0d, 0d, yuv.ColorDifferenceV);

						AlphaToGrayscaleArray.Array[i] = 255;

						RgbArray.Array[index3] = intensity;
						RedToRgbArray.Array[index3] = 0;
						GreenToRgbArray.Array[index3] = 0;
						BlueToRgbArray.Array[index3] = intensity;
						HsvToRgbArray.Array[index3] = hsvToRgb.B;
						HueToRgbArray.Array[index3] = hueToRgb.B;
						SaturationToRgbArray.Array[index3] = saturationToRgb.B;
						ValueToRgbArray.Array[index3] = valueToRgb.B;
						CmykToRgbArray.Array[index3] = cmykToRgb.B;
						CyanToRgbArray.Array[index3] = cyanToRgb.B;
						MagentaToRgbArray.Array[index3] = magentaToRgb.B;
						YellowToRgbArray.Array[index3] = yellowToRgb.B;
						BlackToRgbArray.Array[index3] = blackToRgb.B;
						YuvToRgbArray.Array[index3] = yuvToRgb.B;
						LumaToRgbArray.Array[index3] = lumaToRgb.B;
						ColorDifferenceUToRgbArray.Array[index3] = colorDifferenceUToRgb.B;
						ColorDifferenceVToRgbArray.Array[index3] = colorDifferenceVToRgb.B;

						index3++;

						RgbArray.Array[index3] = intensity;
						RedToRgbArray.Array[index3] = 0;
						GreenToRgbArray.Array[index3] = intensity;
						BlueToRgbArray.Array[index3] = 0;
						HsvToRgbArray.Array[index3] = hsvToRgb.G;
						HueToRgbArray.Array[index3] = hueToRgb.G;
						SaturationToRgbArray.Array[index3] = saturationToRgb.G;
						ValueToRgbArray.Array[index3] = valueToRgb.G;
						CmykToRgbArray.Array[index3] = cmykToRgb.G;
						CyanToRgbArray.Array[index3] = cyanToRgb.G;
						MagentaToRgbArray.Array[index3] = magentaToRgb.G;
						YellowToRgbArray.Array[index3] = yellowToRgb.G;
						BlackToRgbArray.Array[index3] = blackToRgb.G;
						YuvToRgbArray.Array[index3] = yuvToRgb.G;
						LumaToRgbArray.Array[index3] = lumaToRgb.G;
						ColorDifferenceUToRgbArray.Array[index3] = colorDifferenceUToRgb.G;
						ColorDifferenceVToRgbArray.Array[index3] = colorDifferenceVToRgb.G;

						index3++;

						RgbArray.Array[index3] = intensity;
						RedToRgbArray.Array[index3] = intensity;
						GreenToRgbArray.Array[index3] = 0;
						BlueToRgbArray.Array[index3] = 0;
						HsvToRgbArray.Array[index3] = hsvToRgb.R;
						HueToRgbArray.Array[index3] = hueToRgb.R;
						SaturationToRgbArray.Array[index3] = saturationToRgb.R;
						ValueToRgbArray.Array[index3] = valueToRgb.R;
						CmykToRgbArray.Array[index3] = cmykToRgb.R;
						CyanToRgbArray.Array[index3] = cyanToRgb.R;
						MagentaToRgbArray.Array[index3] = magentaToRgb.R;
						YellowToRgbArray.Array[index3] = yellowToRgb.R;
						BlackToRgbArray.Array[index3] = blackToRgb.R;
						YuvToRgbArray.Array[index3] = yuvToRgb.R;
						LumaToRgbArray.Array[index3] = lumaToRgb.R;
						ColorDifferenceUToRgbArray.Array[index3] = colorDifferenceUToRgb.R;
						ColorDifferenceVToRgbArray.Array[index3] = colorDifferenceVToRgb.R;

						RgbaArray.Array[index4] = intensity;
						index4++;
						RgbaArray.Array[index4] = intensity;
						index4++;
						RgbaArray.Array[index4] = intensity;
						index4++;
						RgbaArray.Array[index4] = 255;
					});
					break;
				case ImageModes.RGB:
					nums = Enumerable.Range(0, RgbArray.Array.Length / RgbArray.Channels).Select(i => i * RgbArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbArray.Channels) % Width;
						int y = i / (Width * RgbArray.Channels);

						int index1 = y * GrayscaleArray.Stride + x * GrayscaleArray.Channels;
						int index4 = y * RgbaArray.Stride + x * RgbaArray.Channels;

						byte blue = RgbArray.Array[i];
						byte green = RgbArray.Array[i + 1];
						byte red = RgbArray.Array[i + 2];

						byte intensity = ColorConverter.RgbToGrayscale(red, green, blue);

						HsvColor hsv = ColorConverter.RgbToHsv(red, green, blue);
						CmykColor cmyk = ColorConverter.RgbToCmyk(red, green, blue);
						YuvColor yuv = ColorConverter.RgbToYuv(red, green, blue);

						HsvArray[x, y] = hsv;
						CmykArray[x, y] = cmyk;
						YuvArray[x, y] = yuv;

						Color hsvToRgb = ColorConverter.HsvToRgb(hsv);
						Color cmykToRgb = ColorConverter.CmykToRgb(cmyk);
						Color yuvToRgb = ColorConverter.YuvToRgb(yuv);

#if (RTU_HSV_COMPONENTS)
						Color hueToRgb = ColorConverter.HsvToRgb(hsv.Hue, 1d, 1d);
						Color saturationToRgb = ColorConverter.SaturationToRgb(hsv.Saturation);
						Color valueToRgb = ColorConverter.HsvToRgb(0d, 0d, hsv.Value);
#else
						Color hueToRgb = ColorConverter.HsvToRgb(hsv.Hue, 1d, 1d);
						Color saturationToRgb = ColorConverter.HsvToRgb(0d, hsv.Saturation, 1d);
						Color valueToRgb = ColorConverter.HsvToRgb(0d, 1d, hsv.Value);
#endif

						Color cyanToRgb = ColorConverter.CmykToRgb(cmyk.Cyan, 0d, 0d, 0d);
						Color magentaToRgb = ColorConverter.CmykToRgb(0d, cmyk.Magenta, 0d, 0d);
						Color yellowToRgb = ColorConverter.CmykToRgb(0d, 0d, cmyk.Yellow, 0d);
						Color blackToRgb = ColorConverter.CmykToRgb(0d, 0d, 0d, cmyk.Black);

						Color lumaToRgb = ColorConverter.YuvToRgb(yuv.Luma, 0d, 0d);
						Color colorDifferenceUToRgb = ColorConverter.YuvToRgb(0d, yuv.ColorDifferenceU, 0d);
						Color colorDifferenceVToRgb = ColorConverter.YuvToRgb(0d, 0d, yuv.ColorDifferenceV);

						AlphaToGrayscaleArray.Array[index1] = 255;
						GrayscaleArray.Array[index1] = intensity;

						RedToRgbArray.Array[i] = 0;
						GreenToRgbArray.Array[i] = 0;
						BlueToRgbArray.Array[i] = blue;
						HsvToRgbArray.Array[i] = hsvToRgb.B;
						HueToRgbArray.Array[i] = hueToRgb.B;
						SaturationToRgbArray.Array[i] = saturationToRgb.B;
						ValueToRgbArray.Array[i] = valueToRgb.B;
						CmykToRgbArray.Array[i] = cmykToRgb.B;
						CyanToRgbArray.Array[i] = cyanToRgb.B;
						MagentaToRgbArray.Array[i] = magentaToRgb.B;
						YellowToRgbArray.Array[i] = yellowToRgb.B;
						BlackToRgbArray.Array[i] = blackToRgb.B;
						YuvToRgbArray.Array[i] = yuvToRgb.B;
						LumaToRgbArray.Array[i] = lumaToRgb.B;
						ColorDifferenceUToRgbArray.Array[i] = colorDifferenceUToRgb.B;
						ColorDifferenceVToRgbArray.Array[i] = colorDifferenceVToRgb.B;

						i++;

						RedToRgbArray.Array[i] = 0;
						GreenToRgbArray.Array[i] = green;
						BlueToRgbArray.Array[i] = 0;
						HsvToRgbArray.Array[i] = hsvToRgb.G;
						HueToRgbArray.Array[i] = hueToRgb.G;
						SaturationToRgbArray.Array[i] = saturationToRgb.G;
						ValueToRgbArray.Array[i] = valueToRgb.G;
						CmykToRgbArray.Array[i] = cmykToRgb.G;
						CyanToRgbArray.Array[i] = cyanToRgb.G;
						MagentaToRgbArray.Array[i] = magentaToRgb.G;
						YellowToRgbArray.Array[i] = yellowToRgb.G;
						BlackToRgbArray.Array[i] = blackToRgb.G;
						YuvToRgbArray.Array[i] = yuvToRgb.G;
						LumaToRgbArray.Array[i] = lumaToRgb.G;
						ColorDifferenceUToRgbArray.Array[i] = colorDifferenceUToRgb.G;
						ColorDifferenceVToRgbArray.Array[i] = colorDifferenceVToRgb.G;

						i++;

						RedToRgbArray.Array[i] = red;
						GreenToRgbArray.Array[i] = 0;
						BlueToRgbArray.Array[i] = 0;
						HsvToRgbArray.Array[i] = hsvToRgb.R;
						HueToRgbArray.Array[i] = hueToRgb.R;
						SaturationToRgbArray.Array[i] = saturationToRgb.R;
						ValueToRgbArray.Array[i] = valueToRgb.R;
						CmykToRgbArray.Array[i] = cmykToRgb.R;
						CyanToRgbArray.Array[i] = cyanToRgb.R;
						MagentaToRgbArray.Array[i] = magentaToRgb.R;
						YellowToRgbArray.Array[i] = yellowToRgb.R;
						BlackToRgbArray.Array[i] = blackToRgb.R;
						YuvToRgbArray.Array[i] = yuvToRgb.R;
						LumaToRgbArray.Array[i] = lumaToRgb.R;
						ColorDifferenceUToRgbArray.Array[i] = colorDifferenceUToRgb.R;
						ColorDifferenceVToRgbArray.Array[i] = colorDifferenceVToRgb.R;

						RgbaArray.Array[index4] = blue;
						index4++;
						RgbaArray.Array[index4] = green;
						index4++;
						RgbaArray.Array[index4] = red;
						index4++;
						RgbaArray.Array[index4] = 255;
					});
					break;
				case ImageModes.RGBa:
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index1 = y * GrayscaleArray.Stride + x * GrayscaleArray.Channels;
						int index3 = y * RgbArray.Stride + x * RgbArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];
						byte alpha = RgbaArray.Array[i + 3];

						byte intensity = ColorConverter.RgbToGrayscale(red, green, blue);

						HsvColor hsv = ColorConverter.RgbToHsv(red, green, blue);
						CmykColor cmyk = ColorConverter.RgbToCmyk(red, green, blue);
						YuvColor yuv = ColorConverter.RgbToYuv(red, green, blue);

						HsvArray[x, y] = hsv;
						CmykArray[x, y] = cmyk;
						YuvArray[x, y] = yuv;

						Color hsvToRgb = ColorConverter.HsvToRgb(hsv);
						Color cmykToRgb = ColorConverter.CmykToRgb(cmyk);
						Color yuvToRgb = ColorConverter.YuvToRgb(yuv);

#if (RTU_HSV_COMPONENTS)
						Color hueToRgb = ColorConverter.HsvToRgb(hsv.Hue, 1d, 1d);
						Color saturationToRgb = ColorConverter.SaturationToRgb(hsv.Saturation);
						Color valueToRgb = ColorConverter.HsvToRgb(0d, 0d, hsv.Value);
#else
						Color hueToRgb = ColorConverter.HsvToRgb(hsv.Hue, 1d, 1d);
						Color saturationToRgb = ColorConverter.HsvToRgb(0d, hsv.Saturation, 1d);
						Color valueToRgb = ColorConverter.HsvToRgb(0d, 1d, hsv.Value);
#endif

						Color cyanToRgb = ColorConverter.CmykToRgb(cmyk.Cyan, 0d, 0d, 0d);
						Color magentaToRgb = ColorConverter.CmykToRgb(0d, cmyk.Magenta, 0d, 0d);
						Color yellowToRgb = ColorConverter.CmykToRgb(0d, 0d, cmyk.Yellow, 0d);
						Color blackToRgb = ColorConverter.CmykToRgb(0d, 0d, 0d, cmyk.Black);

						Color lumaToRgb = ColorConverter.YuvToRgb(yuv.Luma, 0d, 0d);
						Color colorDifferenceUToRgb = ColorConverter.YuvToRgb(0, yuv.ColorDifferenceU, 0d);
						Color colorDifferenceVToRgb = ColorConverter.YuvToRgb(0d, 0d, yuv.ColorDifferenceV);

						AlphaToGrayscaleArray.Array[index1] = alpha;
						GrayscaleArray.Array[index1] = intensity;

						RgbArray.Array[index3] = blue;
						RedToRgbArray.Array[index3] = 0;
						GreenToRgbArray.Array[index3] = 0;
						BlueToRgbArray.Array[index3] = blue;
						HsvToRgbArray.Array[index3] = hsvToRgb.B;
						HueToRgbArray.Array[index3] = hueToRgb.B;
						SaturationToRgbArray.Array[index3] = saturationToRgb.B;
						ValueToRgbArray.Array[index3] = valueToRgb.B;
						CmykToRgbArray.Array[index3] = cmykToRgb.B;
						CyanToRgbArray.Array[index3] = cyanToRgb.B;
						MagentaToRgbArray.Array[index3] = magentaToRgb.B;
						YellowToRgbArray.Array[index3] = yellowToRgb.B;
						BlackToRgbArray.Array[index3] = blackToRgb.B;
						YuvToRgbArray.Array[index3] = yuvToRgb.B;
						LumaToRgbArray.Array[index3] = lumaToRgb.B;
						ColorDifferenceUToRgbArray.Array[index3] = colorDifferenceUToRgb.B;
						ColorDifferenceVToRgbArray.Array[index3] = colorDifferenceVToRgb.B;

						index3++;

						RgbArray.Array[index3] = green;
						RedToRgbArray.Array[index3] = 0;
						GreenToRgbArray.Array[index3] = green;
						BlueToRgbArray.Array[index3] = 0;
						HsvToRgbArray.Array[index3] = hsvToRgb.G;
						HueToRgbArray.Array[index3] = hueToRgb.G;
						SaturationToRgbArray.Array[index3] = saturationToRgb.G;
						ValueToRgbArray.Array[index3] = valueToRgb.G;
						CmykToRgbArray.Array[index3] = cmykToRgb.G;
						CyanToRgbArray.Array[index3] = cyanToRgb.G;
						MagentaToRgbArray.Array[index3] = magentaToRgb.G;
						YellowToRgbArray.Array[index3] = yellowToRgb.G;
						BlackToRgbArray.Array[index3] = blackToRgb.G;
						YuvToRgbArray.Array[index3] = yuvToRgb.G;
						LumaToRgbArray.Array[index3] = lumaToRgb.G;
						ColorDifferenceUToRgbArray.Array[index3] = colorDifferenceUToRgb.G;
						ColorDifferenceVToRgbArray.Array[index3] = colorDifferenceVToRgb.G;

						index3++;

						RgbArray.Array[index3] = red;
						RedToRgbArray.Array[index3] = red;
						GreenToRgbArray.Array[index3] = 0;
						BlueToRgbArray.Array[index3] = 0;
						HsvToRgbArray.Array[index3] = hsvToRgb.R;
						HueToRgbArray.Array[index3] = hueToRgb.R;
						SaturationToRgbArray.Array[index3] = saturationToRgb.R;
						ValueToRgbArray.Array[index3] = valueToRgb.R;
						CmykToRgbArray.Array[index3] = cmykToRgb.R;
						CyanToRgbArray.Array[index3] = cyanToRgb.R;
						MagentaToRgbArray.Array[index3] = magentaToRgb.R;
						YellowToRgbArray.Array[index3] = yellowToRgb.R;
						BlackToRgbArray.Array[index3] = blackToRgb.R;
						YuvToRgbArray.Array[index3] = yuvToRgb.R;
						LumaToRgbArray.Array[index3] = lumaToRgb.R;
						ColorDifferenceUToRgbArray.Array[index3] = colorDifferenceUToRgb.R;
						ColorDifferenceVToRgbArray.Array[index3] = colorDifferenceVToRgb.R;
					});
					break;
			}

			ImageHistogram.UpdateHistogram();
		}

		public byte GetIntensity(int x, int y)
		{
			return GrayscaleArray.GetPixelColor(x, y).R;
		}

		public Color GetRgbaColor(int x, int y)
		{
			return RgbaArray.GetPixelColor(x, y);
		}

		public Color GetConvertedRgbaColor(int x, int y)
		{
			return CurrentArray.GetPixelColor(x, y);
		}

		public HsvColor GetHsvColor(int x, int y)
		{
			return HsvArray[x, y];
		}

		public CmykColor GetCmykColor(int x, int y)
		{
			return CmykArray[x, y];
		}

		public YuvColor GetYuvColor(int x, int y)
		{
			return YuvArray[x, y];
		}

		public void SetImageMode(ImageModes imageMode)
		{
			ImageMode = imageMode;

			switch (imageMode)
			{
				case ImageModes.Grayscale:
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Gray8, null, GrayscaleArray.Array, GrayscaleArray.Stride));
					CurrentArray = GrayscaleArray;
					break;
				case ImageModes.RGBa:
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgra32, null, RgbaArray.Array, RgbaArray.Stride));
					CurrentArray = RgbaArray;
					break;
				case ImageModes.RGB:
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, RgbArray.Array, RgbArray.Stride));
					CurrentArray = RgbArray;
					break;
				case ImageModes.Red:
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, RedToRgbArray.Array, RedToRgbArray.Stride));
					CurrentArray = RedToRgbArray;
					break;
				case ImageModes.Green:
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, GreenToRgbArray.Array, GreenToRgbArray.Stride));
					CurrentArray = GreenToRgbArray;
					break;
				case ImageModes.Blue:
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, BlueToRgbArray.Array, BlueToRgbArray.Stride));
					CurrentArray = BlueToRgbArray;
					break;
				case ImageModes.Alpha:
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Gray8, null, AlphaToGrayscaleArray.Array, AlphaToGrayscaleArray.Stride));
					CurrentArray = AlphaToGrayscaleArray;
					break;
				case ImageModes.HSV:
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, HsvToRgbArray.Array, HsvToRgbArray.Stride));
					CurrentArray = HsvToRgbArray;
					break;
				case ImageModes.Hue:
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, HueToRgbArray.Array, HueToRgbArray.Stride));
					CurrentArray = HueToRgbArray;
					break;
				case ImageModes.Saturation:
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, SaturationToRgbArray.Array, SaturationToRgbArray.Stride));
					CurrentArray = SaturationToRgbArray;
					break;
				case ImageModes.Value:
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, ValueToRgbArray.Array, ValueToRgbArray.Stride));
					CurrentArray = ValueToRgbArray;
					break;
				case ImageModes.CMYK:
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CmykToRgbArray.Array, CmykToRgbArray.Stride));
					CurrentArray = CmykToRgbArray;
					break;
				case ImageModes.Cyan:
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CyanToRgbArray.Array, CyanToRgbArray.Stride));
					CurrentArray = CyanToRgbArray;
					break;
				case ImageModes.Magenta:
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, MagentaToRgbArray.Array, MagentaToRgbArray.Stride));
					CurrentArray = MagentaToRgbArray;
					break;
				case ImageModes.Yellow:
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, YellowToRgbArray.Array, YellowToRgbArray.Stride));
					CurrentArray = YellowToRgbArray;
					break;
				case ImageModes.Black:
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, BlackToRgbArray.Array, BlackToRgbArray.Stride));
					CurrentArray = BlackToRgbArray;
					break;
				case ImageModes.YUV:
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, YuvToRgbArray.Array, YuvToRgbArray.Stride));
					CurrentArray = YuvToRgbArray;
					break;
				case ImageModes.Luma:
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, LumaToRgbArray.Array, LumaToRgbArray.Stride));
					CurrentArray = LumaToRgbArray;
					break;
				case ImageModes.ColorDifferenceU:
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, ColorDifferenceUToRgbArray.Array, ColorDifferenceUToRgbArray.Stride));
					CurrentArray = ColorDifferenceUToRgbArray;
					break;
				case ImageModes.ColorDifferenceV:
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, ColorDifferenceVToRgbArray.Array, ColorDifferenceVToRgbArray.Stride));
					CurrentArray = ColorDifferenceVToRgbArray;
					break;
			}
		}

		public void StretchHistogram(HistogramModes histogramMode)
		{
			int[] nums = Enumerable.Range(0, RgbArray.Array.Length / RgbArray.Channels).Select(i => i * RgbArray.Channels).ToArray();

			switch (histogramMode)
			{
				case HistogramModes.Intensity:
					Parallel.ForEach(nums, i =>
					{
						byte blue = RgbArray.Array[i];
						byte green = RgbArray.Array[i + 1];
						byte red = RgbArray.Array[i + 2];

						RgbArray.Array[i] = Calculations.AffineTransformationByte(blue, ImageHistogram.BlueMin, ImageHistogram.BlueMax, 0, 255);
						RgbArray.Array[i + 1] = Calculations.AffineTransformationByte(green, ImageHistogram.GreenMin, ImageHistogram.GreenMax, 0, 255);
						RgbArray.Array[i + 2] = Calculations.AffineTransformationByte(red, ImageHistogram.RedMin, ImageHistogram.RedMax, 0, 255);
					});
					break;
				case HistogramModes.Red:
					Parallel.ForEach(nums, i =>
					{
						byte red = RgbArray.Array[i + 2];

						RgbArray.Array[i + 2] = Calculations.AffineTransformationByte(red, ImageHistogram.RedMin, ImageHistogram.RedMax, 0, 255);
					});
					break;
				case HistogramModes.Green:
					Parallel.ForEach(nums, i =>
					{
						byte green = RgbArray.Array[i + 1];

						RgbArray.Array[i + 1] = Calculations.AffineTransformationByte(green, ImageHistogram.GreenMin, ImageHistogram.GreenMax, 0, 255);
					});
					break;
				case HistogramModes.Blue:
					Parallel.ForEach(nums, i =>
					{
						byte blue = RgbArray.Array[i];

						RgbArray.Array[i] = Calculations.AffineTransformationByte(blue, ImageHistogram.BlueMin, ImageHistogram.BlueMax, 0, 255);
					});
					break;
			}

			CalculateColorspaces(ImageModes.RGB);
		}

		public void PassFilter(float[,] filterMatrix, float filterCoefficient = 0)
		{
			int filterWidth = filterMatrix.GetLength(0);
			int filterHeight = filterMatrix.GetLength(1);

			if (filterWidth != filterHeight || filterWidth % 2 == 0)
			{
				return;
			}

			if (filterCoefficient == 0)
			{
				for (int i = 0; i < filterWidth; i++)
				{
					for (int j = 0; j < filterHeight; j++)
					{
						filterCoefficient += filterMatrix[i, j];
					}
				}
			}

			if(filterCoefficient == 0)
			{
				return;
			}

			int shift = filterWidth / 2;

			int[] nums;
			switch (OriginalImageMode)
			{
				case ImageModes.Grayscale:
					ByteArray tempGrayscaleArray = new ByteArray(GrayscaleArray.Width, GrayscaleArray.Height, GrayscaleArray.Channels);
					nums = Enumerable.Range(0, GrayscaleArray.Array.Length).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = i % Width;
						int y = i / Width;

						if (x < shift || y < shift || x >= Width - shift || y >= Height - shift)
						{
							tempGrayscaleArray.Array[i] = GrayscaleArray.Array[i];
							return;
						}

						float newIntensity = 0;

						for (int j = 0; j < filterWidth; j++)
						{
							for (int k = 0; k < filterHeight; k++)
							{
								int index = (y + k - shift) * GrayscaleArray.Stride + (x + j - shift);
								newIntensity += filterMatrix[j, k] * GrayscaleArray.Array[index];
							}
						}

						newIntensity /= filterCoefficient;

						tempGrayscaleArray.Array[i] = Calculations.ClampToByte(newIntensity);
					});
					GrayscaleArray = tempGrayscaleArray;
					CalculateColorspaces(ImageModes.Grayscale);
					break;
				case ImageModes.RGB:
					ByteArray tempRgbArray = new ByteArray(RgbArray.Width, RgbArray.Height, RgbArray.Channels);
					nums = Enumerable.Range(0, RgbArray.Array.Length / RgbArray.Channels).Select(i => i * RgbArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbArray.Channels) % Width;
						int y = i / (Width * RgbArray.Channels);

						if (x < shift || y < shift || x >= Width - shift || y >= Height - shift)
						{
							tempRgbArray.Array[i] = RgbArray.Array[i];
							tempRgbArray.Array[i + 1] = RgbArray.Array[i + 1];
							tempRgbArray.Array[i + 2] = RgbArray.Array[i + 2];
							return;
						}

						float newRed = 0;
						float newGreen = 0;
						float newBlue = 0;

						for (int j = 0; j < filterWidth; j++)
						{
							for (int k = 0; k < filterHeight; k++)
							{
								int index = (y + k - shift) * RgbArray.Stride + (x + j - shift) * RgbArray.Channels;
								newBlue += filterMatrix[j, k] * RgbArray.Array[index];
								newGreen += filterMatrix[j, k] * RgbArray.Array[index + 1];
								newRed += filterMatrix[j, k] * RgbArray.Array[index + 2];
							}
						}

						newBlue /= filterCoefficient;
						newGreen /= filterCoefficient;
						newRed /= filterCoefficient;

						tempRgbArray.Array[i] = Calculations.ClampToByte(newBlue);
						tempRgbArray.Array[i + 1] = Calculations.ClampToByte(newGreen);
						tempRgbArray.Array[i + 2] = Calculations.ClampToByte(newRed);
					});
					RgbArray = tempRgbArray;
					CalculateColorspaces(ImageModes.RGB);
					break;
				case ImageModes.RGBa:
					ByteArray tempRgbaArray = new ByteArray(RgbaArray.Width, RgbaArray.Height, RgbaArray.Channels);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						if (x < shift || y < shift || x >= Width - shift || y >= Height - shift)
						{
							tempRgbaArray.Array[i] = RgbaArray.Array[i];
							tempRgbaArray.Array[i + 1] = RgbaArray.Array[i + 1];
							tempRgbaArray.Array[i + 2] = RgbaArray.Array[i + 2];
							tempRgbaArray.Array[i + 3] = RgbaArray.Array[i + 3];
							return;
						}

						float newRed = 0;
						float newGreen = 0;
						float newBlue = 0;

						for (int j = 0; j < filterWidth; j++)
						{
							for (int k = 0; k < filterHeight; k++)
							{
								int index = (y + k - shift) * RgbaArray.Stride + (x + j - shift) * RgbaArray.Channels;
								newBlue += filterMatrix[j, k] * RgbaArray.Array[index];
								newGreen += filterMatrix[j, k] * RgbaArray.Array[index + 1];
								newRed += filterMatrix[j, k] * RgbaArray.Array[index + 2];
							}
						}

						newBlue /= filterCoefficient;
						newGreen /= filterCoefficient;
						newRed /= filterCoefficient;

						tempRgbaArray.Array[i] = Calculations.ClampToByte(newBlue);
						tempRgbaArray.Array[i + 1] = Calculations.ClampToByte(newGreen);
						tempRgbaArray.Array[i + 2] = Calculations.ClampToByte(newRed);
						tempRgbaArray.Array[i + 3] = RgbaArray.Array[i + 3];
					});
					RgbaArray = tempRgbaArray;
					CalculateColorspaces(ImageModes.RGBa);
					break;
			}

			ImageHistogram.UpdateHistogram();
		}

		public void LowPassFilter1()
		{
			PassFilter(new float[,]	{{1f, 1f, 1f},
									 {1f, 1f, 1f},
									 {1f, 1f, 1f}});
		}

		public void LowPassFilter2()
		{
			PassFilter(new float[,]	{{1f, 1f, 1f},
									 {1f, 2f, 1f},
									 {1f, 1f, 1f}});
		}

		public void LowPassFilter3()
		{
			PassFilter(new float[,]	{{1f, 2f, 1f},
									 {2f, 4f, 2f},
									 {1f, 2f, 1f}});
		}

		public void HighPassFilter1()
		{
			PassFilter(new float[,]	{{ 0f, -1f,  0f},
									 {-1f,  5f, -1f},
									 { 0f, -1f,  0f}});
		}

		public void HighPassFilter2()
		{
			PassFilter(new float[,]	{{-1f, -1f, -1f},
									 {-1f,  9f, -1f},
									 {-1f, -1f, -1f}});
		}

		public void HighPassFilter3()
		{
			PassFilter(new float[,]	{{ 0f, -2f,  0f},
									 {-2f,  5f, -2f},
									 { 0f, -2f,  0f}});
		}

		public void MedianFilter(int apperture)
		{
			if(apperture > Width || apperture > Height || apperture % 2 == 0)
			{
				return;
			}

			int shift = apperture / 2;

			int[] nums;
			switch (OriginalImageMode)
			{
				case ImageModes.Grayscale:
					ByteArray tempGrayscaleArray = new ByteArray(GrayscaleArray.Width, GrayscaleArray.Height, GrayscaleArray.Channels);
					nums = Enumerable.Range(0, GrayscaleArray.Array.Length).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = i % Width;
						int y = i / Width;

						if (x < shift || y < shift || x >= Width - shift || y >= Height - shift)
						{
							tempGrayscaleArray.Array[i] = GrayscaleArray.Array[i];
							return;
						}

						byte[] intensityMatrix = new byte[apperture * apperture];
						

						for (int j = 0; j < apperture; j++)
						{
							for (int k = 0; k < apperture; k++)
							{
								int index = (y + k - shift) * GrayscaleArray.Stride + (x + j - shift);
								int matrixIndex = k * apperture + j;
								intensityMatrix[matrixIndex] = GrayscaleArray.Array[index];
							}
						}

						Array.Sort(intensityMatrix);

						int median = (apperture * apperture) / 2;
						byte newIntensity = intensityMatrix[median];

						tempGrayscaleArray.Array[i] = newIntensity;
					});
					GrayscaleArray = tempGrayscaleArray;
					CalculateColorspaces(ImageModes.Grayscale);
					break;
				case ImageModes.RGB:
					ByteArray tempRgbArray = new ByteArray(RgbArray.Width, RgbArray.Height, RgbArray.Channels);
					nums = Enumerable.Range(0, RgbArray.Array.Length / RgbArray.Channels).Select(i => i * RgbArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbArray.Channels) % Width;
						int y = i / (Width * RgbArray.Channels);

						if (x < shift || y < shift || x >= Width - shift || y >= Height - shift)
						{
							tempRgbArray.Array[i] = RgbArray.Array[i];
							tempRgbArray.Array[i + 1] = RgbArray.Array[i + 1];
							tempRgbArray.Array[i + 2] = RgbArray.Array[i + 2];
							return;
						}

						int matrixSize = apperture * apperture;
						byte[] redMatrix = new byte[matrixSize];
						byte[] greenMatrix = new byte[matrixSize];
						byte[] blueMatrix = new byte[matrixSize];

						for (int j = 0; j < apperture; j++)
						{
							for (int k = 0; k < apperture; k++)
							{
								int index = (y + k - shift) * RgbArray.Stride + (x + j - shift) * RgbArray.Channels;
								int matrixIndex = k * apperture + j;
								blueMatrix[matrixIndex] = RgbArray.Array[index];
								greenMatrix[matrixIndex] = RgbArray.Array[index + 1];
								redMatrix[matrixIndex] = RgbArray.Array[index + 2];
							}
						}

						Array.Sort(redMatrix);
						Array.Sort(greenMatrix);
						Array.Sort(blueMatrix);

						int median = (apperture * apperture) / 2;

						byte newRed = redMatrix[median];
						byte newGreen = greenMatrix[median];
						byte newBlue = blueMatrix[median];

						tempRgbArray.Array[i] = newBlue;
						tempRgbArray.Array[i + 1] = newGreen;
						tempRgbArray.Array[i + 2] = newRed;
					});
					RgbArray = tempRgbArray;
					CalculateColorspaces(ImageModes.RGB);
					break;
				case ImageModes.RGBa:
					ByteArray tempRgbaArray = new ByteArray(RgbaArray.Width, RgbaArray.Height, RgbaArray.Channels);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						if (x < shift || y < shift || x >= Width - shift || y >= Height - shift)
						{
							tempRgbaArray.Array[i] = RgbaArray.Array[i];
							tempRgbaArray.Array[i + 1] = RgbaArray.Array[i + 1];
							tempRgbaArray.Array[i + 2] = RgbaArray.Array[i + 2];
							tempRgbaArray.Array[i + 3] = RgbaArray.Array[i + 3];
							return;
						}

						int matrixSize = apperture * apperture;
						byte[] redMatrix = new byte[matrixSize];
						byte[] greenMatrix = new byte[matrixSize];
						byte[] blueMatrix = new byte[matrixSize];

						for (int j = 0; j < apperture; j++)
						{
							for (int k = 0; k < apperture; k++)
							{
								int index = (y + k - shift) * RgbaArray.Stride + (x + j - shift) * RgbaArray.Channels;
								int matrixIndex = k * apperture + j;
								blueMatrix[matrixIndex] = RgbaArray.Array[index];
								greenMatrix[matrixIndex] = RgbaArray.Array[index + 1];
								redMatrix[matrixIndex] = RgbaArray.Array[index + 2];
							}
						}

						Array.Sort(redMatrix);
						Array.Sort(greenMatrix);
						Array.Sort(blueMatrix);

						int median = (apperture * apperture) / 2;
						byte newRed = redMatrix[median];
						byte newGreen = greenMatrix[median];
						byte newBlue = blueMatrix[median];

						tempRgbaArray.Array[i] = newBlue;
						tempRgbaArray.Array[i + 1] = newGreen;
						tempRgbaArray.Array[i + 2] = newRed;
						tempRgbaArray.Array[i + 3] = RgbaArray.Array[i + 3];
					});
					RgbaArray = tempRgbaArray;
					CalculateColorspaces(ImageModes.RGBa);
					break;
			}

			ImageHistogram.UpdateHistogram();
		}

		public void Operator2x2(int[,] xOperatorMatrix, int[,] yOperatorMatrix)
		{
			int operatorWidth = xOperatorMatrix.GetLength(0);
			int operatorHeight = xOperatorMatrix.GetLength(1);

			if (operatorWidth != 2 || operatorWidth != operatorHeight || operatorWidth != yOperatorMatrix.GetLength(0) || operatorHeight != yOperatorMatrix.GetLength(1))
			{
				return;
			}

			int[] nums;
			switch (OriginalImageMode)
			{
				case ImageModes.Grayscale:
					ByteArray tempGrayscaleArray = new ByteArray(GrayscaleArray.Width, GrayscaleArray.Height, GrayscaleArray.Channels);
					nums = Enumerable.Range(0, GrayscaleArray.Array.Length).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = i % Width;
						int y = i / Width;

						if (x >= Width - 1 || y >= Height - 1)
						{
							tempGrayscaleArray.Array[i] = 0;
							return;
						}

						int gx = 0;
						int gy = 0;

						for (int j = 0; j < operatorWidth; j++)
						{
							for (int k = 0; k < operatorHeight; k++)
							{
								int index = (y + k) * GrayscaleArray.Stride + (x + j);
								gx += xOperatorMatrix[j, k] * GrayscaleArray.Array[index];
								gy += yOperatorMatrix[j, k] * GrayscaleArray.Array[index];
							}
						}

						double newIntensity = Math.Sqrt(gx * gx + gy * gy);
						tempGrayscaleArray.Array[i] = Calculations.ClampToByte(newIntensity);
					});
					GrayscaleArray = tempGrayscaleArray;
					CalculateColorspaces(ImageModes.Grayscale);
					break;
				case ImageModes.RGB:
					ByteArray tempRgbArray = new ByteArray(RgbArray.Width, RgbArray.Height, RgbArray.Channels);
					nums = Enumerable.Range(0, RgbArray.Array.Length / RgbArray.Channels).Select(i => i * RgbArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbArray.Channels) % Width;
						int y = i / (Width * RgbArray.Channels);

						if (x >= Width - 1 || y >= Height - 1)
						{
							tempRgbArray.Array[i] = 0;
							tempRgbArray.Array[i + 1] = 0;
							tempRgbArray.Array[i + 2] = 0;
							return;
						}

						int gxRed = 0;
						int gyRed = 0;

						int gxGreen = 0;
						int gyGreen = 0;

						int gxBlue = 0;
						int gyBlue = 0;

						for (int j = 0; j < operatorWidth; j++)
						{
							for (int k = 0; k < operatorHeight; k++)
							{
								int index = (y + k) * RgbArray.Stride + (x + j) * RgbArray.Channels;
								gxBlue += xOperatorMatrix[j, k] * RgbArray.Array[index];
								gyBlue += yOperatorMatrix[j, k] * RgbArray.Array[index];
								gxGreen += xOperatorMatrix[j, k] * RgbArray.Array[index + 1];
								gyGreen += yOperatorMatrix[j, k] * RgbArray.Array[index + 1];
								gxRed += xOperatorMatrix[j, k] * RgbArray.Array[index + 1];
								gyRed += yOperatorMatrix[j, k] * RgbArray.Array[index + 2];
							}
						}

						double newRed = Math.Sqrt(gxRed * gxRed + gyRed * gyRed);
						double newGreen = Math.Sqrt(gxGreen * gxGreen + gyGreen * gyGreen);
						double newBlue = Math.Sqrt(gxBlue * gxBlue + gyBlue * gyBlue);

						tempRgbArray.Array[i] = Calculations.ClampToByte(newBlue);
						tempRgbArray.Array[i + 1] = Calculations.ClampToByte(newGreen);
						tempRgbArray.Array[i + 2] = Calculations.ClampToByte(newRed);
					});
					RgbArray = tempRgbArray;
					CalculateColorspaces(ImageModes.RGB);
					break;
				case ImageModes.RGBa:
					ByteArray tempRgbaArray = new ByteArray(RgbaArray.Width, RgbaArray.Height, RgbaArray.Channels);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						if (x >= Width - 1 || y >= Height - 1)
						{
							tempRgbaArray.Array[i] = 0;
							tempRgbaArray.Array[i + 1] = 0;
							tempRgbaArray.Array[i + 2] = 0;
							tempRgbaArray.Array[i + 3] = RgbaArray.Array[i + 3];
							return;
						}

						int gxRed = 0;
						int gyRed = 0;

						int gxGreen = 0;
						int gyGreen = 0;

						int gxBlue = 0;
						int gyBlue = 0;

						for (int j = 0; j < operatorWidth; j++)
						{
							for (int k = 0; k < operatorHeight; k++)
							{
								int index = (y + k) * RgbaArray.Stride + (x + j) * RgbaArray.Channels;
								gxBlue += xOperatorMatrix[j, k] * RgbaArray.Array[index];
								gyBlue += yOperatorMatrix[j, k] * RgbaArray.Array[index];
								gxGreen += xOperatorMatrix[j, k] * RgbaArray.Array[index + 1];
								gyGreen += yOperatorMatrix[j, k] * RgbaArray.Array[index + 1];
								gxRed += xOperatorMatrix[j, k] * RgbaArray.Array[index + 1];
								gyRed += yOperatorMatrix[j, k] * RgbaArray.Array[index + 2];
							}
						}

						double newRed = Math.Sqrt(gxRed * gxRed + gyRed * gyRed);
						double newGreen = Math.Sqrt(gxGreen * gxGreen + gyGreen * gyGreen);
						double newBlue = Math.Sqrt(gxBlue * gxBlue + gyBlue * gyBlue);

						tempRgbaArray.Array[i] = Calculations.ClampToByte(newBlue);
						tempRgbaArray.Array[i + 1] = Calculations.ClampToByte(newGreen);
						tempRgbaArray.Array[i + 2] = Calculations.ClampToByte(newRed);
						tempRgbaArray.Array[i + 3] = RgbaArray.Array[i + 3];
					});
					RgbaArray = tempRgbaArray;
					CalculateColorspaces(ImageModes.RGBa);
					break;
			}

			ImageHistogram.UpdateHistogram();
		}

		public void OperatorOdd(int[,] xOperatorMatrix, int[,] yOperatorMatrix)
		{
			int operatorWidth = xOperatorMatrix.GetLength(0);
			int operatorHeight = xOperatorMatrix.GetLength(1);

			if (operatorWidth != operatorHeight || operatorWidth != yOperatorMatrix.GetLength(0) || operatorHeight != yOperatorMatrix.GetLength(1) || operatorWidth % 2 == 0)
			{
				return;
			}

			int shift = operatorWidth / 2;

			Console.WriteLine("Life is hard, Life is stressful. Nothing changes. Still no one is around to help. Still no peace and tranquility. Shouldn't life be a bit easier?");

			int[] nums;
			switch (OriginalImageMode)
			{
				case ImageModes.Grayscale:
					ByteArray tempGrayscaleArray = new ByteArray(GrayscaleArray.Width, GrayscaleArray.Height, GrayscaleArray.Channels);
					nums = Enumerable.Range(0, GrayscaleArray.Array.Length).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = i % Width;
						int y = i / Width;

						if (x < shift || y < shift || x >= Width - shift || y >= Height - shift)
						{
							tempGrayscaleArray.Array[i] = 0;
							return;
						}

						int gx = 0;
						int gy = 0;

						for (int j = 0; j < operatorWidth; j++)
						{
							for (int k = 0; k < operatorHeight; k++)
							{
								int index = (y + k - shift) * GrayscaleArray.Stride + (x + j - shift);
								gx += xOperatorMatrix[j, k] * GrayscaleArray.Array[index];
								gy += yOperatorMatrix[j, k] * GrayscaleArray.Array[index];
							}
						}

						double newIntensity = Math.Sqrt(gx * gx + gy * gy);

						tempGrayscaleArray.Array[i] = Calculations.ClampToByte(newIntensity);
					});
					GrayscaleArray = tempGrayscaleArray;
					CalculateColorspaces(ImageModes.Grayscale);
					break;
				case ImageModes.RGB:
					ByteArray tempRgbArray = new ByteArray(RgbArray.Width, RgbArray.Height, RgbArray.Channels);
					nums = Enumerable.Range(0, RgbArray.Array.Length / RgbArray.Channels).Select(i => i * RgbArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbArray.Channels) % Width;
						int y = i / (Width * RgbArray.Channels);

						if (x < shift || y < shift || x >= Width - shift || y >= Height - shift)
						{
							tempRgbArray.Array[i] = 0;
							tempRgbArray.Array[i + 1] = 0;
							tempRgbArray.Array[i + 2] = 0;
							return;
						}

						int gxRed = 0;
						int gyRed = 0;

						int gxGreen = 0;
						int gyGreen = 0;

						int gxBlue = 0;
						int gyBlue = 0;

						for (int j = 0; j < operatorWidth; j++)
						{
							for (int k = 0; k < operatorHeight; k++)
							{
								int index = (y + k - shift) * RgbArray.Stride + (x + j - shift) * RgbArray.Channels;
								gxBlue += xOperatorMatrix[j, k] * RgbArray.Array[index];
								gyBlue += yOperatorMatrix[j, k] * RgbArray.Array[index];
								gxGreen += xOperatorMatrix[j, k] * RgbArray.Array[index + 1];
								gyGreen += yOperatorMatrix[j, k] * RgbArray.Array[index + 1];
								gxRed += xOperatorMatrix[j, k] * RgbArray.Array[index + 1];
								gyRed += yOperatorMatrix[j, k] * RgbArray.Array[index + 2];
							}
						}

						double newRed = Math.Sqrt(gxRed * gxRed + gyRed * gyRed);
						double newGreen = Math.Sqrt(gxGreen * gxGreen + gyGreen * gyGreen);
						double newBlue = Math.Sqrt(gxBlue * gxBlue + gyBlue * gyBlue);

						tempRgbArray.Array[i] = Calculations.ClampToByte(newBlue);
						tempRgbArray.Array[i + 1] = Calculations.ClampToByte(newGreen);
						tempRgbArray.Array[i + 2] = Calculations.ClampToByte(newRed);
					});
					RgbArray = tempRgbArray;
					CalculateColorspaces(ImageModes.RGB);
					break;
				case ImageModes.RGBa:
					ByteArray tempRgbaArray = new ByteArray(RgbaArray.Width, RgbaArray.Height, RgbaArray.Channels);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						if (x < shift || y < shift || x >= Width - shift || y >= Height - shift)
						{
							tempRgbaArray.Array[i] = 0;
							tempRgbaArray.Array[i + 1] = 0;
							tempRgbaArray.Array[i + 2] = 0;
							tempRgbaArray.Array[i + 3] = RgbaArray.Array[i + 3];
							return;
						}

						int gxRed = 0;
						int gyRed = 0;

						int gxGreen = 0;
						int gyGreen = 0;

						int gxBlue = 0;
						int gyBlue = 0;

						for (int j = 0; j < operatorWidth; j++)
						{
							for (int k = 0; k < operatorHeight; k++)
							{
								int index = (y + k - shift) * RgbaArray.Stride + (x + j - shift) * RgbaArray.Channels;
								gxBlue += xOperatorMatrix[j, k] * RgbaArray.Array[index];
								gyBlue += yOperatorMatrix[j, k] * RgbaArray.Array[index];
								gxGreen += xOperatorMatrix[j, k] * RgbaArray.Array[index + 1];
								gyGreen += yOperatorMatrix[j, k] * RgbaArray.Array[index + 1];
								gxRed += xOperatorMatrix[j, k] * RgbaArray.Array[index + 1];
								gyRed += yOperatorMatrix[j, k] * RgbaArray.Array[index + 2];
							}
						}

						double newRed = Math.Sqrt(gxRed * gxRed + gyRed * gyRed);
						double newGreen = Math.Sqrt(gxGreen * gxGreen + gyGreen * gyGreen);
						double newBlue = Math.Sqrt(gxBlue * gxBlue + gyBlue * gyBlue);

						tempRgbaArray.Array[i] = Calculations.ClampToByte(newBlue);
						tempRgbaArray.Array[i + 1] = Calculations.ClampToByte(newGreen);
						tempRgbaArray.Array[i + 2] = Calculations.ClampToByte(newRed);
						tempRgbaArray.Array[i + 3] = RgbaArray.Array[i + 3];
					});
					RgbaArray = tempRgbaArray;
					CalculateColorspaces(ImageModes.RGBa);
					break;
			}

			ImageHistogram.UpdateHistogram();
		}

		public void SobelOperator()
		{
			int[,] xMatrix = new int[,] {{-1, 0, 1},
										 {-2, 0, 2},
										 {-1, 0, 1}};

			int[,] yMatrix = new int[,] {{ 1,  2,  1},
										 { 0,  0,  0},
										 {-1, -2, -1}};

			OperatorOdd(xMatrix, yMatrix);
		}

		public void RobertsCrossOperator()
		{
			int[,] xMatrix = new int[,] {{-1, 0},
										 { 0, 1}};

			int[,] yMatrix = new int[,] {{1,  0},
										 {0, -1}};

			Operator2x2(xMatrix, yMatrix);
		}

		public void PrewittOperator()
		{
			int[,] xMatrix = new int[,] {{-1, 0, 1},
										 {-1, 0, 1},
										 {-1, 0, 1}};

			int[,] yMatrix = new int[,] {{ 1,  1,  1},
										 { 0,  0,  0},
										 {-1, -1, -1}};

			OperatorOdd(xMatrix, yMatrix);
		}

		public void AdaptiveVerticalThreshold(HistogramModes histogramMode)
		{
			byte adaptiveVerticalThreshold = ImageHistogram.CalculateAdaptiveVerticalThreshold(histogramMode);
			VerticalThreshold(histogramMode, adaptiveVerticalThreshold, 0);
		}

		public void HorizontalThreshold(HistogramModes histogramMode, int horizontalThreshold)
		{
			int[] nums;

			switch (histogramMode)
			{
				case HistogramModes.Intensity:
					nums = Enumerable.Range(0, GrayscaleArray.Array.Length).ToArray();
					Parallel.ForEach(nums, i =>
					{
						byte intensity = GrayscaleArray.Array[i];

						if (ImageHistogram.Intensity[intensity] < horizontalThreshold)
						{
							GrayscaleArray.Array[i] = 0;
						}
						else
						{
							GrayscaleArray.Array[i] = 255;
						}

					});
					CalculateColorspaces(ImageModes.Grayscale);
					break;
				case HistogramModes.Red:
					nums = Enumerable.Range(0, RgbArray.Array.Length / RgbArray.Channels).Select(i => i * RgbArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						byte red = RgbArray.Array[i + 2];

						if (ImageHistogram.Red[red] < horizontalThreshold)
						{
							RgbArray.Array[i + 2] = 0;
						}
						else
						{
							RgbArray.Array[i + 2] = 255;
						}
					});
					CalculateColorspaces(ImageModes.RGB);
					break;
				case HistogramModes.Green:
					nums = Enumerable.Range(0, RgbArray.Array.Length / RgbArray.Channels).Select(i => i * RgbArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						byte green = RgbArray.Array[i + 1];

						if (ImageHistogram.Green[green] < horizontalThreshold)
						{
							RgbArray.Array[i + 1] = 0;
						}
						else
						{
							RgbArray.Array[i + 1] = 255;
						}
					});
					CalculateColorspaces(ImageModes.RGB);
					break;
				case HistogramModes.Blue:
					nums = Enumerable.Range(0, RgbArray.Array.Length / RgbArray.Channels).Select(i => i * RgbArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						byte blue = RgbArray.Array[i];

						if (ImageHistogram.Blue[blue] < horizontalThreshold)
						{
							RgbArray.Array[i] = 0;
						}
						else
						{
							RgbArray.Array[i] = 255;
						}
					});
					CalculateColorspaces(ImageModes.RGB);
					break;
			}
		}

		public void VerticalThreshold(HistogramModes histogramMode, byte firstVerticalThreshold, byte secondVerticalThreshold)
		{
			int[] nums;

			switch (histogramMode)
			{
				case HistogramModes.Intensity:
					nums = Enumerable.Range(0, GrayscaleArray.Array.Length).ToArray();
					Parallel.ForEach(nums, i =>
					{
						byte intensity = GrayscaleArray.Array[i];

						if (intensity < firstVerticalThreshold)
						{
							GrayscaleArray.Array[i] = 0;
						}
						else if (intensity < secondVerticalThreshold)
						{
							GrayscaleArray.Array[i] = 128;
						}
						else
						{
							GrayscaleArray.Array[i] = 255;
						}

					});
					CalculateColorspaces(ImageModes.Grayscale);
					break;
				case HistogramModes.Red:
					nums = Enumerable.Range(0, RgbArray.Array.Length / RgbArray.Channels).Select(i => i * RgbArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						byte red = RgbArray.Array[i + 2];

						if (red < firstVerticalThreshold)
						{
							RgbArray.Array[i + 2] = 0;
						}
						else if (red < secondVerticalThreshold)
						{
							RgbArray.Array[i + 2] = 128;
						}
						else
						{
							RgbArray.Array[i + 2] = 255;
						}
					});
					CalculateColorspaces(ImageModes.RGB);
					break;
				case HistogramModes.Green:
					nums = Enumerable.Range(0, RgbArray.Array.Length / RgbArray.Channels).Select(i => i * RgbArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						byte green = RgbArray.Array[i + 1];

						if (green < firstVerticalThreshold)
						{
							RgbArray.Array[i + 1] = 0;
						}
						else if (green < secondVerticalThreshold)
						{
							RgbArray.Array[i + 1] = 128;
						}
						else
						{
							RgbArray.Array[i + 1] = 255;
						}
					});
					CalculateColorspaces(ImageModes.RGB);
					break;
				case HistogramModes.Blue:
					nums = Enumerable.Range(0, RgbArray.Array.Length / RgbArray.Channels).Select(i => i * RgbArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						byte blue = RgbArray.Array[i];

						if (blue < firstVerticalThreshold)
						{
							RgbArray.Array[i] = 0;
						}
						else if (blue < secondVerticalThreshold)
						{
							RgbArray.Array[i] = 128;
						}
						else
						{
							RgbArray.Array[i] = 255;
						}
					});
					CalculateColorspaces(ImageModes.RGB);
					break;
			}
		}
	}
}
