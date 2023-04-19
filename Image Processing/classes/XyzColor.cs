using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
	public class XyzColor
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }

		public XyzColor()
		{
			X = 0d;
			Y = 0d;
			Z = 0d;
		}

		public XyzColor(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public override string ToString()
		{
			return string.Format("{0:0.##} {1:0.##} {2:0.##}", X, Y, Z);
		}
	}
}
