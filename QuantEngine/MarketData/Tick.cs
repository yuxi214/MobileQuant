using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantEngine
{
    public class Tick
    {
        // 合约代码;
        private string instrumentID;
        // 最新价
        private double lastPrice;
        //申买价一
        private double bidPrice;
        // 申买量一
        private int bidVolume;
        //申卖价一
        private double askPrice;
        //申卖量一
        private int askVolume;
        //当日均价
        private double averagePrice;
        //数量
        private int volume;
        //持仓量
        private double openInterest;
        //最后修改时间
        private DateTime updateTime;
        //涨停板价
        private double upperLimitPrice;
        //跌停板价
        private double lowerLimitPrice;

        public Tick(string instrumentID, double lastPrice, double bidPrice, int bidVolume, double askPrice, int askVolume, double averagePrice, int volume, double openInterest, DateTime updateTime, double upperLimitPrice, double lowerLimitPrice)
        {
            this.instrumentID = instrumentID;
            this.lastPrice = lastPrice;
            this.bidPrice = bidPrice;
            this.bidVolume = bidVolume;
            this.askPrice = askPrice;
            this.askVolume = askVolume;
            this.averagePrice = averagePrice;
            this.volume = volume;
            this.openInterest = openInterest;
            this.updateTime = updateTime;
            this.upperLimitPrice = upperLimitPrice;
            this.lowerLimitPrice = lowerLimitPrice;
        }

        public string InstrumentID
        {
            get
            {
                return instrumentID;
            }
        }

        public double LastPrice
        {
            get
            {
                return lastPrice;
            }
        }

        public double BidPrice
        {
            get
            {
                return bidPrice;
            }
        }

        public int BidVolume
        {
            get
            {
                return bidVolume;
            }
        }

        public double AskPrice
        {
            get
            {
                return askPrice;
            }
        }

        public int AskVolume
        {
            get
            {
                return askVolume;
            }
        }

        public double AveragePrice
        {
            get
            {
                return averagePrice;
            }
        }

        public int Volume
        {
            get
            {
                return volume;
            }
        }

        public double OpenInterest
        {
            get
            {
                return openInterest;
            }
        }

        public DateTime UpdateTime
        {
            get
            {
                return updateTime;
            }
        }

        public double UpperLimitPrice
        {
            get
            {
                return upperLimitPrice;
            }
        }

        public double LowerLimitPrice
        {
            get
            {
                return lowerLimitPrice;
            }
        }
    }
}
