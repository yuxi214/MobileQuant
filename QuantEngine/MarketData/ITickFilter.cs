using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantEngine
{
    public interface ITickFilter
    {
        bool Check(Tick Tick);
    }

    internal class DefaultTickFilter : ITickFilter
    {
        private Tick lastTick;
        public bool Check(Tick tick)
        {
            //异常值
            if (tick.AskPrice <= 0
                || tick.AskPrice > tick.UpperLimitPrice
                || tick.AskVolume <= 0
                || tick.BidPrice <= 0
                || tick.BidPrice >= tick.AskPrice
                || tick.BidPrice < tick.LowerLimitPrice
                || tick.BidVolume <= 0
                || tick.LastPrice <= 0
                || tick.LastPrice < tick.BidPrice
                || tick.LastPrice > tick.AskPrice
                || tick.LowerLimitPrice <= 0
                || tick.UpperLimitPrice <= 0
                || tick.Volume < 0
                || tick.InstrumentID.Equals(String.Empty)
                || tick.AveragePrice <= 0
                || tick.OpenInterest <= 0)
            {
                return false;
            }

            //涨跌异常
            if (lastTick != null)
            {
                if (tick.LastPrice < lastTick.LastPrice * 0.9d 
                    || tick.LastPrice > lastTick.LastPrice * 1.1d)
                {
                    return false;
                }
            }

            //正常
            return true;
        }
    }
}
