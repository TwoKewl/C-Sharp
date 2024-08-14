namespace JuliaSet;

internal class Renderer
{
	readonly int MaxIterations = 1000;
	readonly double ZoomFactor = 1.01;
	readonly double InitalZoom = 0.75;

	double xMin = -1.75, xMax = 1.75, yMin = -1, yMax = 1;

	private readonly int _width;
	private readonly int _height;
	private readonly int _maxIterations;
	
	public MyBitmap bitmap = new(1920, 1080);

	public Renderer(int width, int height, int maxIterations)
	{
		_width = width;
		_height = height;
		_maxIterations = maxIterations;

		xMin /= InitalZoom;
		xMax /= InitalZoom;
		yMin /= InitalZoom;
		yMax /= InitalZoom;
	}

	public void RenderFrame(int index)
	{
		bitmap.Clear();

		Parallel.For(0, _height, y =>
		{
			for (int x = 0; x < _width; x++)
			{
				int colour = GetColourOfPixel(x, y) * 255 / _maxIterations;
				bitmap.SetPixel(x, y, (byte)(colour * 0.25), (byte)(colour * 0.25), (byte)colour);
			}
		});

		bitmap.Save($"JuliaSet\\Images\\{index:00000}.bmp");
	}

	public void Zoom(Complex ZoomCenter, double ZoomFactor)
	{
		double new_width = (xMax - xMin) / ZoomFactor;
		double new_height = (yMax - yMin) / ZoomFactor;

		xMin = ZoomCenter.x - new_width / 2;
		xMax = ZoomCenter.x + new_width / 2;
		yMin = ZoomCenter.y - new_height / 2;
		yMax = ZoomCenter.y + new_height / 2;
	}

	private int GetColourOfPixel(int x, int y)
	{
		double ComplexX = (xMin / ZoomFactor) + (double)x / _width * ((xMax / ZoomFactor) - (xMin / ZoomFactor));
		double ComplexY = (yMin / ZoomFactor) + (double)y / _height * ((yMax / ZoomFactor) - (yMin / ZoomFactor));
		Complex z = new(ComplexX, ComplexY);
		Complex c = new(-0.78, 0.136);

		for (int i = 0; i < MaxIterations; i++)
		{
			z = z.Square().Add(c);

			if (z.OutOfBounds(4))
			{
				return i;
			}
		}

		return MaxIterations;
	}
}