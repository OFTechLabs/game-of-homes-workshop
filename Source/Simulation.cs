using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Office.Interop.Excel;

namespace GameOfHomes
{
	class Simulation
	{
		private const int NumberOfScenarios = 100;
		private const int Horizon = 30;

		private readonly int _baseYear = DateTime.Now.Year;

		private readonly double[,] _solvencyRatio;
		private readonly double[,] _houses;
		private readonly double[,] _rent;
		private readonly double[,] _sustainability;
		private readonly double[,] _maintenance;
		private readonly double[,] _debt;
		private readonly double[,] _interest;
		private readonly double[,] _scores;
		private readonly double[,] _numberOfHousesScores;
		private readonly double[,] _rentScores;
		private readonly double[,] _sustainabilityScores;
		private readonly double[,] _numberOfCheapHouses;
		private readonly double[,] _numberOfBadHouses;
		private readonly double[] _numberOfBankruptcies;

		public Simulation()
		{
			_solvencyRatio = new double[NumberOfScenarios, Horizon+2];
			_houses = new double[NumberOfScenarios, Horizon + 2];
			_rent = new double[NumberOfScenarios, Horizon + 2];
			_sustainability = new double[NumberOfScenarios, Horizon + 2];
			_maintenance = new double[NumberOfScenarios, Horizon + 2];
			_debt = new double[NumberOfScenarios, Horizon + 2];
			_interest = new double[NumberOfScenarios, Horizon + 2];
			_numberOfCheapHouses = new double[NumberOfScenarios, Horizon + 2];
			_numberOfBadHouses = new double[NumberOfScenarios, Horizon + 2];
			_scores = new double[NumberOfScenarios, Horizon + 2];
			_numberOfHousesScores = new double[NumberOfScenarios, Horizon + 2];
			_rentScores = new double[NumberOfScenarios, Horizon + 2];
			_sustainabilityScores = new double[NumberOfScenarios, Horizon + 2];

			_numberOfBankruptcies = new double[Horizon + 2];
		}

		public void Run()
		{
			Console.Write(@"Simulating scenario ");

			for (var scenario = 0; scenario < NumberOfScenarios; scenario++)
			{
				Console.Write($@"{scenario + 1}");
				Console.SetCursorPosition(20, 1);

				Economy.Read("..\\Scenarios\\Scenario" + scenario + ".csv");
				
				var association = new HousingAssociation(0);
				GatherData(association, scenario, 0);

				for (var t = 0; t <= Horizon; t++)
				{
					Economy.CurrentYear = t;
					association.EndOfYear();
					GatherData(association, scenario, t + 1);
				}
			}
			Report();
		}

		private void GatherData(HousingAssociation association, int scenario, int t)
		{
			_solvencyRatio[scenario, t] = association.SolvencyRatio;
			_houses[scenario, t] = association.RealEstatePortfolio.NumberOfHouses;
			_rent[scenario, t] = association.AverageRent;
			_sustainability[scenario, t] = association.AverageSustainabilityOfHouses;
			_maintenance[scenario, t] = association.AverageMaintenanceExpenses;
			_debt[scenario, t] = association.Debt;
			_interest[scenario, t] = association.Interest;
			_scores[scenario, t] = association.Score;
			_numberOfHousesScores[scenario, t] = association.ScoreNumberOfHouses;
			_rentScores[scenario, t] = association.ScoreRent;
			_sustainabilityScores[scenario, t] = association.ScoreSustainability;
			_numberOfCheapHouses[scenario, t] = association.RealEstatePortfolio.Houses.Count(w => w.MonthlyRent / association.CumulativeInflation < HousingAssociation.LowRent);
			_numberOfBadHouses[scenario, t] = association.RealEstatePortfolio.Houses.Count(w => w.Sustainability < HousingAssociation.Sufficient);
			_numberOfBankruptcies[t] += association.IsBankrupt ? 1 : 0;
		}

