
using System.Text;

namespace GameOfLife
{
	class Program
	{
		struct Cell(int x, int y, bool alive)
		{
			public int X = x;
			public int Y = y;
			public bool Alive = alive;
		}
		
		List<Cell> cells = [];
		public int Width = 50;
		public int Height = 30;
		
		static void Main(string[] args)
		{
			Program game = new();
			game.Init();
			
			while (true)
			{
				game.Tick();
				game.Render();
				
				Thread.Sleep(50);
			}
		}
		
		void Init()
		{
			Random r = new();
			
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					cells.Add(new Cell(x, y, r.Next(2) == 1));
				}
			}
		}
		
		void Render()
		{
			Console.Clear();
			StringBuilder sb = new();

			for (int y = 0; y < Height; y++)
			{
				string line = "";
				
				for (int x = 0; x < Width; x++)
				{
					Cell cell = cells.Find(c => c.X == x && c.Y == y);
					line += cell.Alive ? "██" : "  ";
				}
				
				sb.AppendLine(line);
			}
			
			Console.WriteLine(sb.ToString());
		}
		
		void Tick()
		{
			List<Cell> newCells = [];
			
			foreach (Cell cell in cells)
			{
				int aliveNeighbours = GetAliveNeighbours(cell);

				if (cell.Alive)
				{
					bool willBeAlive = aliveNeighbours == 2 || aliveNeighbours == 3;
					newCells.Add(new Cell(cell.X, cell.Y, willBeAlive));
				}
				else 
				{
					bool willBeAlive = aliveNeighbours == 3;
					newCells.Add(new Cell(cell.X, cell.Y, willBeAlive));
				}
			}
			
			cells = newCells;
		}
		
		int GetAliveNeighbours(Cell cell)
		{
			int aliveNeighbours = 0;
			
			for (int y = -1; y <= 1; y++)
			{
				for (int x = -1; x <= 1; x++)
				{					
					if (x == 0 && y == 0)
					{
						continue;
					}
					
					Cell neighbour = cells.Find(c => c.X == (cell.X + x) % (Width) && c.Y == (cell.Y + y) % (Height));
					if (neighbour.Alive)
					{
						aliveNeighbours++;
					}
				}
			}
			
			return aliveNeighbours;
		}
	}
}