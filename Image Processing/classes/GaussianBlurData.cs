using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
	class GaussianBlurData
	{
		public float Value0 { get; set; } = 0f;
		public float Value1 { get; set; } = 0f;
		public float Value2 { get; set; } = 0f;
		public float Value3 { get; set; } = 0f;
		public float Value4 { get; set; } = 0f;
		public float Value5 { get; set; } = 0f;
		public float Value6 { get; set; } = 0f;
		public float Value7 { get; set; } = 0f;
		public float Value8 { get; set; } = 0f;
		public float Value9 { get; set; } = 0f;
		public float Value10 { get; set; } = 0f;
		public float Value11 { get; set; } = 0f;
		public float Value12 { get; set; } = 0f;
		public float Value13 { get; set; } = 0f;
		public float Value14 { get; set; } = 0f;

		public void setValue(int index, float value)
		{
			switch (index)
			{
				case 0:
					Value0 = value;
					break;
				case 1:
					Value1 = value;
					break;
				case 2:
					Value2 = value;
					break;
				case 3:
					Value3 = value;
					break;
				case 4:
					Value4 = value;
					break;
				case 5:
					Value5 = value;
					break;
				case 6:
					Value6 = value;
					break;
				case 7:
					Value7 = value;
					break;
				case 8:
					Value8 = value;
					break;
				case 9:
					Value9 = value;
					break;
				case 10:
					Value10 = value;
					break;
				case 11:
					Value11 = value;
					break;
				case 12:
					Value12 = value;
					break;
				case 13:
					Value13 = value;
					break;
				case 14:
					Value14 = value;
					break;
			}
		}
	}
}
