# TradeBacktester

***Warning: This is a personal project and has had some core functionality removed to prevent being misused.***

Program designed to read in years of candlestick chart data and output trade result data. The main method of doing so is through the use of conditions, where a trade will be "entered" for every candlestick in the input data, whichever conditions are relevant at the time of "entering" are recorded in a bitmask, and after all the input data has been iterated over, those bitmasks are used to generate win/loss data for all possible combinations of the conditions.