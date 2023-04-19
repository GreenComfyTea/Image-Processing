using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
	class HistogramLinearizationData
	{
		public int Xi { get; set; }
		public double Hx { get; set; }
		public double YHx { get; set; }
		public double YHz { get; set; }
		public int Zj { get; set; }

		public HistogramLinearizationData(int Xi, double Hx, double YHx, double YHz, int Zj)
		{
			this.Xi = Xi;
			this.Hx = Hx;
			this.YHx = YHx;
			this.YHz = YHz;
			this.Zj = Zj;
		}
	}
}
