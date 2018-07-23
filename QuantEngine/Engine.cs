using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantEngine
{
    public class Engine
    {
        private CtpMdProvider mCtpMd = CtpMdProvider.Instance;
        private StrategyManager mStgManager = StrategyManager.Instance;
        private ITickFilter tickFilter = new DefaultTickFilter();

        private static Engine instance = new Engine();

        public static Engine Instance
        {
            get
            {
                return instance;
            }
        }

        public ITickFilter TickFilter
        {
            set
            {
                if (value == null)
                    return;
                tickFilter = value;
            }
        }

        private Engine() { }

        public void Run()
        {
            Account ac = Utils.Config.MyAccount;
            mCtpMd.Login(ac);
            mCtpMd.OnTick += new RtnTick(_RtnTick);
            foreach(string instrumentID in mStgManager.GetInstrumentIDs())
            {
                mCtpMd.SubscribeMarketData(instrumentID);
            }
        }

        private void _RtnTick(Tick tick)
        {
            if (!tickFilter.Check(tick))
                return;
            mStgManager.SendTick(tick);
        }

    }
}
