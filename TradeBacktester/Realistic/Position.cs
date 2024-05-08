using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeBacktester.Realistic
{
	public class Position
	{
		public Position(int entryTime, bool dir, double e, double l, double s, double t)
		{
			entryIndex = entryTime;
			direction = dir;
			entry = e;
			sl = s;
			tp = t;
			lots = l;
		}

		public int EntryIndex
		{
			get => entryIndex;
		}

		public bool Direction
		{
			get => direction;
		}

		public double Entry
		{
			get => entry;
		}

		public double SLSize
		{
			get => sl;
		}

		public double SLPrice
		{
			get => entry + (direction ? -sl : sl);
		}

		public bool IsStoppedOut(int index)
		{
			if (direction)
				return Data.Bars[index].Low <= SLPrice;
			return Data.Bars[index].High >= (SLPrice - Data.Bars[index - 1].Spread);
		}

		public double TP
		{
			get => tp;
		}

		public double Lots
		{
			get => lots;
		}

		private int entryIndex;
		private bool direction;
		private double entry;
		private double sl;
		private double tp;
		private double lots;
	}
}
