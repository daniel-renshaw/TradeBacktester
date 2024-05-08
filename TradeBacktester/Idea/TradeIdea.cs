using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TradeBacktester.Cond;

namespace TradeBacktester.Idea
{
	public abstract class TradeIdea
	{
		public TradeIdea()
		{
			tradesTaken = 0;
			activeTrades = new List<Trade>();

			mgrs = new IdeaManager[]
			{
				new StaticIdeaManager(0.5),
				new StaticIdeaManager(1.0),
				new StaticIdeaManager(1.5),
				new StaticIdeaManager(2.0),
				new StaticIdeaManager(2.5),
				new StaticIdeaManager(3.0),
				new StaticIdeaManager(3.5),
				new StaticIdeaManager(4.0),
				new StaticIdeaManager(4.5),
				new StaticIdeaManager(5.0),
				new TrailingIdeaManager(),
				new TimedIdeaManager(50),
			};
		}

		// this is for global exclusions that apply to all ideas
		public virtual bool CheckForEntry(int index)
		{
			return true;
		}

		public virtual void Print()
		{
			foreach (IdeaManager mgr in mgrs)
			{
				mgr.Print();
			}
		}

		public void EnterTrade(bool dir, double entry, double sl, int entryIndex)
		{
			Trade trade = new Trade(++tradesTaken, dir, entry, sl, entryIndex);
			activeTrades.Add(trade);
			foreach (IdeaManager mgr in mgrs)
			{
				mgr.AddTrade(trade);
			}
		}

		public void ManageTrades(int index)
		{
			for (int i = 0; i < activeTrades.Count; ++i)
			{
				Trade trade = activeTrades[i];
				bool active = false;

				foreach (IdeaManager mgr in mgrs)
				{
					if (mgr.Manage(trade, index))
						active = true;
				}

				if (!active)
					activeTrades.RemoveAt(i--);
			}
		}

		IdeaManager[] mgrs;
		List<Trade> activeTrades;
		int tradesTaken;
	}

	public class TradeIdea_Final : TradeIdea
	{
		public TradeIdea_Final(int y)
		{
			year = y;
		}

		public override bool CheckForEntry(int index)
		{
			if (!base.CheckForEntry(index))
				return false;

			if (year != 0 && !Conditions.IsInYear(index, year))
				return false;

			bool ret = false;
			double entry = Data.Bars[index - 1].High + Data.Bars[index - 1].Spread + Constants.ENTRY_BUFFER;
			if (Data.Bars[index].High >= (entry - Data.Bars[index - 1].Spread) && BuyAboveEntryConditions(index))
			{
				double sl = (Constants.ATR_BASED_SL ? Data.ATR[20] : entry - Data.Bars[index - 1].Low) + Data.Bars[index - 1].Spread + Constants.ENTRY_BUFFER;
				sl *= Constants.SL_MULTI_BUY_ABOVE;
				EnterTrade(true, entry, sl, index);
				ret = true;
			}

			entry = Data.Bars[index - 1].Close;
			if (BuyCloseEntryConditions(index))
			{
				double sl = (Constants.ATR_BASED_SL ? Data.ATR[20] : Data.Bars[index - 1].GetRange()) + Data.Bars[index - 1].Spread + Constants.ENTRY_BUFFER;
				sl *= Constants.SL_MULTI_BUY_CLOSE;
				EnterTrade(true, entry, sl, index);
				ret = true;
			}

			entry = Data.Bars[index - 1].Low - Constants.ENTRY_BUFFER;
			if (Data.Bars[index].Low <= entry && SellBelowEntryConditions(index))
			{
				double sl = (Constants.ATR_BASED_SL ? Data.ATR[20] : Data.Bars[index - 1].High - entry) + Data.Bars[index - 1].Spread + Constants.ENTRY_BUFFER;
				sl *= Constants.SL_MULTI_SELL_BELOW;
				EnterTrade(false, entry, sl, index);
				ret = true;
			}

			entry = Data.Bars[index - 1].Close;
			if (SellCloseEntryConditions(index))
			{
				double sl = (Constants.ATR_BASED_SL ? Data.ATR[20] : Data.Bars[index - 1].GetRange()) + Data.Bars[index - 1].Spread + Constants.ENTRY_BUFFER;
				sl *= Constants.SL_MULTI_SELL_CLOSE;
				EnterTrade(false, entry, sl, index);
				ret = true;
			}

			return ret;
		}

		private bool BuyAboveEntryConditions(int index)
		{
			return
				false;
		}

		private bool BuyCloseEntryConditions(int index)
		{
			return
				false;
		}

		private bool SellBelowEntryConditions(int index)
		{
			return
				false;
		}

		private bool SellCloseEntryConditions(int index)
		{
			return
				true;
		}

		public override void Print()
		{
			Console.WriteLine(string.Format("Final {0}:", year == 0 ? "All" : year.ToString()));
			base.Print();
		}

		private int year;
	}
}
