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

		private readonly Dictionary<OutputType, double[,]> _results;
		
		public Simulation()
		{
			_results = ((OutputType[])Enum.GetValues(typeof(OutputType))).Except(new [] {OutputType.NumberOfBankruptcies}).ToDictionary(ot => ot, ot => new double[NumberOfScenarios, Horizon + 2]);
			_results.Add(OutputType.NumberOfBankruptcies, new double[1, Horizon + 2]);
		}

		public Dictionary<OutputType, double[,]> Run()
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
			_results[OutputType.SolvencyRatio][scenario, t] = association.SolvencyRatio;
			_results[OutputType.Houses][scenario, t] = association.RealEstatePortfolio.NumberOfHouses;
			_results[OutputType.Rent][scenario, t] = association.AverageRent;
			_results[OutputType.Sustainability][scenario, t] = association.AverageSustainabilityOfHouses;
			_results[OutputType.Maintenance][scenario, t] = association.AverageMaintenanceExpenses;
			_results[OutputType.Debt][scenario, t] = association.Debt;
			_results[OutputType.Interest][scenario, t] = association.Interest;
			_results[OutputType.Scores][scenario, t] = association.Score;
			_results[OutputType.NumberOfHousesScore][scenario, t] = association.ScoreNumberOfHouses;
			_results[OutputType.RentScores][scenario, t] = association.ScoreRent;
			_results[OutputType.SustainabilityScores][scenario, t] = association.ScoreSustainability;
			_results[OutputType.NumberOfCheapHouses][scenario, t] = association.RealEstatePortfolio.Houses.Count(w => w.MonthlyRent / association.CumulativeInflation < HousingAssociation.LowRent);
			_results[OutputType.NumberOfBadHouses][scenario, t] = association.RealEstatePortfolio.Houses.Count(w => w.Sustainability < HousingAssociation.Sufficient);
			_results[OutputType.NumberOfBankruptcies][0, t] += association.IsBankrupt ? 1 : 0;
		}
	}
}
