using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeBacktester.Cond;

namespace TradeBacktester.Realistic
{
	public class RealisticTester
	{
		public const int AREA_OF_INTEREST_BAR_COOLDOWN = 5;
		public const double MAX_DAILY_LOSS = Constants.ACCOUNT_SIZE * -0.04;

		public RealisticTester()
		{
			acctYearData = new Dictionary<int, YearData>();
			acctYearData[0] = new YearData(0, Constants.ACCOUNT_SIZE);
			algos = new Algorithm[]
			{
				new Algo_SellTest(ref acctYearData),
				new Algo_BuyTest(ref acctYearData),
			};
		}

		public bool HandleIndex(int index)
		{
			if (Data.Bars[index - 1].Day != Data.Bars[index].Day)
			{
				ResetDailyPL();
				CloseAllPositions(Data.Bars[index - 1].Close);
				CancelOrders();
			}

			if (!Conditions.IsBetweenHours(index, 0, 23) || GetTotalAlgoDailyPL() <= MAX_DAILY_LOSS)
			{
				CloseAllPositions(Data.Bars[index - 1].Close);
				CancelOrders();
				return false;
			}

			foreach (Algorithm algo in algos)
			{
				algo.HandleIndex(index);
			}

			//return Balance <= 0.0;
			return false;
		}

		private double GetTotalAlgoDailyPL()
		{
			double pl = 0.0;

			foreach (Algorithm algo in algos)
			{
				pl += algo.DailyPL;
			}

			return pl;
		}

		private void CloseAllPositions(double exit)
		{
			foreach (Algorithm algo in algos)
			{
				algo.CloseAllPositions(exit);
			}
		}

		private void CancelOrders()
		{
			foreach (Algorithm algo in algos)
			{
				algo.CancelOrders();
			}
		}

		private void ResetDailyPL()
		{
			foreach (Algorithm algo in algos)
			{
				algo.ResetDailyPL();
			}
		}

		public void PrintResults()
		{
			Console.WriteLine("All Algos Combined");

			foreach (KeyValuePair<int, YearData> data in acctYearData)
			{
				YearData yd = data.Value;
				string year = yd.Year == 0 ? "All:" : (yd.Year.ToString() + ":");
				year = year.PadRight(5);
				string wr = string.Format("{0:F1}%", yd.WinRate).PadRight(5);
				string pl = string.Format("{0:C2}", yd.ProfitLoss).PadRight(15);
				string avgpl = string.Format("{0:C2}", yd.AveragePL).PadRight(10);
				string maxdd = string.Format("{0:F1}%", yd.MaxDrawdown).PadRight(8);
				string wins = string.Format("{0}", yd.Wins).PadRight(6);
				string losses = string.Format("{0}", yd.Losses).PadRight(6);
				Console.WriteLine(string.Format("{0} {1} P/L: {2} Avg P/L: {3} Max DD: {4} Wins: {5} Losses: {6}", year, wr, pl, avgpl, maxdd, wins, losses));
			}

			string avgWin = string.Format("{0:C2}", All.AverageWin).PadRight(14);
			string avgLoss = string.Format("{0:C2}", All.AverageLoss).PadRight(14);
			string largestWin = string.Format("{0:C2}", All.LargestWin).PadRight(14);
			string largestLoss = string.Format("{0:C2}", All.LargestLoss).PadRight(14);
			Console.WriteLine(string.Format("Average Win: {0} Average Loss: {1}", avgWin, avgLoss));
			Console.WriteLine(string.Format("Largest Win: {0} Largest Loss: {1}", largestWin, largestLoss));
			Console.WriteLine();

			foreach (Algorithm algo in algos)
			{
				Console.WriteLine(algo.GetName());
				algo.PrintResults();
				Console.WriteLine();
			}
		}

		public YearData All
		{
			get => acctYearData[0];
		}

		private Algorithm[] algos;
		private Dictionary<int, YearData> acctYearData;
	}
}
