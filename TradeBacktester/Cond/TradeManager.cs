using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeBacktester.Cond
{
	public enum TradeManagerTypes : int
	{
		STATIC_0_5,
		STATIC_1_0,
		STATIC_1_5,
		STATIC_2_0,
		STATIC_2_5,
		STATIC_3_0,
		STATIC_3_5,
		STATIC_4_0,
		STATIC_4_5,
		STATIC_5_0,
		TRAILING_STATIC_STOP,
		TRAILING_STOP_PREV_EXTREME,
		EXIT_AFTER_BAR_COUNT,
		TOTAL,
	}

	public class TradeManagement
	{
		private const int EXIT_AFTER_BARS = 50;

		private Dictionary<int, double> trailingStaticStops;
		private Dictionary<int, double> trailingPrevExtremeStops;

		public TradeManagement()
		{
			trailingStaticStops = new Dictionary<int, double>();
			trailingPrevExtremeStops = new Dictionary<int, double>();
		}

		public bool Manage(Trade trade, int index)
		{
			bool ret = false;

			if (trade.IsBeingManaged(TradeManagerTypes.STATIC_0_5))
			{
				if (ManageStatic(0.5, trade, index))
					ret = true;
				else
					trade.SetBeingManaged(TradeManagerTypes.STATIC_0_5, false);
			}

			if (trade.IsBeingManaged(TradeManagerTypes.STATIC_1_0))
			{
				if (ManageStatic(1.0, trade, index))
					ret = true;
				else
					trade.SetBeingManaged(TradeManagerTypes.STATIC_1_0, false);
			}

			if (trade.IsBeingManaged(TradeManagerTypes.STATIC_1_5))
			{
				if (ManageStatic(1.5, trade, index))
					ret = true;
				else
					trade.SetBeingManaged(TradeManagerTypes.STATIC_1_5, false);
			}

			if (trade.IsBeingManaged(TradeManagerTypes.STATIC_2_0))
			{
				if (ManageStatic(2.0, trade, index))
					ret = true;
				else
					trade.SetBeingManaged(TradeManagerTypes.STATIC_2_0, false);
			}

			if (trade.IsBeingManaged(TradeManagerTypes.STATIC_2_5))
			{
				if (ManageStatic(2.5, trade, index))
					ret = true;
				else
					trade.SetBeingManaged(TradeManagerTypes.STATIC_2_5, false);
			}

			if (trade.IsBeingManaged(TradeManagerTypes.STATIC_3_0))
			{
				if (ManageStatic(3.0, trade, index))
					ret = true;
				else
					trade.SetBeingManaged(TradeManagerTypes.STATIC_3_0, false);
			}

			if (trade.IsBeingManaged(TradeManagerTypes.STATIC_3_5))
			{
				if (ManageStatic(3.5, trade, index))
					ret = true;
				else
					trade.SetBeingManaged(TradeManagerTypes.STATIC_3_5, false);
			}

			if (trade.IsBeingManaged(TradeManagerTypes.STATIC_4_0))
			{
				if (ManageStatic(4.0, trade, index))
					ret = true;
				else
					trade.SetBeingManaged(TradeManagerTypes.STATIC_4_0, false);
			}

			if (trade.IsBeingManaged(TradeManagerTypes.STATIC_4_5))
			{
				if (ManageStatic(4.5, trade, index))
					ret = true;
				else
					trade.SetBeingManaged(TradeManagerTypes.STATIC_4_5, false);
			}

			if (trade.IsBeingManaged(TradeManagerTypes.STATIC_5_0))
			{
				if (ManageStatic(5.0, trade, index))
					ret = true;
				else
					trade.SetBeingManaged(TradeManagerTypes.STATIC_5_0, false);
			}

			if (trade.IsBeingManaged(TradeManagerTypes.TRAILING_STATIC_STOP))
			{
				if (ManageTrailingStaticStop(trade, index))
					ret = true;
				else
					trade.SetBeingManaged(TradeManagerTypes.TRAILING_STATIC_STOP, false);
			}

			if (trade.IsBeingManaged(TradeManagerTypes.TRAILING_STOP_PREV_EXTREME))
			{
				if (ManageTrailingStopPrevExtreme(trade, index))
					ret = true;
				else
					trade.SetBeingManaged(TradeManagerTypes.TRAILING_STOP_PREV_EXTREME, false);
			}

			if (trade.IsBeingManaged(TradeManagerTypes.EXIT_AFTER_BAR_COUNT))
			{
				if (ManageExitAfterBarCount(trade, index))
					ret = true;
				else
					trade.SetBeingManaged(TradeManagerTypes.EXIT_AFTER_BAR_COUNT, false);
			}

			return ret;
		}

		private TradeManagerTypes GetStaticType(double ratio)
		{
			switch (ratio)
			{
				case 0.5:
					return TradeManagerTypes.STATIC_0_5;
				case 1.0:
					return TradeManagerTypes.STATIC_1_0;
				case 1.5:
					return TradeManagerTypes.STATIC_1_5;
				case 2.0:
					return TradeManagerTypes.STATIC_2_0;
				case 2.5:
					return TradeManagerTypes.STATIC_2_5;
				case 3.0:
					return TradeManagerTypes.STATIC_3_0;
				case 3.5:
					return TradeManagerTypes.STATIC_3_5;
				case 4.0:
					return TradeManagerTypes.STATIC_4_0;
				case 4.5:
					return TradeManagerTypes.STATIC_4_5;
				case 5.0:
					return TradeManagerTypes.STATIC_5_0;
			}

			return TradeManagerTypes.TOTAL;
		}

		private bool ManageStatic(double ratio, Trade trade, int index)
		{
			// check if trade has reached static sl or tp based on given ratio
			// return false if trade finished, otherwise true
			if (trade.Direction)
			{
				double sl = trade.Entry - trade.StopLoss;
				double tp = trade.Entry + (trade.StopLoss * ratio);

				// if buy trade and bar low is equal or below sl then it lost
				if (Data.Bars[index].Low <= sl)
				{
					Data.CondResults.AddResult(GetStaticType(ratio), false, trade, sl);
					return false;
				}

				// if buy trade and bar high is equal or above tp then it won
				if (Data.Bars[index].High >= tp)
				{
					// If a buy trade "hits" tp on the entry bar, but the entry is below the bar's open, assume that it "hit" the tp before it was in the trade
					if (!trade.EnteredOnBar(Data.Bars[index]) || trade.Entry >= Data.Bars[index].Open)
					{
						Data.CondResults.AddResult(GetStaticType(ratio), true, trade, tp);
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
					Data.CondResults.AddResult(GetStaticType(ratio), false, trade, sl);
					return false;
				}

				// if sell trade and bar low is equal or below tp then it won
				if (Data.Bars[index].Low <= (tp - Data.Bars[index].Spread))
				{
					// If a sell trade "hits" tp on the entry bar, but the entry is above the bar's open, assume that it "hit" the tp before it was in the trade
					if (!trade.EnteredOnBar(Data.Bars[index]) || trade.Entry <= Data.Bars[index].Open)
					{
						Data.CondResults.AddResult(GetStaticType(ratio), true, trade, tp);
						return false;
					}
				}
			}

			return true;
		}

		private bool ManageTrailingStaticStop(Trade trade, int index)
		{
			double sl = 0.0;
			if (!trailingStaticStops.TryGetValue(trade.UID, out sl))
			{
				sl = trade.Direction ? trade.Entry - trade.StopLoss : trade.Entry + trade.StopLoss;
				trailingStaticStops[trade.UID] = sl;
			}

			// check if trade has hit a trailing stop (sl distance away from close of previous bar)
			// return false if trade finished, otherwise true
			if (trade.Direction)
			{
				if (!trade.EnteredOnBar(Data.Bars[index]) && Data.Bars[index - 1].Close - trade.StopLoss > sl)
				{
					sl = Data.Bars[index - 1].Close - trade.StopLoss;
					trailingStaticStops[trade.UID] = sl;
				}

				// if buy trade and bar low is equal or below sl then it closes
				if (Data.Bars[index].Low <= sl)
				{
					// win or loss is determined by if sl is greater than entry or not
					Data.CondResults.AddResult(TradeManagerTypes.TRAILING_STATIC_STOP, sl > trade.Entry, trade, sl);
					trailingStaticStops.Remove(trade.UID);
					return false;
				}
			}
			else
			{
				if (!trade.EnteredOnBar(Data.Bars[index]) && Data.Bars[index - 1].Close + trade.StopLoss < sl)
				{
					sl = Data.Bars[index - 1].Close + trade.StopLoss;
					trailingStaticStops[trade.UID] = sl;
				}

				// if sell trade and bar high is equal or above sl then it closes
				if (Data.Bars[index].High >= (sl - Data.Bars[index].Spread))
				{
					// win or loss is determined by if sl is less than entry or not
					Data.CondResults.AddResult(TradeManagerTypes.TRAILING_STATIC_STOP, sl < trade.Entry, trade, sl);
					trailingStaticStops.Remove(trade.UID);
					return false;
				}
			}

			return true;
		}

		private bool ManageTrailingStopPrevExtreme(Trade trade, int index)
		{
			double sl = 0.0;
			if (!trailingPrevExtremeStops.TryGetValue(trade.UID, out sl))
			{
				sl = trade.Direction ? trade.Entry - trade.StopLoss : trade.Entry + trade.StopLoss;
				trailingPrevExtremeStops[trade.UID] = sl;
			}

			// check if trade has hit a trailing stop (sl distance away from close of previous bar)
			// return false if trade finished, otherwise true
			if (trade.Direction)
			{
				if (!trade.EnteredOnBar(Data.Bars[index]) && Data.Bars[index - 1].Low - Constants.ENTRY_BUFFER > sl)
				{
					sl = Data.Bars[index - 1].Low - Constants.ENTRY_BUFFER;
					trailingPrevExtremeStops[trade.UID] = sl;
				}

				// if buy trade and bar low is equal or below sl then it closes
				if (Data.Bars[index].Low <= sl)
				{
					// win or loss is determined by if sl is greater than entry or not
					Data.CondResults.AddResult(TradeManagerTypes.TRAILING_STOP_PREV_EXTREME, sl > trade.Entry, trade, sl);
					trailingPrevExtremeStops.Remove(trade.UID);
					return false;
				}
			}
			else
			{
				if (!trade.EnteredOnBar(Data.Bars[index]) && Data.Bars[index - 1].High + Data.Bars[index].Spread + Constants.ENTRY_BUFFER < sl)
				{
					sl = Data.Bars[index - 1].High + Data.Bars[index].Spread + Constants.ENTRY_BUFFER;
					trailingPrevExtremeStops[trade.UID] = sl;
				}

				// if sell trade and bar high is equal or above sl then it closes
				if (Data.Bars[index].High >= (sl - Data.Bars[index].Spread))
				{
					// win or loss is determined by if sl is less than entry or not
					Data.CondResults.AddResult(TradeManagerTypes.TRAILING_STOP_PREV_EXTREME, sl < trade.Entry, trade, sl);
					trailingPrevExtremeStops.Remove(trade.UID);
					return false;
				}
			}

			return true;
		}

		private bool ManageExitAfterBarCount(Trade trade, int index)
		{
			// return false if trade finished, otherwise true
			if (trade.Direction)
			{
				double sl = trade.Entry - trade.StopLoss;

				// if buy trade and bar low is equal or below sl then it lost
				if (Data.Bars[index].Low <= sl)
				{
					Data.CondResults.AddResult(TradeManagerTypes.EXIT_AFTER_BAR_COUNT, false, trade, sl);
					return false;
				}

				if (index - trade.EntryBar.Index >= EXIT_AFTER_BARS)
				{
					double exit = Data.Bars[index].Close;
					Data.CondResults.AddResult(TradeManagerTypes.EXIT_AFTER_BAR_COUNT, exit > trade.Entry, trade, Data.Bars[index].Close);
					return false;
				}
			}
			else
			{
				double sl = trade.Entry + trade.StopLoss;

				// if sell trade and bar high is equal or above sl then it lost
				if (Data.Bars[index].High >= (sl - Data.Bars[index].Spread))
				{
					Data.CondResults.AddResult(TradeManagerTypes.EXIT_AFTER_BAR_COUNT, false, trade, sl);
					return false;
				}

				if (index - trade.EntryBar.Index >= EXIT_AFTER_BARS)
				{
					double exit = Data.Bars[index].Close;
					Data.CondResults.AddResult(TradeManagerTypes.EXIT_AFTER_BAR_COUNT, exit < trade.Entry, trade, Data.Bars[index].Close);
					return false;
				}
			}

			return true;
		}
	}
}
