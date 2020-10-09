using System;

namespace GameOfHomes
{
	public class House
	{
		private const double AnnualSustainabilityReduction = 0.98; // Each year the sustainability of a house decreases with 2%.
		private readonly Random _currentTenantLeavesGenerator;

		public double MonthlyRent
		{
			get;
			internal set;
		}

		/// <summary>
		/// The maximum rent that is allowed by law.
		/// </summary>
		public double MaximumAllowedMonthlyRentNewTenant
		{
			get;
			internal set;
		}

		private double _monthlyMaintenanceExpenses;

		public double MonthlyMaintenanceExpenses
		{
			get => _monthlyMaintenanceExpenses;
			// Maximize at  0; maintenance always cost money.
			internal set => _monthlyMaintenanceExpenses = Math.Max(0, value);
		}

		public double AnnualComplainsMaintenanceExpenses => LifeSpan > 0 ? 0 : MonthlyMaintenanceExpenses;

		public double MarketValue
		{
			get;
			internal set;
		}

		public double MutationProbability
		{
			get;
			private set;
		}

		public int LifeSpan
		{
			get;
			internal set;
		}

		public double Sustainability
		{
			get;
			internal set;
		}

		public int Nr
		{
			get;
			private set;
		}

		public bool CurrentTenantLeaves
		{
			get;
			private set;
		}

		public HousingAssociation HousingAssociation
		{
			get;
		}

		public House(int nr, HousingAssociation association, double monthlyRent, double maximumAllowedMonthlyRent, double monthlyMaintenanceExpenses, double marketValue, double mutationProbability,
			int lifeSpan,
			double sustainability)
		{
			Nr = nr;
			HousingAssociation = association;
			MonthlyRent = monthlyRent;
			MaximumAllowedMonthlyRentNewTenant = maximumAllowedMonthlyRent;
			MonthlyMaintenanceExpenses = monthlyMaintenanceExpenses;
			MarketValue = marketValue;
			MutationProbability = mutationProbability;
			LifeSpan = lifeSpan;
			Sustainability = sustainability;
			_currentTenantLeavesGenerator = new Random(Nr);
		}

		/// <summary>
		/// Returns the net spending.
		/// </summary>
		public double EndOfYear()
		{
			CurrentTenantLeaves = _currentTenantLeavesGenerator.NextDouble() >= MutationProbability;

			var amount = 0.0;
			amount += MonthlyMaintenanceExpenses * 12;
			amount += AnnualComplainsMaintenanceExpenses;
			amount -= MonthlyRent * 12;

			MarketValue *= 1.0 + Economy.Get(Series.RealEstate);
			MaximumAllowedMonthlyRentNewTenant *= 1.0 + Economy.Get(Series.PriceInflation);
			MonthlyMaintenanceExpenses *= 1.0 + Economy.Get(Series.PriceInflation);
			LifeSpan -= 1;
			MonthlyRent = UpdateRent();

			Sustainability *= AnnualSustainabilityReduction;

			return amount;
		}

		public double MarketRentalValue => 0.5 * MarketValue + MonthlyRent * 96;

		/// <summary>
		/// Returns the new rent.
		/// </summary>
		protected virtual double UpdateRent()
		{
			double previousRent = MonthlyRent;
			var maximumRent = CurrentTenantLeaves? Math.Max(MaximumAllowedMonthlyRentNewTenant, previousRent) : Math.Max(previousRent, previousRent * (Economy.Get(Series.PriceInflation) + 1));

			var newRent = HousingAssociation.Policy.DetermineNewRent(this);

			if (newRent > maximumRent + 0.01)
			{
				throw new ArgumentException($"The rent ({newRent}) of house with number {Nr} in year {Economy.CurrentYear} and scenario {HousingAssociation.Scenario} is bigger than the maximal allowed rent ({maximumRent})");
			}

			return newRent;
		}
	}
}
