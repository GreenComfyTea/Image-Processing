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

namespace AL140_07
{
	public partial class MainWindow : Window
	{
		private readonly static int MAX_SIZE = 1920 * 1080;
		private bool LowPerformanceMode { get; set; }
		private WriteableBitmap OriginalWriteableBitmap { get; set; }
		private bool AreEventsEnabled { get; set; }
		private IImage Image { get; set; }
		private HistogramModes HistogramMode { get; set; }

		public MainWindow()
		{
			InitializeComponent();

			DisableUI();
			loadImageButton.IsEnabled = true;

			histogramChart.Plot(Histogram.enumerationX, Histogram.enumerationX.Select(v => 0).ToArray());
		}

		private void DisableUI()
		{
			loadImageButton.IsEnabled = false;
			saveImageButton.IsEnabled = false;
			imageModeComboBox.IsEnabled = false;
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

			filtersComboBox.IsEnabled = false;
			applyFilterButton.IsEnabled = false;
			operatorsComboBox.IsEnabled = false;
			applyOperatorButton.IsEnabled = false;

			AreEventsEnabled = false;
		}

		private void EnableUI()
		{
			loadImageButton.IsEnabled = true;
			saveImageButton.IsEnabled = true;
			imageModeComboBox.IsEnabled = true;
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

			filtersComboBox.IsEnabled = true;
			applyFilterButton.IsEnabled = true;
			operatorsComboBox.IsEnabled = true;
			applyOperatorButton.IsEnabled = true;

			AreEventsEnabled = true;
		}

		private void LoadImage(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

			if (openFileDialog.ShowDialog() == true)
			{
				if (openFileDialog.FileName.Length > 0)
				{
					DisableUI();

					OriginalWriteableBitmap = BitmapFactory.FromStream(new MemoryStream(File.ReadAllBytes(openFileDialog.FileName)));

					originalImage.Source = OriginalWriteableBitmap;

					WriteableBitmap resultWriteableBitmap = OriginalWriteableBitmap.Clone();
					resultImage.Source = resultWriteableBitmap;

					if (resultWriteableBitmap.PixelWidth * resultWriteableBitmap.PixelHeight > MAX_SIZE)
					{
						LowPerformanceMode = true;
						Image = new LowPerformanceImage(resultWriteableBitmap);
					}
					else
					{
						LowPerformanceMode = false;
						Image = new HighPerformanceImage(resultWriteableBitmap);
					}

					imageModeComboBox.SelectedItem = Image.ImageMode;

					Task.Run(() => ProcessImage());
				}
			}
		}

		private void ProcessImage()
		{
			Image.CalculateColorspaces(Image.ImageMode);

			Dispatcher.Invoke(() => {
				EnableUI();
				ChangeHistogramMode();
			});
		}

		private void SaveImage(object sender, RoutedEventArgs e)
		{
			if (!AreEventsEnabled)
			{
				return;
			}

			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			saveFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

			if (saveFileDialog.ShowDialog() == true)
			{
				if (saveFileDialog.FileName.Length > 0)
				{
					using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
					{
						PngBitmapEncoder encoder = new PngBitmapEncoder();
						encoder.Frames.Add(BitmapFrame.Create(Image.WriteableBmp));
						encoder.Save(stream);
					}
				}
			}
		}

		private void RestoreOriginal(object sender, RoutedEventArgs e)
		{
			DisableUI();
			WriteableBitmap resultWriteableBitmap = OriginalWriteableBitmap.Clone();
			resultImage.Source = resultWriteableBitmap;

			if (resultWriteableBitmap.PixelWidth * resultWriteableBitmap.PixelHeight > MAX_SIZE)
			{
				LowPerformanceMode = true;
				Image = new LowPerformanceImage(resultWriteableBitmap);
			}
			else
			{
				LowPerformanceMode = false;
				Image = new HighPerformanceImage(resultWriteableBitmap);
			}

			imageModeComboBox.SelectedItem = Image.ImageMode;

			Task.Run(() => ProcessImage());
		}

