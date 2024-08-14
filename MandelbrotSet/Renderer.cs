using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MandelbrotSet;

internal class Renderer(int Width, int Height, double OriginalZoom)
{
	private readonly Color[] _colours = new Color[Width * Height];
	private readonly int _width = Width;
	private readonly int _height = Height;
	private readonly int _maxIterations = 1000;
	private double xMin = -2 / OriginalZoom;
	private double xMax = 1.5 / OriginalZoom;
	private double yMin = -1 / OriginalZoom;
	private double yMax = 1 / OriginalZoom;
	private int Frame = 0;
	
	public void RenderFrame()
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		
		Parallel.For(0, _height, y => 
		{
			for (int x = 0; x < _width; x++)
			{
				double ComplexX = xMin + (double)x / _width * (xMax - xMin);
				double ComplexY = yMin + (double)y / _height * (yMax - yMin);
				
				Complex c = new(ComplexX, ComplexY);
				
				Color colour = CalculateColour(CalculateIterations(c));
				_colours[y * _width + x] = colour;
			}
		});
		
		stopwatch.Stop();
		Console.WriteLine($"Rendering finished in {stopwatch.ElapsedMilliseconds}ms");
		
		Save($"MandelbrotSet/Images/{Frame.ToString().PadLeft(5, '0')}.png");
		Frame++;
	}
	
	public void Zoom(double ZoomFactor, Complex ZoomCenter)
	{
		double xRange = xMax - xMin;
		double yRange = yMax - yMin;
		
		xMin = ZoomCenter.Real - xRange / 2 / ZoomFactor;
		xMax = ZoomCenter.Real + xRange / 2 / ZoomFactor;
		yMin = ZoomCenter.Imaginary - yRange / 2 / ZoomFactor;
		yMax = ZoomCenter.Imaginary + yRange / 2 / ZoomFactor;
	}
	
	public int CalculateIterations(Complex c)
	{
		Complex z = new(0, 0);

		for (int n = 1; n <= _maxIterations; n++)
		{
			z = z * z + c;
			
			if (z.Magnitude > 2)
			{
				return n;
			}
		}
		
		return _maxIterations;
	}
	
	public Color CalculateColour(int iterations) 
	{
		if (iterations == _maxIterations)
		{
			return Color.Black;
		}
		
		double SinOfIterations = Math.Sin(iterations) / 2;
		
		int r = (int)(127.5 * (SinOfIterations / 2 + 0.5));
		int g = (int)(0 * (SinOfIterations / 2 + 0.5));
		int b = (int)(255 * (SinOfIterations / 2 + 0.5));
		
		return Color.FromArgb(r, g, b);
	}
	
	public void Save(string Filename)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		
		Bitmap bitmap = new(_width, _height);
		BitmapData data = bitmap.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
		IntPtr ptr = data.Scan0;

		int bytes = Math.Abs(data.Stride) * bitmap.Height;
		byte[] rgbValues = new byte[bytes];

		for (int i = 0; i < rgbValues.Length; i += 4)
		{			
			int ColourIndex = i / 4;
			Color Colour = _colours[ColourIndex];

			rgbValues[i] = Colour.B;
			rgbValues[i + 1] = Colour.G;
			rgbValues[i + 2] = Colour.R;
			rgbValues[i + 3] = Colour.A;
		}

		Marshal.Copy(rgbValues, 0, ptr, bytes);
		bitmap.UnlockBits(data);

		bitmap.Save(Filename);
		
		stopwatch.Stop();
		Console.WriteLine($"Saving finished in {stopwatch.ElapsedMilliseconds}ms");
	}
}