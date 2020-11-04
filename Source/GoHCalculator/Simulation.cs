using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GoHCalculator
{
	public enum OutputType
	{
		SolvencyRatio,
		Houses,
		Rent,
		Sustainability,
		Maintenance,
		Debt,
		Interest,
		Scores,
		NumberOfHousesScore,
		RentScores,
		SustainabilityScores,
		NumberOfCheapHouses,
		NumberOfBadHouses,
		NumberOfBankruptcies
	}

	public class Simulation
	{
		private const int NumberOfScenarios = 100;
		private const int Horizon = 30;

		private readonly Dictionary<OutputType, double[][]> _results;
		
		public Simulation()
		{
			_results = ((OutputType[])Enum.GetValues(typeof(OutputType))).ToDictionary(ot => ot, ot => 
			{
				var horizon = Horizon + 2;
				var numberOfScenarios = ot == OutputType.NumberOfBankruptcies ? 1 : NumberOfScenarios;
				var result = new double[horizon][];
				for (int t = 0; t < horizon; t++)
				{
					result[t] = new double[numberOfScenarios];
				}
				return result;
			});
		}

		public Dictionary<OutputType, double[][]> Run()
		{
			Console.Write(@"Simulating scenario ");

			for (var scenario = 0; scenario < NumberOfScenarios; scenario++)
			{
				Console.Write($@"{scenario + 1}");
				Console.SetCursorPosition(20, 1);

				Economy.Read(scenario);
				
				var association = new HousingAssociation(0);
				GatherData(association, scenario, 0);

				for (var t = 0; t <= Horizon; t++)
				{
					Economy.CurrentYear = t;
					association.EndOfYear();
					GatherData(association, scenario, t + 1);
				}
			}

			return _results;
		}

		private void GatherData(HousingAssociation association, int scenario, int t)
		{
			_results[OutputType.SolvencyRatio][t][scenario] = association.SolvencyRatio;
			_results[OutputType.Houses][t][scenario] = association.RealEstatePortfolio.NumberOfHouses;
			_results[OutputType.Rent][t][scenario] = association.AverageRent;
			_results[OutputType.Sustainability][t][scenario] = association.AverageSustainabilityOfHouses;
			_results[OutputType.Maintenance][t][scenario] = association.AverageMaintenanceExpenses;
			_results[OutputType.Debt][t][scenario] = association.Debt;
			_results[OutputType.Interest][t][scenario] = association.Interest;
			_results[OutputType.Scores][t][scenario] = association.Score;
			_results[OutputType.NumberOfHousesScore][t][scenario] = association.ScoreNumberOfHouses;
			_results[OutputType.RentScores][t][scenario] = association.ScoreRent;
			_results[OutputType.SustainabilityScores][t][scenario] = association.ScoreSustainability;
			_results[OutputType.NumberOfCheapHouses][t][scenario] = association.RealEstatePortfolio.Houses.Count(w => w.MonthlyRent / association.CumulativeInflation < HousingAssociation.LowRent);
			_results[OutputType.NumberOfBadHouses][t][scenario] = association.RealEstatePortfolio.Houses.Count(w => w.Sustainability < HousingAssociation.Sufficient);
			_results[OutputType.NumberOfBankruptcies][t][0] += association.IsBankrupt ? 1 : 0;
		}
	}
}
