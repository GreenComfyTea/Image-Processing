using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageProcessing
{
	public class MyImage
	{
		public static byte DEFAULT_LIGHTNESS = 50;
		public WriteableBitmap WriteableBmp { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public ByteArray CurrentArray { get; set; }
		public ImageMode ImageMode { get; set; }
		public ByteArray RgbaArray { get; set; }
		public Histogram ImageHistogram { get; set; }

		public MyImage(WriteableBitmap writeableBitmap)
		{
			WriteableBmp = writeableBitmap;
			Width = WriteableBmp.PixelWidth;
			Height = WriteableBmp.PixelHeight;
			int channels = ((WriteableBmp.Format.BitsPerPixel + 7) / 8);

			CurrentArray = new ByteArray(Width, Height, channels);
			WriteableBmp.CopyPixels(CurrentArray.Array, CurrentArray.Stride, 0);

			RgbaArray = CurrentArray;
			ImageMode = ImageMode.RGBa;

			ImageHistogram = new Histogram(this);
			ImageHistogram.UpdateHistogram();
		}

		public byte GetIntensity(int x, int y)
		{
			return ColorConverter.RgbToGrayscale(RgbaArray.GetPixelColor(x, y));
		}

		public System.Windows.Media.Color GetRgbaColor(int x, int y)
		{
			return RgbaArray.GetPixelColor(x, y);
		}

		public System.Windows.Media.Color GetConvertedRgbaColor(int x, int y)
		{
			return CurrentArray.GetPixelColor(x, y);
		}

		public CmykColor GetCmykColor(int x, int y)
		{
			return ColorConverter.RgbToCmyk(RgbaArray.GetPixelColor(x, y));
		}

		public HsvColor GetHsvColor(int x, int y)
		{
			return ColorConverter.RgbToHsv(RgbaArray.GetPixelColor(x, y));
		}

		public YuvColor GetYuvColor(int x, int y)
		{
			return ColorConverter.RgbToYuv(RgbaArray.GetPixelColor(x, y));
		}

		public XyzColor GetSRgbColor(int x, int y)
		{
			return ColorConverter.RgbToSRgb(RgbaArray.GetPixelColor(x, y));
		}

		public XyzColor GetAdobeRgbColor(int x, int y)
		{
			return ColorConverter.RgbToAdobeRgb(RgbaArray.GetPixelColor(x, y));
		}

		public XyzColor GetAppleRgbColor(int x, int y)
		{
			return ColorConverter.RgbToAppleRgb(RgbaArray.GetPixelColor(x, y));
		}

		public XyzColor GetColorMatchRgbColor(int x, int y)
		{
			return ColorConverter.RgbToColorMatchRgb(RgbaArray.GetPixelColor(x, y));
		}

		public XyzColor GetProPhotoRgbColor(int x, int y)
		{
			return ColorConverter.RgbToProPhotoRgb(RgbaArray.GetPixelColor(x, y));
		}

		public LabColor GetSRgbLabColor(int x, int y)
		{
			return ColorConverter.SRgbToLab(ColorConverter.RgbToSRgb(RgbaArray.GetPixelColor(x, y)));
		}

		public LabColor GetAdobeRgbLabColor(int x, int y)
		{
			return ColorConverter.AdobeRgbToLab(ColorConverter.RgbToAdobeRgb(RgbaArray.GetPixelColor(x, y)));
		}

		public LabColor GetAppleRgbLabColor(int x, int y)
		{
			return ColorConverter.AppleRgbToLab(ColorConverter.RgbToAppleRgb(RgbaArray.GetPixelColor(x, y)));
		}

		public LabColor GetColorMatchRgbLabColor(int x, int y)
		{
			return ColorConverter.ColorMatchRgbToLab(ColorConverter.RgbToColorMatchRgb(RgbaArray.GetPixelColor(x, y)));
		}

		public LabColor GetProPhotoRgbLabColor(int x, int y)
		{
			return ColorConverter.ProPhotoRgbToLab(ColorConverter.RgbToProPhotoRgb(RgbaArray.GetPixelColor(x, y)));
		}

		private void ConvertToGrayscalePrivate()
		{
			int[] nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
			Parallel.ForEach(nums, i =>
			{
				int x = (i / RgbaArray.Channels) % Width;
				int y = i / (Width * RgbaArray.Channels);

				byte blue = RgbaArray.Array[i];
				byte green = RgbaArray.Array[i + 1];
				byte red = RgbaArray.Array[i + 2];

				byte intensity = ColorConverter.RgbToGrayscale(red, green, blue);

				RgbaArray.Array[i] = intensity;
				RgbaArray.Array[i + 1] = intensity;
				RgbaArray.Array[i + 2] = intensity;
			});
		}

		public void ConvertToGrayscale()
		{
			ConvertToGrayscalePrivate();
			ImageHistogram.UpdateHistogram();
		}

		public void SetImageMode(ImageMode imageMode)
		{
			ImageMode = imageMode;

			int[] nums;

			switch (imageMode)
			{
				case ImageMode.Grayscale:
					CurrentArray = new ByteArray(Width, Height, 1);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						byte intensity = ColorConverter.RgbToGrayscale(red, green, blue);

						CurrentArray.Array[index] = intensity;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Gray8, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.RGB:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						CurrentArray.Array[index] = blue;
						index++;
						CurrentArray.Array[index] = green;
						index++;
						CurrentArray.Array[index] = red;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.RGBa:
					CurrentArray = RgbaArray;
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgra32, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.Red:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte red = RgbaArray.Array[i + 2];

						CurrentArray.Array[index] = 0;
						index++;
						CurrentArray.Array[index] = 0;
						index++;
						CurrentArray.Array[index] = red;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.Green:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte green = RgbaArray.Array[i + 1];

						CurrentArray.Array[index] = 0;
						index++;
						CurrentArray.Array[index] = green;
						index++;
						CurrentArray.Array[index] = 0;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.Blue:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];

						CurrentArray.Array[index] = blue;
						index++;
						CurrentArray.Array[index] = 0;
						index++;
						CurrentArray.Array[index] = 0;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.Alpha:
					CurrentArray = new ByteArray(Width, Height, 1);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{

						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte alpha = RgbaArray.Array[i + 3];
						CurrentArray.Array[index] = alpha;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Gray8, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.HSV:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						HsvColor hsv = ColorConverter.RgbToHsv(red, green, blue);
						System.Windows.Media.Color hsvToRgb = ColorConverter.HsvToRgb(hsv);

						CurrentArray.Array[index] = hsvToRgb.B;
						index++;
						CurrentArray.Array[index] = hsvToRgb.G;
						index++;
						CurrentArray.Array[index] = hsvToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.Hue:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						HsvColor hsv = ColorConverter.RgbToHsv(red, green, blue);
						System.Windows.Media.Color hueToRgb = ColorConverter.HsvToRgb(hsv.Hue, 1d, 1d);

						CurrentArray.Array[index] = hueToRgb.B;
						index++;
						CurrentArray.Array[index] = hueToRgb.G;
						index++;
						CurrentArray.Array[index] = hueToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.Saturation:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						HsvColor hsv = ColorConverter.RgbToHsv(red, green, blue);
						System.Windows.Media.Color saturationToRgb = ColorConverter.HsvToRgb(0d, hsv.Saturation, 1d);

						CurrentArray.Array[index] = saturationToRgb.B;
						index++;
						CurrentArray.Array[index] = saturationToRgb.G;
						index++;
						CurrentArray.Array[index] = saturationToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.Value:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						HsvColor hsv = ColorConverter.RgbToHsv(red, green, blue);
						System.Windows.Media.Color valueToRgb = ColorConverter.HsvToRgb(0d, 1d, hsv.Value);

						CurrentArray.Array[index] = valueToRgb.B;
						index++;
						CurrentArray.Array[index] = valueToRgb.G;
						index++;
						CurrentArray.Array[index] = valueToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.CMYK:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						CmykColor cmyk = ColorConverter.RgbToCmyk(red, green, blue);
						System.Windows.Media.Color cmykToRgb = ColorConverter.CmykToRgb(cmyk);

						CurrentArray.Array[index] = cmykToRgb.B;
						index++;
						CurrentArray.Array[index] = cmykToRgb.G;
						index++;
						CurrentArray.Array[index] = cmykToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.Cyan:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						CmykColor cmyk = ColorConverter.RgbToCmyk(red, green, blue);
						System.Windows.Media.Color cyanToRgb = ColorConverter.CmykToRgb(cmyk.Cyan, 0d, 0d, 0d);

						CurrentArray.Array[index] = cyanToRgb.B;
						index++;
						CurrentArray.Array[index] = cyanToRgb.G;
						index++;
						CurrentArray.Array[index] = cyanToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.Magenta:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						CmykColor cmyk = ColorConverter.RgbToCmyk(red, green, blue);
						System.Windows.Media.Color magentaToRgb = ColorConverter.CmykToRgb(0d, cmyk.Magenta, 0d, 0d);

						CurrentArray.Array[index] = magentaToRgb.B;
						index++;
						CurrentArray.Array[index] = magentaToRgb.G;
						index++;
						CurrentArray.Array[index] = magentaToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.Yellow:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						CmykColor cmyk = ColorConverter.RgbToCmyk(red, green, blue);
						System.Windows.Media.Color yellowToRgb = ColorConverter.CmykToRgb(0d, 0d, cmyk.Yellow, 0d);

						CurrentArray.Array[index] = yellowToRgb.B;
						index++;
						CurrentArray.Array[index] = yellowToRgb.G;
						index++;
						CurrentArray.Array[index] = yellowToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.Black:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						CmykColor cmyk = ColorConverter.RgbToCmyk(red, green, blue);
						System.Windows.Media.Color blackToRgb = ColorConverter.CmykToRgb(0d, 0d, 0d, cmyk.Black);

						CurrentArray.Array[index] = blackToRgb.B;
						index++;
						CurrentArray.Array[index] = blackToRgb.G;
						index++;
						CurrentArray.Array[index] = blackToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.YUV:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						YuvColor yuv = ColorConverter.RgbToYuv(red, green, blue);
						System.Windows.Media.Color yuvToRgb = ColorConverter.YuvToRgb(yuv);

						CurrentArray.Array[index] = yuvToRgb.B;
						index++;
						CurrentArray.Array[index] = yuvToRgb.G;
						index++;
						CurrentArray.Array[index] = yuvToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.Luma:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						YuvColor yuv = ColorConverter.RgbToYuv(red, green, blue);
						System.Windows.Media.Color lumaToRgb = ColorConverter.YuvToRgb(yuv.Luma, 0d, 0d);

						CurrentArray.Array[index] = lumaToRgb.B;
						index++;
						CurrentArray.Array[index] = lumaToRgb.G;
						index++;
						CurrentArray.Array[index] = lumaToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.ColorDifferenceU:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						YuvColor yuv = ColorConverter.RgbToYuv(red, green, blue);
						System.Windows.Media.Color colorDifferenceUToRgb = ColorConverter.YuvToRgb(0, yuv.ColorDifferenceU, 0d);

						CurrentArray.Array[index] = colorDifferenceUToRgb.B;
						index++;
						CurrentArray.Array[index] = colorDifferenceUToRgb.G;
						index++;
						CurrentArray.Array[index] = colorDifferenceUToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.ColorDifferenceV:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						YuvColor yuv = ColorConverter.RgbToYuv(red, green, blue);
						System.Windows.Media.Color colorDifferenceVToRgb = ColorConverter.YuvToRgb(0d, 0d, yuv.ColorDifferenceV);

						CurrentArray.Array[index] = colorDifferenceVToRgb.B;
						index++;
						CurrentArray.Array[index] = colorDifferenceVToRgb.G;
						index++;
						CurrentArray.Array[index] = colorDifferenceVToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.sRGB_Lab:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{

						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						XyzColor sRgb = ColorConverter.RgbToSRgb(red, green, blue);
						LabColor sRgbLab = ColorConverter.SRgbToLab(sRgb);
						XyzColor sRgbLabToSRgb = ColorConverter.LabToSRgb(sRgbLab);
						System.Windows.Media.Color sRgbToRgb = ColorConverter.SRgbToRgb(sRgbLabToSRgb);

						CurrentArray.Array[index] = sRgbToRgb.B;
						index++;
						CurrentArray.Array[index] = sRgbToRgb.G;
						index++;
						CurrentArray.Array[index] = sRgbToRgb.R;

					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.sRGB_Lightness:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{

						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						XyzColor sRgb = ColorConverter.RgbToSRgb(red, green, blue);
						LabColor sRgbLab = ColorConverter.SRgbToLab(sRgb);
						XyzColor sRgbLabToSRgb = ColorConverter.LabToSRgb(sRgbLab.Lightness, 0, 0);
						System.Windows.Media.Color sRgbToRgb = ColorConverter.SRgbToRgb(sRgbLabToSRgb);

						CurrentArray.Array[index] = sRgbToRgb.B;
						index++;
						CurrentArray.Array[index] = sRgbToRgb.G;
						index++;
						CurrentArray.Array[index] = sRgbToRgb.R;

					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.sRGB_ChromaA:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{

						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						XyzColor sRgb = ColorConverter.RgbToSRgb(red, green, blue);
						LabColor sRgbLab = ColorConverter.SRgbToLab(sRgb);
						XyzColor sRgbLabToSRgb = ColorConverter.LabToSRgb(DEFAULT_LIGHTNESS, sRgbLab.ChromaA, 0);
						System.Windows.Media.Color sRgbToRgb = ColorConverter.SRgbToRgb(sRgbLabToSRgb);

						CurrentArray.Array[index] = sRgbToRgb.B;
						index++;
						CurrentArray.Array[index] = sRgbToRgb.G;
						index++;
						CurrentArray.Array[index] = sRgbToRgb.R;

					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.sRGB_ChromaB:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{

						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						XyzColor sRgb = ColorConverter.RgbToSRgb(red, green, blue);
						LabColor sRgbLab = ColorConverter.SRgbToLab(sRgb);
						XyzColor sRgbLabToSRgb = ColorConverter.LabToSRgb(DEFAULT_LIGHTNESS, 0, sRgbLab.ChromaB);
						System.Windows.Media.Color sRgbToRgb = ColorConverter.SRgbToRgb(sRgbLabToSRgb);

						CurrentArray.Array[index] = sRgbToRgb.B;
						index++;
						CurrentArray.Array[index] = sRgbToRgb.G;
						index++;
						CurrentArray.Array[index] = sRgbToRgb.R;

					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.AdobeRGB_Lab:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{

						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						XyzColor adobeRgb = ColorConverter.RgbToAdobeRgb(red, green, blue);
						LabColor adobeRgbLab = ColorConverter.AdobeRgbToLab(adobeRgb);
						XyzColor adobeRgbLabToAdobeRgb = ColorConverter.LabToAdobeRgb(adobeRgbLab);
						System.Windows.Media.Color adobeRgbToRgb = ColorConverter.AdobeRgbToRgb(adobeRgbLabToAdobeRgb);

						CurrentArray.Array[index] = adobeRgbToRgb.B;
						index++;
						CurrentArray.Array[index] = adobeRgbToRgb.G;
						index++;
						CurrentArray.Array[index] = adobeRgbToRgb.R;

					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.AdobeRGB_Lightness:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{

						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						XyzColor adobeRgb = ColorConverter.RgbToAdobeRgb(red, green, blue);
						LabColor adobeRgbLab = ColorConverter.AdobeRgbToLab(adobeRgb);
						XyzColor adobeRgbLabToAdobeRgb = ColorConverter.LabToAdobeRgb(adobeRgbLab.Lightness, 0, 0);
						System.Windows.Media.Color adobeRgbToRgb = ColorConverter.AdobeRgbToRgb(adobeRgbLabToAdobeRgb);

						CurrentArray.Array[index] = adobeRgbToRgb.B;
						index++;
						CurrentArray.Array[index] = adobeRgbToRgb.G;
						index++;
						CurrentArray.Array[index] = adobeRgbToRgb.R;

					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.AdobeRGB_ChromaA:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{

						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						XyzColor adobeRgb = ColorConverter.RgbToAdobeRgb(red, green, blue);
						LabColor adobeRgbLab = ColorConverter.AdobeRgbToLab(adobeRgb);
						XyzColor adobeRgbLabToAdobeRgb = ColorConverter.LabToAdobeRgb(DEFAULT_LIGHTNESS, adobeRgbLab.ChromaA, 0);
						System.Windows.Media.Color adobeRgbToRgb = ColorConverter.AdobeRgbToRgb(adobeRgbLabToAdobeRgb);

						CurrentArray.Array[index] = adobeRgbToRgb.B;
						index++;
						CurrentArray.Array[index] = adobeRgbToRgb.G;
						index++;
						CurrentArray.Array[index] = adobeRgbToRgb.R;

					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.AdobeRGB_ChromaB:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{

						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						XyzColor adobeRgb = ColorConverter.RgbToAdobeRgb(red, green, blue);
						LabColor adobeRgbLab = ColorConverter.AdobeRgbToLab(adobeRgb);
						XyzColor adobeRgbLabToAdobeRgb = ColorConverter.LabToAdobeRgb(DEFAULT_LIGHTNESS, 0, adobeRgbLab.ChromaB);
						System.Windows.Media.Color adobeRgbToRgb = ColorConverter.AdobeRgbToRgb(adobeRgbLabToAdobeRgb);

						CurrentArray.Array[index] = adobeRgbToRgb.B;
						index++;
						CurrentArray.Array[index] = adobeRgbToRgb.G;
						index++;
						CurrentArray.Array[index] = adobeRgbToRgb.R;

					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.AppleRGB_Lab:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{

						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						XyzColor appleRgb = ColorConverter.RgbToAppleRgb(red, green, blue);
						LabColor appleRgbLab = ColorConverter.AppleRgbToLab(appleRgb);
						XyzColor appleRgbLabToAppleRgb = ColorConverter.LabToAppleRgb(appleRgbLab);
						System.Windows.Media.Color appleRgbToRgb = ColorConverter.AppleRgbToRgb(appleRgbLabToAppleRgb);

						CurrentArray.Array[index] = appleRgbToRgb.B;
						index++;
						CurrentArray.Array[index] = appleRgbToRgb.G;
						index++;
						CurrentArray.Array[index] = appleRgbToRgb.R;

					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.AppleRGB_Lightness:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{

						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						XyzColor appleRgb = ColorConverter.RgbToAppleRgb(red, green, blue);
						LabColor appleRgbLab = ColorConverter.AppleRgbToLab(appleRgb);
						XyzColor appleRgbLabToAppleRgb = ColorConverter.LabToAppleRgb(appleRgbLab.Lightness, 0, 0);
						System.Windows.Media.Color appleRgbToRgb = ColorConverter.AppleRgbToRgb(appleRgbLabToAppleRgb);

						CurrentArray.Array[index] = appleRgbToRgb.B;
						index++;
						CurrentArray.Array[index] = appleRgbToRgb.G;
						index++;
						CurrentArray.Array[index] = appleRgbToRgb.R;

					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.AppleRGB_ChromaA:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{

						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						XyzColor appleRgb = ColorConverter.RgbToAppleRgb(red, green, blue);
						LabColor appleRgbLab = ColorConverter.AppleRgbToLab(appleRgb);
						XyzColor appleRgbLabToAppleRgb = ColorConverter.LabToAppleRgb(DEFAULT_LIGHTNESS, appleRgbLab.ChromaA, 0);
						System.Windows.Media.Color appleRgbToRgb = ColorConverter.AppleRgbToRgb(appleRgbLabToAppleRgb);

						CurrentArray.Array[index] = appleRgbToRgb.B;
						index++;
						CurrentArray.Array[index] = appleRgbToRgb.G;
						index++;
						CurrentArray.Array[index] = appleRgbToRgb.R;

					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.AppleRGB_ChromaB:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{

						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						XyzColor appleRgb = ColorConverter.RgbToAppleRgb(red, green, blue);
						LabColor appleRgbLab = ColorConverter.AppleRgbToLab(appleRgb);
						XyzColor appleRgbLabToAppleRgb = ColorConverter.LabToAppleRgb(DEFAULT_LIGHTNESS, 0, appleRgbLab.ChromaB);
						System.Windows.Media.Color appleRgbToRgb = ColorConverter.AppleRgbToRgb(appleRgbLabToAppleRgb);

						CurrentArray.Array[index] = appleRgbToRgb.B;
						index++;
						CurrentArray.Array[index] = appleRgbToRgb.G;
						index++;
						CurrentArray.Array[index] = appleRgbToRgb.R;

					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.ColorMatchRGB_Lab:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{

						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						XyzColor colorMatchRgb = ColorConverter.RgbToColorMatchRgb(red, green, blue);
						LabColor colorMatchRgbLab = ColorConverter.ColorMatchRgbToLab(colorMatchRgb);
						XyzColor colorMatchRgbLabToColorMatchRgb = ColorConverter.LabToColorMatchRgb(colorMatchRgbLab);
						System.Windows.Media.Color colorMatchRgbToRgb = ColorConverter.ColorMatchRgbToRgb(colorMatchRgbLabToColorMatchRgb);

						CurrentArray.Array[index] = colorMatchRgbToRgb.B;
						index++;
						CurrentArray.Array[index] = colorMatchRgbToRgb.G;
						index++;
						CurrentArray.Array[index] = colorMatchRgbToRgb.R;

					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.ColorMatchRGB_Lightness:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{

						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						XyzColor colorMatchRgb = ColorConverter.RgbToColorMatchRgb(red, green, blue);
						LabColor colorMatchRgbLab = ColorConverter.ColorMatchRgbToLab(colorMatchRgb);
						XyzColor colorMatchRgbLabToColorMatchRgb = ColorConverter.LabToColorMatchRgb(colorMatchRgbLab.Lightness, 0, 0);
						System.Windows.Media.Color colorMatchRgbToRgb = ColorConverter.ColorMatchRgbToRgb(colorMatchRgbLabToColorMatchRgb);

						CurrentArray.Array[index] = colorMatchRgbToRgb.B;
						index++;
						CurrentArray.Array[index] = colorMatchRgbToRgb.G;
						index++;
						CurrentArray.Array[index] = colorMatchRgbToRgb.R;

					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.ColorMatchRGB_ChromaA:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{

						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						XyzColor colorMatchRgb = ColorConverter.RgbToColorMatchRgb(red, green, blue);
						LabColor colorMatchRgbLab = ColorConverter.ColorMatchRgbToLab(colorMatchRgb);
						XyzColor colorMatchRgbLabToColorMatchRgb = ColorConverter.LabToColorMatchRgb(DEFAULT_LIGHTNESS, colorMatchRgbLab.ChromaA, 0);
						System.Windows.Media.Color colorMatchRgbToRgb = ColorConverter.ColorMatchRgbToRgb(colorMatchRgbLabToColorMatchRgb);

						CurrentArray.Array[index] = colorMatchRgbToRgb.B;
						index++;
						CurrentArray.Array[index] = colorMatchRgbToRgb.G;
						index++;
						CurrentArray.Array[index] = colorMatchRgbToRgb.R;

					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.ColorMatchRGB_ChromaB:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{

						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						XyzColor colorMatchRgb = ColorConverter.RgbToColorMatchRgb(red, green, blue);
						LabColor colorMatchRgbLab = ColorConverter.ColorMatchRgbToLab(colorMatchRgb);
						XyzColor colorMatchRgbLabToColorMatchRgb = ColorConverter.LabToColorMatchRgb(DEFAULT_LIGHTNESS, 0, colorMatchRgbLab.ChromaB);
						System.Windows.Media.Color colorMatchRgbToRgb = ColorConverter.ColorMatchRgbToRgb(colorMatchRgbLabToColorMatchRgb);

						CurrentArray.Array[index] = colorMatchRgbToRgb.B;
						index++;
						CurrentArray.Array[index] = colorMatchRgbToRgb.G;
						index++;
						CurrentArray.Array[index] = colorMatchRgbToRgb.R;

					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.ProPhotoRGB_Lab:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{

						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						XyzColor proPhotoRgb = ColorConverter.RgbToProPhotoRgb(red, green, blue);
						LabColor proPhotoRgbLab = ColorConverter.ProPhotoRgbToLab(proPhotoRgb);
						XyzColor proPhotoRgbLabToProPhotoRgb = ColorConverter.LabToProPhotoRgb(proPhotoRgbLab);
						System.Windows.Media.Color proPhotoRgbToRgb = ColorConverter.ProPhotoRgbToRgb(proPhotoRgbLabToProPhotoRgb);

						CurrentArray.Array[index] = proPhotoRgbToRgb.B;
						index++;
						CurrentArray.Array[index] = proPhotoRgbToRgb.G;
						index++;
						CurrentArray.Array[index] = proPhotoRgbToRgb.R;

					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.ProPhotoRGB_Lightness:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{

						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						XyzColor proPhotoRgb = ColorConverter.RgbToProPhotoRgb(red, green, blue);
						LabColor proPhotoRgbLab = ColorConverter.ProPhotoRgbToLab(proPhotoRgb);
						XyzColor proPhotoRgbLabToProPhotoRgb = ColorConverter.LabToProPhotoRgb(proPhotoRgbLab.Lightness, 0, 0);
						System.Windows.Media.Color proPhotoRgbToRgb = ColorConverter.ProPhotoRgbToRgb(proPhotoRgbLabToProPhotoRgb);

						CurrentArray.Array[index] = proPhotoRgbToRgb.B;
						index++;
						CurrentArray.Array[index] = proPhotoRgbToRgb.G;
						index++;
						CurrentArray.Array[index] = proPhotoRgbToRgb.R;

					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.ProPhotoRGB_ChromaA:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{

						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						XyzColor proPhotoRgb = ColorConverter.RgbToProPhotoRgb(red, green, blue);
						LabColor proPhotoRgbLab = ColorConverter.ProPhotoRgbToLab(proPhotoRgb);
						XyzColor proPhotoRgbLabToProPhotoRgb = ColorConverter.LabToProPhotoRgb(DEFAULT_LIGHTNESS, proPhotoRgbLab.ChromaA, 0);
						System.Windows.Media.Color proPhotoRgbToRgb = ColorConverter.ProPhotoRgbToRgb(proPhotoRgbLabToProPhotoRgb);

						CurrentArray.Array[index] = proPhotoRgbToRgb.B;
						index++;
						CurrentArray.Array[index] = proPhotoRgbToRgb.G;
						index++;
						CurrentArray.Array[index] = proPhotoRgbToRgb.R;

					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageMode.ProPhotoRGB_ChromaB:
					CurrentArray = new ByteArray(Width, Height, 3);
					nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{

						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * CurrentArray.Stride + x * CurrentArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						XyzColor proPhotoRgb = ColorConverter.RgbToProPhotoRgb(red, green, blue);
						LabColor proPhotoRgbLab = ColorConverter.ProPhotoRgbToLab(proPhotoRgb);
						XyzColor proPhotoRgbLabToProPhotoRgb = ColorConverter.LabToProPhotoRgb(DEFAULT_LIGHTNESS, 0, proPhotoRgbLab.ChromaB);
						System.Windows.Media.Color proPhotoRgbToRgb = ColorConverter.ProPhotoRgbToRgb(proPhotoRgbLabToProPhotoRgb);

						CurrentArray.Array[index] = proPhotoRgbToRgb.B;
						index++;
						CurrentArray.Array[index] = proPhotoRgbToRgb.G;
						index++;
						CurrentArray.Array[index] = proPhotoRgbToRgb.R;

					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
			}
			WriteableBmp.Freeze();
		}

		public void StretchHistogram(HistogramMode histogramMode)
		{
			int[] nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();

			switch (histogramMode)
			{
				case HistogramMode.Intensity:
					if (ImageHistogram.RedMin == 0 && ImageHistogram.GreenMin == 0 && ImageHistogram.BlueMin == 0 &&
						ImageHistogram.RedMax == 255 && ImageHistogram.GreenMax == 255 && ImageHistogram.BlueMax == 255)
					{
						return;
					}

					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * RgbaArray.Stride + x * RgbaArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						byte newBlue = Calculations.AffineTransformationByte(blue, ImageHistogram.BlueMin, ImageHistogram.BlueMax, 0, 255);
						byte newGreen = Calculations.AffineTransformationByte(green, ImageHistogram.GreenMin, ImageHistogram.GreenMax, 0, 255);
						byte newRed = Calculations.AffineTransformationByte(red, ImageHistogram.RedMin, ImageHistogram.RedMax, 0, 255);

						RgbaArray.Array[i] = newBlue;
						RgbaArray.Array[i + 1] = newGreen;
						RgbaArray.Array[i + 2] = newRed;
					});
					break;
				case HistogramMode.Red:
					if (ImageHistogram.RedMin == 0 && ImageHistogram.RedMax == 255)
					{
						return;
					}

					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * RgbaArray.Stride + x * RgbaArray.Channels;

						byte red = RgbaArray.Array[i + 2];

						byte newRed = Calculations.AffineTransformationByte(red, ImageHistogram.RedMin, ImageHistogram.RedMax, 0, 255);

						RgbaArray.Array[i + 2] = newRed;
					});
					break;
				case HistogramMode.Green:
					if (ImageHistogram.GreenMin == 0 && ImageHistogram.GreenMax == 255)
					{
						return;
					}

					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * RgbaArray.Stride + x * RgbaArray.Channels;

						byte green = RgbaArray.Array[i + 1];

						byte newGreen = Calculations.AffineTransformationByte(green, ImageHistogram.RedMin, ImageHistogram.RedMax, 0, 255);

						RgbaArray.Array[i + 1] = newGreen;
					});
					break;
				case HistogramMode.Blue:
					if (ImageHistogram.BlueMin == 0 && ImageHistogram.BlueMax == 255)
					{
						return;
					}

					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * RgbaArray.Stride + x * RgbaArray.Channels;

						byte blue = RgbaArray.Array[i];

						byte newBlue = Calculations.AffineTransformationByte(blue, ImageHistogram.RedMin, ImageHistogram.RedMax, 0, 255);

						RgbaArray.Array[i] = newBlue;
					});
					break;
			}

			ImageHistogram.UpdateHistogram();
		}

		private void PassFilterPrivate(float[,] filterMatrix, float filterCoefficient = 0f)
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

			if (filterCoefficient == 0)
			{
				return;
			}

			int shift = filterWidth / 2;

			ByteArray tempRgbaArray = new ByteArray(RgbaArray.Width, RgbaArray.Height, RgbaArray.Channels);
			int[] nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
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
		}

		public void PassFilter(float[,] filterMatrix, float filterCoefficient = 0f)
		{
			PassFilterPrivate(filterMatrix, filterCoefficient);
			ImageHistogram.UpdateHistogram();
		}

		public void LowPassFilter1()
		{
			PassFilter(new float[,] {{1f, 1f, 1f},
									 {1f, 1f, 1f},
									 {1f, 1f, 1f}});
		}

		public void LowPassFilter2()
		{
			PassFilter(new float[,] {{1f, 1f, 1f},
									 {1f, 2f, 1f},
									 {1f, 1f, 1f}});
		}

		public void LowPassFilter3()
		{
			PassFilter(new float[,] {{1f, 2f, 1f},
									 {2f, 4f, 2f},
									 {1f, 2f, 1f}});
		}

		public void HighPassFilter1()
		{
			PassFilter(new float[,] {{ 0f, -1f,  0f},
									 {-1f,  5f, -1},
									 { 0f, -1f,  0f}});
		}

		public void HighPassFilter2()
		{
			PassFilter(new float[,] {{-1f, -1f, -1},
									 {-1f,  9f, -1},
									 {-1f, -1f, -1}});
		}

		public void HighPassFilter3()
		{
			PassFilter(new float[,] {{ 0f, -2f,  0},
									 {-2f,  5f, -2},
									 { 0f, -2f,  0}});
		}

		public void MedianFilter(int aperture)
		{
			if (aperture > Width || aperture > Height || aperture % 2 == 0)
			{
				return;
			}

			int shift = aperture / 2;

			ByteArray tempRgbaArray = new ByteArray(RgbaArray.Width, RgbaArray.Height, RgbaArray.Channels);
			int[] nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
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

				int matrixSize = aperture * aperture;
				byte[] redMatrix = new byte[matrixSize];
				byte[] greenMatrix = new byte[matrixSize];
				byte[] blueMatrix = new byte[matrixSize];

				for (int j = 0; j < aperture; j++)
				{
					for (int k = 0; k < aperture; k++)
					{
						int index = (y + k - shift) * RgbaArray.Stride + (x + j - shift) * RgbaArray.Channels;
						int matrixIndex = k * aperture + j;
						blueMatrix[matrixIndex] = RgbaArray.Array[index];
						greenMatrix[matrixIndex] = RgbaArray.Array[index + 1];
						redMatrix[matrixIndex] = RgbaArray.Array[index + 2];
					}
				}

				Array.Sort(redMatrix);
				Array.Sort(greenMatrix);
				Array.Sort(blueMatrix);

				int median = (aperture * aperture) / 2;
				byte newRed = redMatrix[median];
				byte newGreen = greenMatrix[median];
				byte newBlue = blueMatrix[median];

				tempRgbaArray.Array[i] = newBlue;
				tempRgbaArray.Array[i + 1] = newGreen;
				tempRgbaArray.Array[i + 2] = newRed;
				tempRgbaArray.Array[i + 3] = RgbaArray.Array[i + 3];
			});
			RgbaArray = tempRgbaArray;

			ImageHistogram.UpdateHistogram();
		}

		public void GaussianBlur(int size = 5, double sigma = 1.4d)
		{
			if (size < 3) return;
			if (size % 2 == 0) return;

			double one = 2d * Math.Pow(sigma, 2d);
			double two = one * Math.PI;

			int shift = (size - 1) / 2;

			float[,] filterMatrix = new float[size, size];

			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					double three = Math.Pow(i - shift, 2d) + Math.Pow(j - shift, 2d);
					filterMatrix[i, j] = Calculations.ClampToFloat(Math.Exp(-three / one) / two);
				}
			}

			PassFilter(filterMatrix);
		}

		private void GaussianBlurPrivate(int size = 5, double sigma = 1.4d)
		{
			if (size < 3) return;
			if (size % 2 == 0) return;

			double one = 2d * Math.Pow(sigma, 2d);
			double two = one * Math.PI;

			int shift = (size - 1) / 2;

			float[,] filterMatrix = new float[size, size];

			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					double three = Math.Pow(i - shift, 2d) + Math.Pow(j - shift, 2d);
					filterMatrix[i, j] = Calculations.ClampToFloat(Math.Exp(-three / one) / two);
				}
			}

			PassFilterPrivate(filterMatrix);
		}

		public void GradientOperator(int[,] xOperatorMatrix, int[,] yOperatorMatrix)
		{
			int operatorWidth = xOperatorMatrix.GetLength(0);
			int operatorHeight = xOperatorMatrix.GetLength(1);

			if (operatorWidth != operatorHeight || operatorWidth != yOperatorMatrix.GetLength(0) || operatorHeight != yOperatorMatrix.GetLength(1) || operatorWidth < 2)
			{
				return;
			}

			int shift1 = operatorWidth / 2;
			int shift2 = shift1;

			if (operatorWidth % 2 == 0)
			{
				shift1 -= 1;
			}

			ByteArray tempRgbaArray = new ByteArray(RgbaArray.Width, RgbaArray.Height, RgbaArray.Channels);
			int[] nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
			Parallel.ForEach(nums, i =>
			{
				int x = (i / RgbaArray.Channels) % Width;
				int y = i / (Width * RgbaArray.Channels);

				if (x < shift1 || y < shift1 || x >= Width - shift2 || y >= Height - shift2)
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
						int index = (y + k - shift1) * RgbaArray.Stride + (x + j - shift1) * RgbaArray.Channels;
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

			GradientOperator(xMatrix, yMatrix);
		}

		public void RobertsCrossOperator()
		{
			int[,] xMatrix = new int[,] {{-1, 0},
										 { 0, 1}};

			int[,] yMatrix = new int[,] {{1,  0},
										 {0, -1}};

			GradientOperator(xMatrix, yMatrix);
		}

		public void PrewittOperator()
		{
			int[,] xMatrix = new int[,] {{-1, 0, 1},
										 {-1, 0, 1},
										 {-1, 0, 1}};

			int[,] yMatrix = new int[,] {{ 1,  1,  1},
										 { 0,  0,  0},
										 {-1, -1, -1}};

			GradientOperator(xMatrix, yMatrix);
		}

		public void AdaptiveVerticalThreshold(HistogramMode histogramMode)
		{
			byte adaptiveVerticalThreshold = ImageHistogram.CalculateAdaptiveVerticalThreshold(histogramMode);
			VerticalThreshold(histogramMode, adaptiveVerticalThreshold, 0);
		}

		public void HorizontalThreshold(HistogramMode histogramMode, int horizontalThreshold)
		{
			int[] nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();

			switch (histogramMode)
			{
				case HistogramMode.Intensity:
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						byte intensity = ColorConverter.RgbToGrayscale(red, green, blue);

						if (ImageHistogram.Intensity[intensity] < horizontalThreshold)
						{
							RgbaArray.Array[i] = 0;
							RgbaArray.Array[i + 1] = 0;
							RgbaArray.Array[i + 2] = 0;
						}
						else
						{
							RgbaArray.Array[i] = 255;
							RgbaArray.Array[i + 1] = 255;
							RgbaArray.Array[i + 2] = 255;
						}
					});
					break;
				case HistogramMode.Red:
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * RgbaArray.Stride + x * RgbaArray.Channels;

						byte red = RgbaArray.Array[i + 2];

						if (ImageHistogram.Red[red] < horizontalThreshold)
						{
							RgbaArray.Array[i + 2] = 0;
						}
						else
						{
							RgbaArray.Array[i + 2] = 255;
						}
					});
					break;
				case HistogramMode.Green:
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * RgbaArray.Stride + x * RgbaArray.Channels;

						byte green = RgbaArray.Array[i + 1];

						if (ImageHistogram.Green[green] < horizontalThreshold)
						{
							RgbaArray.Array[i + 1] = 0;
						}
						else
						{
							RgbaArray.Array[i + 1] = 255;
						}
					});
					break;
				case HistogramMode.Blue:
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * RgbaArray.Stride + x * RgbaArray.Channels;

						byte blue = RgbaArray.Array[i];

						if (ImageHistogram.Blue[blue] < horizontalThreshold)
						{
							RgbaArray.Array[i] = 0;
						}
						else
						{
							RgbaArray.Array[i] = 255;
						}
					});
					break;
			}

			ImageHistogram.UpdateHistogram();
		}

		public void VerticalThreshold(HistogramMode histogramMode, byte firstVerticalThreshold, byte secondVerticalThreshold)
		{
			int[] nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();

			switch (histogramMode)
			{
				case HistogramMode.Intensity:
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						byte intensity = ColorConverter.RgbToGrayscale(red, green, blue);

						if (intensity < firstVerticalThreshold)
						{
							RgbaArray.Array[i] = 0;
							RgbaArray.Array[i + 1] = 0;
							RgbaArray.Array[i + 2] = 0;
						}
						else if (intensity < secondVerticalThreshold)
						{
							RgbaArray.Array[i] = 128;
							RgbaArray.Array[i + 1] = 128;
							RgbaArray.Array[i + 2] = 128;
						}
						else
						{
							RgbaArray.Array[i] = 255;
							RgbaArray.Array[i + 1] = 255;
							RgbaArray.Array[i + 2] = 255;
						}
					});
					break;
				case HistogramMode.Red:
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * RgbaArray.Stride + x * RgbaArray.Channels;

						byte red = RgbaArray.Array[i + 2];

						if (red < firstVerticalThreshold)
						{
							RgbaArray.Array[i + 2] = 0;
						}
						else if (red < secondVerticalThreshold)
						{
							RgbaArray.Array[i + 2] = 128;
						}
						else
						{
							RgbaArray.Array[i + 2] = 255;
						}
					});
					break;
				case HistogramMode.Green:
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * RgbaArray.Stride + x * RgbaArray.Channels;

						byte green = RgbaArray.Array[i + 1];

						if (green < firstVerticalThreshold)
						{
							RgbaArray.Array[i + 1] = 0;
						}
						else if (green < secondVerticalThreshold)
						{
							RgbaArray.Array[i + 1] = 128;
						}
						else
						{
							RgbaArray.Array[i + 1] = 255;
						}
					});
					break;
				case HistogramMode.Blue:
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * RgbaArray.Stride + x * RgbaArray.Channels;

						byte blue = RgbaArray.Array[i];

						if (blue < firstVerticalThreshold)
						{
							RgbaArray.Array[i] = 0;
						}
						else if (blue < secondVerticalThreshold)
						{
							RgbaArray.Array[i] = 128;
						}
						else
						{
							RgbaArray.Array[i] = 255;
						}
					});
					break;
			}

			ImageHistogram.UpdateHistogram();
		}

		public void GammaCorrection(double gamma)
		{
			int[] nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();

			Parallel.ForEach(nums, i =>
			{
				int x = (i / RgbaArray.Channels) % Width;
				int y = i / (Width * RgbaArray.Channels);

				byte blue = RgbaArray.Array[i];
				byte green = RgbaArray.Array[i + 1];
				byte red = RgbaArray.Array[i + 2];

				double normalizedBlue = blue / 255d;
				double normalizedGreen = green / 255d;
				double normalizedRed = red / 255d;

				RgbaArray.Array[i] = Calculations.ClampToByte(255d * Math.Pow(normalizedBlue, gamma));
				RgbaArray.Array[i + 1] = Calculations.ClampToByte(255d * Math.Pow(normalizedGreen, gamma));
				RgbaArray.Array[i + 2] = Calculations.ClampToByte(255d * Math.Pow(normalizedRed, gamma));
			});

			ImageHistogram.UpdateHistogram();
		}

		public void LogarithmicTransformation(double logarithmConstant = -1d)
		{
			int[] nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();

			double logarithmConstantRed = logarithmConstant;
			double logarithmConstantGreen = logarithmConstant;
			double logarithmConstantBlue = logarithmConstant;

			if (logarithmConstant == -1d)
			{

				logarithmConstantRed = 255d / Math.Log10(ImageHistogram.RedMax + 1d);
				logarithmConstantGreen = 255d / Math.Log10(ImageHistogram.GreenMax + 1d);
				logarithmConstantBlue = 255d / Math.Log10(ImageHistogram.BlueMax + 1d);
				Console.WriteLine("{0}\t{1}\t{2}", logarithmConstantRed, logarithmConstantGreen, logarithmConstantBlue);
			}

			Parallel.ForEach(nums, i =>
			{
				int x = (i / RgbaArray.Channels) % Width;
				int y = i / (Width * RgbaArray.Channels);

				byte blue = RgbaArray.Array[i];
				byte green = RgbaArray.Array[i + 1];
				byte red = RgbaArray.Array[i + 2];

				RgbaArray.Array[i] = Calculations.ClampToByte(logarithmConstantRed * Math.Log10(blue + 1d));
				RgbaArray.Array[i + 1] = Calculations.ClampToByte(logarithmConstantGreen * Math.Log10(green + 1d));
				RgbaArray.Array[i + 2] = Calculations.ClampToByte(logarithmConstantBlue * Math.Log10(red + 1d));
			});

			ImageHistogram.UpdateHistogram();
		}

		public void ExponentialCorrection(double exponentConstant)
		{
			int[] nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();

			Parallel.ForEach(nums, i =>
			{
				int x = (i / RgbaArray.Channels) % Width;
				int y = i / (Width * RgbaArray.Channels);

				byte blue = RgbaArray.Array[i];
				byte green = RgbaArray.Array[i + 1];
				byte red = RgbaArray.Array[i + 2];

				double normalizedBlue = blue / 255d;
				double normalizedGreen = green / 255d;
				double normalizedRed = red / 255d;

				RgbaArray.Array[i] = Calculations.ClampToByte(255d * exponentConstant * (Math.Pow(2d, normalizedBlue) - 1));
				RgbaArray.Array[i + 1] = Calculations.ClampToByte(255d * exponentConstant * (Math.Pow(2d, normalizedGreen) - 1));
				RgbaArray.Array[i + 2] = Calculations.ClampToByte(255d * exponentConstant * (Math.Pow(2d, normalizedRed) - 1));
			});

			ImageHistogram.UpdateHistogram();
		}

		public void HistogramLinearization(HistogramMode histogramMode)
		{
			int[] nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();

			byte[] newRedArray;
			byte[] newGreenArray;
			byte[] newBlueArray;

			switch (histogramMode)
			{
				case HistogramMode.Intensity:
					ImageHistogram.GetLinearizationCumulativeRed(newRedArray = new byte[256]);
					ImageHistogram.GetLinearizationCumulativeGreen(newGreenArray = new byte[256]);
					ImageHistogram.GetLinearizationCumulativeBlue(newBlueArray = new byte[256]);
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * RgbaArray.Stride + x * RgbaArray.Channels;

						byte blue = RgbaArray.Array[i];
						byte green = RgbaArray.Array[i + 1];
						byte red = RgbaArray.Array[i + 2];

						RgbaArray.Array[i] = newBlueArray[blue];
						RgbaArray.Array[i + 1] = newGreenArray[green];
						RgbaArray.Array[i + 2] = newRedArray[red];
					});
					break;
				case HistogramMode.Red:
					ImageHistogram.GetLinearizationCumulativeRed(newRedArray = new byte[256]);
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * RgbaArray.Stride + x * RgbaArray.Channels;

						byte red = RgbaArray.Array[i + 2];

						RgbaArray.Array[i + 2] = newRedArray[red];
					});
					break;
				case HistogramMode.Green:
					ImageHistogram.GetLinearizationCumulativeGreen(newGreenArray = new byte[256]);
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * RgbaArray.Stride + x * RgbaArray.Channels;

						byte green = RgbaArray.Array[i + 1];

						RgbaArray.Array[i + 1] = newGreenArray[green];
					});
					break;
				case HistogramMode.Blue:
					ImageHistogram.GetLinearizationCumulativeBlue(newBlueArray = new byte[256]);
					Parallel.ForEach(nums, i =>
					{
						int x = (i / RgbaArray.Channels) % Width;
						int y = i / (Width * RgbaArray.Channels);

						int index = y * RgbaArray.Stride + x * RgbaArray.Channels;

						byte blue = RgbaArray.Array[i];

						RgbaArray.Array[i] = newBlueArray[blue];
					});
					break;
			}

			ImageHistogram.UpdateHistogram();
		}

		public void CannyOperator(int gaussianBlurSize = 5, double gaussianBlurSigma = 1d, Operator operator_ = Operator.SobelOperator, byte threshold1 = 64, byte threshold2 = 128)
		{
			ConvertToGrayscalePrivate();
			GaussianBlurPrivate(gaussianBlurSize, gaussianBlurSigma);

			byte[,] gradientMagnitudes = new byte[Width, Height];
			Direction[,] gradientDirections = new Direction[Width, Height];

			switch (operator_)
			{
				case Operator.SobelOperator:
					SobelOperatorCanny(gradientMagnitudes, gradientDirections);
					break;
				case Operator.RobertsCrossOperator:
					RobertsCrossOperatorCanny(gradientMagnitudes, gradientDirections);
					break;
				case Operator.PrewittOperator:
					PrewittOperatorCanny(gradientMagnitudes, gradientDirections);
					break;
				default:
					SobelOperatorCanny(gradientMagnitudes, gradientDirections);
					break;
			}

			gradientMagnitudes = NonMaximumSuppression_DoubleThreshold(gradientMagnitudes, gradientDirections, threshold1, threshold2);
			Hysteresis(gradientMagnitudes);
			ImageHistogram.UpdateHistogram();
		}

		private void Hysteresis(byte[,] gradientMagnitudes)
		{
			int[] nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
			Parallel.ForEach(nums, i =>
			{
				int x = (i / RgbaArray.Channels) % Width;
				int y = i / (Width * RgbaArray.Channels);

				if (gradientMagnitudes[x, y] == 255)
				{
					RgbaArray.Array[i] = 255;
					RgbaArray.Array[i + 1] = 255;
					RgbaArray.Array[i + 2] = 255;
					return;
				}

				if (gradientMagnitudes[x, y] == 0)
				{
					RgbaArray.Array[i] = 0;
					RgbaArray.Array[i + 1] = 0;
					RgbaArray.Array[i + 2] = 0;
					return;
				}

				bool leftBorder = x == 0;
				bool rightBorder = x == Width - 1;
				bool topBorder = y == 0;
				bool bottomBorder = y == Height - 1;

				if (!leftBorder)
				{
					if (gradientMagnitudes[x - 1, y] == 255)
					{
						RgbaArray.Array[i] = 255;
						RgbaArray.Array[i + 1] = 255;
						RgbaArray.Array[i + 2] = 255;
						return;
					}
				}

				if (!rightBorder)
				{
					if (gradientMagnitudes[x + 1, y] == 255)
					{
						RgbaArray.Array[i] = 255;
						RgbaArray.Array[i + 1] = 255;
						RgbaArray.Array[i + 2] = 255;
						return;
					}
				}

				if (!topBorder)
				{
					if (gradientMagnitudes[x, y - 1] == 255)
					{
						RgbaArray.Array[i] = 255;
						RgbaArray.Array[i + 1] = 255;
						RgbaArray.Array[i + 2] = 255;
						return;
					}
				}

				if (!bottomBorder)
				{
					if (gradientMagnitudes[x, y + 1] == 255)
					{
						RgbaArray.Array[i] = 255;
						RgbaArray.Array[i + 1] = 255;
						RgbaArray.Array[i + 2] = 255;
						return;
					}
				}

				if (!leftBorder && !topBorder)
				{
					if (gradientMagnitudes[x - 1, y - 1] == 255)
					{
						RgbaArray.Array[i] = 255;
						RgbaArray.Array[i + 1] = 255;
						RgbaArray.Array[i + 2] = 255;
						return;
					}
				}

				if (!leftBorder && !bottomBorder)
				{
					if (gradientMagnitudes[x - 1, y + 1] == 255)
					{
						RgbaArray.Array[i] = 255;
						RgbaArray.Array[i + 1] = 255;
						RgbaArray.Array[i + 2] = 255;
						return;
					}
				}

				if (!rightBorder && !topBorder)
				{
					if (gradientMagnitudes[x + 1, y - 1] == 255)
					{
						RgbaArray.Array[i] = 255;
						RgbaArray.Array[i + 1] = 255;
						RgbaArray.Array[i + 2] = 255;
						return;
					}
				}

				if (!rightBorder && !bottomBorder)
				{
					if (gradientMagnitudes[x + 1, y + 1] == 255)
					{
						RgbaArray.Array[i] = 255;
						RgbaArray.Array[i + 1] = 255;
						RgbaArray.Array[i + 2] = 255;
						return;
					}
				}

				RgbaArray.Array[i] = 0;
				RgbaArray.Array[i + 1] = 0;
				RgbaArray.Array[i + 2] = 0;
			});
		}

		private byte[,] NonMaximumSuppression_DoubleThreshold(byte[,] gradientMagnitudes, Direction[,] gradientDirections, byte threshold1 = 64, byte threshold2 = 128)
		{
			byte[,] newGradientMagnitudes = new byte[Width, Height];

			int[] nums = Enumerable.Range(1, Width - 2).ToArray();
			Parallel.ForEach(nums, x =>
			{
				for (int y = 1; y < Height - 1; y++)
				{

					if (gradientDirections[x, y] == Direction.Horizontal
						&& (gradientMagnitudes[x - 1, y] > gradientMagnitudes[x, y]
						|| gradientMagnitudes[x + 1, y] > gradientMagnitudes[x, y]))
					{
						continue;
					}

					if (gradientDirections[x, y] == Direction.Vertical
						&& (gradientMagnitudes[x, y - 1] > gradientMagnitudes[x, y]
						|| gradientMagnitudes[x, y + 1] > gradientMagnitudes[x, y]))
					{
						continue;
					}

					if (gradientDirections[x, y] == Direction.MainDiagonal
						&& (gradientMagnitudes[x - 1, y - 1] > gradientMagnitudes[x, y]
						|| gradientMagnitudes[x + 1, y + 1] > gradientMagnitudes[x, y]))
					{
						continue;
					}

					if (gradientDirections[x, y] == Direction.AntiDiagonal
						&& (gradientMagnitudes[x + 1, y - 1] > gradientMagnitudes[x, y]
						|| gradientMagnitudes[x - 1, y + 1] > gradientMagnitudes[x, y]))
					{
						continue;
					}

					if (gradientMagnitudes[x, y] >= threshold2) newGradientMagnitudes[x, y] = 255;
					else if (gradientMagnitudes[x, y] >= threshold1) newGradientMagnitudes[x, y] = 127;
				}
			});
			return newGradientMagnitudes;
		}

		private Direction GetDirection(int x, int y)
		{
			double radian = Math.Atan2(y, x);

			if (radian >= 0.353429d && radian < 1.0603d) return Direction.MainDiagonal;
			if (radian >= 1.060300d && radian < 1.7671d) return Direction.Vertical;
			if (radian >= 1.767100d && radian < 2.4740d) return Direction.AntiDiagonal;
			return Direction.Horizontal;
		}

		private void GradientOperatorCanny(int[,] xOperatorMatrix, int[,] yOperatorMatrix, byte[,] gradientMagnitudes, Direction[,] gradientDirections)
		{
			int operatorWidth = xOperatorMatrix.GetLength(0);
			int operatorHeight = xOperatorMatrix.GetLength(1);

			if (operatorWidth != operatorHeight || operatorWidth != yOperatorMatrix.GetLength(0) || operatorHeight != yOperatorMatrix.GetLength(1) || operatorWidth < 2)
			{
				return;
			}

			int shift1 = operatorWidth / 2;
			int shift2 = shift1;

			if (operatorWidth % 2 == 0)
			{
				shift1 -= 1;
			}

			ByteArray tempRgbaArray = new ByteArray(RgbaArray.Width, RgbaArray.Height, RgbaArray.Channels);
			int[] nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
			Parallel.ForEach(nums, i =>
			{
				int x = (i / RgbaArray.Channels) % Width;
				int y = i / (Width * RgbaArray.Channels);

				if (x < shift1 || y < shift1 || x >= Width - shift2 || y >= Height - shift2)
				{
					tempRgbaArray.Array[i] = 0;
					tempRgbaArray.Array[i + 1] = 0;
					tempRgbaArray.Array[i + 2] = 0;
					tempRgbaArray.Array[i + 3] = RgbaArray.Array[i + 3];
					return;
				}

				int gradientX = 0;
				int gradientY = 0;

				for (int j = 0; j < operatorWidth; j++)
				{
					for (int k = 0; k < operatorHeight; k++)
					{
						int index = (y + k - shift1) * RgbaArray.Stride + (x + j - shift1) * RgbaArray.Channels;
						gradientX += xOperatorMatrix[j, k] * RgbaArray.Array[index];
						gradientY += yOperatorMatrix[j, k] * RgbaArray.Array[index];
					}
				}

				gradientMagnitudes[x, y] = Calculations.ClampToByte(Math.Sqrt(gradientX * gradientX + gradientY * gradientY));
				gradientDirections[x, y] = GetDirection(gradientX, gradientY);


				tempRgbaArray.Array[i] = gradientMagnitudes[x, y];
				tempRgbaArray.Array[i + 1] = gradientMagnitudes[x, y];
				tempRgbaArray.Array[i + 2] = gradientMagnitudes[x, y];
				tempRgbaArray.Array[i + 3] = RgbaArray.Array[i + 3];
			});
			RgbaArray = tempRgbaArray;
		}

		private void SobelOperatorCanny(byte[,] gradientMagnitudes, Direction[,] gradientDirections)
		{
			int[,] xMatrix = new int[,] {{-1, 0, 1},
										 {-2, 0, 2},
										 {-1, 0, 1}};

			int[,] yMatrix = new int[,] {{ 1,  2,  1},
										 { 0,  0,  0},
										 {-1, -2, -1}};

			GradientOperatorCanny(xMatrix, yMatrix, gradientMagnitudes, gradientDirections);
		}

		private void RobertsCrossOperatorCanny(byte[,] gradientMagnitudes, Direction[,] gradientDirections)
		{
			int[,] xMatrix = new int[,] {{-1, 0},
										 { 0, 1}};

			int[,] yMatrix = new int[,] {{1,  0},
										 {0, -1}};

			GradientOperatorCanny(xMatrix, yMatrix, gradientMagnitudes, gradientDirections);
		}

		private void PrewittOperatorCanny(byte[,] gradientMagnitudes, Direction[,] gradientDirections)
		{
			int[,] xMatrix = new int[,] {{-1, 0, 1},
										 {-1, 0, 1},
										 {-1, 0, 1}};

			int[,] yMatrix = new int[,] {{ 1,  1,  1},
										 { 0,  0,  0},
										 {-1, -1, -1}};

			GradientOperatorCanny(xMatrix, yMatrix, gradientMagnitudes, gradientDirections);
		}

		public void RegionGrowing(int x, int y, byte threshold = 30)
		{
			if (x < 0 || x >= Width || y < 0 || y >= Height) return;

			RegionGrowingPrivate(x, y, System.Windows.Media.Color.FromRgb(0, 255, 255), threshold);

			ImageHistogram.UpdateHistogram();
		}

		private void RegionGrowingPrivate(int x, int y, System.Windows.Media.Color color, byte threshold = 30, bool[,] visited = null)
		{
			int baseIndex = RgbaArray.GetIndex(x, y);

			byte baseBlue = RgbaArray.Array[baseIndex];
			byte baseGreen = RgbaArray.Array[baseIndex + 1];
			byte baseRed = RgbaArray.Array[baseIndex + 2];

			if (visited == null)
			{
				visited = new bool[Width, Height];
			}

			visited[x, y] = true;

			Stack<Point> pixelsToProcess = new Stack<Point>();
			pixelsToProcess.Push(new Point(x, y));

			while (pixelsToProcess.Count > 0)
			{
				Point point = pixelsToProcess.Pop();

				int index = RgbaArray.GetIndex(point.X, point.Y);

				byte blue = RgbaArray.Array[index];
				byte green = RgbaArray.Array[index + 1];
				byte red = RgbaArray.Array[index + 2];

				bool blueCheck = Math.Abs(blue - baseBlue) > threshold;
				bool greenCheck = Math.Abs(green - baseGreen) > threshold;
				bool redCheck = Math.Abs(red - baseRed) > threshold;

				if (blueCheck || greenCheck || redCheck) continue;

				RgbaArray.Array[index] = color.B;
				RgbaArray.Array[index + 1] = color.G;
				RgbaArray.Array[index + 2] = color.R;

				bool leftBorder = point.X == 0;
				bool rightBorder = point.X == Width - 1;
				bool topBorder = point.Y == 0;
				bool bottomBorder = point.Y == Height - 1;

				if (!leftBorder && !visited[point.X - 1, point.Y])
				{
					pixelsToProcess.Push(new Point(point.X - 1, point.Y));
					visited[point.X - 1, point.Y] = true;
				}

				if (!rightBorder && !visited[point.X + 1, point.Y])
				{
					pixelsToProcess.Push(new Point(point.X + 1, point.Y));
					visited[point.X + 1, point.Y] = true;
				}

				if (!topBorder && !visited[point.X, point.Y - 1])
				{
					pixelsToProcess.Push(new Point(point.X, point.Y - 1));
					visited[point.X, point.Y - 1] = true;
				}

				if (!bottomBorder && !visited[point.X, point.Y + 1])
				{
					pixelsToProcess.Push(new Point(point.X, point.Y + 1));
					visited[point.X, point.Y + 1] = true;
				}

				if (!leftBorder && !topBorder && !visited[point.X - 1, point.Y - 1])
				{
					pixelsToProcess.Push(new Point(point.X - 1, point.Y - 1));
					visited[point.X - 1, point.Y - 1] = true;
				}

				if (!leftBorder && !bottomBorder && !visited[point.X - 1, point.Y + 1])
				{
					pixelsToProcess.Push(new Point(point.X - 1, point.Y + 1));
					visited[point.X - 1, point.Y + 1] = true;
				}

				if (!rightBorder && !topBorder && !visited[point.X + 1, point.Y - 1])
				{
					pixelsToProcess.Push(new Point(point.X + 1, point.Y - 1));
					visited[point.X + 1, point.Y - 1] = true;
				}

				if (!rightBorder && !bottomBorder && !visited[point.X + 1, point.Y + 1])
				{
					pixelsToProcess.Push(new Point(point.X + 1, point.Y + 1));
					visited[point.X + 1, point.Y + 1] = true;
				}
			}
		}

		public void AutomaticRegionGrowing(byte threshold = 30)
		{
			bool[,] visited = new bool[Width, Height];

			Random random = new Random();

			for(int x = 0; x < Width; x++)
			{
				for(int y = 0; y < Height; y++)
				{
					if(!visited[x, y])
					{
						System.Windows.Media.Color color = System.Windows.Media.Color.FromRgb(
							Calculations.ClampToByte(random.Next(0, 255)),
							Calculations.ClampToByte(random.Next(0, 255)),
							Calculations.ClampToByte(random.Next(0, 255))
						);
						RegionGrowingPrivate(x, y, color, threshold, visited);
					}
				}
			}

			ImageHistogram.UpdateHistogram();
		}
	}
}