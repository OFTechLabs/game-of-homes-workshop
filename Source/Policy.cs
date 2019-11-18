using System.Collections.Generic;

namespace GameOfHomes
{
	public abstract class Policy
	{
		protected HousingAssociation HousingAssociation
		{
			get;
		}
		protected Policy(HousingAssociation association)
		{
			HousingAssociation = association;
		}

		public abstract double DetermineNewRent(House house);
		public abstract bool DetermineSell(House house);
		public abstract bool DetermineExecutionNewDevelopmentProject(NewDevelopmentProjectType newDevelopmentProjectType);
		public abstract bool DetermineExecutionRenovationProject(House house, RenovationProjectType renovationProjectType);
	}
}
