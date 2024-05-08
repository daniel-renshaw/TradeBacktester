using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeBacktester.Cond;

namespace TradeBacktester.Realistic
{
	public class YearData
	{
		public YearData(int y, double startBalance)
		{
			year = y;
			balance = new List<double>();
			balance.Add(startBalance);
			winAmt = 0.0;
			lossAmt = 0.0;
			wins = 0;
			losses = 0;
			largestWin = 0.0;
			largestLoss = 0.0;
		}

		public void PositionClosed(bool win, double pl)
		{
			if (pl < -2000.0)
				throw new Exception("yikes");
			balance.Add(Balance + pl);
			if (win)
			{
				++wins;
				winAmt += pl;
				if (pl > largestWin)
					largestWin = pl;
			}
			else
			{
				++losses;
				lossAmt += pl;
				if (pl < largestLoss)
					largestLoss = pl;
			}
		}

		public double Balance
		{
			get => balance.Last();
		}

		public double ProfitLoss
		{
			get => balance.Last() - balance.First();
		}

		public double AveragePL
		{
			get => ProfitLoss / (Wins + Losses);
		}

		public int Year
		{
			get => year;
		}

		public int Wins
		{
			get => wins;
		}

		public int Losses
		{
			get => losses;
		}

		public double WinRate
		{
			get => wins / (double)(wins + losses) * 100.0;
		}

		public double MaxDrawdown
		{
			get => Util.GetMaxDrawdown(ref balance) * 100.0;
		}

		public double AverageWin
		{
			get => winAmt / wins;
		}

		public double AverageLoss
		{
			get => lossAmt / losses;
		}

		public double LargestWin
		{
			get => largestWin;
		}

		public double LargestLoss
		{
			get => largestLoss;
		}

		public void PrintBalanceValues()
		{
			for (int i = 0; i < balance.Count; ++i)
			{
				Console.WriteLine(string.Format("{0:C2}", balance[i]));
			}
		}

		private double winAmt;
		private double lossAmt;
		private double largestWin;
		private double largestLoss;
		private int wins;
		private int losses;
		private List<double> balance;
		private int year;
	}

	public abstract class Algorithm
	{
		public Algorithm(ref Dictionary<int, YearData> acct)
		{
			acctYearData = acct;
			positions = new List<Position>();
			orders = new List<Order>();
			yearData = new Dictionary<int, YearData>();
			yearData[0] = new YearData(0, Constants.ACCOUNT_SIZE);
			dailyPL = 0.0;
			algoSL = 0.0;
			algoDirection = false;
		}

		public abstract void HandleIndex(int index);
		public abstract string GetName();

		protected void PlaceOrder(bool dir, double entry)
		{
			orders.Add(new Order(dir, entry));
		}

		protected void FillOrder(int oIndex, int index)
		{
			OpenPosition(index, orders[oIndex].Direction, orders[oIndex].Entry, 1.0);
			orders.RemoveAt(oIndex);
		}

		public void CancelOrders()
		{
			if (orders.Count > 0)
				orders.Clear();
		}

		protected void OpenPosition(int index, bool dir, double entry, double lotMod)
		{
			double sl = Constants.ATR_BASED_SL ? Data.ATR[20] : Data.Bars[index - 1].GetRange();
			if (dir)
				sl *= Constants.SL_MULTI_BUY_CLOSE;
			else
				sl *= Constants.SL_MULTI_SELL_CLOSE;

			positions.Add(new Position(index, dir, entry, Util.CalculateLots(sl, Constants.COMPOUNDING ? Balance : Constants.ACCOUNT_SIZE) * lotMod, sl, 0.0));
		}

		public void CloseAllPositions(double exit)
		{
			while (positions.Count > 0)
				ClosePosition(0, exit);
			algoSL = 0.0;
		}

