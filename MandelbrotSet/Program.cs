
using System.Numerics;

namespace MandelbrotSet;

class Program
{
	public static void Main(string[] args)
	{
		Renderer renderer = new(1920, 1080, 0.5);
		Complex ZoomCenter = new(0.294463399, 0.01632929);
		
		for (int i = 0; i < 5000; i++)
		{
			renderer.RenderFrame();
			renderer.Zoom(1.01, ZoomCenter);
		}
	}
}