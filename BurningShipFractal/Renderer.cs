using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;

namespace BurningShipFractal
{
	internal class Renderer
	{
		public static double xMin = -7, xMax = 7, yMin = -4, yMax = 4;
		public static int Iterations = 1000;
		public static double EscapeRadius = 2;
		public static int Frame = 0;
		public static Bitmap bitmap;
		public static BitmapData data;
		public static IntPtr ptr;

		public static void RenderFrame(int Width, int Height)
		{
			Color[] colours = new Color[Width * Height];

			Parallel.For(0, Height, y =>
			{
				for (int x = 0; x < Width; x++)
				{
					double t = GetColour(x, y, Width, Height);
					double logT = t == Iterations ? 0 : Math.Log(t + 1) / Math.Log(Iterations + 1); // Logarithmic mapping
					Color Colour = Color.FromArgb((int)(logT * 100), (int)(logT * 255 * 0), (int)(logT * 255));
					colours[y * Width + x] = Colour;
				}
			});

			if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "BurningShipFractal", "Images"))) Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "BurningShipFractal", "Images"));

			SaveImage($"BurningShipFractal\\Images\\{Frame.ToString().PadLeft(4, '0')}.png", colours, Width, Height);
			Frame++;
		}
		
		public static void Zoom(Complex ZoomCenter, double ZoomFactor)
		{
			double new_width = (xMax - xMin) / ZoomFactor;
			double new_height = (yMax - yMin) / ZoomFactor;

			xMin = ZoomCenter.Real - new_width / 2;
			xMax = ZoomCenter.Real + new_width / 2;
			yMin = ZoomCenter.Imaginary - new_height / 2;
			yMax = ZoomCenter.Imaginary + new_height / 2;
		}

		public static void SaveImage(string Filename, Color[] Colours, int Width, int Height)
		{			
			bitmap = new Bitmap(Width, Height);
			data = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
			ptr = data.Scan0;

			int bytes = Math.Abs(data.Stride) * bitmap.Height;
			byte[] rgbValues = new byte[bytes];

			for (int i = 0; i < rgbValues.Length; i += 4)
			{
				int ColourIndex = i / 4;
				Color Colour = Colours[ColourIndex];

				rgbValues[i] = Colour.B;
				rgbValues[i + 1] = Colour.G;
				rgbValues[i + 2] = Colour.R;
				rgbValues[i + 3] = Colour.A;
			}

			Marshal.Copy(rgbValues, 0, ptr, bytes);
			bitmap.UnlockBits(data);

			bitmap.Save(Filename);
		}

		public static int GetColour(int x, int y, int Width, int Height)
		{
			double ComplexX = xMin + (double)x / Width * (xMax - xMin);
			double ComplexY = yMin + (double)y / Height * (yMax - yMin);

			Complex z = new(0, 0);
			Complex c = new(ComplexX, ComplexY);

			for (int n = 0; n < Iterations; n++)
			{
				z = new Complex(Math.Abs(z.Real), Math.Abs(z.Imaginary)) * new Complex(Math.Abs(z.Real), Math.Abs(z.Imaginary)) + c;
				if (z.Real * z.Real + z.Imaginary * z.Imaginary > EscapeRadius * EscapeRadius) return n;
			}

			return Iterations;
		}
	}
}