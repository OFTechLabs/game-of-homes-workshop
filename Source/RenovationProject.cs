namespace GameOfHomes
{
	public class RenovationProject : Project<RenovationProjectType>
	{
		public RenovationProject(RenovationProjectType projectType) : base(projectType)
		{
		}

		public double Execute(House house)
		{
			house.MarketValue = ProjectType.MarketValue.Execute(house.MarketValue);
			house.Sustainability = ProjectType.Sustainability.Execute(house.Sustainability);
			house.MonthlyRent = ProjectType.Rent.Execute(house.MonthlyRent);
			house.MaximumAllowedMonthlyRentNewTenant = ProjectType.MaximumAllowedRent.Execute(house.MaximumAllowedMonthlyRentNewTenant);
			house.LifeSpan = (int)ProjectType.LifeSpan.Execute(house.LifeSpan);
			house.MonthlyMaintenanceExpenses = ProjectType.Maintenance.Execute(house.MonthlyMaintenanceExpenses);
			return Expenses;
		}
	}
}
