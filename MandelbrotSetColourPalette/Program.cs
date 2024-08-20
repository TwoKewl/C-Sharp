
using System.Diagnostics;

namespace MandelbrotSetColourPalette;

class Program
{
	public static void Main(string[] args)
	{
		Renderer r = new(1920, 1080, 10000);
		r.RenderVideo();
	}
}