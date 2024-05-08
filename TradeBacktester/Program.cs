#define COND

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.IO;
using System.Diagnostics;
using TradeBacktester.Idea;
using TradeBacktester.Cond;
using TradeBacktester.Quant;
using TradeBacktester.Realistic;

namespace TradeBacktester
{
	class Program
	{
		static void Main(string[] args)
		{
			// Get currency formatting right
			CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
			culture.NumberFormat.CurrencyNegativePattern = 1;
			Thread.CurrentThread.CurrentCulture = culture;

			// Start the runtime timer
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			// Setup console.out to write to file instead
			FileStream ostrm;
			StreamWriter writer;
			TextWriter oldOut = Console.Out;
			try
			{
#if IDEA
				ostrm = new FileStream("./IdeaData.txt", FileMode.Create, FileAccess.Write);
#elif COND
				ostrm = new FileStream("./CondData.txt", FileMode.Create, FileAccess.Write);
#elif QUANT
				ostrm = new FileStream("./QuantData.txt", FileMode.Create, FileAccess.Write);
#elif REAL
				ostrm = new FileStream("./RealData.txt", FileMode.Create, FileAccess.Write);
#endif
				writer = new StreamWriter(ostrm);
				writer.AutoFlush = true;
			}
			catch (Exception e)
			{
				Console.WriteLine("Cannot open Data.txt for writing");
				Console.WriteLine(e.Message);
				return;
			}
			Console.SetOut(writer);

			// Do the stuff
			LoadBarData();
			Run();

			// Set output back to the console
			Console.SetOut(oldOut);
			writer.Close();
			ostrm.Close();

			// Print stopwatch and summary data
			stopwatch.Stop();
			PrintConsoleData(stopwatch);

			Console.Beep();
		}

		static void LoadBarData()
		{
			// Open and load bar data
			string curDir = AppDomain.CurrentDomain.BaseDirectory;
			string file = System.IO.Path.Combine(curDir, @"filehere.csv");
			string filepath = System.IO.Path.GetFullPath(file);
			Data.Bars.Load(filepath);
		}

		static void Run()
		{
#if IDEA
			IdeaTester it = new IdeaTester();
#elif COND
			ConditionTester ct = new ConditionTester();
#elif QUANT
			QuantAnalyzer qa = new QuantAnalyzer();
#elif REAL
			RealisticTester rt = new RealisticTester();
#endif

			for (int i = 1; i < Data.Bars.Count; ++i)
			{
				// don't check for trades for the first 200 bars to give time for indicators to calculate accurate values
				if (i >= 200)
				{
#if IDEA
					it.HandleIndex(i);
#elif COND
					ct.HandleIndex(i);
#elif QUANT
					qa.HandleIndex(i);
#elif REAL
					if (rt.HandleIndex(i))
						break;
#endif
				}

				// Update stuff afters so trades can't see into the future
				Data.ATR.Update(Data.Bars[i], Data.Bars[i - 1]);
				Data.EMA.Update(Data.Bars[i]);
				Data.RSI.Update(Data.Bars[i], Data.Bars[i - 1]);
				Data.PP.Update(Data.Bars[i]);
				Data.Fractals.Update(i);
				Data.BBands.Update(i);
				Data.Stochastic.Update(i);
				Data.ParabolicSAR.Update(i);
				Data.VolumeByPrice.Update(i);
			}

#if IDEA
			it.PrintResults();
#elif COND
			ct.PrintResults();
#elif QUANT
			qa.PrintResults();
#elif REAL
			rt.PrintResults();
#endif
		}

		static void PrintConsoleData(Stopwatch sw)
		{
			Console.WriteLine(string.Format("Runtime: {0:00}:{1:00}:{2:00}.{3:00}", sw.Elapsed.Hours, sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.Milliseconds / 10));
#if COND
			for (TradeManagerTypes i = 0; i < TradeManagerTypes.TOTAL; ++i)
			{
				Console.WriteLine(string.Format("{0}: {1:F1}%, P/L: {2:C2}", i.ToString(), Data.HighestWinRates[(int)i], Data.HighestPL[(int)i]));
			}
#endif
		}
	}
}
