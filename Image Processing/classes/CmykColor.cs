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
			Cyan = 0;
			Magenta = 0;
			Yellow = 0;
			Black = 0;
		}

		public CmykColor(double cyan, double magenta, double yellow, double black)
		{
			Cyan = cyan;
			Magenta = magenta;
			Yellow = yellow;
			Black = black;
		}
	}
}
