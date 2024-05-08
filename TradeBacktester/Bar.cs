using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TradeBacktester
{
	public class Bars
	{
		public Bars()
		{
			bars = new List<Bar>();
		}

		public Bar this[int i]
		{
			get { return bars[i]; }
			set { bars[i] = value; }
		}

		public void Load(string filepath)
		{
			if (!File.Exists(filepath))
				return;

			using (StreamReader sr = File.OpenText(filepath))
			{
				int index = 0;
				string s = String.Empty;
				sr.ReadLine(); // dump the first line because it isn't data
				while ((s = sr.ReadLine()) != null)
				{
					Add(new Bar(index++, s));
				}
			}
		}

		public void Add(Bar bar)
		{
			bars.Add(bar);
		}

		public int Count
		{
			get { return bars.Count; }
		}

		public double GetHighForPeriodFromIndex(int period, int index)
		{
			double highest = 0.0;
			for (int i = index; i > index - period; --i)
			{
				if (i < 0)
					break;
				if (bars[i].High > highest)
					highest = bars[i].High;
			}

			return highest;
		}

		public double GetLowForPeriodFromIndex(int period, int index)
		{
			double lowest = 9999999.9;
			for (int i = index; i > index - period; --i)
			{
				if (i < 0)
					break;
				if (bars[i].Low < lowest)
					lowest = bars[i].Low;
			}

			return lowest;
		}

		private List<Bar> bars;
	}

	public class Bar
	{
		public Bar(int i, int numPrevBars)
		{
			index = i;

			year = Data.Bars[Data.Bars.Count - 1].year;
			month = Data.Bars[Data.Bars.Count - 1].month;
			day = Data.Bars[Data.Bars.Count - 1].day;
			weekDay = Data.Bars[Data.Bars.Count - 1].weekDay;
			hour = Data.Bars[Data.Bars.Count - 1].hour;
			minute = Data.Bars[Data.Bars.Count - numPrevBars].minute;

			open = Data.Bars[Data.Bars.Count - numPrevBars].open;
			close = Data.Bars[Data.Bars.Count - 1].close;
			high = Data.Bars[Data.Bars.Count - 1].high;
			low = Data.Bars[Data.Bars.Count - 1].low;
			tickVol = Data.Bars[Data.Bars.Count - 1].tickVol;
			spread = Data.Bars[Data.Bars.Count - 1].spread;

			for (int j = Data.Bars.Count - 2; j > Data.Bars.Count - numPrevBars - 1; --j)
			{
				if (Data.Bars[j].high > high)
					high = Data.Bars[j].high;
				if (Data.Bars[j].low < low)
					low = Data.Bars[j].low;
				tickVol += Data.Bars[j].tickVol;
			}
		}

		public Bar(int i, string str)
		{
			index = i;
			Parse(str);
		}

		public void Parse(string str)
		{
			string[] split = str.Split('\t');
			string[] date = split[0].Split('.');
			int y = Convert.ToInt32(date[0]);
			int m = Convert.ToInt32(date[1]);
			int d = Convert.ToInt32(date[2]);
			string[] time = split[1].Split(':');
			int h = Convert.ToInt32(time[0]);
			int min = Convert.ToInt32(time[1]);

			DateTime dt = new DateTime(y, m, d, h, min, 0);
			year = dt.Year;
			month = dt.Month;
			day = d;
			weekDay = dt.DayOfWeek;
			hour = dt.Hour;
			minute = dt.Minute;
			open = Convert.ToDouble(split[2]);
			high = Convert.ToDouble(split[3]);
			low = Convert.ToDouble(split[4]);
			close = Convert.ToDouble(split[5]);
			tickVol = Convert.ToInt32(split[6]);
			spread = Convert.ToDouble(split[8]) * Constants.POINT;
			//if (spread < Constants.MIN_SPREAD)
				spread = Constants.MIN_SPREAD;
		}

		public bool IsBullish()
		{
			if (open <= close)
				return true;
			return false;
		}

		public bool IsBearish()
		{
			if (open > close)
				return true;
			return false;
		}

		public bool HasTopTailPercentInRange(double min, double max)
		{
			double fullSize = high - low;
			double topTailSize = high;
			if (open > close)
				topTailSize -= open;
			else
				topTailSize -= close;
			double topTailPerc = topTailSize / fullSize;
			if (topTailPerc >= min && topTailPerc <= max)
				return true;
			return false;
		}

		public bool HasBottomTailPercentInRange(double min, double max)
		{
			double fullSize = high - low;
			double bottomTailSize = open > close ? close : open;
			bottomTailSize -= low;

			double bottomTailPerc = bottomTailSize / fullSize;
			if (bottomTailPerc >= min && bottomTailPerc <= max)
				return true;
			return false;
		}

		public bool CloseOnOrNearHigh()
		{
			if (close < open)
				return false;

			double buffer = 0.1;
			double fullSize = high - low;
			double distFromHigh = high - close;

			return (distFromHigh / fullSize) <= buffer;
		}

		public bool CloseOnOrNearLow()
		{
			if (open < close)
				return false;

			double buffer = 0.1;
			double fullSize = high - low;
			double distFromLow = close - low;

			return (distFromLow / fullSize) <= buffer;
		}

		public double GetRange()
		{
			return high - low;
		}

		public bool IsWideRange(double atr)
		{
			if (GetRange() > (atr * 2.0))
				return true;
			return false;
		}

		public bool IsNarrowRange(double atr)
		{
			if (GetRange() < (atr * 0.5))
				return true;
			return false;
		}

		public bool IsMostlyBody()
		{
			double range = high - low;
			double body = open > close ? open - close : close - open;
			if (body / range >= 0.8)
				return true;
			return false;
		}

		public bool IsMostlyWick()
		{
			double range = high - low;
			double body = open > close ? open - close : close - open;
			if (body / range <= 0.2)
				return true;
			return false;
		}

		public double GetOverlapPercent(Bar bar)
		{
			double overlap = (high > bar.High ? bar.High : high) - (low > bar.Low ? low : bar.Low);
			if (overlap < 0.0)
				return 0.0;

			return overlap / GetRange();
		}

		public bool CrossesMA(double ma)
		{
			return high > ma && low < ma;
		}

		public bool IsInside(Bar bar)
		{
			return high < bar.High && low > bar.Low;
		}

		public bool IsOutside(Bar bar)
		{
			return high > bar.High && low < bar.Low;
		}

		public double GetTVB()
		{
			return (3 * close) - (low + open + high);
		}

		public int Year
		{
			get => year;
			set => year = value;
		}

		public int Month
		{
			get => month;
			set => month = value;
		}

		public int Day
		{
			get => day;
			set => day = value;
		}

		public DayOfWeek WeekDay
		{
			get => weekDay;
			set => weekDay = value;
		}

		public int Hour
		{
			get => hour;
			set => hour = value;
		}

		public int Minute
		{
			get => minute;
			set => minute = value;
		}

		public int TickVol
		{
			get => tickVol;
			set => tickVol = value;
		}

		public double High
		{
			get => high;
			set => high = value;
		}

		public double Low
		{
			get => low;
			set => low = value;
		}

		public double Open
		{
			get => open;
			set => open = value;
		}

		public double Close
		{
			get => close;
			set => close = value;
		}

		public double Spread
		{
			get => spread;
			set => spread = value;
		}

		public int Index
		{
			get => index;
			set => index = value;
		}

		private int index;
		private int year;
		private int month;
		private int day;
		private DayOfWeek weekDay;
		private int hour;
		private int minute;
		private int tickVol;
		private double high;
		private double low;
		private double open;
		private double close;
		private double spread;
	}
}
