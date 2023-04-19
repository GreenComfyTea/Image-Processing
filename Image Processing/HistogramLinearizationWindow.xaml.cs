using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImageProcessing
{
	/// <summary>
	/// Interaction logic for HistogramLinearizationWindow.xaml
	/// </summary>
	public partial class HistogramLinearizationWindow : Window
	{
		public HistogramLinearizationWindow()
		{
			InitializeComponent();
		}

		public void UpdateData(HistogramMode histogramMode, MyImage myImage)
		{
			double size = myImage.Width * myImage.Height;
			double step = 1d / 256d;

			switch (histogramMode)
			{
				case HistogramMode.Intensity:
					byte[] newIntensityArray = new byte[256];
					double[] relativeCumulativeIntensity = myImage.ImageHistogram.GetLinearizationCumulativeIntensity(newIntensityArray);
					for (int i = 0; i < 256; i++)
					{
						histogramLinearizationListView.Items.Add(
							new HistogramLinearizationData(
								i,
								myImage.ImageHistogram.Intensity[i] / size,
								relativeCumulativeIntensity[i],
								step * (i + 1),
								newIntensityArray[i]
							)
						);
					}
					break;
				case HistogramMode.Red:
					byte[] newRedArray = new byte[256];
					double[] relativeCumulativeRed = myImage.ImageHistogram.GetLinearizationCumulativeRed(newRedArray);
					for (int i = 0; i < 256; i++)
					{
						histogramLinearizationListView.Items.Add(
							new HistogramLinearizationData(
								i,
								myImage.ImageHistogram.Red[i] / size,
								relativeCumulativeRed[i],
								step * (i + 1),
								newRedArray[i]
							)
						);
					}
					break;
				case HistogramMode.Green:
					byte[] newGreenArray = new byte[256];
					double[] relativeCumulativeGreen = myImage.ImageHistogram.GetLinearizationCumulativeGreen(newGreenArray);
					for (int i = 0; i < 256; i++)
					{
						histogramLinearizationListView.Items.Add(
							new HistogramLinearizationData(
								i,
								myImage.ImageHistogram.Green[i] / size,
								relativeCumulativeGreen[i],
								step * (i + 1),
								newGreenArray[i]
							)
						);
					}
					break;
				case HistogramMode.Blue:
					byte[] newBlueArray = new byte[256];
					double[] relativeCumulativeBlue = myImage.ImageHistogram.GetLinearizationCumulativeBlue(newBlueArray);
					for (int i = 0; i < 256; i++)
					{
						histogramLinearizationListView.Items.Add(
							new HistogramLinearizationData(
								i,
								myImage.ImageHistogram.Blue[i] / size,
								relativeCumulativeBlue[i],
								step * (i + 1),
								newBlueArray[i]
							)
						);
					}
					break;
			}
		}
	}
}
