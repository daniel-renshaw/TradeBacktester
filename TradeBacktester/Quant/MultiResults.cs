using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeBacktester.Cond;

namespace TradeBacktester
{
	public abstract class MultiResult
	{
		public abstract void Update(MultiResult res);
		public abstract double Print(ConditionMask mask, bool expanded);
		public abstract bool IsDone();
	}

	public class MultiResults
	{
		public MultiResults()
		{
			data = new Dictionary<ConditionMask, MultiResult>();
			filterMask = ConditionMask.NONE;
		}

		public void AddResult(ConditionMask mask, MultiResult newData)
		{
			if (filterMask != ConditionMask.NONE)
				mask = ConditionMask.START;

			MultiResult res;
			if (!data.TryGetValue(mask, out res))
				res = (MultiResult)Activator.CreateInstance(newData.GetType());

			res.Update(newData);
			data[mask] = res;
		}

		public void GenerateFinalResults()
		{
			Dictionary<ConditionMask, MultiResult> newData = new Dictionary<ConditionMask, MultiResult>();

			foreach (KeyValuePair<ConditionMask, MultiResult> entry in data)
			{
				for (ConditionMask i = ConditionMask.START; i < ConditionMask.END; ++i)
				{
					if (Conditions.CheckForConditionSkip(ref i, entry.Key))
						continue;

					if (entry.Key != i && entry.Key.HasFlag(i))
					{
						MultiResult res;
						if (!newData.TryGetValue(i, out res))
							res = (MultiResult)Activator.CreateInstance(entry.Value.GetType());
						res.Update(entry.Value);
						newData[i] = res;
					}
				}
			}

			foreach (KeyValuePair<ConditionMask, MultiResult> entry in newData)
			{
				MultiResult res;
				if (data.TryGetValue(entry.Key, out res))
				{
					res.Update(entry.Value);
					data[entry.Key] = res;
				}
				else
					data[entry.Key] = entry.Value;
			}
		}

		public void PrintResults()
		{
			if (filterMask != ConditionMask.NONE)
			{
				MultiResult res;
				if (data.TryGetValue(ConditionMask.START, out res))
					res.Print(filterMask, true);
				return;
			}

			GenerateFinalResults();
			double best = 0.0;

			foreach (KeyValuePair<ConditionMask, MultiResult> entry in data)
			{
				double ret = entry.Value.Print(entry.Key, false);
				if (ret > best)
					best = ret;
			}

			Console.WriteLine(string.Format("Best Result: {0:F2}", best));
		}

		public ConditionMask FilterMask
		{
			get => filterMask;
		}

		private ConditionMask filterMask;
		private Dictionary<ConditionMask, MultiResult> data;
	}
}
