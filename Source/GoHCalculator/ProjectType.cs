using System.Diagnostics;

namespace GoHCalculator
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