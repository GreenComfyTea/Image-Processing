using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AL140_07
{
	public class Histogram
	{
		public static int[] enumerationX = Enumerable.Range(0, 256).ToArray();
		private IImage Image { get; set; }
		public int[] Intensity { get; set; }
		public int[] Red { get; set; }
		public int[] Green { get; set; }
		public int[] Blue { get; set; }
		public byte IntensityMin { get; set; }
		public byte IntensityMax { get; set; }
		public byte RedMin { get; set; }
		public byte RedMax { get; set; }
		public byte GreenMin { get; set; }
		public byte GreenMax { get; set; }
		public byte BlueMin { get; set; }
		public byte BlueMax { get; set; }

		public int IntensityMaxValue { get; set; }
		public int RedMaxValue { get; set; }
		public int GreenMaxValue { get; set; }
		public int BlueMaxValue { get; set; }

		private readonly object lockObject = new object();

		public Histogram(IImage image)
		{
			Image = image;
		}

		public void UpdateHistogram()
		{
			Intensity = new int[256];
			Red = new int[256];
			Green = new int[256];
			Blue = new int[256];

			IntensityMin = 255;
			IntensityMax = 0;
			RedMin = 255;
			RedMax = 0;
			GreenMin = 255;
			GreenMax = 0;
			BlueMin = 255;
			BlueMax = 0;

			IntensityMaxValue = 0;
			RedMaxValue = 0;
			GreenMaxValue = 0;
			BlueMaxValue = 0;

			int[] nums = Enumerable.Range(0, Image.Width).ToArray();
			Parallel.ForEach(nums, x =>
			{
				for (int y = 0; y < Image.Height; y++)
				{
					Color color = Image.GetRgbaColor(x, y);
					byte intensity = Image.GetIntensity(x, y);

					lock (lockObject)
					{
						if (intensity < IntensityMin)
						{
							IntensityMin = intensity;
						}
						else if (intensity > IntensityMax)
						{
							IntensityMax = intensity;
						}

						if (color.R < RedMin)
						{
							RedMin = color.R;
						}
						else if (color.R > RedMax)
						{
							RedMax = color.R;
						}

						if (color.G < GreenMin)
						{
							GreenMin = color.G;
						}
						else if (color.G > GreenMax)
						{
							GreenMax = color.G;
						}

						if (color.B < BlueMin)
						{
							BlueMin = color.B;
						}
						else if (color.B > BlueMax)
						{
							BlueMax = color.B;
						}
					
						Intensity[intensity]++;
						Red[color.R]++;
						Green[color.G]++;
						Blue[color.B]++;

						if(Intensity[intensity] > IntensityMaxValue)
						{
							IntensityMaxValue = Intensity[intensity];
						}

						if (Red[intensity] > RedMaxValue)
						{
							RedMaxValue = Red[intensity];
						}

						if (Green[intensity] > GreenMaxValue)
						{
							GreenMaxValue = Green[intensity];
						}

						if (Blue[intensity] > BlueMaxValue)
						{
							BlueMaxValue = Blue[intensity];
						}
					}
				}
			});
		}

		public byte CalculateAdaptiveVerticalThreshold(HistogramModes histogramMode)
		{

			int[] array = null;

			byte min = 0;
			byte max = 0;

			switch (histogramMode)
			{
				case HistogramModes.Intensity:
					array = Intensity;
					min = IntensityMin;
					max = IntensityMax;
					break;
				case HistogramModes.Red:
					array = Red;
					min = RedMin;
					max = RedMax;
					break;
				case HistogramModes.Green:
					array = Green;
					min = GreenMin;
					max = GreenMax;
					break;
				case HistogramModes.Blue:
					array = Blue;
					min = BlueMin;
					max = BlueMax;
					break;
			}

			byte previousThreshold = 0;
			byte threshold = Calculations.ClampToByte((max - min) / 2);

			while (previousThreshold != threshold)
			{
				byte lessAverage = CalculateAverageByte(array, min, threshold);
				byte greatAverage = CalculateAverageByte(array, Calculations.ClampToByte(threshold + 1), max);

				previousThreshold = threshold;
				threshold = Calculations.ClampToByte((lessAverage + greatAverage) / 2);
			}

			return threshold;
		}

		private byte CalculateAverageByte(int[] array, byte min, byte max)
		{
			int sum = 0;
			int count = 0;
			for (int value = min; value <= max; value++)
			{
				sum += value * array[value];
				count += array[value];
			}
			return Calculations.ClampToByte(sum / count);
		}
	}
}
