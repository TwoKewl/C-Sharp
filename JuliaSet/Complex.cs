namespace JuliaSet;

public struct Complex(double X, double Y)
{
	public double x = X, y = Y;
	
	public Complex Add(Complex other)
	{
		return new(x + other.x, y + other.y);
	}
	
	public Complex Square()
	{
		return new(x * x - y * y, x * y + y * x);
	}
	
	public bool OutOfBounds(int bounds)
	{
		if (x * x + y * y > bounds)
		{
			return true;
		}
		
		return false;
	}
}