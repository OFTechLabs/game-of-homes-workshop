using System.Collections.Generic;

namespace GoHCalculator
{
	public class NewDevelopmentProjectType : ProjectType
	{
		public static readonly NewDevelopmentProjectType StudentFlat = new NewDevelopmentProjectType(nameof(StudentFlat), initialMaximumMonthlyRent: 400, monthlyMaintenanceCosts: 250, marketValueIfEmpty: 80000, numberOfHouses: 25, sustainability: 35, constructionCosts: 70000);
		public static readonly NewDevelopmentProjectType ZeroEmissionHouse = new NewDevelopmentProjectType(nameof(ZeroEmissionHouse), initialMaximumMonthlyRent: 1400, monthlyMaintenanceCosts: 500, marketValueIfEmpty: 320000, numberOfHouses: 5, sustainability: 95, constructionCosts: 300000);
		public static readonly NewDevelopmentProjectType Flat = new NewDevelopmentProjectType(nameof(Flat), initialMaximumMonthlyRent: 550, monthlyMaintenanceCosts: 250, marketValueIfEmpty: 120000, numberOfHouses: 20, sustainability: 60, constructionCosts: 110000);
		public static readonly NewDevelopmentProjectType TerracedHouse = new NewDevelopmentProjectType(nameof(TerracedHouse), initialMaximumMonthlyRent: 600, monthlyMaintenanceCosts: 300, marketValueIfEmpty: 125000, numberOfHouses: 15, sustainability: 55, constructionCosts: 120000);
		public static readonly NewDevelopmentProjectType SemiDetachedHouse = new NewDevelopmentProjectType(nameof(SemiDetachedHouse), initialMaximumMonthlyRent: 650, monthlyMaintenanceCosts: 350, marketValueIfEmpty: 200000, numberOfHouses: 10, sustainability: 40, constructionCosts: 180000);

		private NewDevelopmentProjectType(string name, double initialMaximumMonthlyRent, double monthlyMaintenanceCosts, double marketValueIfEmpty, int numberOfHouses, int sustainability, double constructionCosts):
			base(name, constructionCosts)
		{
			InitialMaximumMonthlyRent = initialMaximumMonthlyRent;
			InitialMonthlyMaintenanceExpenses = monthlyMaintenanceCosts;
			InitialMarketValue = marketValueIfEmpty;
			NumberOfHouses = numberOfHouses;
			Sustainability = sustainability;
		}

		public static IEnumerable<NewDevelopmentProjectType> GetAll()
		{
			yield return StudentFlat;
			yield return ZeroEmissionHouse;
			yield return Flat;
			yield return TerracedHouse;
			yield return SemiDetachedHouse;
		}

		private double InitialMaximumMonthlyRent
		{
			get;
		}

		public double MaximumMonthlyRent
		{
			get;
			private set;
		}

		private double InitialMonthlyMaintenanceExpenses
		{
			get;
		}

		public double MonthlyMaintenanceExpenses
		{
			get;
			private set;
		}

		private double InitialMarketValue
		{
			get;
		}

		public double MarketValue
		{
			get;
			private set;
		}

		public int NumberOfHouses
		{
			get;
		}

		public int Sustainability
		{
			get;
		}

		public virtual void EndOfYear()
		{
			if (Economy.CurrentYear == 0)
			{
				Reset();
			}

			MarketValue *= 1.0 + Economy.Get(Series.RealEstate);
			MaximumMonthlyRent *= 1.0 + Economy.Get(Series.RealEstate);
			MonthlyMaintenanceExpenses *= 1.0 + Economy.Get(Series.PriceInflation);
		}

		private void Reset()
		{
			MarketValue = InitialMarketValue;
			MaximumMonthlyRent = InitialMaximumMonthlyRent;
			MonthlyMaintenanceExpenses = InitialMonthlyMaintenanceExpenses;
		}
	}
}