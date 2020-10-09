namespace GoHCalculator
{
	public class NewDevelopmentProject : Project<NewDevelopmentProjectType>
	{
		/// <summary>
		/// The maximum rent of each house in the project.
		/// </summary>
		public double MaximumMonthlyRent
		{
			get;
		}

		/// <summary>
		/// Monthly maintenance expenses of each house in the project.
		/// </summary>
		public double MonthlyMaintenanceExpenses
		{
			get;
		}

		/// <summary>
		/// Market value of each house in the project.
		/// </summary>
		public double MarketValue
		{
			get;
		}

		public int NumberOfHouses
		{
			get;
		}

		public int Sustainability
		{
			get;
		}

		public NewDevelopmentProject(NewDevelopmentProjectType projectType) 
			: base(projectType)
		{
			MaximumMonthlyRent = projectType.MaximumMonthlyRent;
			MonthlyMaintenanceExpenses = projectType.MonthlyMaintenanceExpenses;
			MarketValue = projectType.MarketValue;
			NumberOfHouses = projectType.NumberOfHouses;
			Sustainability = projectType.Sustainability;
		}

		public void BuildNewHouse(HousingAssociation association, double monthlyRent)
		{
			association.RealEstatePortfolio.AddNewHouse(association, monthlyRent, MaximumMonthlyRent, MonthlyMaintenanceExpenses, MarketValue, 0.1, 50, Sustainability);
		}

		public double ExecuteProject(HousingAssociation association)
		{
			for (var i = 0; i < NumberOfHouses; i++)
			{
				BuildNewHouse(association, MaximumMonthlyRent);
			}
			return Expenses * NumberOfHouses;
		}
	}
}
