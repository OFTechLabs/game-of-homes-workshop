using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GameOfHomes
{
	public class HousingAssociation
	{
		private LoanPortfolio _loanPortfolio;
		private readonly List<NewDevelopmentProjectType> _executedDevelopmentProjects = new List<NewDevelopmentProjectType>();
		readonly Dictionary<House, List<RenovationProjectType>> _executedRenovationProjects = new Dictionary<House, List<RenovationProjectType>>();
		public const double BankruptcyThreshold = 0.2;

		public double Debt => _loanPortfolio.Debt;

		public double Interest => _loanPortfolio.Interest;

		public double AnnualCosts
		{
			get;
		}

		public Policy Policy
		{
			get;
		}

		public RealEstatePortfolio RealEstatePortfolio
		{
			get;
			private set;
		}

		public int Scenario
		{
			get;
			set;
		}

		public HousingAssociation(double annualCosts)
		{
			Policy = new PolicyImplementation(this);
			InitHouses();

			double totalDebt = RealEstatePortfolio.TotalValue * 0.6;
			InitLoans(totalDebt); // E.g. 10 mln

			AnnualCosts = annualCosts; // E.g. 500.000
		}


		public void EndOfYear()
		{
			var expensesHouses = RealEstatePortfolio.EndOfYear();
			var earningsSales = SellHouses();

			foreach (var newDevelopmentProjectType in NewDevelopmentProjectType.GetAll())
			{
				newDevelopmentProjectType.EndOfYear();
			}

			var expensesNewDevelopment = ExecuteNewDevelopmentProjects();

			foreach (var renovationProjectProjectType in RenovationProjectType.GetAll())
			{
				renovationProjectProjectType.EndOfYear();
			}

			var expensesRenovation = ExecuteRenovations();

			var expensesLoans = _loanPortfolio.EndOfYear();

			// If we have spent money we need to attract a new loan.
			var totalExpenses = expensesLoans + expensesHouses + AnnualCosts - _loanPortfolio.Cash - earningsSales + expensesNewDevelopment + expensesRenovation;
			if (totalExpenses > 0)
			{
				var previousDebt = _loanPortfolio.Debt;
				AttractLoan(totalExpenses);
				_loanPortfolio.Cash = 0;

				// Assert that a loan really has been attracted.
				Debug.Assert(Math.Abs(previousDebt + totalExpenses - _loanPortfolio.Debt) < 0.01);
			}
			else
			{
				_loanPortfolio.Cash = -totalExpenses;
			}

			CumulativeInflation *= Economy.Get(Series.PriceInflation) + 1;

			IsBankrupt |= SolvencyRatio < BankruptcyThreshold;
		}

		public bool IsRenovationProjectExecuted(House house, RenovationProjectType renovationProjectType)
		{
			if (!_executedRenovationProjects.ContainsKey(house))
			{
				_executedRenovationProjects.Add(house, new List<RenovationProjectType>());
			}

			return _executedRenovationProjects[house].Contains(renovationProjectType);
		}


		private double ExecuteRenovations()
		{
			var expenses = 0.0;

			foreach (var house in RealEstatePortfolio.Houses)
			{
				foreach (var renovationProjectType in RenovationProjectType.GetAll())
				{
					if (!IsRenovationProjectExecuted(house, renovationProjectType) && Policy.DetermineExecutionRenovationProject(house, renovationProjectType))
					{
						var project = new RenovationProject(renovationProjectType);
						expenses += project.Execute(house);

						_executedRenovationProjects[house].Add(renovationProjectType);
					}
				}
			}

			return expenses;
		}

		private double SellHouses()
		{
			var gain = 0.0;
			foreach (var house in RealEstatePortfolio.Houses.ToList())
			{
				if (Policy.DetermineSell(house))
				{
					if (house.CurrentTenantLeaves)
					{
						gain += house.MarketValue;
					}
					else
					{
						gain += house.MarketValue * 0.7;
					}
					RealEstatePortfolio.RemoveHouse(house);
				}
			}
			return gain;
		}

		public bool IsBankrupt
		{
			get;
			set;
		}

		public double SolvencyRatio => 1 - Debt / (RealEstatePortfolio.TotalValue + _loanPortfolio.Cash);
		
		private const int InitialNumberOfHouses = 200;

		private void InitHouses()
		{
			RealEstatePortfolio = new RealEstatePortfolio();

			var random = new Random(42);

			for (var i = 0; i < InitialNumberOfHouses; i++)
			{
				var sustainability = (int)(random.NextDouble() * 60) + 20;
				var monthlyRent = 310.0 + random.NextDouble() * 180.0 + sustainability * 2; // A typical rent is typically approximate 70% of the market rent.
				var monthlyMarketRent = monthlyRent + random.NextDouble() * 180 + sustainability * 3;
				var monthlyMaintenanceExpenses = 200.0 + random.NextDouble() * 300.0;
				var value = 100000.0 + random.NextDouble() * 50000.0 + sustainability * 1000.0;
				var mutationProbability = 0.10;
				var lifeSpan = (int)(random.NextDouble() * 50);

				RealEstatePortfolio.AddNewHouse(this, monthlyRent, monthlyMarketRent, monthlyMaintenanceExpenses, value, mutationProbability, lifeSpan, sustainability);
			}
		}

		// A fraction of 1 / InitialMaturityLoan is paid of each year.
		private const int InitialMaturityLoan = 1; // 20 years is more realistic.

		private void InitLoans(double totalDebt)
		{
			_loanPortfolio = new LoanPortfolio();
			var valueOfSingleLoan = totalDebt / InitialMaturityLoan;

			for (var maturity = 1; maturity <= InitialMaturityLoan; maturity++)
			{
				var interestRate = Economy.GetInterestRate(maturity);
				_loanPortfolio.AddNewLoan(valueOfSingleLoan, interestRate, maturity);
			}

			Debug.Assert(Math.Abs(_loanPortfolio.Debt - totalDebt) < 0.01);
		}

		private void AttractLoan(double amount)
		{
			const int maturity = 10;
			var interestRate = Economy.GetInterestRate(maturity);
			_loanPortfolio.AddNewLoan(amount, interestRate, maturity);
		}

		public bool IsNewDevelopmentProjectExecuted(NewDevelopmentProjectType newDevelopmentProjectType)
		{
			return _executedDevelopmentProjects.Contains(newDevelopmentProjectType);
		}

		public double ExecuteNewDevelopmentProjects()
		{
			var expenses = 0.0;

			foreach (var projectType in NewDevelopmentProjectType.GetAll())
			{
				if (!IsNewDevelopmentProjectExecuted(projectType) && Policy.DetermineExecutionNewDevelopmentProject(projectType))
				{
					_executedDevelopmentProjects.Add(projectType);
					var project = new NewDevelopmentProject(projectType);
					expenses += project.ExecuteProject(this);
				}
			}

			return expenses;
		}

		public double AverageRent
		{
			get
			{
				if (RealEstatePortfolio.NumberOfHouses <= 0)
				{
					return 0;
				}

				return RealEstatePortfolio.Houses.Average(w => w.MonthlyRent);
			}
		}

		public double AverageMaintenanceExpenses
		{
			get
			{
				if (RealEstatePortfolio.NumberOfHouses <= 0)
				{
					return 0;
				}

				return RealEstatePortfolio.Houses.Average(w => w.MonthlyMaintenanceExpenses + w.AnnualComplainsMaintenanceExpenses);
			}
		}

		public double AverageSustainabilityOfHouses
		{
			get
			{
				if (RealEstatePortfolio.NumberOfHouses <= 0)
				{
					return 0;
				}

				return RealEstatePortfolio.Houses.Average(w => w.Sustainability);
			}
		}

		private const int MinimumNumberOfHouses = 180;
		private const int MaximumNumberOfHouses = 250;
		private const int ScoreHouse = 10;
		private const int FineHouse = 30;

		public double Score
		{
			get
			{
				if (IsBankrupt)
				{
					return 0;
				}

				return Math.Max(0, ScoreNumberOfHouses + ScoreSustainability + ScoreRent + 1000);
			}
		}

		public double CumulativeInflation
		{
			get;
			private set;
		} = 1;

		public double ScoreNumberOfHouses
		{
			get
			{
				double result = 0;
				int numberOfHouses = RealEstatePortfolio.NumberOfHouses;
				if (numberOfHouses < MinimumNumberOfHouses)
				{
					result -= (MinimumNumberOfHouses - numberOfHouses) * FineHouse;
				}
				else if (numberOfHouses > MaximumNumberOfHouses)
				{
					result += (MaximumNumberOfHouses - MinimumNumberOfHouses) * ScoreHouse;
				}
				else
				{
					result += (numberOfHouses - MinimumNumberOfHouses) * ScoreHouse;
				}

				return result;
			}
		}

		public const double Sufficient = 30;
		private const double Good = 70;
		private const double FineIfBad = 1000;
		private const double BonusIfSufficient = 5;
		private const double BonusIfGood = 2;

		public double ScoreSustainability
		{
			get
			{
				double result = 0;

				foreach (var house in RealEstatePortfolio.Houses)
				{
					if (house.Sustainability < Sufficient)
					{
						result -= FineIfBad;
					}
					else
					{
						var sustainability = Math.Min(house.Sustainability, Good);
						result += (sustainability - Sufficient) * BonusIfSufficient;

						if (house.Sustainability > Good)
						{
							result += (house.Sustainability - Good) * BonusIfGood;
						}
					}
				}

				return RealEstatePortfolio.NumberOfHouses == 0 ? 0 : result / RealEstatePortfolio.NumberOfHouses;
			}
		}

		private const double BonusLowRent = 100;
		private const int NumberOfLowRentHouses = 50;
		public const double LowRent = 400;
		private const double ScaleFactorRent = 400;

		public double ScoreRent
		{
			get
			{
				var faction = RealEstatePortfolio.Houses.Any() ? RealEstatePortfolio.Houses.Average(w => w.MonthlyRent / w.MaximumAllowedMonthlyRentNewTenant) : 1;
				var result = (1 - faction) * ScaleFactorRent;
				if (RealEstatePortfolio.Houses.Count(w => w.MonthlyRent / CumulativeInflation < LowRent) >= NumberOfLowRentHouses)
				{
					result += BonusLowRent;
				}

				return result;
			}
		}
	}
}
