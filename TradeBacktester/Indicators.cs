using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeBacktester
{
	public class Indicators
	{
		public class EMAWrapper
		{
			public EMAWrapper()
			{
				emas = new Dictionary<int, EMA>();
				emas[1] = new EMA(1);
				emas[2] = new EMA(2);
				emas[3] = new EMA(3);
				emas[4] = new EMA(4);
				emas[5] = new EMA(5);
				emas[6] = new EMA(6);
				emas[7] = new EMA(7);
				emas[8] = new EMA(8);
				emas[9] = new EMA(9);
				emas[10] = new EMA(10);
				emas[11] = new EMA(11);
				emas[12] = new EMA(12);
				emas[13] = new EMA(13);
				emas[14] = new EMA(14);
				emas[15] = new EMA(15);
				emas[16] = new EMA(16);
				emas[17] = new EMA(17);
				emas[18] = new EMA(18);
				emas[19] = new EMA(19);
				emas[20] = new EMA(20);
				emas[21] = new EMA(21);
				emas[22] = new EMA(22);
				emas[23] = new EMA(23);
				emas[24] = new EMA(24);
				emas[25] = new EMA(25);
				emas[26] = new EMA(26);
				emas[27] = new EMA(27);
				emas[28] = new EMA(28);
				emas[29] = new EMA(29);
				emas[30] = new EMA(30);
				emas[31] = new EMA(31);
				emas[32] = new EMA(32);
				emas[33] = new EMA(33);
				emas[34] = new EMA(34);
				emas[35] = new EMA(35);
				emas[36] = new EMA(36);
				emas[37] = new EMA(37);
				emas[38] = new EMA(38);
				emas[39] = new EMA(39);
				emas[40] = new EMA(40);
				emas[41] = new EMA(41);
				emas[42] = new EMA(42);
				emas[43] = new EMA(43);
				emas[44] = new EMA(44);
				emas[45] = new EMA(45);
				emas[46] = new EMA(46);
				emas[47] = new EMA(47);
				emas[48] = new EMA(48);
				emas[49] = new EMA(49);
				emas[50] = new EMA(50);
				emas[100] = new EMA(100);
				emas[125] = new EMA(125);
				emas[150] = new EMA(150);
				emas[200] = new EMA(200);
				emas[250] = new EMA(250);
				emas[300] = new EMA(300);
				emas[350] = new EMA(350);
				emas[400] = new EMA(400);
				emas[500] = new EMA(500);
			}

			public double this[int i]
			{
				get
				{
					if (emas.ContainsKey(i))
						return emas[i].Get();

					throw new Exception("Tried to get invalid EMA");
				}
			}

			public bool Crossover(int a, int b)
			{
				if (!emas.ContainsKey(a) || !emas.ContainsKey(b))
					return false;

				return emas[a].Crossed(emas[b]);
			}

			public void Update(Bar bar)
			{
				foreach (KeyValuePair<int, EMA> entry in emas)
				{
					entry.Value.Update(bar.Close);
				}
			}

			private Dictionary<int, EMA> emas;
		}

		public class ATRWrapper
		{
			public ATRWrapper()
			{
				atrs = new Dictionary<int, ATR>();
				atrs[1] = new ATR(1);
				atrs[2] = new ATR(2);
				atrs[3] = new ATR(3);
				atrs[4] = new ATR(4);
				atrs[5] = new ATR(5);
				atrs[6] = new ATR(6);
				atrs[7] = new ATR(7);
				atrs[8] = new ATR(8);
				atrs[9] = new ATR(9);
				atrs[10] = new ATR(10);
				atrs[11] = new ATR(11);
				atrs[12] = new ATR(12);
				atrs[13] = new ATR(13);
				atrs[14] = new ATR(14);
				atrs[15] = new ATR(15);
				atrs[16] = new ATR(16);
				atrs[17] = new ATR(17);
				atrs[18] = new ATR(18);
				atrs[19] = new ATR(19);
				atrs[20] = new ATR(20);
				atrs[21] = new ATR(21);
				atrs[22] = new ATR(22);
				atrs[23] = new ATR(23);
				atrs[24] = new ATR(24);
				atrs[25] = new ATR(25);
			}

			public double this[int i]
			{
				get
				{
					if (atrs.ContainsKey(i))
						return atrs[i].Get();

					throw new Exception("Tried to get invalid ATR");
				}
			}

			public void Update(Bar bar, Bar prevBar)
			{
				foreach (KeyValuePair<int, ATR> entry in atrs)
				{
					entry.Value.Update(prevBar == null ? bar.Close : prevBar.Close, bar);
				}
			}

			private Dictionary<int, ATR> atrs;
		}

		public class RSIWrapper
		{
			public RSIWrapper()
			{
				rsis = new Dictionary<int, RSI>();
				rsis[2] = new RSI(2);
				rsis[3] = new RSI(3);
				rsis[4] = new RSI(4);
				rsis[5] = new RSI(5);
				rsis[6] = new RSI(6);
				rsis[7] = new RSI(7);
				rsis[8] = new RSI(8);
				rsis[9] = new RSI(9);
				rsis[10] = new RSI(10);
				rsis[11] = new RSI(11);
				rsis[12] = new RSI(12);
				rsis[13] = new RSI(13);
				rsis[14] = new RSI(14);
				rsis[15] = new RSI(15);
				rsis[16] = new RSI(16);
				rsis[17] = new RSI(17);
				rsis[18] = new RSI(18);
				rsis[19] = new RSI(19);
				rsis[20] = new RSI(20);
				rsis[21] = new RSI(21);
				rsis[22] = new RSI(22);
				rsis[23] = new RSI(23);
				rsis[24] = new RSI(24);
				rsis[25] = new RSI(25);
			}

			public double this[int i]
			{
				get
				{
					if (rsis.ContainsKey(i))
						return rsis[i].Get();

					throw new Exception("Tried to get invalid RSI");
				}
			}

			public void Update(Bar bar, Bar prevBar)
			{
				foreach (KeyValuePair<int, RSI> entry in rsis)
				{
					entry.Value.Update(prevBar == null ? bar.Close : prevBar.Close, bar);
				}
			}

			private Dictionary<int, RSI> rsis;
		}

		public class EMA
		{
			public EMA(int p)
			{
				period = p;
				alpha = 2.0 / (period + 1.0);
				value = 0.0;
				prevValues = new double[] { 0.0, 0.0 };
			}

			public void Update(double close)
			{
				prevValues[1] = prevValues[0];
				prevValues[0] = value;
				value = Math.Round((alpha * close) + ((1 - alpha) * value), 5);
			}

			public double Get()
			{
				return value;
			}

			public bool Crossed(EMA ema)
			{
				if (prevValues[1] > ema.prevValues[1])
					return prevValues[0] < ema.prevValues[0];
				else if (prevValues[1] < ema.prevValues[1])
					return prevValues[0] > ema.prevValues[0];
				else
					return false;
			}

			private double value;
			private double[] prevValues;
			private double alpha;
			private int period;
		}

		public class ATR
		{
			public ATR(int p)
			{
				period = p;
				value = 0.0;
			}

			public void Update(double prevClose, Bar bar)
			{
				double largest = bar.High - bar.Low;
				if (prevClose > bar.High)
					largest = prevClose - bar.Low;
				if (prevClose < bar.Low)
					largest = bar.High - prevClose;

				value = Math.Round(((value * (period - 1)) + largest) / period, 5);
			}

			public double Get()
			{
				return value;
			}

			private double value;
			private int period;
		}

		public class RSI
		{
			public RSI(int p)
			{
				period = p;
				bars = 1;
				pos = 0.0;
				neg = 0.0;
				values = new List<double>();
			}

			public void Update(double prevClose, Bar bar)
			{
				double diff = bar.Close - prevClose;

				if (bars <= period)
				{
					if (diff > 0)
						pos += diff;
					else
						neg += diff;

					if (bars == period)
					{
						pos /= period;
						neg /= period;

						if (neg != 0.0)
							values.Add(100.0 - (100.0 / (1.0 + (pos / neg))));
						else if (pos != 0.0)
							values.Add(100.0);
						else
							values.Add(50.0);
					}

					++bars;
				}
				else
				{
					pos = (pos * (period - 1) + (diff > 0.0 ? diff : 0.0)) / period;
					neg = (neg * (period - 1) + (diff < 0.0 ? -diff : 0.0)) / period;

					if (neg != 0.0)
						values.Add(100.0 - (100.0 / (1.0 + (pos / neg))));
					else if (pos != 0.0)
						values.Add(100.0);
					else
						values.Add(50.0);

					if (values.Count > period)
						values.RemoveAt(0);
				}
			}

			public double Get()
			{
				return values[period - 1];
			}

			private List<double> values;
			private int period;
			private int bars;
			private double pos;
			private double neg;
		}

		public class PivotPoints
		{
			public PivotPoints()
			{
				curMonth = 0;
				curDay = 0;
				high = 0.0;
				low = 0.0;
				close = 0.0;
				p = 0.0;
				r1 = 0.0;
				r2 = 0.0;
				s1 = 0.0;
				s2 = 0.0;
			}

			public void Update(Bar bar)
			{
				if (bar.Month != curMonth || bar.Day != curDay)
				{
					CalculatePoints();
					high = bar.High;
					low = bar.Low;
					close = bar.Close;
					curMonth = bar.Month;
					curDay = bar.Day;
				}
				else
				{
					if (bar.High > high)
						high = bar.High;
					if (bar.Low < low)
						low = bar.Low;
					close = bar.Close;
				}
			}

			private void CalculatePoints()
			{
				if (curMonth == 0)
					return;

				p = (high + low + close) / 3;
				r1 = (p * 2) - low;
				s1 = (p * 2) - high;
				r2 = p + (high - low);
				s2 = p - (high - low);
			}

			public double P
			{
				get => p;
			}

			public double R1
			{
				get => r1;
			}

			public double R2
			{
				get => r2;
			}

			public double S1
			{
				get => s1;
			}

			public double S2
			{
				get => s2;
			}

			private int curMonth;
			private int curDay;
			private double high;
			private double low;
			private double close;
			private double p;
			private double r1;
			private double r2;
			private double s1;
			private double s2;
		}

		public class FractalPair
		{
			public FractalPair(bool high, Bar b)
			{
				IsHigh = high;
				Bar = b;
			}

			public bool IsHigh;
			public Bar Bar;
		}

		public class Fractals
		{
			public Fractals()
			{
				fractals = new List<FractalPair>();
			}

			public void Update(int index)
			{
				// fractal needs 2 bars to the left and 2 bars to the right, so 5 minimum
				if (index < 5)
					return;

				int fractalIndex = index - 2;
				if (Data.Bars[fractalIndex - 1].Low > Data.Bars[fractalIndex].Low)
				{
					if (Data.Bars[fractalIndex - 2].Low > Data.Bars[fractalIndex - 1].Low)
					{
						if (Data.Bars[fractalIndex].Low < Data.Bars[fractalIndex + 1].Low)
						{
							if (Data.Bars[fractalIndex + 1].Low < Data.Bars[fractalIndex + 2].Low)
							{
								FractalPair pair = new FractalPair(false, Data.Bars[fractalIndex]);
								fractals.Add(pair);
								Data.FractalsByPrice.AddFractal(pair);
								if (fractals.Count > MAX_FRACTALS)
									fractals.RemoveAt(0);
							}
						}
					}
				}
				else if (Data.Bars[fractalIndex - 1].High < Data.Bars[fractalIndex].High)
				{
					if (Data.Bars[fractalIndex - 2].High < Data.Bars[fractalIndex - 1].High)
					{
						if (Data.Bars[fractalIndex].High > Data.Bars[fractalIndex + 1].High)
						{
							if (Data.Bars[fractalIndex + 1].High > Data.Bars[fractalIndex + 2].High)
							{
								FractalPair pair = new FractalPair(true, Data.Bars[fractalIndex]);
								fractals.Add(pair);
								Data.FractalsByPrice.AddFractal(pair);
								if (fractals.Count > MAX_FRACTALS)
									fractals.RemoveAt(0);
							}
						}
					}
				}
			}

			public FractalPair this[int i]
			{
				get
				{
					if (i >= 0 && i < fractals.Count)
						return fractals[fractals.Count - i - 1];

					return null;
				}
			}

			public FractalPair High(int h)
			{
				if (h >= fractals.Count || h < 0)
					return null;

				for (int i = fractals.Count - 1; i >= 0; --i)
				{
					if (fractals[i].IsHigh)
					{
						if (h == 0)
							return fractals[i];

						--h;
					}
				}

				return null;
			}

			public FractalPair Low(int l)
			{
				if (l >= fractals.Count || l < 0)
					return null;

				for (int i = fractals.Count - 1; i >= 0; --i)
				{
					if (!fractals[i].IsHigh)
					{
						if (l == 0)
							return fractals[i];

						--l;
					}
				}

				return null;
			}

			public double LatestBullishFibRetracement(int index)
			{
				double highest = 0.0;
				double lowest = 9999999.9;
				int highestIndex = 0;

				for (int i = fractals.Count - 1; i >= 0; --i)
				{
					if (fractals[i].IsHigh)
					{
						if (fractals[i].Bar.High > highest)
						{
							highest = fractals[i].Bar.High;
							highestIndex = i;
							continue;
						}
						else
							break;
					}

				}

				for (int i = highestIndex; i >= 0; --i)
				{
					if (!fractals[i].IsHigh)
					{
						if (fractals[i].Bar.Low < lowest)
						{
							lowest = fractals[i].Bar.Low;
							continue;
						}
						else
							break;
					}

				}

				return (highest - Data.Bars[index].Low) / (highest - lowest);
			}

			public int GetLowOfLastBullRun(int index)
			{
				double highest = 0.0;
				double lowest = 9999999.9;
				int saveIndex = 0;

				for (int i = fractals.Count - 1; i >= 0; --i)
				{
					if (fractals[i].IsHigh)
					{
						if (fractals[i].Bar.High > highest)
						{
							highest = fractals[i].Bar.High;
							saveIndex = i;
							continue;
						}
						else
							break;
					}

				}

				for (int i = saveIndex; i >= 0; --i)
				{
					if (!fractals[i].IsHigh)
					{
						if (fractals[i].Bar.Low < lowest)
						{
							lowest = fractals[i].Bar.Low;
							saveIndex = i;
							continue;
						}
						else
							break;
					}

				}

				return saveIndex;
			}

			public int GetHighOfLastBearRun(int index)
			{
				double highest = 0.0;
				double lowest = 9999999.9;
				int saveIndex = 0;

				for (int i = fractals.Count - 1; i >= 0; --i)
				{
					if (!fractals[i].IsHigh)
					{
						if (fractals[i].Bar.Low < lowest)
						{
							lowest = fractals[i].Bar.Low;
							saveIndex = i;
							continue;
						}
						else
							break;
					}

				}

				for (int i = saveIndex; i >= 0; --i)
				{
					if (fractals[i].IsHigh)
					{
						if (fractals[i].Bar.High > highest)
						{
							highest = fractals[i].Bar.High;
							saveIndex = i;
							continue;
						}
						else
							break;
					}

				}

				return saveIndex;
			}

			public bool BarCouldBeFractalHigh(int index)
			{
				return Data.Bars[index].High > Data.Bars[index - 1].High && Data.Bars[index - 1].High > Data.Bars[index - 2].High;
			}

			public bool BarCouldBeFractalLow(int index)
			{
				return Data.Bars[index].Low < Data.Bars[index - 1].Low && Data.Bars[index - 1].Low < Data.Bars[index - 2].Low;
			}

			private const int MAX_FRACTALS = 40;

			private List<FractalPair> fractals;
		}

		public class BollingerBands
		{
			public BollingerBands(int p)
			{
				upper = new List<double>();
				mid = new List<double>();
				lower = new List<double>();
				period = p;
			}

			public void Update(int index)
			{
				mid.Add(Data.EMA[period]);
				if (mid.Count > period)
					mid.RemoveAt(0);

				double stdDev = GetStandardDeviation(index);

				upper.Add(mid[mid.Count - 1] + (2 * stdDev));
				if (upper.Count > period)
					upper.RemoveAt(0);

				lower.Add(mid[mid.Count - 1] - (2 * stdDev));
				if (lower.Count > period)
					lower.RemoveAt(0);
			}

			private double GetStandardDeviation(int index)
			{
				if (index < period)
					return 0.0;

				double stdDev = 0.0;
				for (int i = 0; i < period; ++i)
					stdDev += Math.Pow(Data.Bars[index - i].Close - mid[mid.Count - 1], 2);
				stdDev = Math.Sqrt(stdDev / period);

				return stdDev;
			}

			public bool IsInUpperZone(double val)
			{
				return val <= upper[upper.Count - 1] && val > mid[mid.Count - 1];
			}

			public bool IsInLowerZone(double val)
			{
				return val >= lower[lower.Count - 1] && val < mid[mid.Count - 1];
			}

			public double GetZoneRange()
			{
				return upper[upper.Count - 1] - mid[mid.Count - 1];
			}

			public bool IsSquished()
			{
				double val = GetZoneRange();
				return (upper[upper.Count - 1] / val) <= 20;
			}

			private List<double> upper;
			private List<double> mid;
			private List<double> lower;
			private int period;
		}

		public class Stochastic
		{
			public Stochastic(int p)
			{
				kValues = new List<double>() { 0.0, 0.0, 0.0 };
				dValues = new List<double>() { 0.0, 0.0, 0.0 };
				period = p;
			}

			public void Update(int index)
			{
				double high = GetHighForPeriod(index);
				double low = GetLowForPeriod(index);
				double close = Data.Bars[index].Close;

				double kVal = (close - low) / (high - low) * 100.0;
				kValues.Add(kVal);
				kValues.RemoveAt(0);

				dValues.Add(GetD());
				dValues.RemoveAt(0);
			}

			public bool CrossUp()
			{
				return kValues[1] < dValues[1] && kValues[2] > dValues[2];
			}

			public bool CrossDown()
			{
				return kValues[1] > dValues[1] && kValues[2] < dValues[2];
			}

			private double GetHighForPeriod(int index)
			{
				double highest = 0.0;
				for (int i = index - 1; i > index - period - 1; --i)
				{
					if (i < 0)
						break;
					if (Data.Bars[i].High > highest)
						highest = Data.Bars[i].High;
				}
				return highest;
			}

			private double GetLowForPeriod(int index)
			{
				double lowest = 9999999.9;
				for (int i = index - 1; i > index - period - 1; --i)
				{
					if (i < 0)
						break;
					if (Data.Bars[i].Low < lowest)
						lowest = Data.Bars[i].Low;
				}
				return lowest;
			}

			private double GetD()
			{
				return (kValues[0] + kValues[1] + kValues[2]) / 3.0;
			}

			private List<double> kValues;
			private List<double> dValues;
			private int period;
		}

		public class ParabolicSAR
		{
			public ParabolicSAR()
			{
				values = new List<double> { 0.0, 0.0, 0.0 };
				direction = true;
				minAF = 0.02;
				curAF = minAF;
				maxAF = 0.2;
				eHigh = 0.0;
				eLow = 9999999.9;
			}

			public void Update(int index)
			{
				if (index == 0)
				{
					values.Add(Data.Bars[index].Close);
					values.RemoveAt(0);
				}

				if (index < 5)
				{
					if (Data.Bars[index].High > eHigh)
						eHigh = Data.Bars[index].High;
					if (Data.Bars[index].Low < eLow)
						eLow = Data.Bars[index].Low;

					if (index == 4)
					{
						if (Data.Bars[index].Close > values[2])
						{
							direction = true;
							values.Add(eLow);
							values.RemoveAt(0);
						}
						else
						{
							direction = false;
							values.Add(eHigh);
							values.RemoveAt(0);
						}
					}

					return;
				}

				// double check that last bar didn't take out sar
				if (direction && Data.Bars[index].Low < values[2])
				{
					direction = false;
					curAF = minAF;
					eLow = Data.Bars[index].Low;
					values.Add(eHigh);
					values.RemoveAt(0);
					return;
				}
				else if (!direction && Data.Bars[index].High > values[2])
				{
					direction = true;
					curAF = minAF;
					eHigh= Data.Bars[index].High;
					values.Add(eLow);
					values.RemoveAt(0);
					return;
				}

				values.Add(direction ? GetRisingSAR(index) : GetFallingSAR(index));
				values.RemoveAt(0);

				if (direction)
				{
					if (eHigh < Data.Bars[index].High)
					{
						eHigh = Data.Bars[index].High;
						curAF += minAF;
						if (curAF > maxAF)
							curAF = maxAF;
					}
				}
				else
				{
					if (eLow > Data.Bars[index].Low)
					{
						eLow = Data.Bars[index].Low;
						curAF += minAF;
						if (curAF > maxAF)
							curAF = maxAF;
					}
				}
			}

			private double GetRisingSAR(int index)
			{
				double sar = values[2] + curAF * (eHigh - values[2]);
				if (sar > Data.Bars[index - 1].Low)
					sar = Data.Bars[index - 1].Low;
				if (sar > Data.Bars[index - 2].Low)
					sar = Data.Bars[index - 2].Low;
				return sar;
			}

			private double GetFallingSAR(int index)
			{
				double sar = values[2] - curAF * (values[2] - eLow);
				if (sar < Data.Bars[index - 1].High)
					sar = Data.Bars[index - 1].High;
				if (sar < Data.Bars[index - 2].High)
					sar = Data.Bars[index - 2].High;
				return sar;
			}

			public double this[int i]
			{
				get
				{
					if (i > 2)
						return double.NaN;
					return values[i];
				}
			}

			private List<double> values;
			private bool direction;
			private double eHigh;
			private double eLow;
			private double minAF;
			private double curAF;
			private double maxAF;
		}

		public class VolumeByPrice
		{
			public class VBPSet
			{
				public VBPSet()
				{
					total = 0;
					bars = new List<Bar>();
				}

				public void AddBar(Bar bar)
				{
					total += bar.TickVol;
					bars.Add(bar);
				}

				public bool CheckAndRemoveOldBar(int index, int period)
				{
					if (index - period < bars[0].Index)
						return false;

					total -= bars[0].TickVol;
					bars.RemoveAt(0);
					return true;
				}

				public int Count
				{
					get => bars.Count;
				}

				public int Value
				{
					get => total;
				}

				private int total;
				private List<Bar> bars;
			}

			public VolumeByPrice(int p)
			{
				values = new Dictionary<double, VBPSet>();
				period = p;
			}

			public void Update(int index)
			{
				double closeArea = Util.FloorToVal(Data.Bars[index].Close, Constants.VBP_AREA_SIZE);
				VBPSet area;
				if (!values.TryGetValue(closeArea, out area))
					area = new VBPSet();

				area.AddBar(Data.Bars[index]);
				values[closeArea] = area;

				RemoveOldBar(index);
			}

			private void RemoveOldBar(int index)
			{
				if (index < period)
					return;

				foreach (KeyValuePair<double, VBPSet> entry in values)
				{
					if (entry.Value.CheckAndRemoveOldBar(index, period))
					{
						if (entry.Value.Count == 0)
							values.Remove(entry.Key);
						break;
					}
				}
			}

			public bool BarInTopPercentile(Bar bar, double percentile)
			{
				double area = Util.FloorToVal(bar.Close, Constants.VBP_AREA_SIZE);
				int min = 99999999;
				int max = -1;

				foreach (KeyValuePair<double, VBPSet> entry in values)
				{
					if (entry.Value.Value < min)
						min = entry.Value.Value;
					if (entry.Value.Value > max)
						max = entry.Value.Value;
				}

				int diff = max - min;
				int percDiff = (int)(diff * percentile);
				return values[area].Value > (min + percDiff);
			}

			public bool BarInBottomPercentile(Bar bar, double percentile)
			{
				double area = Util.FloorToVal(bar.Close, Constants.VBP_AREA_SIZE);
				int min = 99999999;
				int max = -1;

				foreach (KeyValuePair<double, VBPSet> entry in values)
				{
					if (entry.Value.Value < min)
						min = entry.Value.Value;
					if (entry.Value.Value > max)
						max = entry.Value.Value;
				}

				int diff = max - min;
				int percDiff = (int)(diff * percentile);
				return values[area].Value < (min + percDiff);
			}

			private Dictionary<double, VBPSet> values;
			private int period;
		}

		public class FractalsByPrice
		{
			public class FBPSet
			{
				public FBPSet()
				{
					pairs = new List<FractalPair>();
				}

				public void AddFractal(FractalPair pair)
				{
					pairs.Add(pair);
				}

				public void RemoveFractal()
				{
					pairs.RemoveAt(0);
				}

				public int Count
				{
					get => pairs.Count;
				}

				private List<FractalPair> pairs;
			}

			public FractalsByPrice(int max)
			{
				values = new Dictionary<double, FBPSet>();
				keyOrder = new List<double>();
				maxFractals = max;
			}

			public void AddFractal(FractalPair pair)
			{
				double price = Util.FloorToVal(pair.IsHigh ? pair.Bar.High : pair.Bar.Low, Constants.FBP_AREA_SIZE);
				FBPSet area;
				if (!values.TryGetValue(price, out area))
					area = new FBPSet();

				area.AddFractal(pair);
				values[price] = area;
				keyOrder.Add(price);

				RemoveOldFractal();
			}

			private void RemoveOldFractal()
			{
				if (keyOrder.Count < maxFractals)
					return;

				double price = keyOrder[0];
				values[price].RemoveFractal();
				if (values[price].Count <= 0)
					values.Remove(price);
				keyOrder.RemoveAt(0);
			}

			public int GetFractalCountForPrice(double price)
			{
				double area = Util.FloorToVal(price, Constants.FBP_AREA_SIZE);
				if (values.ContainsKey(area))
					return values[area].Count;
				return 0;
			}

			public bool PriceInTopPercentile(double price, double percentile)
			{
				double area = Util.FloorToVal(price, Constants.FBP_AREA_SIZE);
				int min = 99999999;
				int max = -1;

				foreach (KeyValuePair<double, FBPSet> entry in values)
				{
					if (entry.Value.Count < min)
						min = entry.Value.Count;
					if (entry.Value.Count > max)
						max = entry.Value.Count;
				}

				int diff = max - min;
				int percDiff = (int)(diff * percentile);
				int count = values.ContainsKey(area) ? values[area].Count : 0;
				return count > (min + percDiff);
			}

			public bool PriceInBottomPercentile(double price, double percentile)
			{
				double area = Util.FloorToVal(price, Constants.FBP_AREA_SIZE);
				int min = 99999999;
				int max = -1;

				foreach (KeyValuePair<double, FBPSet> entry in values)
				{
					if (entry.Value.Count < min)
						min = entry.Value.Count;
					if (entry.Value.Count > max)
						max = entry.Value.Count;
				}

				int diff = max - min;
				int percDiff = (int)(diff * percentile);
				int count = values.ContainsKey(area) ? values[area].Count : 0;
				return count < (min + percDiff);
			}

			private Dictionary<double, FBPSet> values;
			private List<double> keyOrder;
			private int maxFractals;
		}
	}
}
