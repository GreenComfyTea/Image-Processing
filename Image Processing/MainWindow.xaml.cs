using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageProcessing
{
	public partial class MainWindow : Window
	{
		private WriteableBitmap OriginalWriteableBitmap { get; set; }
		private bool AreEventsEnabled { get; set; }
		private MyImage Img { get; set; }
		private HistogramMode HistogramMode { get; set; }
		private bool LeftButtonDownResult { get; set; }

		public MainWindow()
		{
			InitializeComponent();

			DisableUI();
			loadImageButton.IsEnabled = true;

			histogramChart.Plot(Histogram.enumerationX, Histogram.enumerationX.Select(v => 0).ToArray());
			gammaSlider.Value = 1d;
			logarithmConstantSlider.Maximum = 5d * 255d / Math.Log10(256d);
			logarithmConstantSlider.Value = 255d / Math.Log10(256d);
			exponentConstantSlider.Value = 1d;
			gaussianBlurSizeSlider.Value = 5d;
			gaussianBlurSigmaSlider.Minimum = 0.5d;
			gaussianBlurSigmaSlider.Value = 1d;
			regionGrowingThresholdSlider.Value = 30d;

			firstVerticalThresholdSlider.Value = 64;
			secondVerticalThresholdSlider.Value = 128;

			LeftButtonDownResult = false;
		}

		private void DisableUI()
		{
			loadImageButton.IsEnabled = false;
			saveImageButton.IsEnabled = false;
			imageModeComboBox.IsEnabled = false;
			convertToGrayscaleButton.IsEnabled = false;
			restoreButton.IsEnabled = false;

			stretchHistogramButton.IsEnabled = false;
			histogramModeComboBox.IsEnabled = false;

			adaptiveVerticalThresholdButton.IsEnabled = false;
			horizontalThresholdSlider.IsEnabled = false;
			horizontalThresholdTextBox.IsEnabled = false;
			horizontalThresholdButton.IsEnabled = false;
			firstVerticalThresholdSlider.IsEnabled = false;
			firstVerticalThresholdTextBox.IsEnabled = false;
			secondVerticalThresholdSlider.IsEnabled = false;
			secondVerticalThresholdTextBox.IsEnabled = false;
			verticalThresholdButton.IsEnabled = false;

			histogramLinearizationButton.IsEnabled = false;
			showHistogramLinearizationDataButton.IsEnabled = false;

			filtersComboBox.IsEnabled = false;
			applyFilterButton.IsEnabled = false;
			operatorsComboBox.IsEnabled = false;
			applyOperatorButton.IsEnabled = false;

			gaussianBlurSizeSlider.IsEnabled = false;
			gaussianBlurSigmaSlider.IsEnabled = false;
			gaussianBlurSizeTextBox.IsEnabled = false;
			gaussianBlurSigmaTextBox.IsEnabled = false;
			gaussianBlurButton.IsEnabled = false;
			showGaussianFilterDataButton.IsEnabled = false;
			cannyOperatorButton.IsEnabled = false;

			gammaSlider.IsEnabled = false;
			gammaTextBox.IsEnabled = false;
			gammaCorrectionButton.IsEnabled = false;
			logarithmConstantSlider.IsEnabled = false;
			logarithmConstantTextBox.IsEnabled = false;
			manualLogarithmicTransformationButton.IsEnabled = false;
			adaptiveLogarithmicTransformationButton.IsEnabled = false;
			exponentConstantSlider.IsEnabled = false;
			exponentConstantTextBox.IsEnabled = false;
			exponentialTransformationButton.IsEnabled = false;
			
			regionGrowingThresholdSlider.IsEnabled = false;
			regionGrowingThresholdTextBox.IsEnabled = false;
			enableRegionGrowingCheckBox.IsEnabled = false;
			automaticRegionGrowingButton.IsEnabled = false;

			AreEventsEnabled = false;
		}

		private void EnableUI()
		{
			loadImageButton.IsEnabled = true;
			saveImageButton.IsEnabled = true;
			imageModeComboBox.IsEnabled = true;
			convertToGrayscaleButton.IsEnabled = true;
			restoreButton.IsEnabled = true;

			stretchHistogramButton.IsEnabled = true;
			histogramModeComboBox.IsEnabled = true;

			adaptiveVerticalThresholdButton.IsEnabled = true;
			horizontalThresholdSlider.IsEnabled = true;
			horizontalThresholdTextBox.IsEnabled = true;
			horizontalThresholdButton.IsEnabled = true;
			firstVerticalThresholdSlider.IsEnabled = true;
			firstVerticalThresholdTextBox.IsEnabled = true;
			secondVerticalThresholdSlider.IsEnabled = true;
			secondVerticalThresholdTextBox.IsEnabled = true;
			verticalThresholdButton.IsEnabled = true;

			histogramLinearizationButton.IsEnabled = true;
			showHistogramLinearizationDataButton.IsEnabled = true;

			filtersComboBox.IsEnabled = true;
			applyFilterButton.IsEnabled = true;
			operatorsComboBox.IsEnabled = true;
			applyOperatorButton.IsEnabled = true;

			gaussianBlurSizeSlider.IsEnabled = true;
			gaussianBlurSigmaSlider.IsEnabled = true;
			gaussianBlurSizeTextBox.IsEnabled = true;
			gaussianBlurSigmaTextBox.IsEnabled = true;
			gaussianBlurButton.IsEnabled = true;
			showGaussianFilterDataButton.IsEnabled = true;
			cannyOperatorButton.IsEnabled = true;

			gammaSlider.IsEnabled = true;
			gammaTextBox.IsEnabled = true;
			gammaCorrectionButton.IsEnabled = true;
			logarithmConstantSlider.IsEnabled = true;
			logarithmConstantTextBox.IsEnabled = true;
			manualLogarithmicTransformationButton.IsEnabled = true;
			adaptiveLogarithmicTransformationButton.IsEnabled = true;
			exponentConstantSlider.IsEnabled = true;
			exponentConstantTextBox.IsEnabled = true;
			exponentialTransformationButton.IsEnabled = true;

			regionGrowingThresholdSlider.IsEnabled = true;
			regionGrowingThresholdTextBox.IsEnabled = true;
			enableRegionGrowingCheckBox.IsEnabled = true;
			automaticRegionGrowingButton.IsEnabled = true;

			AreEventsEnabled = true;
		}

		private void LoadImage(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			openFileDialog.Filter = "Img Files|*.jpg;*.jpeg;*.png;*.bmp";

			if (openFileDialog.ShowDialog() == true)
			{
				if (openFileDialog.FileName.Length > 0)
				{
					DisableUI();

					OriginalWriteableBitmap = BitmapFactory.ConvertToPbgra32Format(BitmapFactory.FromStream(new MemoryStream(File.ReadAllBytes(openFileDialog.FileName))));
					originalImage.Source = OriginalWriteableBitmap;

					WriteableBitmap resultWriteableBitmap = OriginalWriteableBitmap.Clone();
					resultImage.Source = resultWriteableBitmap;

					Img = new MyImage(resultWriteableBitmap);

					imageModeComboBox.SelectedItem = Img.ImageMode;

					EnableUI();
					ChangeHistogramMode();
				}
			}
		}

		private void SaveImage(object sender, RoutedEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			saveFileDialog.Filter = "Img Files|*.jpg;*.jpeg;*.png;*.bmp";

			if (saveFileDialog.ShowDialog() == true)
			{
				if (saveFileDialog.FileName.Length > 0)
				{
					using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
					{
						PngBitmapEncoder encoder = new PngBitmapEncoder();
						encoder.Frames.Add(BitmapFrame.Create(Img.WriteableBmp));
						encoder.Save(stream);
					}
				}
			}
		}

		private void ConvertToGrayscale(object sender, RoutedEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			DisableUI();
			double exponent = exponentConstantSlider.Value;

			Task.Run(() => ConvertToGrayscale());
		}

		private void ConvertToGrayscale()
		{
			Img.ConvertToGrayscale();

			Dispatcher.Invoke(() => {
				Img.SetImageMode(Img.ImageMode);
				resultImage.Source = Img.WriteableBmp;
				ChangeHistogramMode();
				EnableUI();
			});
		}

		private void RestoreOriginal(object sender, RoutedEventArgs e)
		{
			RestoreOriginal();
		}

		private void RestoreOriginal()
		{
			DisableUI();
			WriteableBitmap resultWriteableBitmap = OriginalWriteableBitmap.Clone();
			resultImage.Source = resultWriteableBitmap;

			Img = new MyImage(resultWriteableBitmap);

			imageModeComboBox.SelectedItem = Img.ImageMode;

			EnableUI();
			ChangeHistogramMode();
		}

		private void ClearLabels()
		{
			positionX.Text = "-1";
			positionY.Text = "-1";

			intensityTextBlock.Text = "-1";

			originalRedTextBlock.Text = "-1";
			originalGreenTextBlock.Text = "-1";
			originalBlueTextBlock.Text = "-1";
			originalAlphaTextBlock.Text = "-1";

			convertedRedTextBlock.Text = "-1";
			convertedGreenTextBlock.Text = "-1";
			convertedBlueTextBlock.Text = "-1";
			convertedAlphaTextBlock.Text = "-1";

			hueTextBlock.Text = "-1";
			saturationTextBlock.Text = "-1";
			valueTextBlock.Text = "-1";

			cyanTextBlock.Text = "-1";
			magentaTextBlock.Text = "-1";
			yellowTextBlock.Text = "-1";
			blackTextBlock.Text = "-1";

			yTextBlock.Text = "-1";
			uTextBlock.Text = "-1";
			vTextBlock.Text = "-1";

			sRgbXTextBlock.Text = "-1";
			sRgbYTextBlock.Text = "-1";
			sRgbZTextBlock.Text = "-1";

			adobeRgbXTextBlock.Text = "-1";
			adobeRgbYTextBlock.Text = "-1";
			adobeRgbZTextBlock.Text = "-1";

			appleRgbXTextBlock.Text = "-1";
			appleRgbYTextBlock.Text = "-1";
			appleRgbZTextBlock.Text = "-1";

			colorMatchRgbXTextBlock.Text = "-1";
			colorMatchRgbYTextBlock.Text = "-1";
			colorMatchRgbZTextBlock.Text = "-1";

			proPhotoRgbXTextBlock.Text = "-1";
			proPhotoRgbYTextBlock.Text = "-1";
			proPhotoRgbZTextBlock.Text = "-1";

			sRgbLabLightnessTextBlock.Text = "-1";
			sRgbLabChromaATextBlock.Text = "-1";
			sRgbLabChromaBTextBlock.Text = "-1";

			adobeRgbLabLightnessTextBlock.Text = "-1";
			adobeRgbLabChromaATextBlock.Text = "-1";
			adobeRgbLabChromaBTextBlock.Text = "-1";

			appleRgbLabLightnessTextBlock.Text = "-1";
			appleRgbLabChromaATextBlock.Text = "-1";
			appleRgbLabChromaBTextBlock.Text = "-1";

			colorMatchRgbLabLightnessTextBlock.Text = "-1";
			colorMatchRgbLabChromaATextBlock.Text = "-1";
			colorMatchRgbLabChromaBTextBlock.Text = "-1";

			proPhotoRgbLabLightnessTextBlock.Text = "-1";
			proPhotoRgbLabChromaATextBlock.Text = "-1";
			proPhotoRgbLabChromaBTextBlock.Text = "-1";
		}

		private void UpdateLabels(int x, int y)
		{
			positionX.Text = x.ToString();
			positionY.Text = y.ToString();

			byte intensity = Img.GetIntensity(x, y);
			Color originalRgba = Img.GetRgbaColor(x, y);
			Color convertedRgba = Img.GetConvertedRgbaColor(x, y);
			HsvColor hsv = Img.GetHsvColor(x, y);
			CmykColor cmyk = Img.GetCmykColor(x, y);
			YuvColor yuv = Img.GetYuvColor(x, y);

			XyzColor sRgb = Img.GetSRgbColor(x, y);
			XyzColor adobeRgb = Img.GetAdobeRgbColor(x, y);
			XyzColor appleRgb = Img.GetAppleRgbColor(x, y);
			XyzColor colorMatchRgb = Img.GetColorMatchRgbColor(x, y);
			XyzColor proPhotoRgb = Img.GetProPhotoRgbColor(x, y);

			LabColor sRgbLab = Img.GetSRgbLabColor(x, y);
			LabColor adobeRgbLab = Img.GetAdobeRgbLabColor(x, y);
			LabColor appleRgbLab = Img.GetAppleRgbLabColor(x, y);
			LabColor colorMatchRgbLab = Img.GetColorMatchRgbLabColor(x, y);
			LabColor proPhotoRgbLab = Img.GetProPhotoRgbLabColor(x, y);

			intensityTextBlock.Text = intensity.ToString();

			originalRedTextBlock.Text = originalRgba.R.ToString();
			originalGreenTextBlock.Text = originalRgba.G.ToString();
			originalBlueTextBlock.Text = originalRgba.B.ToString();
			originalAlphaTextBlock.Text = originalRgba.A.ToString();

			convertedRedTextBlock.Text = convertedRgba.R.ToString();
			convertedGreenTextBlock.Text = convertedRgba.G.ToString();
			convertedBlueTextBlock.Text = convertedRgba.B.ToString();
			convertedAlphaTextBlock.Text = convertedRgba.A.ToString();

			hueTextBlock.Text = string.Format("{0:0.#}", hsv.Hue);
			saturationTextBlock.Text = string.Format("{0:0.##}", hsv.Saturation);
			valueTextBlock.Text = string.Format("{0:0.##}", hsv.Value);

			cyanTextBlock.Text = string.Format("{0:0.##}", cmyk.Cyan);
			magentaTextBlock.Text = string.Format("{0:0.##}", cmyk.Magenta);
			yellowTextBlock.Text = string.Format("{0:0.##}", cmyk.Yellow);
			blackTextBlock.Text = string.Format("{0:0.##}", cmyk.Black);

			yTextBlock.Text = string.Format("{0:0.##}", yuv.Luma);
			uTextBlock.Text = string.Format("{0:0.##}", yuv.ColorDifferenceU);
			vTextBlock.Text = string.Format("{0:0.##}", yuv.ColorDifferenceV);

			sRgbXTextBlock.Text = string.Format("{0:0.##}", sRgb.X);
			sRgbYTextBlock.Text = string.Format("{0:0.##}", sRgb.Y);
			sRgbZTextBlock.Text = string.Format("{0:0.##}", sRgb.Z);

			adobeRgbXTextBlock.Text = string.Format("{0:0.##}", adobeRgb.X);
			adobeRgbYTextBlock.Text = string.Format("{0:0.##}", adobeRgb.Y);
			adobeRgbZTextBlock.Text = string.Format("{0:0.##}", adobeRgb.Z);

			appleRgbXTextBlock.Text = string.Format("{0:0.##}", appleRgb.X);
			appleRgbYTextBlock.Text = string.Format("{0:0.##}", appleRgb.Y);
			appleRgbZTextBlock.Text = string.Format("{0:0.##}", appleRgb.Z);

			colorMatchRgbXTextBlock.Text = string.Format("{0:0.##}", colorMatchRgb.X);
			colorMatchRgbYTextBlock.Text = string.Format("{0:0.##}", colorMatchRgb.Y);
			colorMatchRgbZTextBlock.Text = string.Format("{0:0.##}", colorMatchRgb.Z);

			proPhotoRgbXTextBlock.Text = string.Format("{0:0.##}", proPhotoRgb.X);
			proPhotoRgbYTextBlock.Text = string.Format("{0:0.##}", proPhotoRgb.Y);
			proPhotoRgbZTextBlock.Text = string.Format("{0:0.##}", proPhotoRgb.Z);

			sRgbLabLightnessTextBlock.Text = sRgbLab.Lightness.ToString();
			sRgbLabChromaATextBlock.Text = sRgbLab.ChromaA.ToString();
			sRgbLabChromaBTextBlock.Text = sRgbLab.ChromaB.ToString();

			adobeRgbLabLightnessTextBlock.Text = adobeRgbLab.Lightness.ToString();
			adobeRgbLabChromaATextBlock.Text = adobeRgbLab.ChromaA.ToString();
			adobeRgbLabChromaBTextBlock.Text = adobeRgbLab.ChromaB.ToString();

			appleRgbLabLightnessTextBlock.Text = appleRgbLab.Lightness.ToString();
			appleRgbLabChromaATextBlock.Text = appleRgbLab.ChromaA.ToString();
			appleRgbLabChromaBTextBlock.Text = appleRgbLab.ChromaB.ToString();

			colorMatchRgbLabLightnessTextBlock.Text = colorMatchRgbLab.Lightness.ToString();
			colorMatchRgbLabChromaATextBlock.Text = colorMatchRgbLab.ChromaA.ToString();
			colorMatchRgbLabChromaBTextBlock.Text = colorMatchRgbLab.ChromaB.ToString();

			proPhotoRgbLabLightnessTextBlock.Text = proPhotoRgbLab.Lightness.ToString();
			proPhotoRgbLabChromaATextBlock.Text = proPhotoRgbLab.ChromaA.ToString();
			proPhotoRgbLabChromaBTextBlock.Text = proPhotoRgbLab.ChromaB.ToString();
		}

		private void MouseMoveOriginal(object sender, MouseEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			try
			{
				double ratioX = e.GetPosition(originalImage).X / originalImage.ActualWidth;
				double ratioY = e.GetPosition(originalImage).Y / originalImage.ActualHeight;

				int x = Calculations.ClampToInt(ratioX * Img.Width);
				int y = Calculations.ClampToInt(ratioY * Img.Height);

				if (x >= Img.Width || y >= Img.Height)
				{
					return;
				}

				UpdateLabels(x, y);
			}
			catch (NullReferenceException ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		private void MouseMoveResult(object sender, MouseEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			try
			{
				double ratioX = e.GetPosition(resultImage).X / resultImage.ActualWidth;
				double ratioY = e.GetPosition(resultImage).Y / resultImage.ActualHeight;

				int x = Calculations.ClampToInt(ratioX * Img.Width);
				int y = Calculations.ClampToInt(ratioY * Img.Height);

				if (x >= Img.Width || y >= Img.Height)
				{
					return;
				}

				UpdateLabels(x, y);
			}
			catch (NullReferenceException ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		private new void MouseLeave(object sender, MouseEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			ClearLabels();
		}

		private void ImageModeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			if (imageModeComboBox.SelectedIndex == -1)
			{
				return;
			}

			DisableUI();
			ImageMode imageMode = (ImageMode) imageModeComboBox.SelectedIndex;

			Task.Run(() => ProcessImageModeChange(imageMode));
		}

		private void ProcessImageModeChange(ImageMode ImgMode)
		{
			Img.SetImageMode(ImgMode);
			Dispatcher.Invoke(() => {
				resultImage.Source = Img.WriteableBmp;
				EnableUI();
			});
		}

		private void HistogramModeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			ChangeHistogramMode();
		}

		private void ChangeHistogramMode()
		{
			HistogramMode = (HistogramMode)histogramModeComboBox.SelectedIndex;

			switch (HistogramMode)
			{
				case HistogramMode.Intensity:
					histogramChart.Stroke = new SolidColorBrush(Colors.Black);
					histogramChart.Color = histogramChart.Stroke;
					histogramChart.Description = "Intensity";
					histogramChart.Plot(Histogram.enumerationX, Img.ImageHistogram.Intensity);
					histogramChart.Plot(Histogram.enumerationX, Img.ImageHistogram.Intensity);
					horizontalThresholdSlider.Maximum = Img.ImageHistogram.IntensityMaxValue;
					break;
				case HistogramMode.Red:
					histogramChart.Stroke = new SolidColorBrush(Colors.Red);
					histogramChart.Color = histogramChart.Stroke;
					histogramChart.Description = "Red";
					histogramChart.Plot(Histogram.enumerationX, Img.ImageHistogram.Red);
					horizontalThresholdSlider.Maximum = Img.ImageHistogram.RedMaxValue;
					break;
				case HistogramMode.Green:
					histogramChart.Stroke = new SolidColorBrush(Colors.Green);
					histogramChart.Color = histogramChart.Stroke;
					histogramChart.Description = "Green";
					histogramChart.Plot(Histogram.enumerationX, Img.ImageHistogram.Green);
					horizontalThresholdSlider.Maximum = Img.ImageHistogram.GreenMaxValue;
					break;
				case HistogramMode.Blue:
					histogramChart.Stroke = new SolidColorBrush(Colors.Blue);
					histogramChart.Color = histogramChart.Stroke;
					histogramChart.Description = "Blue";
					histogramChart.Plot(Histogram.enumerationX, Img.ImageHistogram.Blue);
					horizontalThresholdSlider.Maximum = Img.ImageHistogram.BlueMaxValue;
					break;
			}
		}

		private void StretchHistogram(object sender, RoutedEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			DisableUI();

			Task.Run(() => StretchHistogram());

		}

		private void StretchHistogram()
		{
			Img.StretchHistogram(HistogramMode);

			Dispatcher.Invoke(() => {
				Img.SetImageMode(Img.ImageMode);
				resultImage.Source = Img.WriteableBmp;
				ChangeHistogramMode();
				EnableUI();
			});
		}

		private void ApplyFilter(object sender, RoutedEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			DisableUI();
			Filter filter = (Filter)filtersComboBox.SelectedIndex;

			Task.Run(() => ApplyFilter(filter));
		}

		private void ApplyFilter(Filter filter)
		{
			switch (filter)
			{
				case Filter.LowPassFilter1:
					Img.LowPassFilter1();
					break;
				case Filter.LowPassFilter2:
					Img.LowPassFilter2();
					break;
				case Filter.LowPassFilter3:
					Img.LowPassFilter3();
					break;
				case Filter.HighPassFilter1:
					Img.HighPassFilter1();
					break;
				case Filter.HighPassFilter2:
					Img.HighPassFilter2();
					break;
				case Filter.HighPassFilter3:
					Img.HighPassFilter3();
					break;
				case Filter.MedianFilter3x3:
					Img.MedianFilter(3);
					break;
				case Filter.MedianFilter5x5:
					Img.MedianFilter(5);
					break;
				case Filter.MedianFilter7x7:
					Img.MedianFilter(7);
					break;
			}

			Dispatcher.Invoke(() => {
				Img.SetImageMode(Img.ImageMode);
				resultImage.Source = Img.WriteableBmp;
				ChangeHistogramMode();
				EnableUI();
			});
		}

		private void ApplyOperator(object sender, RoutedEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			DisableUI();
			Operator operator_ = (Operator)operatorsComboBox.SelectedIndex;

			Task.Run(() => ApplyOperator(operator_));
		}

		private void ApplyOperator(Operator operator_)
		{
			switch (operator_)
			{
				case Operator.SobelOperator:
					Img.SobelOperator();
					break;
				case Operator.RobertsCrossOperator:
					Img.RobertsCrossOperator();
					break;
				case Operator.PrewittOperator:
					Img.PrewittOperator();
					break;
			}

			Dispatcher.Invoke(() => {
				Img.SetImageMode(Img.ImageMode);
				resultImage.Source = Img.WriteableBmp;
				ChangeHistogramMode();
				EnableUI();
			});
		}

		private void AdaptiveVerticalThreshold(object sender, RoutedEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			DisableUI();

			Task.Run(() => AdaptiveVerticalThreshold());
		}

		private void AdaptiveVerticalThreshold()
		{
			Img.AdaptiveVerticalThreshold(HistogramMode);

			Dispatcher.Invoke(() => {
				Img.SetImageMode(Img.ImageMode);
				resultImage.Source = Img.WriteableBmp;
				ChangeHistogramMode();
				EnableUI();
			});
		}

		private void HorizontalThreshold(object sender, RoutedEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			DisableUI();
			int horizontalThreshold = Calculations.ClampToInt(horizontalThresholdSlider.Value);

			Task.Run(() => HorizontalThreshold(horizontalThreshold));
		}

		private void HorizontalThreshold(int horizontalThreshold)
		{
			Img.HorizontalThreshold(HistogramMode, horizontalThreshold);

			Dispatcher.Invoke(() => {
				Img.SetImageMode(Img.ImageMode);
				resultImage.Source = Img.WriteableBmp;
				ChangeHistogramMode();
				EnableUI();
			});
		}

		private void VerticalThreshold(object sender, RoutedEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			DisableUI();
			byte verticalThreshold1 = Calculations.ClampToByte(firstVerticalThresholdSlider.Value);
			byte verticalThreshold2 = Calculations.ClampToByte(secondVerticalThresholdSlider.Value);

			Task.Run(() => VerticalThreshold(verticalThreshold1, verticalThreshold2));
		}

		private void VerticalThreshold(byte verticalThreshold1, byte verticalThreshold2)
		{
			Img.VerticalThreshold(HistogramMode, verticalThreshold1, verticalThreshold2);

			Dispatcher.Invoke(() => {
				Img.SetImageMode(Img.ImageMode);
				resultImage.Source = Img.WriteableBmp;
				ChangeHistogramMode();
				EnableUI();
			});
		}

		private void HorizontalThresholdChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			horizontalThresholdTextBox.Text = horizontalThresholdSlider.Value.ToString();
		}

		private void FirstVerticalThresholdChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			firstVerticalThresholdTextBox.Text = firstVerticalThresholdSlider.Value.ToString();
			secondVerticalThresholdSlider.Minimum = firstVerticalThresholdSlider.Value;
		}

		private void SecondVerticalThresholdChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			secondVerticalThresholdTextBox.Text = secondVerticalThresholdSlider.Value.ToString();
		}

		private void GammaChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			gammaTextBox.Text = string.Format("{0:0.##}", gammaSlider.Value);
		}

		private void LogarithmConstantChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			logarithmConstantTextBox.Text = string.Format("{0:0.##}", logarithmConstantSlider.Value);
		}

		private void ExponentConstantChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			exponentConstantTextBox.Text = string.Format("{0:0.##}", exponentConstantSlider.Value.ToString());
		}

		private void GaussianBlurSizeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (gaussianBlurSizeTextBox == null) return;

			gaussianBlurSizeTextBox.Text = gaussianBlurSizeSlider.Value.ToString();
		}

		private void GaussianBlurSigmaChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			gaussianBlurSigmaTextBox.Text = string.Format("{0:0.##}", gaussianBlurSigmaSlider.Value);
		}

		private void RegionGrowingThresholdChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (regionGrowingThresholdTextBox == null) return;

			regionGrowingThresholdTextBox.Text = regionGrowingThresholdSlider.Value.ToString();
		}

		private void GammaCorrection(object sender, RoutedEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			DisableUI();
			double gamma = gammaSlider.Value;

			Task.Run(() => GammaCorrection(gamma));
		}

		private void GammaCorrection(double gamma)
		{
			Img.GammaCorrection(gamma);

			Dispatcher.Invoke(() => {
				Img.SetImageMode(Img.ImageMode);
				resultImage.Source = Img.WriteableBmp;
				ChangeHistogramMode();
				EnableUI();
			});
		}

		private void ManualLogarithmicTransformation(object sender, RoutedEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			DisableUI();
			double logarithm = logarithmConstantSlider.Value;
			Task.Run(() => ManualLogarithmicTransformation(logarithm));
		}

		private void ManualLogarithmicTransformation(double logarithm)
		{
			Img.LogarithmicTransformation(logarithm);

			Dispatcher.Invoke(() => {
				Img.SetImageMode(Img.ImageMode);
				resultImage.Source = Img.WriteableBmp;
				ChangeHistogramMode();
				EnableUI();
			});
		}

		private void AdaptiveLogarithmicTransformation(object sender, RoutedEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			DisableUI();

			Task.Run(() => AdaptiveLogarithmicTransformation());
		}

		private void AdaptiveLogarithmicTransformation()
		{
			Img.LogarithmicTransformation();

			Dispatcher.Invoke(() => {
				Img.SetImageMode(Img.ImageMode);
				resultImage.Source = Img.WriteableBmp;
				ChangeHistogramMode();
				EnableUI();
			});
		}

		private void ExponentialTransformation(object sender, RoutedEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			DisableUI();
			double exponent = exponentConstantSlider.Value;

			Task.Run(() => ExponentialTransformation(exponent));
		}

		private void ExponentialTransformation(double exponent)
		{
			Img.ExponentialCorrection(exponent);

			Dispatcher.Invoke(() => {
				Img.SetImageMode(Img.ImageMode);
				resultImage.Source = Img.WriteableBmp;
				ChangeHistogramMode();
				EnableUI();
			});
		}

		private void HistogramLinearization(object sender, RoutedEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			DisableUI();

			Task.Run(() => HistogramLinearization());
		}

		private void HistogramLinearization()
		{
			Img.HistogramLinearization(HistogramMode);

			Dispatcher.Invoke(() => {
				Img.SetImageMode(Img.ImageMode);
				resultImage.Source = Img.WriteableBmp;
				ChangeHistogramMode();
				EnableUI();
			});
		}

		private void ShowHistogramLinearizationData(object sender, RoutedEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			DisableUI();

			HistogramLinearizationWindow histogramLinearizationWindow = new HistogramLinearizationWindow();
			histogramLinearizationWindow.UpdateData(HistogramMode, Img);
			histogramLinearizationWindow.Show();
			
			EnableUI();
		}

		private void ShowGaussianFilterData(object sender, RoutedEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			DisableUI();

			int size = Calculations.ClampToInt(gaussianBlurSizeSlider.Value);
			double sigma = gaussianBlurSigmaSlider.Value;

			GaussianBlurDataWindow gaussianBlurDataWindow = new GaussianBlurDataWindow();
			gaussianBlurDataWindow.UpdateData(size, sigma);
			gaussianBlurDataWindow.Show();

			EnableUI();
		}

		private void GaussianBlur(object sender, RoutedEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			DisableUI();

			int size = Calculations.ClampToInt(gaussianBlurSizeSlider.Value);
			double sigma = gaussianBlurSigmaSlider.Value;

			Task.Run(() => GaussianBlur(size, sigma));
		}

		private void GaussianBlur(int size, double sigma)
		{
			Img.GaussianBlur(size, sigma);

			Dispatcher.Invoke(() => {
				Img.SetImageMode(Img.ImageMode);
				resultImage.Source = Img.WriteableBmp;
				ChangeHistogramMode();
				EnableUI();
			});
		}

		private void CannyOperator(object sender, RoutedEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			DisableUI();

			int gaussianBlurSize = Calculations.ClampToInt(gaussianBlurSizeSlider.Value);
			double gaussianBlurSigma = gaussianBlurSigmaSlider.Value;
			byte verticalThreshold1 = Calculations.ClampToByte(firstVerticalThresholdSlider.Value);
			byte verticalThreshold2 = Calculations.ClampToByte(secondVerticalThresholdSlider.Value);
			Operator operator_ = (Operator) operatorsComboBox.SelectedIndex;

			Task.Run(() => CannyOperator(gaussianBlurSize, gaussianBlurSigma, operator_, verticalThreshold1, verticalThreshold2));
		}

		private void CannyOperator(int size, double sigma, Operator operator_, byte threshold1, byte threshold2)
		{
			Img.CannyOperator(size, sigma, operator_, threshold1, threshold2);

			Dispatcher.Invoke(() => {
				Img.SetImageMode(Img.ImageMode);
				resultImage.Source = Img.WriteableBmp;
				ChangeHistogramMode();
				EnableUI();
			});
		}

		private void MouseLeftButtonDownResult(object sender, MouseButtonEventArgs e)
		{
			if (enableRegionGrowingCheckBox.IsChecked == true)
			{
				LeftButtonDownResult = true;
			}
		}

		private void MouseLeftButtonUpResult(object sender, MouseButtonEventArgs e)
		{
			if (!AreEventsEnabled || !LeftButtonDownResult)
			{
				return;
			}

			LeftButtonDownResult = false;

			try
			{
				double ratioX = e.GetPosition(resultImage).X / resultImage.ActualWidth;
				double ratioY = e.GetPosition(resultImage).Y / resultImage.ActualHeight;

				int x = Calculations.ClampToInt(ratioX * Img.Width);
				int y = Calculations.ClampToInt(ratioY * Img.Height);

				if (x >= Img.Width || y >= Img.Height)
				{
					return;
				}

				DisableUI();

				byte threshold = Calculations.ClampToByte(regionGrowingThresholdSlider.Value);

				Task.Run(() => RegionGrowing(x, y, threshold));
			}
			catch (NullReferenceException ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		private void RegionGrowing(int x, int y, byte threshold)
		{
			Img.RegionGrowing(x, y, threshold);

			Dispatcher.Invoke(() => {
				Img.SetImageMode(Img.ImageMode);
				resultImage.Source = Img.WriteableBmp;
				ChangeHistogramMode();
				EnableUI();
			});
		}

		private void AutomaticRegionGrowing(object sender, RoutedEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			DisableUI();

			byte threshold = Calculations.ClampToByte(regionGrowingThresholdSlider.Value);

			Task.Run(() => AutomaticRegionGrowing(threshold));
		}

		private void AutomaticRegionGrowing(byte threshold)
		{
			Img.AutomaticRegionGrowing(threshold);

			Dispatcher.Invoke(() => {
				Img.SetImageMode(Img.ImageMode);
				resultImage.Source = Img.WriteableBmp;
				ChangeHistogramMode();
				EnableUI();
			});
		}
	}
}
