namespace GoHCalculator
{
	class PolicyImplementation : Policy
	{
		public PolicyImplementation(HousingAssociation association): base(association)
		{	
		}

		/// Implement function DetermineNewRent below.
		/// It determines the new house rent price of the house for a given house for the current year in the simulation.
		/// 
		/// You can use the following variables:
		/// 
		///     house.MonthlyRent                                       The rent before adjustment (effectively last year's rent)
		///     Economy.Get(Series.PriceInflation)                      The price inflation of the current year, as a fraction (so 0.01 means 1%)
		///     HousingAssociation.SolvencyRatio                        The current solvency ratio of the housing association, which must remain above 0.2 (=20%)    
		///
		///     house.CurrentTenantLeaves                               Returns true if the current tenant has left at the start of the year and a new tenant arrives
		///     house.MaximumAllowedMonthlyRentNewTenant                The maximum rent you may ask a new tenant
		/// 
		///     Economy.CurrentYear                                     The current year (counted as 1,2,3,4...)
		///     house.MarketValue                                       The current market value of the house
		///     house.MonthlyMaintenanceExpenses                        The current monthly maintenance costs for the house
		///     house.Sustainability                                    The current sustainability rating of the house, a number between 0 and 80 where 70 is considered as good.
		///     house.LifeSpan                                          The remaining lifespan of the house, in years. If negative, maintenance costs will increase more.
		/// 
		///     house.HousingAssociation.AverageRent                    The average rent of all houses
        ///     house.HousingAssociation.AverageSustainabilityOfHouses  The average sustainability of all houses
        ///     house.HousingAssociation.AverageMaintenanceExpenses     The average maintenance expenses of all houses
        ///     
		/// If you have enough knowledge of C# such that you can handle IEnumberables, you can even dig through all properties of all houses with the following IEnumerable:
        /// 
        ///     house.HousingAssociation.RealEstatePortfolio.Houses     The list of all houses owned by the housing association
		///
		/// You may not increase the rent for the current tenant with more than the price inflation. However, if the current tenant leaves
		/// and a new tenant arrives, the housing association may ask a new tenant the maximum allowed rent (house.MaximumAllowedMonthlyRentNewTenant).
		/// 
		public override double DetermineNewRent(House house)
		{
			return house.MonthlyRent;
		}

		/// Implement function DetermineSell below.
        /// Determine if the house must be sold in the given year. 
        /// The association can sell a house for the best price if the current tenant leaves the house, so if house.CurrentTenantLeaves is true.
        /// You can use the same variables as for DetermineNewRent.
		public override bool DetermineSell(House house)
		{
			return false;
		}

		/// Implement function DetermineExecutionNewDevelopmentProject below.
        /// With this function you can order whether to execute a new development project. 
        /// The function will be called for every simulation year, for every project type that has not yet been executed until then.
        /// The following project types exist:
		///      NewDevelopmentProjectType.StudentFlat
		///      NewDevelopmentProjectType.ZeroEmissionHouse
		///      NewDevelopmentProjectType.Flat
		///      NewDevelopmentProjectType.TerracedHouse
		///      NewDevelopmentProjectType.SemiDetachedHouse
		/// 
		/// Please check the handouts for detailed information about the projects.
        /// 
        /// The following variables can be useful:
        ///     Economy.CurrentYear                            The current year (counted as 1,2,3,4...)
        ///     newDevelopmentProjectType                      The project type of the supplied project. See the handouts for details per project
        ///     HousingAssociation.SolvencyRatio               The current solvency ratio of the housing association, which must remain above 0.2 (=20%)    
        ///    
		public override bool DetermineExecutionNewDevelopmentProject(NewDevelopmentProjectType newDevelopmentProjectType)
		{
			//if (Economy.CurrentYear == 3 && newDevelopmentProjectType == NewDevelopmentProjectType.Flat)
			//{
			//	return true;
			//}

			return false;
		}

	    /// Implement function DetermineExecutionRenovationProject below.
        /// With this function you can order whether to execute a renovation for . 
        /// The function will be called for every house, for every simulation year and for every renovation type that has not yet been executed until then for that house.
        /// The following renovations types exist:
        ///     RenovationProjectType.SolarPanel,
		///     RenovationProjectType.DoubleGlazedWindows,
		///     RenovationProjectType.MajorRenovation,
		///     RenovationProjectType.MinorRenovation,
		///     RenovationProjectType.ReplacementOfKitchen,
		///     RenovationProjectType.RebuildFlat,
		///     RenovationProjectType.RebuildTerracedHouse,
		///     RenovationProjectType.RebuildSemiDetachedHouse.
		/// 
		/// Please check the handouts for detailed information about the projects.
        ///
        /// The following variables can be useful:
        ///     Economy.CurrentYear                            The current year (counted as 1,2,3,4...)
        ///     renovationProjectType                          The project type of the supplied project. See the handouts for details per project
        ///     HousingAssociation.SolvencyRatio               The current solvency ratio of the housing association, which must remain above 0.2 (=20%)    
        ///     
		public override bool DetermineExecutionRenovationProject(House house, RenovationProjectType renovationProjectType)
		{
			//if (Economy.CurrentYear == 3 && renovationProjectType == RenovationProjectType.DoubleGlazedWindows)
			//{
			//	return true;
			//}

			return false;
		}
	}
}