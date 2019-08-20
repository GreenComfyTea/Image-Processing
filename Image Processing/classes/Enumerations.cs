using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
	public enum ImageModes
	{
		Grayscale,
		RGB,
		RGBa,
		Red,
		Green,
		Blue,
		Alpha,
		HSV,
		Hue,
		Saturation,
		Value,
		CMYK,
		Cyan,
		Magenta,
		Yellow,
		Black,
		YUV,
		Luma,
		ColorDifferenceU,
		ColorDifferenceV
	}

	public enum HistogramModes
	{
		Intensity,
		Red,
		Green,
		Blue
	}

	public enum Filters
	{
		LowPassFilter1,
		LowPassFilter2,
		LowPassFilter3,
		HighPassFilter1,
		HighPassFilter2,
		HighPassFilter3,
		MedianFilter3x3,
		MedianFilter5x5,
		MedianFilter7x7
	}

	public enum Operators
	{
		SobelOperator,
		RobertsCrossOperator,
		PrewittOperator,
	}
}