		private void ClearLabels()
		{
			positionX.Text = "-1";
			positionY.Text = "-1";

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
		}

		private void UpdateLabels(int x, int y)
		{
			positionX.Text = x.ToString();
			positionY.Text = y.ToString();

			Color originalRgba = Image.GetRgbaColor(x, y);
			Color convertedRgba = Image.GetConvertedRgbaColor(x, y);
			HsvColor hsv = Image.GetHsvColor(x, y);
			CmykColor cmyk = Image.GetCmykColor(x, y);
			YuvColor yuv = Image.GetYuvColor(x, y);

			originalRedTextBlock.Text = originalRgba.R.ToString();
			originalGreenTextBlock.Text = originalRgba.G.ToString();
			originalBlueTextBlock.Text = originalRgba.B.ToString();
			originalAlphaTextBlock.Text = originalRgba.A.ToString();

			convertedRedTextBlock.Text = convertedRgba.R.ToString();
			convertedGreenTextBlock.Text = convertedRgba.G.ToString();
			convertedBlueTextBlock.Text = convertedRgba.B.ToString();
			convertedAlphaTextBlock.Text = convertedRgba.A.ToString();

			hueTextBlock.Text = String.Format("{0:0.#}", hsv.Hue);
			saturationTextBlock.Text = String.Format("{0:0.##}", hsv.Saturation);
			valueTextBlock.Text = String.Format("{0:0.##}", hsv.Value);

			cyanTextBlock.Text = String.Format("{0:0.##}", cmyk.Cyan);
			magentaTextBlock.Text = String.Format("{0:0.##}", cmyk.Magenta);
			yellowTextBlock.Text = String.Format("{0:0.##}", cmyk.Yellow);
			blackTextBlock.Text = String.Format("{0:0.##}", cmyk.Black);

			yTextBlock.Text = String.Format("{0:0.##}", yuv.Luma);
			uTextBlock.Text = String.Format("{0:0.##}", yuv.ColorDifferenceU);
			vTextBlock.Text = String.Format("{0:0.##}", yuv.ColorDifferenceV);
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

				int x = Convert.ToInt32(ratioX * Image.Width);
				int y = Convert.ToInt32(ratioY * Image.Height);

				if (x >= Image.Width || y >= Image.Height)
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

				int x = Convert.ToInt32(ratioX * Image.Width);
				int y = Convert.ToInt32(ratioY * Image.Height);

				if (x >= Image.Width || y >= Image.Height)
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
			ImageModes imageMode = (ImageModes)imageModeComboBox.SelectedIndex;

			if (LowPerformanceMode)
			{

				Task.Run(() => ProcessImageModeChange(imageMode));
			}
			else
			{
				Image.SetImageMode(imageMode);
				resultImage.Source = Image.WriteableBmp;
				EnableUI();
			}
		}

