#define RTU_HSV_COMPONENTS

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageProcessing
{
	class LowPerformanceImage : IImage
	{
		public WriteableBitmap WriteableBmp { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public ByteArray CurrentArray { get; set; }
		public ImageModes ImageMode { get; set; }
		public ByteArray RgbaArray { get; set; }
		public Histogram ImageHistogram { get; set; }

		public LowPerformanceImage(WriteableBitmap writeableBitmap)
		{
			WriteableBmp = writeableBitmap;
			Width = WriteableBmp.PixelWidth;
			Height = WriteableBmp.PixelHeight;
			int channels = ((WriteableBmp.Format.BitsPerPixel + 7) / 8);

			CurrentArray = new ByteArray(Width, Height, channels);
			WriteableBmp.CopyPixels(CurrentArray.Array, CurrentArray.Stride, 0);

			switch (channels)
			{
				case 1:
					ImageMode = ImageModes.Grayscale;
					break;
				case 3:
					ImageMode = ImageModes.RGB;
					break;
				case 4:
					ImageMode = ImageModes.RGBa;
					break;
			}

			RgbaArray = new ByteArray(Width, Height, 4);

			ImageHistogram = new Histogram(this);
		}

		public void CalculateColorspaces(ImageModes imageMode)
		{
			int[] nums;

			switch (imageMode)
			{
				case ImageModes.Grayscale:
					nums = Enumerable.Range(0, CurrentArray.Array.Length).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = i % Width;
						int y = i / Width;

						int index = y * RgbaArray.Stride + x * RgbaArray.Channels;

						byte intensity = CurrentArray.Array[i];

						RgbaArray.Array[index] = intensity;
						index++;
						RgbaArray.Array[index] = intensity;
						index++;
						RgbaArray.Array[index] = intensity;
						index++;
						RgbaArray.Array[index] = 255;
					});
					break;
				case ImageModes.RGB:
					nums = Enumerable.Range(0, CurrentArray.Array.Length / CurrentArray.Channels).Select(i => i * CurrentArray.Channels).ToArray();
					Parallel.ForEach(nums, i =>
					{
						int x = (i / CurrentArray.Channels) % Width;
						int y = i / (Width * CurrentArray.Channels);

						int index = y * RgbaArray.Stride + x * RgbaArray.Channels;

						byte blue = CurrentArray.Array[i];
						byte green = CurrentArray.Array[i + 1];
						byte red = CurrentArray.Array[i + 2];

						RgbaArray.Array[index] = blue;
						index++;
						RgbaArray.Array[index] = green;
						index++;
						RgbaArray.Array[index] = red;
						index++;
						RgbaArray.Array[index] = 255;
					});
					break;
				case ImageModes.RGBa:
					RgbaArray = CurrentArray;
					break;
			}

			ImageHistogram.UpdateHistogram();
		}

		public byte GetIntensity(int x, int y)
		{
			return ColorConverter.RgbToGrayscale(RgbaArray.GetPixelColor(x, y));
		}

		public Color GetRgbaColor(int x, int y)
		{
			return RgbaArray.GetPixelColor(x, y);
		}

		public Color GetConvertedRgbaColor(int x, int y)
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

		public void SetImageMode(ImageModes imageMode)
		{
			ImageMode = imageMode;

			int[] nums;

			switch (imageMode)
			{
				case ImageModes.Grayscale:
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
				case ImageModes.RGB:
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
				case ImageModes.RGBa:
					CurrentArray = RgbaArray;
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgra32, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageModes.Red:
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
				case ImageModes.Green:
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
				case ImageModes.Blue:
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
				case ImageModes.Alpha:
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
				case ImageModes.HSV:
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
						Color hsvToRgb = ColorConverter.HsvToRgb(hsv);

						CurrentArray.Array[index] = hsvToRgb.B;
						index++;
						CurrentArray.Array[index] = hsvToRgb.G;
						index++;
						CurrentArray.Array[index] = hsvToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageModes.Hue:
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
						Color hueToRgb = ColorConverter.HsvToRgb(hsv.Hue, 1d, 1d);

						CurrentArray.Array[index] = hueToRgb.B;
						index++;
						CurrentArray.Array[index] = hueToRgb.G;
						index++;
						CurrentArray.Array[index] = hueToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageModes.Saturation:
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
#if (RTU_HSV_COMPONENTS)
						Color saturationToRgb = ColorConverter.SaturationToRgb(hsv.Saturation);
#else
						Color saturationToRgb = ColorConverter.HsvToRgb(0d, hsv.Saturation, 1d);
#endif

						CurrentArray.Array[index] = saturationToRgb.B;
						index++;
						CurrentArray.Array[index] = saturationToRgb.G;
						index++;
						CurrentArray.Array[index] = saturationToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageModes.Value:
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
#if (RTU_HSV_COMPONENTS)
						Color valueToRgb = ColorConverter.HsvToRgb(0d, 0d, hsv.Value);
#else
						Color valueToRgb = ColorConverter.HsvToRgb(0d, 1d, hsv.Value);
#endif

						CurrentArray.Array[index] = valueToRgb.B;
						index++;
						CurrentArray.Array[index] = valueToRgb.G;
						index++;
						CurrentArray.Array[index] = valueToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageModes.CMYK:
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
						Color cmykToRgb = ColorConverter.CmykToRgb(cmyk);

						CurrentArray.Array[index] = cmykToRgb.B;
						index++;
						CurrentArray.Array[index] = cmykToRgb.G;
						index++;
						CurrentArray.Array[index] = cmykToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageModes.Cyan:
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
						Color cyanToRgb = ColorConverter.CmykToRgb(cmyk.Cyan, 0d, 0d, 0d);

						CurrentArray.Array[index] = cyanToRgb.B;
						index++;
						CurrentArray.Array[index] = cyanToRgb.G;
						index++;
						CurrentArray.Array[index] = cyanToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageModes.Magenta:
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
						Color magentaToRgb = ColorConverter.CmykToRgb(0d, cmyk.Magenta, 0d, 0d);

						CurrentArray.Array[index] = magentaToRgb.B;
						index++;
						CurrentArray.Array[index] = magentaToRgb.G;
						index++;
						CurrentArray.Array[index] = magentaToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageModes.Yellow:
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
						Color yellowToRgb = ColorConverter.CmykToRgb(0d, 0d, cmyk.Yellow, 0d);

						CurrentArray.Array[index] = yellowToRgb.B;
						index++;
						CurrentArray.Array[index] = yellowToRgb.G;
						index++;
						CurrentArray.Array[index] = yellowToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageModes.Black:
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
						Color blackToRgb = ColorConverter.CmykToRgb(0d, 0d, 0d, cmyk.Black);

						CurrentArray.Array[index] = blackToRgb.B;
						index++;
						CurrentArray.Array[index] = blackToRgb.G;
						index++;
						CurrentArray.Array[index] = blackToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageModes.YUV:
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
						Color yuvToRgb = ColorConverter.YuvToRgb(yuv);

						CurrentArray.Array[index] = yuvToRgb.B;
						index++;
						CurrentArray.Array[index] = yuvToRgb.G;
						index++;
						CurrentArray.Array[index] = yuvToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageModes.Luma:
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
						Color lumaToRgb = ColorConverter.YuvToRgb(yuv.Luma, 0d, 0d);

						CurrentArray.Array[index] = lumaToRgb.B;
						index++;
						CurrentArray.Array[index] = lumaToRgb.G;
						index++;
						CurrentArray.Array[index] = lumaToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageModes.ColorDifferenceU:
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
						Color colorDifferenceUToRgb = ColorConverter.YuvToRgb(0, yuv.ColorDifferenceU, 0d);

						CurrentArray.Array[index] = colorDifferenceUToRgb.B;
						index++;
						CurrentArray.Array[index] = colorDifferenceUToRgb.G;
						index++;
						CurrentArray.Array[index] = colorDifferenceUToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
				case ImageModes.ColorDifferenceV:
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
						Color colorDifferenceVToRgb = ColorConverter.YuvToRgb(0d, 0d, yuv.ColorDifferenceV);

						CurrentArray.Array[index] = colorDifferenceVToRgb.B;
						index++;
						CurrentArray.Array[index] = colorDifferenceVToRgb.G;
						index++;
						CurrentArray.Array[index] = colorDifferenceVToRgb.R;
					});
					WriteableBmp = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr24, null, CurrentArray.Array, CurrentArray.Stride));
					break;
			}
			WriteableBmp.Freeze();
		}

		public void StretchHistogram(HistogramModes histogramMode)
		{
			int[] nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();

			switch (histogramMode)
			{
				case HistogramModes.Intensity:
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
				case HistogramModes.Red:
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
				case HistogramModes.Green:
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
				case HistogramModes.Blue:
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
									 {-1f,  5f, -1f},
									 { 0f, -1f,  0f}});
		}

		public void HighPassFilter2()
		{
			PassFilter(new float[,] {{-1f, -1f, -1f},
									 {-1f,  9f, -1f},
									 {-1f, -1f, -1f}});
		}

		public void HighPassFilter3()
		{
			PassFilter(new float[,] {{ 0f, -2f,  0f},
									 {-2f,  5f, -2f},
									 { 0f, -2f,  0f}});
		}

		public void MedianFilter(int apperture)
		{
			if (apperture > Width || apperture > Height || apperture % 2 == 0)
			{
				return;
			}

			int shift = apperture / 2;

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

			ByteArray tempRgbaArray = new ByteArray(RgbaArray.Width, RgbaArray.Height, RgbaArray.Channels);
			int[] nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
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
			ByteArray tempRgbaArray = new ByteArray(RgbaArray.Width, RgbaArray.Height, RgbaArray.Channels);
			int[] nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();
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
			int[] nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();

			switch (histogramMode)
			{
				case HistogramModes.Intensity:
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
				case HistogramModes.Red:
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
				case HistogramModes.Green:
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
				case HistogramModes.Blue:
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

		public void VerticalThreshold(HistogramModes histogramMode, byte firstVerticalThreshold, byte secondVerticalThreshold)
		{
			int[] nums = Enumerable.Range(0, RgbaArray.Array.Length / RgbaArray.Channels).Select(i => i * RgbaArray.Channels).ToArray();

			switch (histogramMode)
			{
				case HistogramModes.Intensity:
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
				case HistogramModes.Red:
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
				case HistogramModes.Green:
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
				case HistogramModes.Blue:
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
	}
}
