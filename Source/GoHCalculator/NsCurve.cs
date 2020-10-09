using System;
using static System.Double;

namespace GameOfHomes
{
	/// <summary>
	/// Term structure curve modeled by Nelson-Siegel parameters.
	/// </summary>
	public class NsCurve
	{
		/// <summary>
		/// Short parameter (maturity 0).
		/// </summary>
		public double Short
		{
			get;
			set;
		}

		/// <summary>
		/// Long parameter (maturity infinity).
		/// </summary>
		public double Long
		{
			get;
			set;
		}

		/// <summary>
		/// Convexity parameter (tau 1).
		/// </summary>
		public double Convexity
		{
			get;
			set;
		}

		/// <summary>
		/// Returns the interest rate yield at the specific maturity in years.
		/// </summary>
		public double GetYield(double maturity)
		{
			// The limit value to zero is short.
			if (maturity < Epsilon)
			{
				return Short;
			}

			var scaledMaturity = maturity / Convexity;
			var exponent = Math.Exp(-scaledMaturity);
			var beta1 = Short - Long;
			var result = Long + beta1 * (1 - exponent) / scaledMaturity;
			return result;
		}
	}
}