		private void Report()
		{
			Console.WriteLine();
			Console.WriteLine(@"Filling excel report...");

			var excelApp = new Application();
			var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
			var file = Path.Combine(path, "Workbook.xlsx");
			var workBook = excelApp.Workbooks.Open(file);

			try
			{
				var sheetSolvencyRatio = (_Worksheet)workBook.Worksheets["SolvencyRatio"];
				var sheetHouses = (_Worksheet)workBook.Worksheets["Number of houses"];
				var sheetRent = (_Worksheet)workBook.Worksheets["Average rent"];
				var sheetSustainability = (_Worksheet)workBook.Worksheets["Average sustainability"];
				var sheetMaintenance = (_Worksheet)workBook.Worksheets["Average maintenance"];
				var sheetDebt = (_Worksheet)workBook.Worksheets["Debt"];
				var sheetInterest = (_Worksheet)workBook.Worksheets["Interest"];
				var sheetNumberOfCheapHouses = (_Worksheet)workBook.Worksheets["Number of cheap houses"];
				var sheetNumberOfBadHouses = (_Worksheet)workBook.Worksheets["Number of bad houses"];
				var sheetScores = (_Worksheet)workBook.Worksheets["Scores"];
				var sheetStatistics = (_Worksheet)workBook.Worksheets["Statistics"];

				excelApp.Calculation = XlCalculation.xlCalculationManual;
				excelApp.ScreenUpdating = false;

				for (var year = 0; year <= Horizon + 1; year++)
				{
					FillRange(year, sheetSolvencyRatio, (scenario, t) => _solvencyRatio[scenario, t] * 100);
					FillRange(year, sheetHouses, (scenario, t) => _houses[scenario, t]);
					FillRange(year, sheetRent, (scenario, t) => _rent[scenario, t]);
					FillRange(year, sheetSustainability, (scenario, t) => _sustainability[scenario, t]);
					FillRange(year, sheetMaintenance, (scenario, t) => _maintenance[scenario, t]);
					FillRange(year, sheetDebt, (scenario, t) => _debt[scenario, t]);
					FillRange(year, sheetInterest, (scenario, t) => _interest[scenario, t]);
					FillRange(year, sheetScores, (scenario, t) => _scores[scenario, t]);
					FillRange(year, sheetNumberOfCheapHouses, (scenario, t) => _numberOfCheapHouses[scenario, t]);
					FillRange(year, sheetNumberOfBadHouses, (scenario, t) => _numberOfBadHouses[scenario, t]);

					sheetStatistics.Cells[year + 2, 1] = _numberOfBankruptcies[year];
					sheetStatistics.Cells[year + 2, 2] = Average(year, _scores);
					sheetStatistics.Cells[year + 2, 3] = Average(year, _sustainability);
					sheetStatistics.Cells[year + 2, 4] = Average(year, _rent);
					sheetStatistics.Cells[year + 2, 5] = Average(year, _interest);
					sheetStatistics.Cells[year + 2, 6] = Average(year, _houses);

					sheetStatistics.Cells[year + 2, 8] = Average(year, _numberOfHousesScores);
					sheetStatistics.Cells[year + 2, 9] = Average(year, _rentScores);
					sheetStatistics.Cells[year + 2, 10] = Average(year, _sustainabilityScores);
				}
			}
			finally
			{
				excelApp.Calculation = XlCalculation.xlCalculationAutomatic;
				excelApp.ScreenUpdating = true;
				excelApp.Visible = true;
				workBook.Activate();
			}
		}

		private static double Average(int year, double[,] scenarios)
		{
			var sum = 0.0;

			for (var scenario = 0; scenario < NumberOfScenarios; scenario++)
			{
				sum += scenarios[scenario, year];
			}

			return sum / NumberOfScenarios;
		}

		private void FillRange(int year, _Worksheet sheet, Func<int, int, double> getValue)
		{
			var cellRange = $"A{year + 1}:CW{year + 1}";
			var range = sheet.Range[cellRange];
			var values = new double[1, NumberOfScenarios + 1];
			values[0, 0] = _baseYear + year;
			for (var scenario = 0; scenario < NumberOfScenarios; scenario++)
			{
				values[0, scenario + 1] = getValue(scenario, year);
			}

			range.Value2 = values;
		}
	}
}
