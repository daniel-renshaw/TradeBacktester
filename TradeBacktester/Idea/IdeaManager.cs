using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeBacktester.Idea
{
	public abstract class IdeaManager
	{
		public IdeaManager()
		{
			wins = 0;
			losses = 0;
			acctBal = new List<double>();
			acctBal.Add(Constants.ACCOUNT_SIZE);
			activeTrades = new HashSet<int>();
		}

		public virtual void AddTrade(Trade trade)
		{
			activeTrades.Add(trade.UID);
		}

		protected virtual void FinishTrade(bool win, Trade trade, double exit)
		{
			if (win)
				++wins;
			else
				++losses;

			acctBal.Add(acctBal.Last() + trade.CalculatePL(exit));
			activeTrades.Remove(trade.UID);
		}

		public void Print()
		{
			double winPerc = wins / (double)(wins + losses) * 100;
			double pl = acctBal.Last() - Constants.ACCOUNT_SIZE;
			Console.WriteLine(string.Format("  {0}: {1:F1}%, P/L: {2:C2}, Avg P/L: {6:C2}, Max DD: {5:F1}%, Wins: {3}, Losses: {4}", GetDescription(), winPerc, pl, wins, losses, Util.GetMaxDrawdown(ref acctBal) * 100.0, pl / (wins + losses)));
		}

		public abstract bool Manage(Trade trade, int index);
		public abstract String GetDescription();

		protected int wins;
		protected int losses;
		protected List<double> acctBal;
		protected HashSet<int> activeTrades;
	}

	public class StaticIdeaManager : IdeaManager
	{
		public StaticIdeaManager(double r)
		{
			ratio = r;
		}

		public override bool Manage(Trade trade, int index)
		{
			if (!activeTrades.Contains(trade.UID))
				return false;

			// check if trade has reached static sl or tp based on given ratio
			// return false if trade finished, otherwise true
			if (trade.Direction)
			{
				double sl = trade.Entry - trade.StopLoss;
				double tp = trade.Entry + (trade.StopLoss * ratio);

				// if buy trade and bar low is equal or below sl then it lost
				if (Data.Bars[index].Low <= sl)
				{
					FinishTrade(false, trade, sl);
					return false;
				}

				// if buy trade and bar high is equal or above tp then it won
				if (Data.Bars[index].High >= tp)
				{
					// If a buy trade "hits" tp on the entry bar, but the entry is below the bar's open, assume that it "hit" the tp before it was in the trade
					if (!trade.EnteredOnBar(Data.Bars[index]) || trade.Entry >= Data.Bars[index].Open)
					{
						FinishTrade(true, trade, tp);
						return false;
					}
				}
			}
			else
			{
				double sl = trade.Entry + trade.StopLoss;
				double tp = trade.Entry - (trade.StopLoss * ratio);

				// if sell trade and bar high is equal or above sl then it lost
				if (Data.Bars[index].High >= (sl - Data.Bars[index].Spread))
				{
					FinishTrade(false, trade, sl);
					return false;
				}

				// if sell trade and bar low is equal or below tp then it won
				if (Data.Bars[index].Low <= (tp - Data.Bars[index].Spread))
				{
					// If a sell trade "hits" tp on the entry bar, but the entry is above the bar's open, assume that it "hit" the tp before it was in the trade
					if (!trade.EnteredOnBar(Data.Bars[index]) || trade.Entry <= Data.Bars[index].Open)
					{
						FinishTrade(true, trade, tp);
						return false;
					}
				}
			}

			return true;
		}

		public override String GetDescription()
		{
			return string.Format("Static {0:0.00} reward ratio", ratio);
		}

		double ratio;
	}

	public class TimedIdeaManager : IdeaManager
	{
		public TimedIdeaManager(int b)
		{
			bars = b;
		}

		public override bool Manage(Trade trade, int index)
		{
			if (!activeTrades.Contains(trade.UID))
				return false;

			if (trade.Direction)
			{
				double sl = trade.Entry - trade.StopLoss;

				// if buy trade and bar low is equal or below sl then it lost
				if (Data.Bars[index].Low <= sl)
				{
					FinishTrade(false, trade, sl);
					return false;
				}

				if (index - trade.EntryBar.Index >= bars)
				{
					double exit = Data.Bars[index].Close;
					FinishTrade(exit > trade.Entry ? true : false, trade, exit);
					return false;
				}
			}
			else
			{
				double sl = trade.Entry + trade.StopLoss;

				// if sell trade and bar high is equal or above sl then it lost
				if (Data.Bars[index].High >= (sl - Data.Bars[index].Spread))
				{
					FinishTrade(false, trade, sl);
					return false;
				}

				if (index - trade.EntryBar.Index >= bars)
				{
					double exit = Data.Bars[index].Close;
					FinishTrade(exit < trade.Entry ? true : false, trade, exit);
					return false;
				}
			}

			return true;
		}

		public override String GetDescription()
		{
			return string.Format("Close after {0} bars", bars);
		}

		int bars;
	}

	public class TrailingIdeaManager : IdeaManager
	{
		public TrailingIdeaManager()
		{
			stops = new Dictionary<int, double>();
		}

		public override void AddTrade(Trade trade)
		{
			base.AddTrade(trade);
			stops[trade.UID] = trade.Direction ? trade.Entry - trade.StopLoss : trade.Entry + trade.StopLoss;
		}

		protected override void FinishTrade(bool win, Trade trade, double exit)
		{
			base.FinishTrade(win, trade, exit);
			stops.Remove(trade.UID);
		}

		public override bool Manage(Trade trade, int index)
		{
			if (!activeTrades.Contains(trade.UID))
				return false;

			if (!stops.ContainsKey(trade.UID))
				return false;

			double sl = stops[trade.UID];

			// check if trade has hit a trailing stop (sl distance away from close of previous bar)
			// return false if trade finished, otherwise true
			if (trade.Direction)
			{
				if (!trade.EnteredOnBar(Data.Bars[index]) && Data.Bars[index - 1].Close - trade.StopLoss > sl)
				{
					sl = Data.Bars[index - 1].Close - trade.StopLoss;
					stops[trade.UID] = sl;
				}

				// if buy trade and bar low is equal or below sl then it closes
				if (Data.Bars[index].Low <= sl)
				{
					// win or loss is determined by if sl is greater than entry or not
					FinishTrade(sl > trade.Entry, trade, sl);
					return false;
				}
			}
			else
			{
				if (!trade.EnteredOnBar(Data.Bars[index]) && Data.Bars[index - 1].Close + trade.StopLoss < sl)
				{
					sl = Data.Bars[index - 1].Close + trade.StopLoss;
					stops[trade.UID] = sl;
				}

				// if sell trade and bar high is equal or above sl then it closes
				if (Data.Bars[index].High >= (sl - Data.Bars[index].Spread))
				{
					// win or loss is determined by if sl is less than entry or not
					FinishTrade(sl < trade.Entry, trade, sl);
					return false;
				}
			}

			return true;
		}

		public override String GetDescription()
		{
			return string.Format("Trailing Static Stop");
		}

		private Dictionary<int, double> stops;
	}
}
