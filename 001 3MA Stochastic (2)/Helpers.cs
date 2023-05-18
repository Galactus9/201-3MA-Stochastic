using cAlgo.API.Indicators;
using cAlgo.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaStochastic
{
    public class Helpers
    {
        public static string TrandDesizioner(MovingAverage slowMa, DataSeries priceData)
        {
            var price = priceData.Last(0);
            if (price > slowMa.Result.Last(0) * 1.01)
            {
                return $"UpTrent";
            }
            else if (price < slowMa.Result.Last(0) * 0.99)
            {
                return $"DownTrent";
            }
            else
            {
                return $"NoTrent";
            }
        }
        public static string MaCheck(MovingAverage midMa, MovingAverage fastMa, DataSeries priceData, string trent)
        {

            var price = priceData.Last(0);
            var mma = midMa.Result.Last(0);
            var fma = fastMa.Result.Last(0);
            if (trent == "UpTrent" && mma > price)
            {
                return "Pass";
            }
            else if (trent == "DownTrent" && mma < price)
            {
                return "Pass";
            }
            else return "Fail";
        }
        public static string StochasticCheck(double upBorder, double downBorder, StochasticOscillator stochastic, string trent)
        {

            var prevStochastic = stochastic.PercentD;
            prevStochastic.ToList().RemoveAt(0);
            if (trent == "DownTrent" && stochastic.PercentD.Last(0) > upBorder && stochastic.PercentD.IsFalling() /*&& prevStochastic.IsFalling()*/)
            {
                return "Pass";
            }
            else if (trent == "UpTrent" && stochastic.PercentD.Last(0) < downBorder && stochastic.PercentD.IsRising() /*&& prevStochastic.IsRising()*/)
            {
                return "Pass";
            }
            else return "Fail";
        }
    }
}
