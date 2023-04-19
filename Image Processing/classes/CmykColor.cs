using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ImageProcessing
{
	/*
	Cyan: [0; 1]
	Magenta: [0; 1]
	Yellow: [0; 1]
	Black: [0; 1]
	*/

	public class CmykColor
	{
		public double Cyan { get; set; }
		public double Magenta { get; set; }
		public double Yellow { get; set; }
		public double Black { get; set; }

		public CmykColor()
		{
			Cyan = 0d;
			Magenta = 0d;
			Yellow = 0d;
			Black = 0d;
		}

		public CmykColor(double cyan, double magenta, double yellow, double black)
		{
			Cyan = cyan;
			Magenta = magenta;
			Yellow = yellow;
			Black = black;
		}

		public override string ToString()
		{
			return string.Format("{0:0.##} {1:0.##} {2:0.##} {3:0.##}", Cyan, Magenta, Yellow, Black);
		}
	}
}
