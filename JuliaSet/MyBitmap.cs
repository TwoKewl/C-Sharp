using System.Text;

namespace JuliaSet;

public class MyBitmap
{
	private readonly int _width;
	private readonly int _height;
	private readonly byte[] _data;

	public MyBitmap(int width, int height)
	{
		_data = new byte[width * height * 3];
		_width = width;
		_height = height;
	}

	public void Clear()
	{
		Array.Clear(_data, 0, _data.Length);
	}

	public void SetPixel(int x, int y, byte r, byte g, byte b)
	{
		int index = (y * _width + x) * 3;
		_data[index] = b;
		_data[index + 1] = g;
		_data[index + 2] = r;
	}

	public void Save(string filename)
	{
		using var file = File.OpenWrite(filename);
		using var writer = new BinaryWriter(file);

		writer.Write(Encoding.ASCII.GetBytes("BM"));
		writer.Write(14 + 40 + _data.Length);
		writer.Write(0);
		writer.Write(14 + 40);

		writer.Write(40);
		writer.Write(_width);
		writer.Write(_height);
		writer.Write((ushort)1);
		writer.Write((ushort)24);
		writer.Write(0);
		writer.Write(_data.Length);
		writer.Write(0);
		writer.Write(0);
		writer.Write(0);
		writer.Write(0);

		writer.Write(_data);
	}
}
