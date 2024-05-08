using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeBacktester.Cond;

namespace TradeBacktester.Quant
{
	public class Signal
	{
		public Signal(ConditionMask mask, double price, bool dir)
		{
			signalMask = mask;
			signalPrice = price;
			direction = dir;
			maxAdverse = price;
			maxFavorable = price;
		}

		public virtual bool Update(int index)
		{
			return false;
		}

		protected ConditionMask signalMask;
		protected double signalPrice;
		protected bool direction;
		protected double maxAdverse;
		protected double maxFavorable;
	}

	public class BarByBarSignalMultiResult : MultiResult
	{
		public const int NUM_BARS_TO_COLLECT_DATA = 50;

		public BarByBarSignalMultiResult()
		{
			remainingBars = NUM_BARS_TO_COLLECT_DATA;
			numDataPoints = new int[NUM_BARS_TO_COLLECT_DATA];
			totalAdverse = new double[NUM_BARS_TO_COLLECT_DATA];
			totalFavorable = new double[NUM_BARS_TO_COLLECT_DATA];
		}

		public void AddData(double fav, double adv)
		{
			++numDataPoints[NUM_BARS_TO_COLLECT_DATA - remainingBars];
			totalAdverse[NUM_BARS_TO_COLLECT_DATA - remainingBars] += adv;
			totalFavorable[NUM_BARS_TO_COLLECT_DATA - remainingBars] += fav;
			--remainingBars;
		}

		public override void Update(MultiResult res)
		{
			BarByBarSignalMultiResult mr = (BarByBarSignalMultiResult)res;
			for (int i = 0; i < NUM_BARS_TO_COLLECT_DATA; ++i)
			{
				numDataPoints[i] += mr.numDataPoints[i];
				totalFavorable[i] += mr.totalFavorable[i];
				totalAdverse[i] += mr.totalAdverse[i];
			}
		}

		public override bool IsDone()
		{
			return remainingBars == 0;
		}

		public override double Print(ConditionMask mask, bool expanded)
		{
			if (!expanded && numDataPoints[0] < Constants.MIN_SIGNALS)
				return 0.0;

			double highest = 0.0;
			string output = "";

			output += string.Format("{0}", mask.ToString()) + Environment.NewLine;
			output += string.Format("Number of signals: {0}", numDataPoints[0]) + Environment.NewLine;

			for (int i = 0; i < NUM_BARS_TO_COLLECT_DATA; ++i)
			{
				double ratio = totalFavorable[i] / totalAdverse[i];
				if (i > 0 && ratio > highest)
					highest = ratio;
				if (expanded)
					output += string.Format("Bar {0}: {1:F2}", i + 1, ratio) + Environment.NewLine;
			}

			output += string.Format("Highest: {0:F2}", highest) + Environment.NewLine;

			if (highest > Constants.MIN_RATIO || expanded)
			{
				Console.Write(output);
				return highest;
			}

			return 0.0;
		}

		private int remainingBars;
		private int[] numDataPoints;
		private double[] totalFavorable;
		private double[] totalAdverse;
	}

	public class BarByBarSignal : Signal
	{
		public BarByBarSignal(ConditionMask mask, double price, bool dir) : base(mask, price, dir)
		{
			result = new BarByBarSignalMultiResult();
		}

		public override bool Update(int index)
		{
			double favAmt = 0.0;
			double advAmt = 0.0;

			if (direction)
			{
				if (Data.Bars[index].High > maxFavorable)
					maxFavorable = Data.Bars[index].High;
				if (Data.Bars[index].Low < maxAdverse)
					maxAdverse = Data.Bars[index].Low;

				favAmt = maxFavorable > signalPrice ? (maxFavorable - signalPrice) / signalPrice : 0.0;
				advAmt = signalPrice > maxAdverse ? (signalPrice - maxAdverse) / signalPrice : 0.0;

				result.AddData(favAmt, advAmt);
			}
			else
			{
				if (Data.Bars[index].High > maxAdverse)
					maxAdverse = Data.Bars[index].High;
				if (Data.Bars[index].Low < maxFavorable)
					maxFavorable = Data.Bars[index].Low;

				favAmt = signalPrice > maxFavorable ? (signalPrice - maxFavorable) / signalPrice : 0.0;
				advAmt = maxAdverse > signalPrice ? (maxAdverse - signalPrice) / signalPrice : 0.0;

				result.AddData(favAmt, advAmt);
			}

			// we're done with this signal
			if (result.IsDone())
			{
				Data.MultiResults.AddResult(signalMask, result);
				return true;
			}

			return false;
		}

		private BarByBarSignalMultiResult result;
	}
}
