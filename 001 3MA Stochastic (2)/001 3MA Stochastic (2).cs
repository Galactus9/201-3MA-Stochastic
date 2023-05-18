using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cAlgo.API;
using cAlgo.API.Collections;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using MaStochastic;

namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None)]
    public class NewcBot2 : Robot
    {
        [Parameter("Source")]
        public DataSeries Source { get; set; }

        [Parameter("Slow MA Period", DefaultValue = 200)]
        public int SlowMaPeriod { get; set; }

        [Parameter("Mid MA Period", DefaultValue = 50)]
        public int MidMaPeriod { get; set; }

        [Parameter("Fast MA Period", DefaultValue = 20)]
        public int FastMaPeriod { get; set; }

        private MovingAverage slowMa;
        private MovingAverage midMa;
        private MovingAverage fastMa;
        private StochasticOscillator stochastic;
        private Bars H4;
        private StochasticOscillator stochasticH4;
        private string maCheck;
        private string stochasticCheck;
        private string trent;
        private DataSeries closePrices;
        //private Symbol[] tradeList;

        protected override void OnStart()
        {
            H4 = MarketData.GetBars(TimeFrame.Hour4);
            slowMa = Indicators.MovingAverage(Source, SlowMaPeriod, MovingAverageType.Exponential);
            midMa = Indicators.MovingAverage(Source, MidMaPeriod, MovingAverageType.Exponential);
            fastMa = Indicators.MovingAverage(Source, FastMaPeriod, MovingAverageType.Exponential);
            stochastic = Indicators.StochasticOscillator(9, 3, 9, MovingAverageType.Exponential);
            stochasticH4 = Indicators.StochasticOscillator(H4, 9, 3, 9, MovingAverageType.Exponential);
        }

        protected override void OnBar()
        {
            closePrices = Bars.ClosePrices;
            trent = Helpers.TrandDesizioner(slowMa, closePrices);
            maCheck = Helpers.MaCheck(midMa, fastMa, closePrices, trent);
            stochasticCheck = Helpers.StochasticCheck(35, 35, stochastic, trent);
            var pipsAmount = Math.Abs((slowMa.Result.Last(0) - closePrices.LastValue) / Symbol.PipSize);
            if (pipsAmount > 125)
            {
                pipsAmount = pipsAmount / 2;
            }
            var tradeAmount = (Account.Equity * 0.009) / (pipsAmount * Symbol.PipValue);
            tradeAmount = Symbol.NormalizeVolumeInUnits(tradeAmount, RoundingMode.Down);
            Print(pipsAmount);

            #region ModifyPosition
            //var LongPosition = Positions.Find($"Long1");
            var LongPosition1 = Positions.Find($"Long2");
            var LongPosition2 = Positions.Find($"Long3");
            //var ShortPosition = Positions.Find($"Short1");
            var ShortPosition1 = Positions.Find($"Short2");
            var ShortPosition2 = Positions.Find($"Short3");

            
            //if (LongPosition == null & LongPosition1 != null & LongPosition2 != null)
            //{
            //    ModifyPosition(LongPosition1, LongPosition1.EntryPrice + Symbol.PipSize * 10, LongPosition1.TakeProfit);
            //    //ModifyPosition(LongPosition2, LongPosition2.EntryPrice + Symbol.PipSize * 10, LongPosition2.TakeProfit);
            //}

            //if (ShortPosition == null & ShortPosition1 != null & ShortPosition2 != null)
            //{
            //    ModifyPosition(ShortPosition1, ShortPosition1.EntryPrice - Symbol.PipSize * 10, ShortPosition1.TakeProfit);
            //    //ModifyPosition(ShortPosition2, ShortPosition2.EntryPrice - Symbol.PipSize * 10, ShortPosition2.TakeProfit);
            //}

            if (LongPosition1 == null & LongPosition2 != null)
            {
                //ModifyPosition(LongPosition2, LongPosition2.EntryPrice + Symbol.PipSize * 70, LongPosition2.TakeProfit);
                ModifyPosition(LongPosition2, LongPosition2.EntryPrice + Symbol.PipSize * 10, LongPosition2.TakeProfit);
            }

            if (ShortPosition1 == null & ShortPosition2 != null)
            {
                //ModifyPosition(ShortPosition2, ShortPosition2.EntryPrice - Symbol.PipSize * 70, ShortPosition2.TakeProfit);
                ModifyPosition(ShortPosition2, ShortPosition2.EntryPrice - Symbol.PipSize * 10, ShortPosition2.TakeProfit);
            }
            #endregion


            if (trent == "UpTrent" && maCheck == "Pass" && stochasticCheck == "Pass" && LongPosition1 == null )
            {
                ExecuteMarketOrder(TradeType.Buy, SymbolName, tradeAmount, $"Long1", pipsAmount, pipsAmount);
                ExecuteMarketOrder(TradeType.Buy, SymbolName, tradeAmount, $"Long2", pipsAmount, pipsAmount * 1.5);
                ExecuteMarketOrder(TradeType.Buy, SymbolName, tradeAmount, $"Long3", pipsAmount, pipsAmount * 2);
            }
            if (trent == "DownTrent" && maCheck == "Pass" && stochasticCheck == "Pass" && ShortPosition1 == null)
            {
                ExecuteMarketOrder(TradeType.Sell, SymbolName, tradeAmount, $"Short1", pipsAmount, pipsAmount);
                ExecuteMarketOrder(TradeType.Sell, SymbolName, tradeAmount, $"Short2", pipsAmount, pipsAmount * 1.5);
                ExecuteMarketOrder(TradeType.Sell, SymbolName, tradeAmount, $"Short3", pipsAmount, pipsAmount * 2);
            }
        }

        protected override void OnStop()
        {
            // Handle cBot stop here
        }
    }
}