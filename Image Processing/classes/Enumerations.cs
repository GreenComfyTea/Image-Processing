using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
	public enum ImageMode
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
		ColorDifferenceV,
		sRGB_Lab,
		sRGB_Lightness,
		sRGB_ChromaA,
		sRGB_ChromaB,
		AdobeRGB_Lab,
		AdobeRGB_Lightness,
		AdobeRGB_ChromaA,
		AdobeRGB_ChromaB,
		AppleRGB_Lab,
		AppleRGB_Lightness,
		AppleRGB_ChromaA,
		AppleRGB_ChromaB,
		ColorMatchRGB_Lab,
		ColorMatchRGB_Lightness,
		ColorMatchRGB_ChromaA,
		ColorMatchRGB_ChromaB,
		ProPhotoRGB_Lab,
		ProPhotoRGB_Lightness,
		ProPhotoRGB_ChromaA,
		ProPhotoRGB_ChromaB
	}

	public enum HistogramMode
	{
		Intensity,
		Red,
		Green,
		Blue
	}

	public enum Filter
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

	public enum Operator
	{
		SobelOperator,
		RobertsCrossOperator,
		PrewittOperator
	}

	public enum Direction
	{
		Horizontal,
		Vertical,
		MainDiagonal,
		AntiDiagonal
	}
}
