namespace GameOfHomes
{
	public abstract class Project<TProjectType> where TProjectType : ProjectType
	{
		protected double Expenses
		{
			get;
		}

		public TProjectType ProjectType
		{
			get;
		}

		protected Project(TProjectType projectType)
		{
			Expenses = projectType.ConstructionCosts;
			ProjectType = projectType;
		}
	}
}
