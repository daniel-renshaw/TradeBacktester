using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeBacktester.Cond;

namespace TradeBacktester
{
	public class Trades
	{
		public Trades()
		{
			trades = new List<Trade>();
			tradesTaken = 0;
		}

		private void EnterTrade(bool dir, double entry, double sl, int entryIndex)
		{
			Trade trade = new Trade(++tradesTaken, dir, entry, sl, entryIndex);
			trades.Add(trade);
		}

		public void CheckForEntry(int index)
		{
			if (Conditions.IsOnDayOfWeek(index, DayOfWeek.Sunday))
				return;

			if (!Conditions.IsBetweenHours(index, 11, 18))
				return;

			TradeTypes type = TradeTypes.SellClose;
			CheckForEntryForType(type, index);
		}

		private void CheckForEntryForType(TradeTypes type, int index)
		{
			switch (type)
			{
				case TradeTypes.BuyAbove:
					CheckForBuyAboveEntry(index);
					break;
				case TradeTypes.BuyBelow:
					CheckForBuyBelowEntry(index);
					break;
				case TradeTypes.BuyClose:
					CheckForBuyCloseEntry(index);
					break;
				case TradeTypes.SellAbove:
					CheckForSellAboveEntry(index);
					break;
				case TradeTypes.SellBelow:
					CheckForSellBelowEntry(index);
					break;
				case TradeTypes.SellClose:
					CheckForSellCloseEntry(index);
					break;
			}
		}

		private void CheckForBuyAboveEntry(int index)
		{
			double entry = Data.Bars[index - 1].High + Data.Bars[index - 1].Spread + Constants.ENTRY_BUFFER;
			if (Data.Bars[index].High >= (entry - Data.Bars[index - 1].Spread))
			{
				double sl = (Constants.ATR_BASED_SL ? Data.ATR[20] : entry - Data.Bars[index - 1].Low) + Data.Bars[index - 1].Spread + Constants.ENTRY_BUFFER;
				sl *= Constants.SL_MULTI_BUY_ABOVE;
				EnterTrade(true, entry, sl, index);
			}
		}

		private void CheckForBuyBelowEntry(int index)
		{
			double entry = Data.Bars[index - 1].Low - Constants.ENTRY_BUFFER;
			if (Data.Bars[index].Low <= (entry - Data.Bars[index - 1].Spread))
			{
				double sl = (Constants.ATR_BASED_SL ? Data.ATR[20] : Data.Bars[index - 1].High - entry) + Data.Bars[index - 1].Spread + Constants.ENTRY_BUFFER;
				sl *= Constants.SL_MULTI_BUY_BELOW;
				EnterTrade(true, entry, sl, index);
			}
		}

		private void CheckForBuyCloseEntry(int index)
		{
			double entry = Data.Bars[index - 1].Close;
			double sl = (Constants.ATR_BASED_SL ? Data.ATR[20] : Data.Bars[index - 1].GetRange()) + Data.Bars[index - 1].Spread + Constants.ENTRY_BUFFER;
			sl *= Constants.SL_MULTI_BUY_CLOSE;
			EnterTrade(true, entry, sl, index);
		}

		private void CheckForSellAboveEntry(int index)
		{
			double entry = Data.Bars[index - 1].High + Constants.ENTRY_BUFFER;
			if (Data.Bars[index].High >= entry)
			{
				double sl = (Constants.ATR_BASED_SL ? Data.ATR[20] : entry - Data.Bars[index - 1].Low) + Data.Bars[index - 1].Spread + Constants.ENTRY_BUFFER;
				sl *= Constants.SL_MULTI_SELL_ABOVE;
				EnterTrade(false, entry, sl, index);
			}
		}

		private void CheckForSellBelowEntry(int index)
		{
			double entry = Data.Bars[index - 1].Low - Constants.ENTRY_BUFFER;
			if (Data.Bars[index].Low <= entry)
			{
				double sl = (Constants.ATR_BASED_SL ? Data.ATR[20] : Data.Bars[index - 1].High - entry) + Data.Bars[index - 1].Spread + Constants.ENTRY_BUFFER;
				sl *= Constants.SL_MULTI_SELL_BELOW;
				EnterTrade(false, entry, sl, index);
			}
		}

		private void CheckForSellCloseEntry(int index)
		{
			double entry = Data.Bars[index - 1].Close;
			double sl = (Constants.ATR_BASED_SL ? Data.ATR[20] : Data.Bars[index - 1].GetRange()) + Data.Bars[index - 1].Spread + Constants.ENTRY_BUFFER;
			sl *= Constants.SL_MULTI_SELL_CLOSE;
			EnterTrade(false, entry, sl, index);
		}

		public void ManageTrades(int index)
		{
			for (int i = 0; i < trades.Count; ++i)
			{
				Trade trade = trades[i];
				if (!Data.TradeManagement.Manage(trade, index))
					trades.RemoveAt(i--);
			}
		}

		private List<Trade> trades;
		private int tradesTaken;
	}

	public class Trade
	{
		public Trade(int id, bool dir, double e, double s, int entryIndex)
		{
			uid = id;
			direction = dir;
			entry = e;
			sl = s;
			riskAmt = Constants.ACCOUNT_SIZE * Constants.RISK_PERCENT;
			lots = riskAmt / (sl / Constants.POINT);
			entryBar = Data.Bars[entryIndex];
			entryMask = Conditions.GetConditionBitMask(entryIndex);
			mgrs = new bool[(int)TradeManagerTypes.TOTAL];
			for (int i = 0; i < (int)TradeManagerTypes.TOTAL; ++i)
			{
				mgrs[i] = true;
			}
		}

		public double CalculatePL(double exit)
		{
			return ((exit - entry) * lots * 100000.0 * (direction ? 1.0 : -1.0)) - (lots * Constants.COMMISSION_PER_LOT);
		}

		public bool EnteredOnBar(Bar bar)
		{
			return bar.Year == entryBar.Year && bar.Month == entryBar.Month && bar.Day == entryBar.Day && bar.Hour == entryBar.Hour && bar.Minute == entryBar.Minute;
		}

		public int UID
		{
			get => uid;
		}

		public bool Direction
		{
			get => direction;
		}

		public double Entry
		{
			get => entry;
		}

		public double StopLoss
		{
			get => sl;
		}

		public ConditionMask EntryMask
		{
			get => entryMask;
			set => entryMask = value;
		}

		public bool IsBeingManaged(TradeManagerTypes type)
		{
			return mgrs[(int)type];
		}

		public void SetBeingManaged(TradeManagerTypes type, bool val)
		{
			mgrs[(int)type] = val;
		}

		public Bar EntryBar
		{
			get => entryBar;
		}

		private int uid;
		private bool direction;
		private Bar entryBar;
		private double lots;
		private double riskAmt;
		private double entry;
		private double sl;
		private ConditionMask entryMask;
		private bool[] mgrs;
	}
}
