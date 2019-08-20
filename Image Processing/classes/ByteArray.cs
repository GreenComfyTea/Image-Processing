using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AL140_07
{
    public class ByteArray
    {
		public byte[] Array { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int Channels { get; set; }
		public int Stride { get; set; }

		public ByteArray(ByteArray byteArray)
		{
			Array = (byte[]) byteArray.Array.Clone();
			Width = byteArray.Width;
			Height = byteArray.Height;
			Channels = byteArray.Channels;
			Stride = byteArray.Stride;
		}

		public ByteArray(int width, int height, int channels)
		{
			Width = width;
			Height = height;
			Channels = channels;
			Stride = CalculateStride(width, channels);

			Array = new byte[Stride * Height];
		}

		public int GetIndex(int x, int y)
		{
			return y * Stride + x * Channels;
		}

		public Color GetPixelColor(int x, int y)
		{
			if (Channels == 0)
			{
				return Colors.Black;
			}

			int index = GetIndex(x, y);

			if (Channels == 1)
			{
				byte intensity = Array[index];
				return Color.FromRgb(intensity, intensity, intensity);
			}

			if (Channels == 3)
			{
				byte blue = Array[index++];
				byte green = Array[index++];
				byte red = Array[index];
				return Color.FromRgb(red, green, blue);
			}

			if (Channels == 4)
			{
				byte blue = Array[index++];
				byte green = Array[index++];
				byte red = Array[index++];
				byte alpha = Array[index];
				return Color.FromArgb(alpha, red, green, blue);
			}

			return Colors.Black;
		}

		public static int CalculateStride(int width, int channels)
		{
			return width * channels;
		}
	}
}
