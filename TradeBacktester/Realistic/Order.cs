using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeBacktester.Realistic
{
	public class Order
	{
		public Order (bool dir, double e)
		{
			direction = dir;
			entry = e;
		}

		public bool Direction
		{
			get => direction;
		}

		public double Entry
		{
			get => entry;
		}

		public bool IsFilled(int index)
		{
			if (direction)
			{
				double entryVal = entry - Data.Bars[index - 1].Spread;
				return Data.Bars[index].High >= entryVal && entryVal >= Data.Bars[index].Low;
			}

			return Data.Bars[index].Low <= entry && entry <= Data.Bars[index].High;
		}

		private bool direction;
		private double entry;
	}
}
