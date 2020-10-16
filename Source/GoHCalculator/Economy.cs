using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GoHCalculator
{
	public static class Economy
	{
        private static List<List<double>> _series;

		public static void Read(int scenario)
		{
			var fileName = $@"..\..\Scenarios\Scenario{scenario}.csv";
            var lines = File.ReadAllLines(fileName);
			var result = lines.Select(l => l.Split(';').Select(s => double.Parse(s, CultureInfo.InvariantCulture.NumberFormat)).ToList());
			_series = result.ToList();
			CurrentYear = 0;
		}

		public static int CurrentYear
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the value of the series as a decimal (not a percentage).
		/// </summary>
		public static double Get(Series series)
		{
            Debug.Assert(0 <= CurrentYear && CurrentYear < _series.Count);
            return _series[CurrentYear][(int)series];
		}

		/// <summary>
		/// Gets the value of the interest rate as a decimal (not a percentage).
		/// </summary>
		public static double GetInterestRate(double maturity)
		{
			var nsCurve = new NsCurve
			{
				Short = Get(Series.InterestRateOneYears),
				Long = Get(Series.InterestRateTwentyYears),
				Convexity = 1.8
			};
			return nsCurve.GetYield(maturity);
		}
	}
}
