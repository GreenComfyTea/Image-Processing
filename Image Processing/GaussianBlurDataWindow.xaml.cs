using System;
using System.Collections.Generic;
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
	/// Interaction logic for GaussianBlurData.xaml
	/// </summary>
	public partial class GaussianBlurDataWindow : Window
	{
		public GaussianBlurDataWindow()
		{
			InitializeComponent();
		}

		public void UpdateData(int size, double sigma)
		{
			if (size < 3) return;
			if (size % 2 == 0) return;

			double one = 2d * Math.Pow(sigma, 2d);
			double two = one * Math.PI;

			int shift = (size - 1) / 2;

			float[,] filterMatrix = new float[size, size];

			GaussianBlurData[] data = new GaussianBlurData[size];

			for (int i = 0; i < size; i++)
			{
				data[i] = new GaussianBlurData();

				for (int j = 0; j < size; j++)
				{
					double three = Math.Pow(i - shift, 2d) + Math.Pow(j - shift, 2d);
					filterMatrix[i, j] = Calculations.ClampToFloat(Math.Exp(-three / one) / two);

					data[i].setValue(j, filterMatrix[i, j]);
				}
				GaussianBlurListView.Items.Add(data[i]);
			}
		}
	}
}
