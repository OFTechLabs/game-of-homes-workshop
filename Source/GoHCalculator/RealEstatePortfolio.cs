using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GameOfHomes
{
	public class RealEstatePortfolio
	{
		public List<House> Houses
		{
			get;
		} = new List<House>();

		public int NumberOfHouses => Houses.Count;

		public double TotalValue => Houses.Sum(w => w.MarketRentalValue);

		/// <summary>
		/// Returns the net expenses. 
		/// </summary>
		public double EndOfYear() => Houses.Sum(w => w.EndOfYear());

		public int GetNewNumber()
		{
			var max = 1;
			if (Houses.Any())
			{
				max += Houses.Max(w => w.Nr);
			}

			return max;
		}

		public void AddNewHouse(HousingAssociation association, double monthlyRent, double maximumAllowedMonthlyRent, double monthlyMaintenanceExpenses, double marketValue, double mutationProbability,
								int lifeSpan, double sustainability)
		{
			var newHouse = new House(GetNewNumber(), association, monthlyRent, maximumAllowedMonthlyRent, monthlyMaintenanceExpenses, marketValue, mutationProbability, lifeSpan, sustainability);
			Houses.Add(newHouse);
		}

		public void RemoveHouse(House house)
		{
			Houses.Remove(house);
		}
	}
}
