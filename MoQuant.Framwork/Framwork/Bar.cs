using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoQuant.Framwork {
    public class Bar {
        // 合约代码;
        private string mInstrumentID;
        // 开始价
        private double mOpenPrice;
        // 最高价
        private double mHighPrice;
        // 最低价
        private double mLowPrice;
        // 结束价
        private double mClosePrice;
        // 成交量
        private long mVolume;
        // 持仓量
        private double mOpenInterest;
        // 开始时间
        private DateTime mBeginTime;
        // 长度
        private long mSize;

        public Bar(string instrumentId, double openPrice, double highPrice, double lowPrice, double closePrice, long volume, double openInterest, DateTime beginTime, long size)
        {
            mInstrumentID = instrumentId;
            mOpenPrice = openPrice;
            mHighPrice = highPrice;
            mLowPrice = lowPrice;
            mClosePrice = closePrice;
            mVolume = volume;
            mOpenInterest = openInterest;
            mBeginTime = beginTime;
            mSize = size;
        }

        public string InstrumentId
        {
            get { return mInstrumentID; }
            set { mInstrumentID = value; }
        }

        public double OpenPrice
        {
            get { return mOpenPrice; }
            set { mOpenPrice = value; }
        }

        public double HighPrice
        {
            get { return mHighPrice; }
            set { mHighPrice = value; }
        }

        public double LowPrice
        {
            get { return mLowPrice; }
            set { mLowPrice = value; }
        }

        public double ClosePrice
        {
            get { return mClosePrice; }
            set { mClosePrice = value; }
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

        public DateTime BeginTime
        {
            get { return mBeginTime; }
            set { mBeginTime = value; }
        }

        public long Size
        {
            get { return mSize; }
            set { mSize = value; }
        }
    }
}