		private void ProcessImageModeChange(ImageModes imageMode)
		{
			Image.SetImageMode(imageMode);
			Dispatcher.Invoke(() => {
				resultImage.Source = Image.WriteableBmp;
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
			HistogramMode = (HistogramModes)histogramModeComboBox.SelectedIndex;

			switch (HistogramMode)
			{
				case HistogramModes.Intensity:
					histogramChart.Stroke = new SolidColorBrush(Colors.Black);
					histogramChart.Color = histogramChart.Stroke;
					histogramChart.Description = "Intensity";
					histogramChart.Plot(Histogram.enumerationX, Image.ImageHistogram.Intensity);
					horizontalThresholdSlider.Maximum = Image.ImageHistogram.IntensityMaxValue;
					break;
				case HistogramModes.Red:
					histogramChart.Stroke = new SolidColorBrush(Colors.Red);
					histogramChart.Color = histogramChart.Stroke;
					histogramChart.Description = "Red";
					histogramChart.Plot(Histogram.enumerationX, Image.ImageHistogram.Red);
					horizontalThresholdSlider.Maximum = Image.ImageHistogram.RedMaxValue;
					break;
				case HistogramModes.Green:
					histogramChart.Stroke = new SolidColorBrush(Colors.Green);
					histogramChart.Color = histogramChart.Stroke;
					histogramChart.Description = "Green";
					histogramChart.Plot(Histogram.enumerationX, Image.ImageHistogram.Green);
					horizontalThresholdSlider.Maximum = Image.ImageHistogram.GreenMaxValue;
					break;
				case HistogramModes.Blue:
					histogramChart.Stroke = new SolidColorBrush(Colors.Blue);
					histogramChart.Color = histogramChart.Stroke;
					histogramChart.Description = "Blue";
					histogramChart.Plot(Histogram.enumerationX, Image.ImageHistogram.Blue);
					horizontalThresholdSlider.Maximum = Image.ImageHistogram.BlueMaxValue;
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
			Image.StretchHistogram(HistogramMode);

			Dispatcher.Invoke(() => {
				Image.SetImageMode(Image.ImageMode);
				resultImage.Source = Image.WriteableBmp;
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
			Filters filter = (Filters)filtersComboBox.SelectedIndex;

			Task.Run(() => ApplyFilter(filter));
		}

		private void ApplyFilter(Filters filter)
		{
			switch (filter)
			{
				case Filters.LowPassFilter1:
					Image.LowPassFilter1();
					break;
				case Filters.LowPassFilter2:
					Image.LowPassFilter2();
					break;
				case Filters.LowPassFilter3:
					Image.LowPassFilter3();
					break;
				case Filters.HighPassFilter1:
					Image.HighPassFilter1();
					break;
				case Filters.HighPassFilter2:
					Image.HighPassFilter2();
					break;
				case Filters.HighPassFilter3:
					Image.HighPassFilter3();
					break;
				case Filters.MedianFilter3x3:
					Image.MedianFilter(3);
					break;
				case Filters.MedianFilter5x5:
					Image.MedianFilter(5);
					break;
				case Filters.MedianFilter7x7:
					Image.MedianFilter(7);
					break;
			}

			Dispatcher.Invoke(() => {
				Image.SetImageMode(Image.ImageMode);
				resultImage.Source = Image.WriteableBmp;
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
			Operators operator_ = (Operators)operatorsComboBox.SelectedIndex;

			Task.Run(() => ApplyOperator(operator_));
		}

		private void ApplyOperator(Operators operator_)
		{
			switch (operator_)
			{
				case Operators.SobelOperator:
					Image.SobelOperator();
					break;
				case Operators.RobertsCrossOperator:
					Image.RobertsCrossOperator();
					break;
				case Operators.PrewittOperator:
					Image.PrewittOperator();
					break;
			}

			Dispatcher.Invoke(() => {
				Image.SetImageMode(Image.ImageMode);
				resultImage.Source = Image.WriteableBmp;
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
			Image.AdaptiveVerticalThreshold(HistogramMode);

			Dispatcher.Invoke(() => {
				Image.SetImageMode(Image.ImageMode);
				resultImage.Source = Image.WriteableBmp;
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
			int horizontalThreshold = Convert.ToInt32(horizontalThresholdSlider.Value);

			Task.Run(() => HorizontalThreshold(horizontalThreshold));
		}

		private void HorizontalThreshold(int horizontalThreshold)
		{
			Image.HorizontalThreshold(HistogramMode, horizontalThreshold);

			Dispatcher.Invoke(() => {
				Image.SetImageMode(Image.ImageMode);
				resultImage.Source = Image.WriteableBmp;
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
			byte firstVerticalThreshold = Convert.ToByte(firstVerticalThresholdSlider.Value);
			byte secondVerticalThreshold = Convert.ToByte(secondVerticalThresholdSlider.Value);

			Task.Run(() => VerticalThreshold(firstVerticalThreshold, secondVerticalThreshold));
		}

		private void VerticalThreshold(byte firstVerticalThreshold, byte secondVerticalThreshold)
		{
			Image.VerticalThreshold(HistogramMode, firstVerticalThreshold, secondVerticalThreshold);

			Dispatcher.Invoke(() => {
				Image.SetImageMode(Image.ImageMode);
				resultImage.Source = Image.WriteableBmp;
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
	}
}
