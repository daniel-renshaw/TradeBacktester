using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeBacktester.Cond
{
	public class CondResult
	{
		public class CondResultData
		{
			public CondResultData()
			{
				Wins = 0;
				Losses = 0;
				Balance = Constants.ACCOUNT_SIZE;
			}

			public int Wins;
			public int Losses;
			public double Balance;
		}

		public CondResult()
		{
			mgrs = new CondResultData[(int)TradeManagerTypes.TOTAL];
			for (int i = 0; i < (int)TradeManagerTypes.TOTAL; ++i)
			{
				mgrs[i] = new CondResultData();
			}
		}

		public void Update(int type, bool win, double pl)
		{
			if (win)
				++mgrs[type].Wins;
			else
				++mgrs[type].Losses;

			mgrs[type].Balance += pl;
		}

		public void Update(CondResult res)
		{
			for (int i = 0; i < (int)TradeManagerTypes.TOTAL; ++i)
			{
				mgrs[i].Wins += res.mgrs[i].Wins;
				mgrs[i].Losses += res.mgrs[i].Losses;
				mgrs[i].Balance += res.mgrs[i].Balance - Constants.ACCOUNT_SIZE;
			}
		}

		public double GetManagerWinPercent(TradeManagerTypes type)
		{
			return mgrs[(int)type].Wins / (double)(mgrs[(int)type].Wins + mgrs[(int)type].Losses) * 100.0;
		}

		public double GetManagerPL(TradeManagerTypes type)
		{
			double ret = mgrs[(int)type].Balance - Constants.ACCOUNT_SIZE;
			if (ret > Data.HighestPL[(int)type])
				Data.HighestPL[(int)type] = ret;
			return ret;
		}

		public int GetManagerWins(TradeManagerTypes type)
		{
			return mgrs[(int)type].Wins;
		}

		public int GetManagerLosses(TradeManagerTypes type)
		{
			return mgrs[(int)type].Losses;
		}

		public int GetManagerTotalTrades(TradeManagerTypes type)
		{
			return mgrs[(int)type].Wins + mgrs[(int)type].Losses;
		}

		private CondResultData[] mgrs;
	}

	public class CondResults
	{
		public CondResults()
		{
			data = new Dictionary<ConditionMask, CondResult>();
		}

		public void AddResult(TradeManagerTypes type, bool win, Trade trade, double exit)
		{
			CondResult res;
			if (!data.TryGetValue(trade.EntryMask, out res))
				res = new CondResult();

			res.Update((int)type, win, trade.CalculatePL(exit));
			data[trade.EntryMask] = res;
		}

		public void GenerateFinalResults()
		{
			Dictionary<ConditionMask, CondResult> newData = new Dictionary<ConditionMask, CondResult>();

			foreach (KeyValuePair<ConditionMask, CondResult> entry in data)
			{
				for (ConditionMask i = ConditionMask.START; i < ConditionMask.END; ++i)
				{
					if (Conditions.CheckForConditionSkip(ref i, entry.Key))
						continue;

					if (entry.Key != i && entry.Key.HasFlag(i))
					{
						CondResult res;
						if (!newData.TryGetValue(i, out res))
							res = new CondResult();
						res.Update(entry.Value);
						newData[i] = res;
					}
				}
			}

			foreach (KeyValuePair<ConditionMask, CondResult> entry in newData)
			{
				CondResult res;
				if (data.TryGetValue(entry.Key, out res))
				{
					res.Update(entry.Value);
					data[entry.Key] = res;
				}
				else
					data[entry.Key] = entry.Value;
			}
		}

		public void PrintResults()
		{
			// filter out results below these values
			const double static05Base = 61.0;
			const double static10Base = 46.0;
			const double static15Base = 37.0;
			const double static20Base = 31.0;
			const double static25Base = 26.0;
			const double static30Base = 23.0;
			const double static35Base = 20.0;
			const double static40Base = 18.0;
			const double static45Base = 16.0;
			const double static50Base = 15.0;
			const double trailingStaticStopBase = 34.0;
			const double trailingStopPrevExtremeBase = 34.0;
			const double timedExitBase = 0.0;

			double static05WinPerc = 0.0;
			double static10WinPerc = 0.0;
			double static15WinPerc = 0.0;
			double static20WinPerc = 0.0;
			double static25WinPerc = 0.0;
			double static30WinPerc = 0.0;
			double static35WinPerc = 0.0;
			double static40WinPerc = 0.0;
			double static45WinPerc = 0.0;
			double static50WinPerc = 0.0;
			double trailingStaticStopWinPerc = 0.0;
			double trailingStopPrevExtremeWinPerc = 0.0;
			double timedExitWinPerc = 0.0;

			int numConditions = (int)Math.Log((double)ConditionMask.END, 2);
			int[] checkStats_Good = new int[numConditions];
			int[] checkStats_Bad = new int[numConditions];
			double[] checkStats_HighestIncrease = new double[numConditions];

			foreach (KeyValuePair<ConditionMask, CondResult> entry in data)
			{
				static05WinPerc = entry.Value.GetManagerWinPercent(TradeManagerTypes.STATIC_0_5);
				static10WinPerc = entry.Value.GetManagerWinPercent(TradeManagerTypes.STATIC_1_0);
				static15WinPerc = entry.Value.GetManagerWinPercent(TradeManagerTypes.STATIC_1_5);
				static20WinPerc = entry.Value.GetManagerWinPercent(TradeManagerTypes.STATIC_2_0);
				static25WinPerc = entry.Value.GetManagerWinPercent(TradeManagerTypes.STATIC_2_5);
				static30WinPerc = entry.Value.GetManagerWinPercent(TradeManagerTypes.STATIC_3_0);
				static35WinPerc = entry.Value.GetManagerWinPercent(TradeManagerTypes.STATIC_3_5);
				static40WinPerc = entry.Value.GetManagerWinPercent(TradeManagerTypes.STATIC_4_0);
				static45WinPerc = entry.Value.GetManagerWinPercent(TradeManagerTypes.STATIC_4_5);
				static50WinPerc = entry.Value.GetManagerWinPercent(TradeManagerTypes.STATIC_5_0);
				trailingStaticStopWinPerc = entry.Value.GetManagerWinPercent(TradeManagerTypes.TRAILING_STATIC_STOP);
				trailingStopPrevExtremeWinPerc = entry.Value.GetManagerWinPercent(TradeManagerTypes.TRAILING_STOP_PREV_EXTREME);
				timedExitWinPerc = entry.Value.GetManagerWinPercent(TradeManagerTypes.EXIT_AFTER_BAR_COUNT);

				if (static20WinPerc > static20Base && entry.Value.GetManagerTotalTrades(TradeManagerTypes.STATIC_2_0) > Constants.MINIMUM_TRADES)
					CheckStats(entry, static20WinPerc, ref checkStats_Good, ref checkStats_Bad, ref checkStats_HighestIncrease);

				bool print = false;
				string output = entry.Key.ToString() + Environment.NewLine;

				if (CheckAndPrintManagementType(entry, TradeManagerTypes.STATIC_0_5, static05Base, static05WinPerc, ref output))
					print = true;
				if (CheckAndPrintManagementType(entry, TradeManagerTypes.STATIC_1_0, static10Base, static10WinPerc, ref output))
					print = true;
				if (CheckAndPrintManagementType(entry, TradeManagerTypes.STATIC_1_5, static15Base, static15WinPerc, ref output))
					print = true;
				if (CheckAndPrintManagementType(entry, TradeManagerTypes.STATIC_2_0, static20Base, static20WinPerc, ref output))
					print = true;
				if (CheckAndPrintManagementType(entry, TradeManagerTypes.STATIC_2_5, static25Base, static25WinPerc, ref output))
					print = true;
				if (CheckAndPrintManagementType(entry, TradeManagerTypes.STATIC_3_0, static30Base, static30WinPerc, ref output))
					print = true;
				if (CheckAndPrintManagementType(entry, TradeManagerTypes.STATIC_3_5, static35Base, static35WinPerc, ref output))
					print = true;
				if (CheckAndPrintManagementType(entry, TradeManagerTypes.STATIC_4_0, static40Base, static40WinPerc, ref output))
					print = true;
				if (CheckAndPrintManagementType(entry, TradeManagerTypes.STATIC_4_5, static45Base, static45WinPerc, ref output))
					print = true;
				if (CheckAndPrintManagementType(entry, TradeManagerTypes.STATIC_5_0, static50Base, static50WinPerc, ref output))
					print = true;
				if (CheckAndPrintManagementType(entry, TradeManagerTypes.TRAILING_STATIC_STOP, trailingStaticStopBase, trailingStaticStopWinPerc, ref output))
					print = true;
				if (CheckAndPrintManagementType(entry, TradeManagerTypes.TRAILING_STOP_PREV_EXTREME, trailingStopPrevExtremeBase, trailingStopPrevExtremeWinPerc, ref output))
					print = true;
				if (CheckAndPrintManagementType(entry, TradeManagerTypes.EXIT_AFTER_BAR_COUNT, timedExitBase, timedExitWinPerc, ref output))
					print = true;

				if (print)
					Console.Write(output);
			}

			PrintCheckStats(ref checkStats_Good, ref checkStats_Bad, ref checkStats_HighestIncrease);
		}

		private void CheckStats(KeyValuePair<ConditionMask, CondResult> entry, double winPerc, ref int[] good, ref int[] bad, ref double[] highest)
		{
			int index = 0;
			for (ConditionMask i = ConditionMask.START; i < ConditionMask.END; i += (ulong)i, ++index)
			{
				if (entry.Key.HasFlag(i))
				{
					ConditionMask withoutFlag = entry.Key & ~i;
					CondResult res;
					if (data.TryGetValue(withoutFlag, out res))
					{
						double increase = winPerc - res.GetManagerWinPercent(TradeManagerTypes.STATIC_2_0);
						if (increase > highest[index])
							highest[index] = increase;

						if (increase > 0.0)
							++good[index];
						else
							++bad[index];
					}
				}
			}
		}

		private void PrintCheckStats(ref int[] good, ref int[] bad, ref double[] highest)
		{
			int index = 0;
			for (ConditionMask i = ConditionMask.START; i < ConditionMask.END; i += (ulong)i, ++index)
			{
				Console.WriteLine(string.Format("{0} Good = {1} Bad = {2} Highest Increase = {3:F1}%", i.ToString().PadRight(30, ' '), good[index].ToString().PadRight(8), bad[index].ToString().PadRight(8), highest[index]));
			}
		}

		private bool CheckAndPrintManagementType(KeyValuePair<ConditionMask, CondResult> entry, TradeManagerTypes type, double winPercBase, double winPerc, ref string output)
		{
			double pl = entry.Value.GetManagerPL(type);
			int wins = entry.Value.GetManagerWins(type);
			int losses = entry.Value.GetManagerLosses(type);
			double avgPL = pl / (wins + losses);

			if (winPerc > winPercBase && entry.Value.GetManagerTotalTrades(type) > Constants.MINIMUM_TRADES && pl > Constants.MINIMUM_PL && avgPL > Constants.MINIMUM_AVG_PL)
			{
				if (winPerc > Data.HighestWinRates[(int)type])
					Data.HighestWinRates[(int)type] = winPerc;

				string desc = "";
				switch (type)
				{
					case TradeManagerTypes.STATIC_0_5:
						desc = "  Static 0.5 reward ratio:    ";
						break;
					case TradeManagerTypes.STATIC_1_0:
						desc = "  Static 1.0 reward ratio:    ";
						break;
					case TradeManagerTypes.STATIC_1_5:
						desc = "  Static 1.5 reward ratio:    ";
						break;
					case TradeManagerTypes.STATIC_2_0:
						desc = "  Static 2.0 reward ratio:    ";
						break;
					case TradeManagerTypes.STATIC_2_5:
						desc = "  Static 2.5 reward ratio:    ";
						break;
					case TradeManagerTypes.STATIC_3_0:
						desc = "  Static 3.0 reward ratio:    ";
						break;
					case TradeManagerTypes.STATIC_3_5:
						desc = "  Static 3.5 reward ratio:    ";
						break;
					case TradeManagerTypes.STATIC_4_0:
						desc = "  Static 4.0 reward ratio:    ";
						break;
					case TradeManagerTypes.STATIC_4_5:
						desc = "  Static 4.5 reward ratio:    ";
						break;
					case TradeManagerTypes.STATIC_5_0:
						desc = "  Static 5.0 reward ratio:    ";
						break;
					case TradeManagerTypes.TRAILING_STATIC_STOP:
						desc = "  Trailing Static Stop:       ";
						break;
					case TradeManagerTypes.TRAILING_STOP_PREV_EXTREME:
						desc = "  Trailing Stop Prev Extreme: ";
						break;
					case TradeManagerTypes.EXIT_AFTER_BAR_COUNT:
						desc = "  Exit After Bar Count:       ";
						break;
				}

				output += string.Format("{0}{1:F1}%, P/L: {2:C2}, Avg P/L: {5:C2}, Wins: {3}, Losses: {4}", desc, winPerc, pl, wins, losses, avgPL);
				output += Environment.NewLine;
				return true;
			}

			return false;
		}

		private Dictionary<ConditionMask, CondResult> data;
	}
}
