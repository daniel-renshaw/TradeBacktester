using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeBacktester.Idea
{
	public class IdeaTester
	{
		TradeIdea[] ideas;

		public IdeaTester()
		{
			ideas = new TradeIdea[]
			{
				new TradeIdea_Final(0),
				new TradeIdea_Final(2011),
				new TradeIdea_Final(2012),
				new TradeIdea_Final(2013),
				new TradeIdea_Final(2014),
				new TradeIdea_Final(2015),
				new TradeIdea_Final(2016),
				new TradeIdea_Final(2017),
				new TradeIdea_Final(2018),
				new TradeIdea_Final(2019),
				new TradeIdea_Final(2020),
				new TradeIdea_Final(2021),
			};
		}

		public void HandleIndex(int index)
		{
			foreach (TradeIdea ti in ideas)
			{
				ti.CheckForEntry(index);
				ti.ManageTrades(index);
			}
		}

		public void PrintResults()
		{
			foreach (TradeIdea ti in ideas)
			{
				ti.Print();
			}
		}
	}
}
