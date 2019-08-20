using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AL140_07
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
			Luma = 0;
			ColorDifferenceU = 0;
			ColorDifferenceV = 0;
		}

		public YuvColor(double y, double u, double v)
		{
			Luma = y;
			ColorDifferenceU = u;
			ColorDifferenceV = v;
		}
	}
}