		protected void ClosePosition(int posIndex, double exit)
		{
			Position pos = positions[posIndex];
			double pl = Util.CalculatePL(pos.Entry, exit, pos.Lots, pos.Direction, true);
			int year = Data.Bars[pos.EntryIndex].Year;
			bool win = pos.Direction ? pos.Entry < exit : pos.Entry > exit;
			dailyPL += pl;

			YearData yd;
			if (!yearData.TryGetValue(year, out yd))
				yd = new YearData(year, Constants.ACCOUNT_SIZE);

			YearData acctYd;
			if (!acctYearData.TryGetValue(year, out acctYd))
				acctYd = new YearData(year, Constants.ACCOUNT_SIZE);

			All.PositionClosed(win, pl);
			yd.PositionClosed(win, pl);
			yearData[year] = yd;

			acctYearData[0].PositionClosed(win, pl);
			acctYd.PositionClosed(win, pl);
			acctYearData[year] = acctYd;

			positions.RemoveAt(posIndex);
		}

		public void PrintResults()
		{
			// Don't print anything if we didn't take any trades
			if ((All.Wins + All.Losses) == 0)
				return;

			foreach (KeyValuePair<int, YearData> data in yearData)
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
		}

		protected void UpdateAlgoSL(double sl)
		{
			if (algoSL == 0.0)
			{
				algoSL = sl;
				return;
			}

			if (algoDirection)
			{
				if (sl > algoSL)
					algoSL = sl;
			}
			else
			{
				if (sl < algoSL)
					algoSL = sl;
			}
		}

		protected bool IsAlgoStoppedOut(int index)
		{
			if (positions.Count == 0)
				return false;

			if (algoDirection)
				return Data.Bars[index].Low <= algoSL;
			return Data.Bars[index].High >= (algoSL - Data.Bars[index].Spread);
		}

		public void ResetDailyPL()
		{
			dailyPL = 0.0;
		}

		public YearData All
		{
			get => yearData[0];
		}

		public double Balance
		{
			get => yearData[0].Balance;
		}

		public double DailyPL
		{
			get => dailyPL;
		}

		protected Dictionary<int, YearData> acctYearData;
		protected Dictionary<int, YearData> yearData;
		protected List<Position> positions;
		protected List<Order> orders;
		protected double dailyPL;
		protected double algoSL;
		protected bool algoDirection;
	}

	public class Algo_SellTest : Algorithm
	{
		public Algo_SellTest(ref Dictionary<int, YearData> acct) : base(ref acct)
		{
		}

		public override string GetName()
		{
			return "Sell Test";
		}

		public override void HandleIndex(int index)
		{
			for (int i = positions.Count - 1; i >= 0; --i)
			{
				Position pos = positions[i];

				if (pos.IsStoppedOut(index))
				{
					ClosePosition(i, pos.SLPrice);
					continue;
				}

				if (index - pos.EntryIndex >= 50)
					ClosePosition(i, Data.Bars[index].Close);
			}

			if (EntryConditions(index))
				OpenPosition(index, false, Data.Bars[index].Close, 1.0);
		}

		private bool EntryConditions(int index)
		{
			return
				Conditions.IsCondition1Present(index) &&
				Conditions.IsCondition2Present(index) &&
				true;
		}
	}

	public class Algo_BuyTest : Algorithm
	{
		public Algo_BuyTest(ref Dictionary<int, YearData> acct) : base(ref acct)
		{
		}

		public override string GetName()
		{
			return "Buy Test";
		}

		public override void HandleIndex(int index)
		{
			for (int i = positions.Count - 1; i >= 0; --i)
			{
				Position pos = positions[i];

				if (pos.IsStoppedOut(index))
				{
					ClosePosition(i, pos.SLPrice);
					continue;
				}

				if (index - pos.EntryIndex >= 50)
					ClosePosition(i, Data.Bars[index].Close);
			}

			if (EntryConditions(index))
				OpenPosition(index, true, Data.Bars[index].Close, 1.0);
		}

		private bool EntryConditions(int index)
		{
			return
				Conditions.IsCondition1Present(index) &&
				Conditions.IsCondition2Present(index) &&
				true;
		}
	}
}
