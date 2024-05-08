using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * have every trade have bitmask of conditions
 * log wins/losses for every trade/management combo in hashmap with bitmask as key
 * after all bars iterated, iterate through hashmap and divy up wins/losses to every combination that the bitmask matches (so if 11100, then 10000, 11000, 01000, etc)
 * generate win percentages and compare to baseline, ditching any below the baseline
 * print results
 */

namespace TradeBacktester.Cond
{
	[Flags]
	public enum ConditionMask : ulong
	{
		NONE								= 0x0,
		START								= 0x1,
		PutCondition1Here					= 0x1,
		PutCondition2Here					= 0x2,
		PutCondition3Here					= 0x4,
		PutCondition4Here					= 0x8,
		PutCondition5Here					= 0x10,
		PutCondition6Here					= 0x20,
		PutCondition7Here					= 0x40,
		PutCondition8Here					= 0x80,
		PutCondition9Here					= 0x100,
		PutCondition10Here					= 0x200,
		PutCondition11Here					= 0x400,
		PutCondition12Here					= 0x800,
		PutCondition13Here					= 0x1000,
		PutCondition14Here					= 0x2000,
		PutCondition15Here					= 0x4000,
		PutCondition16Here					= 0x8000,
		PutCondition17Here					= 0x10000,
		PutCondition18Here					= 0x20000,
		PutCondition19Here					= 0x40000,
		PutCondition20Here					= 0x80000,
		END									= 0x100000,
	}

	public static class Conditions
	{
		public static bool CheckForConditionSkip(ref ConditionMask i, ConditionMask key)
		{
			for (ConditionMask iter = ConditionMask.START; iter < ConditionMask.END; iter += (ulong)iter)
			{
				if (i.HasFlag(iter) && !key.HasFlag(i))
				{
					i += (ulong)iter - 1;
					return true;
				}
			}

			return false;
		}

		public static ConditionMask GetConditionBitMask(int index)
		{
			ConditionMask bitmask = 0;

			if (IsCondition1Present(index))
				bitmask |= ConditionMask.PutCondition1Here;
			if (IsCondition2Present(index))
				bitmask |= ConditionMask.PutCondition2Here;
			if (IsCondition3Present(index))
				bitmask |= ConditionMask.PutCondition3Here;
			if (IsCondition4Present(index))
				bitmask |= ConditionMask.PutCondition4Here;
			if (IsCondition5Present(index))
				bitmask |= ConditionMask.PutCondition5Here;
			if (IsCondition6Present(index))
				bitmask |= ConditionMask.PutCondition6Here;
			if (IsCondition7Present(index))
				bitmask |= ConditionMask.PutCondition7Here;
			if (IsCondition8Present(index))
				bitmask |= ConditionMask.PutCondition8Here;
			if (IsCondition9Present(index))
				bitmask |= ConditionMask.PutCondition9Here;
			if (IsCondition10Present(index))
				bitmask |= ConditionMask.PutCondition10Here;
			if (IsCondition11Present(index))
				bitmask |= ConditionMask.PutCondition11Here;
			if (IsCondition12Present(index))
				bitmask |= ConditionMask.PutCondition12Here;
			if (IsCondition13Present(index))
				bitmask |= ConditionMask.PutCondition13Here;
			if (IsCondition14Present(index))
				bitmask |= ConditionMask.PutCondition14Here;
			if (IsCondition15Present(index))
				bitmask |= ConditionMask.PutCondition15Here;
			if (IsCondition16Present(index))
				bitmask |= ConditionMask.PutCondition16Here;
			if (IsCondition17Present(index))
				bitmask |= ConditionMask.PutCondition17Here;
			if (IsCondition18Present(index))
				bitmask |= ConditionMask.PutCondition18Here;
			if (IsCondition19Present(index))
				bitmask |= ConditionMask.PutCondition19Here;
			if (IsCondition20Present(index))
				bitmask |= ConditionMask.PutCondition20Here;

			return bitmask;
		}

		public static bool IsCondition1Present(int index)
		{
			return true;
		}

		public static bool IsCondition2Present(int index)
		{
			return true;
		}

		public static bool IsCondition3Present(int index)
		{
			return true;
		}

		public static bool IsCondition4Present(int index)
		{
			return true;
		}

		public static bool IsCondition5Present(int index)
		{
			return true;
		}

		public static bool IsCondition6Present(int index)
		{
			return true;
		}

		public static bool IsCondition7Present(int index)
		{
			return true;
		}

		public static bool IsCondition8Present(int index)
		{
			return true;
		}

		public static bool IsCondition9Present(int index)
		{
			return true;
		}

		public static bool IsCondition10Present(int index)
		{
			return true;
		}

		public static bool IsCondition11Present(int index)
		{
			return true;
		}

		public static bool IsCondition12Present(int index)
		{
			return true;
		}

		public static bool IsCondition13Present(int index)
		{
			return true;
		}

		public static bool IsCondition14Present(int index)
		{
			return true;
		}

		public static bool IsCondition15Present(int index)
		{
			return true;
		}

		public static bool IsCondition16Present(int index)
		{
			return true;
		}

		public static bool IsCondition17Present(int index)
		{
			return true;
		}

		public static bool IsCondition18Present(int index)
		{
			return true;
		}

		public static bool IsCondition19Present(int index)
		{
			return true;
		}

		public static bool IsCondition20Present(int index)
		{
			return true;
		}

		public static bool IsInYear(int index, int year)
		{
			return Data.Bars[index].Year == year;
		}

		public static bool IsInYearMonth(int index, int year, int month)
		{
			return Data.Bars[index].Year == year && Data.Bars[index - 1].Month == month;
		}

		public static bool IsOnDayOfWeek(int index, DayOfWeek day)
		{
			return Data.Bars[index].WeekDay == day;
		}

		public static bool IsBetweenHours(int index, int h1, int h2)
		{
			return Data.Bars[index].Hour >= h1 && Data.Bars[index].Hour <= h2;
		}

		public static bool BarIsBullish(int index)
		{
			return Data.Bars[index].IsBullish();
		}

		public static bool BarIsBearish(int index)
		{
			return Data.Bars[index].IsBearish();
		}

		public static bool BarCloseOnOrNearHigh(int index)
		{
			return Data.Bars[index].CloseOnOrNearHigh();
		}

		public static bool BarCloseOnOrNearLow(int index)
		{
			return Data.Bars[index].CloseOnOrNearLow();
		}
	}
}
