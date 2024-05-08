using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeBacktester.Cond
{
	public class ConditionTester
	{
		public void HandleIndex(int index)
		{
			Data.Trades.CheckForEntry(index);
			Data.Trades.ManageTrades(index);
		}

		public void PrintResults()
		{
			Data.CondResults.GenerateFinalResults();
			Data.CondResults.PrintResults();
		}
	}
}
