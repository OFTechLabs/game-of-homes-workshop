using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace GoHCalculator
{
	public static class Program
	{
		public static void Main()
		{
			Console.CursorVisible = false;
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
			File.WriteAllText(@".\wwwroot\results.json", jsonString);

			Console.WriteLine("Starting the host");

			var info = new ProcessStartInfo
			{
				WorkingDirectory = @"..\..\Source\GoHDashboard\Blazor-Dashboard", 
				Arguments = "run",
				FileName = "dotnet"
			};
			Process.Start(info);

			OpenBrowser("http://localhost:5000");
		}

		private static void OpenBrowser(string url)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); // Works ok on windows
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				Process.Start("xdg-open", url);  // Works ok on linux
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				Process.Start("open", url); // Not tested
			}
			else
			{
				Console.WriteLine("Please open a browser manually");
			}
		}
	}
}
