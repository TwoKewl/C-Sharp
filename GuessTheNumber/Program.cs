
namespace GuessTheNumber
{
	class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("A number between 1 and 100 has been chosen.");
			new Program().Guess();
		}
		
		readonly int NumberChosen = new Random().Next(1, 101);
		int GuessCount = 0;
		
		public void Guess()
		{
			Console.Write("Enter your guess: ");
			string numberGuessed = Console.ReadLine();
			
			try
			{
				int num = Int32.Parse(numberGuessed);
				if (num < NumberChosen)
				{
					Console.WriteLine("The number is higher than your guess");
					GuessCount++;
					Guess();
				}
				else if (num > NumberChosen)
				{
					Console.WriteLine("The number is lower than your guess");
					GuessCount++;
					Guess();
				}
				else
				{
					Console.WriteLine($"You guessed the number in {GuessCount} guesses!");
					return;
				}
			}
			catch
			{
				Console.WriteLine("Invalid Guess!");
				Guess();
			}
		}
	}
}