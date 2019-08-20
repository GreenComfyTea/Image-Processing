using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageProcessing
{
	public interface IImage
	{
		WriteableBitmap WriteableBmp { get; set; }
		int Width { get; set; }
		int Height { get; set; }
		ByteArray CurrentArray { get; set; }
		ImageModes ImageMode { get; set; }
		Histogram ImageHistogram { get; set; }

		void SetImageMode(ImageModes imageMode);
		void CalculateColorspaces(ImageModes imageMode);
		byte GetIntensity(int x, int y);
		Color GetRgbaColor(int x, int y);
		Color GetConvertedRgbaColor(int x, int y);
		HsvColor GetHsvColor(int x, int y);
		CmykColor GetCmykColor(int x, int y);
		YuvColor GetYuvColor(int x, int y);
		void StretchHistogram(HistogramModes histogramMode);
		void PassFilter(float[,] filterMatrix, float filterCoefficient = 0);
		void LowPassFilter1();
		void LowPassFilter2();
		void LowPassFilter3();
		void HighPassFilter1();
		void HighPassFilter2();
		void HighPassFilter3();
		void MedianFilter(int apperture);
		void Operator2x2(int[,] xOperatorMatrix, int[,] yOperatorMatrix);
		void OperatorOdd(int[,] xOperatorMatrix, int[,] yOperatorMatrix);
		void SobelOperator();
		void RobertsCrossOperator();
		void PrewittOperator();
		void AdaptiveVerticalThreshold(HistogramModes histogramMode);
		void HorizontalThreshold(HistogramModes histogramMode, int horizontalThreshold);
		void VerticalThreshold(HistogramModes histogramMode, byte firstVerticalThreshold, byte secondVerticalThreshold);
	}
}
