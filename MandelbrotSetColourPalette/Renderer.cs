#pragma warning disable CA1416

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MandelbrotSetColourPalette;

internal class Renderer
{
	private readonly int _width;
	private readonly int _height;
	private readonly int _maxIterations;
	private readonly Color[] _palette = new Color[16];
	private readonly Color[] Colours;
	private readonly int _colourOffset = 2;
	
	private readonly Bitmap bmp;
	private IntPtr ptr;
	private BitmapData data;
	
	private readonly double zoomFactor = 1.01;
	private readonly double zoomCenterX = -1.7448040357363987725692945992446235350195;
	private readonly double zoomCenterY = 0.0220634162483497688327955079626004071454;
	private readonly Stopwatch sw = new();
	private double xMin = -4.0;
	private double xMax = 3.0;
	private double yMin = -2.0;
	private double yMax = 2.0;
	private int frame = 0;

	public Renderer(int height, int width, int maxIterations) 
	{
		_width = height;
		_height = width;
		_maxIterations = maxIterations;
		
		bmp = new(_width, _height);
		Colours = new Color[_width * _height];
		
		_palette[0] = Color.FromArgb(66, 30, 15);
		_palette[1] = Color.FromArgb(25, 7, 26);
		_palette[2] = Color.FromArgb(9, 1, 47);
		_palette[3] = Color.FromArgb(4, 4, 73);
		_palette[4] = Color.FromArgb(0, 7, 100);
		_palette[5] = Color.FromArgb(12, 44, 138);
		_palette[6] = Color.FromArgb(24, 82, 177);
		_palette[7] = Color.FromArgb(57, 125, 209);
		_palette[8] = Color.FromArgb(134, 181, 229);
		_palette[9] = Color.FromArgb(211, 236, 248);
		_palette[10] = Color.FromArgb(241, 233, 191);
		_palette[11] = Color.FromArgb(248, 201, 95);
		_palette[12] = Color.FromArgb(255, 170, 0);
		_palette[13] = Color.FromArgb(204, 128, 0);
		_palette[14] = Color.FromArgb(153, 87, 0);
		_palette[15] = Color.FromArgb(106, 52, 3);
	}
	
	public void RenderVideo()
	{
		while (true)
		{
			Zoom();
			RenderFrame();
			Console.WriteLine("Frame " + frame + " rendered.");
		}
	}

	public void RenderFrame()
	{
		sw.Start();
		Parallel.For(0, _height, y => 
		{
			for (int x = 0; x < _width; x++)
			{
				double cx = xMin + (xMax - xMin) * x / _width;
				double cy = yMin + (yMax - yMin) * y / _height;
				double[] value = Mandelbrot(cx, cy);
				Colours[y * _width + x] = SmoothColour(value[1], value[2], (int)value[0]);
			}
		});
		sw.Stop();
		Console.WriteLine("Frame " + frame + " calculated in " + sw.ElapsedMilliseconds + "ms.");
		sw.Reset();
		
		data = bmp.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
		ptr = data.Scan0;
		
		int bytes = Math.Abs(data.Stride) * bmp.Height;
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
		bmp.UnlockBits(data);
		
		if (!Directory.Exists("MandelbrotSetColourPalette/Images")) Directory.CreateDirectory("MandelbrotSetColourPalette/Images");
		
		bmp.Save("MandelbrotSetColourPalette/Images/" + frame.ToString().PadLeft(5, '0') + ".png", ImageFormat.Png);
		frame += 1;
	}
	
	public double[] Mandelbrot(double cx, double cy)
	{
		double zx = 0;
		double zy = 0;
		
		for (int i = 0; i < _maxIterations; i++)
		{
			double temp = zx * zx - zy * zy + cx;
			zy = 2 * zx * zy + cy;
			zx = temp;
			
			if (zx * zx + zy * zy > 256) return [i, zx, zy];
		}
		
		return [_maxIterations, zx, zy];
	}
	
	public Color BasicColour(int val)
	{
		if (val == _maxIterations) return Color.Black;
		if (val == 0) return _palette[2];
		
		int index = (val + 2) % _palette.Length;
		return _palette[index];
	}
	
	public Color SmoothColour(double x, double y, int val)
	{
		if (val == _maxIterations) return Color.Black;
		
		double log_zn = Math.Log(x * x + y * y) / 2;
		double nu = Math.Log(log_zn / Math.Log(2)) / Math.Log(2);
		double iter = val + 1 - nu;
		
		int index1 = (int)(iter + _colourOffset) % _palette.Length;
		int index2 = (index1 + 1) % _palette.Length;
		double percent = iter % 1;
		
		return LinearInterpolate(_palette[index1], _palette[index2], percent);
	}
	
	private static Color LinearInterpolate(Color c1, Color c2, double percent)
	{
		int r = (int)(c1.R + percent * (c2.R - c1.R));
		int g = (int)(c1.G + percent * (c2.G - c1.G));
		int b = (int)(c1.B + percent * (c2.B - c1.B));
		
		return Color.FromArgb(r, g, b);
	}
	
	public void Zoom()
	{
		double new_width = (xMax - xMin) / zoomFactor;
		double new_height = (yMax - yMin) / zoomFactor;

		xMin = zoomCenterX - new_width / 2;
		xMax = zoomCenterX + new_width / 2;
		yMin = zoomCenterY - new_height / 2;
		yMax = zoomCenterY + new_height / 2;
	}
}