namespace GameOfHomes
{
	class Loan
	{
		public double Value
		{
			get;
			private set;
		}

		public double InterestRate
		{
			get;
		}

		public double RemainingMaturity
		{
			get;
			private set;
		}

		public Loan(double value, double interestRate, double maturity)
		{
			Value = value;
			InterestRate = interestRate;
			RemainingMaturity = maturity;
			Interest = Value * InterestRate;
		}

		public double Interest
		{
			get;
			private set;
		}

		/// <summary>
		/// Returns the amount of money that is paid off plus the paid interest.
		/// </summary>
		public double EndOfYear()
		{
			Interest = Value * InterestRate;

			var amount = Interest;

			RemainingMaturity -= 1;

			// Check if the loan must be paid off.
			if (RemainingMaturity <= 0)
			{
				amount += Value;

				// Make sure that an expired loan is not causing expenses accidentally.
				Value = 0.0;
			}

			return amount;
		}
	}
}
