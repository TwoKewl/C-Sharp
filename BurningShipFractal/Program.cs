using System.Diagnostics;
using System.Numerics;

namespace BurningShipFractal;

class Program
{
	public static Complex zoomCenter = new(-1.6280584219514331, -0.0081955774200810291);
	
	public static void Main(string[] args)
	{
		Stopwatch sw = new();
		for (int i = 0; i < 3000; i++)
		{
			sw.Start();
			Renderer.RenderFrame(1920, 1080);
			Renderer.Zoom(zoomCenter, 1.01);
			sw.Stop();
			
			Console.WriteLine($"Finished frame {Renderer.Frame} in {sw.ElapsedMilliseconds}ms.");
			sw.Reset();
		}
	}
}