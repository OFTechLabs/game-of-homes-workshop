using System.Collections.Generic;
using System.Linq;

namespace GameOfHomes
{
	class LoanPortfolio
	{
		private readonly List<Loan> _loans = new List<Loan>();

		public double Cash
		{
			get;
			set;
		}

		public double Debt => _loans.Sum(l => l.Value);

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
			var expenses = _loans.Sum(l => l.EndOfYear());
			Interest = _loans.Sum(l => l.Interest);

			// Remove all expired loans.
			_loans.RemoveAll(s => s.RemainingMaturity <= 0);

			return expenses;
		}

		public void AddNewLoan(double amount, double interestRate, double maturity)
		{
			_loans.Add(new Loan(amount, interestRate, maturity));
		}
	}
}
