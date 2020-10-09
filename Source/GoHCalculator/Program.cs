using System;

namespace GoHCalculator
{
	public static class Program
	{
		public static void Main()
		{
			Console.CursorVisible = false;
			Console.WriteLine(@"Game of Homes......");
			new Simulation().Run();
		}
	}
}
