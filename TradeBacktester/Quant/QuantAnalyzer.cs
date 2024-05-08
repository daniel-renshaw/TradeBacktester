using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeBacktester.Cond;

namespace TradeBacktester.Quant
{
	public class QuantAnalyzer
	{
		public QuantAnalyzer()
		{
			signals = new List<Signal>();
		}

		public void HandleIndex(int index)
		{
			// Save 2019-2021 for out of sample tests
			if (Data.Bars[index].Year > 2018 || Data.Bars[index].Year == 2011)
				return;

			// Only care about 4-11am
			if (!Conditions.IsBetweenHours(index, 11, 18))
				return;

			TradeTypes type = TradeTypes.SellBelow;
			double price = GetSignalPriceByTradeType(type, index);
			if (double.IsNaN(price))
				return;

			ConditionMask signalMask = Conditions.GetConditionBitMask(index);
			if (signalMask.HasFlag(Data.MultiResults.FilterMask))
				signals.Add(new BarByBarSignal(signalMask, price, type < TradeTypes.SellAbove));

			UpdateSignals(index);
		}

		private double GetSignalPriceByTradeType(TradeTypes type, int index)
		{
			switch (type)
			{
				case TradeTypes.BuyAbove:
					if (Data.Bars[index].High > Data.Bars[index - 1].High)
						return Data.Bars[index - 1].High + Constants.POINT;
					return double.NaN;
				case TradeTypes.BuyBelow:
					if (Data.Bars[index].Low < Data.Bars[index - 1].Low)
						return Data.Bars[index - 1].Low - Constants.POINT;
					return double.NaN;
				case TradeTypes.BuyClose:
					return Data.Bars[index - 1].Close;
				case TradeTypes.SellAbove:
					if (Data.Bars[index].High > Data.Bars[index - 1].High)
						return Data.Bars[index - 1].High + Constants.POINT;
					return double.NaN;
				case TradeTypes.SellBelow:
					if (Data.Bars[index].Low < Data.Bars[index - 1].Low)
						return Data.Bars[index - 1].Low - Constants.POINT;
					return double.NaN;
				case TradeTypes.SellClose:
					return Data.Bars[index - 1].Close;
			}

			return double.NaN;
		}

		private void UpdateSignals(int index)
		{
			for (int i = 0; i < signals.Count; ++i)
			{
				Signal s = signals[i];
				if (s.Update(index))
					signals.RemoveAt(i--);
			}
		}

		public void PrintResults()
		{
			Data.MultiResults.PrintResults();
		}

		private List<Signal> signals;
	}
}
