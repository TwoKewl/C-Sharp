using System.Drawing;

namespace ImageInverser
{
	class Program
	{
		public static void Main(string[] args)
		{
			Bitmap image = new(Path.Combine(Environment.CurrentDirectory, "ImageInverser", "Sierpinski Triangle.jpg"));
			InversePixels(image);
		}
		
		public static void InversePixels(Bitmap image)
		{
			Bitmap newImage = new(image.Width, image.Height);
			int Width = image.Width, Height = image.Height;
			
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					Color Colour = image.GetPixel(x, y);
					Color NewColour = Color.FromArgb(255 - Colour.R, 255 - Colour.G, 255 - Colour.B);
					
					newImage.SetPixel(x, y, NewColour);
				}
			}
			
			WriteImage("ImageInverser\\Result.png", newImage);
		}
		
		public static void WriteImage(string Filename, Bitmap image)
		{
			image.Save(Filename);
		}
	}
}