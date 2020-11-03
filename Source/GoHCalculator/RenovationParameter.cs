namespace GoHCalculator
{
	public struct RenovationParameter
	{
		public readonly RenovationType Type;
		public readonly double Value;

		public RenovationParameter(RenovationType type, double value)
		{
			Type = type;
			Value = value;
		}

		public double Execute(double old)
		{
			if (Type == RenovationType.Delta)
			{
				return old + Value;
			}

			return Value;
		}

		public static RenovationParameter operator *(RenovationParameter parameter, double value)
		{
			return new RenovationParameter(parameter.Type, parameter.Value * value);
		}
	}
}
