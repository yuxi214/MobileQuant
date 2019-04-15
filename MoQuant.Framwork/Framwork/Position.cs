using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoQuant.Framwork {
    public class Position
    {
        private string mStrategyName;
        private string mInstrumentID;
        private int mVol = 0;
        private DateTime mLastTime;

        public string StrategyName
        {
            get { return mStrategyName; }
            set { mStrategyName = value; }
        }

        public string InstrumentId
        {
            get { return mInstrumentID; }
            set { mInstrumentID = value; }
        }

        public int Vol
        {
            get { return mVol; }
            set { mVol = value; }
        }

        public DateTime LastTime
        {
            get { return mLastTime; }
            set { mLastTime = value; }
        }
    }
}
