using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoQuant.Framwork.Strategy {
    internal class Position
    {
        private string mStrategyName;
        private string mInstrumentID;
        private int mVol = 0;
        private DateTime mLastTime;

        public Position(string strategyName, string instrumentID, int vol, DateTime lastTime)
        {
            this.mStrategyName = strategyName;
            this.mInstrumentID = instrumentID;
            this.mVol = vol;
            this.mLastTime = lastTime;
        }

        public string StrategyName
        {
            get
            {
                return mStrategyName;
            }
        }

        public string InstrumentID
        {
            get
            {
                return mInstrumentID;
            }
        }

        public int Vol
        {
            get
            {
                return mVol;
            }
        }

        public DateTime LastTime
        {
            get
            {
                return mLastTime;
            }
        }
    }
}
