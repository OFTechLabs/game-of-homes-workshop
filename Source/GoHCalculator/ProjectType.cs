using System.Diagnostics;

namespace GameOfHomes
{
	[DebuggerDisplay("{Name}")]
	public abstract class ProjectType
	{
		public string Name
		{
			get;
		}

		public double ConstructionCosts
		{
			get;
		}

		protected ProjectType(string name, double constructionCosts)
		{
			Name = name;
			ConstructionCosts = constructionCosts;
		}
	}
}