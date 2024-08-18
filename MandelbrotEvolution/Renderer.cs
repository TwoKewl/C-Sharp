using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MandelbrotEvolution;

internal class Renderer
{
	public int Width;
	public int Height;
	public Bitmap Image;
	public BitmapData Data;
	public IntPtr ptr;
	public Pixel[] Pixels;
	public int iteration = 0;
	
	public float xMin = -1.75f * 2f;
	public float xMax = 1.75f * 2f;
	public float yMin = -1.0f * 2f;
	public float yMax = 1.0f * 2f;
	
	public Renderer(int width, int height)
	{
		Width = width;
		Height = height;
		Image = new(width, height);
		Pixels = new Pixel[width * height];
		
		for (int i = 0; i < Pixels.Length; i++)
		{
			int x = i % width;
			int y = i / width;
			
			double xCoord = xMin + (xMax - xMin) * x / width;
			double yCoord = yMin + (yMax - yMin) * y / height;
			
			Pixels[i] = new Pixel(xCoord, yCoord);
			Pixels[i].SetColour(Color.FromArgb(0, 0, 0));
		}
	}
	
	public void GetNextGeneration()
	{
		Parallel.For(0, Pixels.Length, i =>
		{
			Pixel pixel = Pixels[i];
			Complex c = new(pixel.x, pixel.y);
			Complex z = new(0, 0);
			
			for (int j = 0; j < iteration; j++)
			{
				z = z * z + c;
				if (z.Magnitude > 2)
				{
					Pixels[i].SetColour(Color.FromArgb(0, 255, 0)); // Green Screen
					break;
				}
			}
		});

		iteration++;
		
		Data = Image.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
		ptr = Data.Scan0;
		
		int bytes = Math.Abs(Data.Stride) * Image.Height;
		byte[] rgbValues = new byte[bytes];
		
		for (int i = 0; i < rgbValues.Length; i += 4)
		{
			int ColourIndex = i / 4;
			Color Colour = Pixels[ColourIndex].Colour;
			
			rgbValues[i] = Colour.B;
			rgbValues[i + 1] = Colour.G;
			rgbValues[i + 2] = Colour.R;
			rgbValues[i + 3] = Colour.A;
		}
		
		Marshal.Copy(rgbValues, 0, ptr, bytes);
		Image.UnlockBits(Data);
	}
	
	public void SaveAsImage(string path)
	{
		Image.Save(path, ImageFormat.Png);
	}
}