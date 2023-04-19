using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
	/*
	Luma: [0; 1]
	Color Difference U: [-0.436; 0.436]
	Color Differece V: [-0.615; 0.615]
	*/

	public class YuvColor
	{
		public double Luma { get; set; }
		public double ColorDifferenceU { get; set; }
		public double ColorDifferenceV { get; set; }

		public YuvColor()
		{
			Luma = 0d;
			ColorDifferenceU = 0d;
			ColorDifferenceV = 0d;
		}

		public YuvColor(double y, double u, double v)
		{
			Luma = y;
			ColorDifferenceU = u;
			ColorDifferenceV = v;
		}

		public override string ToString()
		{
			return string.Format("{0:0.##} {1:0.##} {2:0.##}", Luma , ColorDifferenceU, ColorDifferenceV);
		}
	}
}
