using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace GoHCalculator
{
	public static class Program
	{
		public static void Main()
		{
			//Console.CursorVisible = false;
			Console.WriteLine(@"Game of Homes......");
			var rawData = new Simulation().Run();
				
			var output = rawData.ToDictionary(p => p.Key.ToString(), p =>
			{
				var values = new List<double>();
				for (var r = 0; r < p.Value.GetLength(0); r++)
				{
					for (var c = 0; c < p.Value.GetLength(1); c++)
					{
						values.Add(p.Value[r,c]);
					}
				}
				return values;
			});

			Console.WriteLine(@"Creating output json file......");
			var jsonString = JsonSerializer.Serialize(output);
			File.WriteAllText(@"..\..\bin\debug\wwwroot\results.json", jsonString);
		}
	}
}
