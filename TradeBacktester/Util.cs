using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeBacktester
{
	public static class Util
	{
		public static List<List<T>> GetAllCombos<T>(List<T> list)
		{
			List<List<T>> result = new List<List<T>>();
			// head
			result.Add(new List<T>());
			result.Last().Add(list[0]);
			if (list.Count == 1)
				return result;
			// tail
			List<List<T>> tailCombos = GetAllCombos(list.Skip(1).ToList());
			tailCombos.ForEach(combo =>
			{
				result.Add(new List<T>(combo));
				combo.Add(list[0]);
				result.Add(new List<T>(combo));
			});
			return result;
		}

		public static double FloorToVal(double num, double val)
		{
			return Math.Floor((num + 0.00000001) / val) * val;
		}

		public static double CalculatePL(double entry, double exit, double lots, bool direction, bool withCommission)
		{
			return ((exit - entry) * lots * 100000.0 * (direction ? 1.0 : -1.0)) - (withCommission ? lots * Constants.COMMISSION_PER_LOT : 0.0);
		}

		public static double CalculateLots(double slSize, double acctSize)
		{
			double risk = acctSize * Constants.RISK_PERCENT;
			return risk / (slSize / Constants.POINT);
		}

		public static double GetMaxDrawdown(ref List<double> balance)
		{
			double maxDD = 0.0;
			double high = Constants.ACCOUNT_SIZE;
			double low = high;

			for (int i = 0; i < balance.Count; ++i)
			{
				if (balance[i] > high)
				{
					if (low < high)
					{
						double dd = (low - high) / high;
						if (dd < maxDD)
							maxDD = dd;
					}

					high = balance[i];
					low = balance[i];
				}
				else if (balance[i] < low)
					low = balance[i];
			}

			if (low < high)
			{
				double dd = (low - high) / high;
				if (dd < maxDD)
					maxDD = dd;
			}

			return maxDD;
		}
	}
}
