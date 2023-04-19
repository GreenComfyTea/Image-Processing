using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
	/*
	Luma: [0; 100]
	Color Difference U: [-100; 100]
	Color Differece V: [-100; 100]
	*/

	public class LabColor
	{
		public byte Lightness { get; set; }
		public sbyte ChromaA { get; set; }
		public sbyte ChromaB { get; set; }

		public LabColor()
		{
			Lightness = 0;
			ChromaA = 0;
			ChromaB = 0;
		}

		public LabColor(byte lightness, sbyte chromaA, sbyte chromaB)
		{
			Lightness = lightness;
			ChromaA = chromaA;
			ChromaB = chromaB;
		}

		public override string ToString()
		{
			return string.Format("{0} {1} {2}", Lightness, ChromaA, ChromaB);
		}
	}
}
