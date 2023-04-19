using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ImageProcessing
{
	/*
	Hue: [0; 360)
	Saturation: [0; 1]
	Value: [0; 1]
	*/

	public class HsvColor
	{
		public double Hue { get; set; }
		public double Saturation { get; set; }
		public double Value { get; set; }

		public HsvColor()
		{
			Hue = 0d;
			Saturation = 0d;
			Value = 0d;
		}

		public HsvColor(double hue, double saturation, double value)
		{
			Hue = hue;
			Saturation = saturation;
			Value = value;
		}

		public override string ToString()
		{
			return string.Format("{0:0.##} {1:0.##} {2:0.##}", Hue, Saturation, Value);
		}
	}
}
