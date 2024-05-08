using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeBacktester
{
	public enum TradeTypes
	{
		BuyAbove,
		BuyBelow,
		BuyClose,
		SellAbove,
		SellBelow,
		SellClose,
	};

	public static class Constants
	{
		public const double POINT = 0.00001;
		public const double PIP = 0.0001;
		public const double MIN_SPREAD = 0.00002;
		public const double ACCOUNT_SIZE = 100000.0;
		public const double MINIMIM_SL_SIZE = 0.000;
		public const double RISK_PERCENT = 0.01;
		public const double COMMISSION_PER_LOT = 3.0;
		public const double ENTRY_BUFFER = 0.00002;
		public const double VBP_AREA_SIZE = 0.0002;
		public const double FBP_AREA_SIZE = 0.0003;
		public const double SL_MULTI_BUY_ABOVE = 1.75;
		public const double SL_MULTI_BUY_BELOW = 1.75;
		public const double SL_MULTI_BUY_CLOSE = 1.75;
		public const double SL_MULTI_SELL_ABOVE = 1.75;
		public const double SL_MULTI_SELL_BELOW = 1.75;
		public const double SL_MULTI_SELL_CLOSE = 1.75;

		public const int MIN_SIGNALS = 4000;
		public const double MIN_RATIO = 1.15;

		public const int MINIMUM_TRADES = 2000;
		public const double MINIMUM_PL = 750000.0;
		//public const double MINIMUM_PL = -99999999.0;
		public const double MINIMUM_AVG_PL = -99999999.0;

		public const bool ATR_BASED_SL = true;
		public const bool COMPOUNDING = false;
	}
}
