using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoQuant.Framwork {
    public class Tick
    {
        // 合约代码;
        private string mInstrumentID;
        // 最新价
        private double mLastPrice;
        //申买价一
        private double mBidPrice;
        // 申买量一
        private long mBidVolume;
        //申卖价一
        private double mAskPrice;
        //申卖量一
        private long mAskVolume;
        //当日均价
        private double mAveragePrice;
        //数量
        private long mVolume;
        //持仓量
        private double mOpenInterest;
        //最后修改时间
        private DateTime mUpdateTime;
        //涨停板价
        private double mUpperLimitPrice;
        //跌停板价
        private double mLowerLimitPrice;

        public Tick(string instrumentId, double lastPrice, double bidPrice, long bidVolume, double askPrice, long askVolume, double averagePrice, long volume, double openInterest, DateTime updateTime, double upperLimitPrice, double lowerLimitPrice)
        {
            mInstrumentID = instrumentId;
            mLastPrice = lastPrice;
            mBidPrice = bidPrice;
            mBidVolume = bidVolume;
            mAskPrice = askPrice;
            mAskVolume = askVolume;
            mAveragePrice = averagePrice;
            mVolume = volume;
            mOpenInterest = openInterest;
            mUpdateTime = updateTime;
            mUpperLimitPrice = upperLimitPrice;
            mLowerLimitPrice = lowerLimitPrice;
        }

        public string InstrumentId
        {
            get { return mInstrumentID; }
            set { mInstrumentID = value; }
        }

        public double LastPrice
        {
            get { return mLastPrice; }
            set { mLastPrice = value; }
        }

        public double BidPrice
        {
            get { return mBidPrice; }
            set { mBidPrice = value; }
        }

        public long BidVolume
        {
            get { return mBidVolume; }
            set { mBidVolume = value; }
        }

        public double AskPrice
        {
            get { return mAskPrice; }
            set { mAskPrice = value; }
        }

        public long AskVolume
        {
            get { return mAskVolume; }
            set { mAskVolume = value; }
        }

        public double AveragePrice
        {
            get { return mAveragePrice; }
            set { mAveragePrice = value; }
        }

        public long Volume
        {
            get { return mVolume; }
            set { mVolume = value; }
        }

        public double OpenInterest
        {
            get { return mOpenInterest; }
            set { mOpenInterest = value; }
        }

        public DateTime UpdateTime
        {
            get { return mUpdateTime; }
            set { mUpdateTime = value; }
        }

        public double UpperLimitPrice
        {
            get { return mUpperLimitPrice; }
            set { mUpperLimitPrice = value; }
        }

        public double LowerLimitPrice
        {
            get { return mLowerLimitPrice; }
            set { mLowerLimitPrice = value; }
        }
    }
}
