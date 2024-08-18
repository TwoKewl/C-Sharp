using System.Diagnostics;
using System.Drawing;

namespace MandelbrotEvolution;

class Program
{
	public static void Main(string[] args)
	{
		Stopwatch sw = new();
		Renderer r = new(1920, 1080);
		string path = "MandelbrotEvolution/Images/";
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		
		for (int i = 0; i < 1000; i++)
		{
			sw.Start();
			r.GetNextGeneration();
			r.SaveAsImage($"{path}{i.ToString().PadLeft(4, '0')}.png");
			sw.Stop();
			Console.WriteLine($"Image {i + 1} saved in {sw.ElapsedMilliseconds}ms");
			sw.Reset();
		}
	}
}