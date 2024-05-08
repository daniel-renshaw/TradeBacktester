using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeBacktester.Cond;
using TradeBacktester.Quant;

namespace TradeBacktester
{
	public class Data
	{
		private static Bars _bars = new Bars();
		public static Bars Bars => _bars;

		private static Indicators.ATRWrapper _atr = new Indicators.ATRWrapper();
		public static Indicators.ATRWrapper ATR => _atr;

		private static Indicators.EMAWrapper _ema = new Indicators.EMAWrapper();
		public static Indicators.EMAWrapper EMA => _ema;

		private static Indicators.RSIWrapper _rsi = new Indicators.RSIWrapper();
		public static Indicators.RSIWrapper RSI => _rsi;

		private static Indicators.PivotPoints _pp = new Indicators.PivotPoints();
		public static Indicators.PivotPoints PP => _pp;

		private static Indicators.Fractals _fractals = new Indicators.Fractals();
		public static Indicators.Fractals Fractals => _fractals;

		private static Indicators.BollingerBands _bbands = new Indicators.BollingerBands(20);
		public static Indicators.BollingerBands BBands => _bbands;

		private static Indicators.Stochastic _stoch = new Indicators.Stochastic(20);
		public static Indicators.Stochastic Stochastic => _stoch;

		private static Indicators.ParabolicSAR _psar = new Indicators.ParabolicSAR();
		public static Indicators.ParabolicSAR ParabolicSAR => _psar;

		private static Indicators.VolumeByPrice _volbyprice = new Indicators.VolumeByPrice(500);
		public static Indicators.VolumeByPrice VolumeByPrice => _volbyprice;

		private static Indicators.FractalsByPrice _fracbyprice = new Indicators.FractalsByPrice(500);
		public static Indicators.FractalsByPrice FractalsByPrice => _fracbyprice;

		private static CondResults _condresults = new CondResults();
		public static CondResults CondResults => _condresults;

		private static Trades _trades = new Trades();
		public static Trades Trades => _trades;

		private static TradeManagement _tradeManagement = new TradeManagement();
		public static TradeManagement TradeManagement => _tradeManagement;

		public static double[] HighestPL = new double[(int)TradeManagerTypes.TOTAL];
		public static double[] HighestWinRates = new double[(int)TradeManagerTypes.TOTAL];

		private static MultiResults _multiresults = new MultiResults();
		public static MultiResults MultiResults => _multiresults;
	}
}
