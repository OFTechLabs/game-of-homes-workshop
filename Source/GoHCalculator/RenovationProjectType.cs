using System.Collections.Generic;

namespace GoHCalculator
{
	public class RenovationProjectType : ProjectType
	{
		public RenovationParameter Rent
		{
			get;
			private set;
		}

		public double InitialRent
		{
			get;
		}

		public RenovationParameter MaximumAllowedRent
		{
			get;
			private set;
		}

		public double InitialMaximumAllowedRent
		{
			get;
		}

		public RenovationParameter Maintenance
		{
			get;
			private set;
		}

		public double InitialMaintenance
		{
			get;
		}

		public RenovationParameter MarketValue
		{
			get;
			private set;
		}

		public double InitialMarketValue
		{
			get;
		}

		public RenovationParameter LifeSpan
		{
			get;
		}

		public RenovationParameter Sustainability
		{
			get;
		}

		public static readonly RenovationProjectType SolarPanels = new RenovationProjectType(nameof(SolarPanels), rent: 50, initialMaximumAllowedRent: 50, maintenanceCosts: 20, marketValueIfEmpty: 5000, lifeSpan: 0, sustainability: 15, constructionCosts: 8000, renovationType: RenovationType.Delta);
		public static readonly RenovationProjectType DoubleGlazedWindows = new RenovationProjectType(nameof(DoubleGlazedWindows), rent: 0, initialMaximumAllowedRent: 25, maintenanceCosts: -50, marketValueIfEmpty: 0, lifeSpan: 2, sustainability: 5, constructionCosts: 3000, renovationType: RenovationType.Delta);
		public static readonly RenovationProjectType MajorRenovation = new RenovationProjectType(nameof(MajorRenovation), rent: 0, initialMaximumAllowedRent: 75, maintenanceCosts: -20, marketValueIfEmpty: 0, lifeSpan: 20, sustainability: 10, constructionCosts: 15000, renovationType: RenovationType.Delta);
		public static readonly RenovationProjectType MinorRenovation = new RenovationProjectType(nameof(MinorRenovation), rent: 0, initialMaximumAllowedRent: 0, maintenanceCosts: -10, marketValueIfEmpty: 0, lifeSpan: 4, sustainability: 4, constructionCosts: 3500, renovationType: RenovationType.Delta);
		public static readonly RenovationProjectType ReplacementOfKitchen = new RenovationProjectType(nameof(ReplacementOfKitchen), rent: 0, initialMaximumAllowedRent: 0, maintenanceCosts: 0, marketValueIfEmpty: 1000, lifeSpan: 0, sustainability: 0, constructionCosts: 5000, renovationType: RenovationType.Delta);
		public static readonly RenovationProjectType RebuildFlat = new RenovationProjectType(nameof(RebuildFlat), rent: 400, initialMaximumAllowedRent: 500, maintenanceCosts: 300, marketValueIfEmpty: 80000, lifeSpan: 50, sustainability: 50, constructionCosts: 65000, renovationType: RenovationType.Value);
		public static readonly RenovationProjectType RebuildTerracedHouse = new RenovationProjectType(nameof(RebuildTerracedHouse), rent: 550, initialMaximumAllowedRent: 650, maintenanceCosts: 400, marketValueIfEmpty: 120000, lifeSpan: 50, sustainability: 45, constructionCosts: 100000, renovationType: RenovationType.Value);
		public static readonly RenovationProjectType RebuildSemiDetachedHouse = new RenovationProjectType(nameof(RebuildSemiDetachedHouse), rent: 950, initialMaximumAllowedRent: 1250, maintenanceCosts: 600, marketValueIfEmpty: 250000, lifeSpan: 50, sustainability: 35, constructionCosts: 225000, renovationType: RenovationType.Value);

		private RenovationProjectType(string name, double rent, double initialMaximumAllowedRent, double maintenanceCosts, double marketValueIfEmpty, int lifeSpan, int sustainability, double constructionCosts, RenovationType renovationType) : base(name, constructionCosts)
		{
			InitialRent = rent;
			Rent = new RenovationParameter(renovationType, rent);
			InitialMaximumAllowedRent = initialMaximumAllowedRent;
			MaximumAllowedRent = new RenovationParameter(renovationType, initialMaximumAllowedRent);
			InitialMaintenance = maintenanceCosts;
			Maintenance = new RenovationParameter(renovationType, maintenanceCosts);
			InitialMarketValue = marketValueIfEmpty;
			MarketValue = new RenovationParameter(renovationType, marketValueIfEmpty);
			LifeSpan = new RenovationParameter(renovationType, lifeSpan);
			Sustainability = new RenovationParameter(renovationType, sustainability);
		}

		public static IEnumerable<RenovationProjectType> GetAll()
		{
			yield return SolarPanels;
			yield return DoubleGlazedWindows;
			yield return MajorRenovation;
			yield return MinorRenovation;
			yield return ReplacementOfKitchen;
			yield return RebuildFlat;
			yield return RebuildTerracedHouse;
			yield return RebuildSemiDetachedHouse;
		}

		public virtual void EndOfYear()
		{
			if (Economy.CurrentYear == 0)
			{
				Reset();
			}

			MarketValue = MarketValue * (Economy.Get(Series.RealEstate) + 1);
			Rent = Rent * (Economy.Get(Series.RealEstate) + 1);
			MaximumAllowedRent = MaximumAllowedRent * (Economy.Get(Series.RealEstate) + 1);
			Maintenance = Maintenance * (Economy.Get(Series.RealEstate) + 1);
		}

		private void Reset()
		{
			MarketValue = new RenovationParameter(MarketValue.Type, InitialMarketValue);
			Rent = new RenovationParameter(Rent.Type, InitialRent);
			MaximumAllowedRent = new RenovationParameter(MaximumAllowedRent.Type, InitialMaximumAllowedRent);
			Maintenance = new RenovationParameter(Maintenance.Type, InitialMaintenance);
		}
	}
}
