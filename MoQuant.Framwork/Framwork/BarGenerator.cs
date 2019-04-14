using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoQuant.Framwork {
    internal delegate void OnBarDataDelegate(Min1Bar bar);
    internal class BarGenerator {
        //
        internal event OnBarDataDelegate OnBarData;
        //
        private string mInstrumentID;
        private List<Tick> mTickList = new List<Tick>();
        private Min1Bar mBar;
        //
        private long mMinute = 0;

        //
        public BarGenerator(string instrumentID) {
            mInstrumentID = instrumentID;
        }

        internal void addTick(Tick tick) {
            //
            long minute = tick.UpdateTime.Hour * 100 + tick.UpdateTime.Minute;
            if (!mMinute.Equals(minute)) {
                OnBarData?.Invoke(mBar);
                mBar = null;
            }
            //
            if (mBar == null) {
                mBar = new Min1Bar();
                mBar.InstrumentID = mInstrumentID;
                mBar.OpenTime = tick.UpdateTime;
                mBar.OpenPrice = tick.LastPrice;
                mBar.HighPrice = tick.LastPrice;
                mBar.LowPrice = tick.LastPrice;
                mBar.ClosePrice = tick.LastPrice;
            } else {
                mBar.HighPrice = mBar.HighPrice < tick.LastPrice ? tick.LastPrice:mBar.HighPrice;
                mBar.LowPrice = mBar.LowPrice > tick.LastPrice ? tick.LastPrice : mBar.LowPrice;
                mBar.ClosePrice = tick.LastPrice;
            }
            mBar.Volume = tick.Volume;
            mBar.OpenInterest = tick.OpenInterest;
            mBar.Ticks.Add(tick);
        }
    }


}
