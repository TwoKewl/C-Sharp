
using System.Drawing;

namespace MandelbrotEvolution;

struct Pixel(double X, double Y)
{
	public double x = X;
	public double y = Y;
	public Color Colour;
	
	public void SetColour(Color colour)
	{
		Colour = colour;
	}
}