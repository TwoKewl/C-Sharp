namespace JuliaSet;

class Program
{
	public static void Main()
	{
		Complex zoomCenter = new(-0.0028050215118, 0.003064073756579);
		double ZoomFactor = 1.0025;

		Renderer renderer = new(1920, 1080, 1000);

		for (int i = 0; i < 5000; i++)
		{
			renderer.RenderFrame(i);
			renderer.Zoom(zoomCenter, ZoomFactor);
			Console.WriteLine($"Frame {i + 1} rendered");
		}
	}
}