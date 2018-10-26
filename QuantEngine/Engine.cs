using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantEngine
{
    public class Engine
    {
        private IMdProvider mMp = CtpMdProvider.Instance;
        private ITdProvider mTp = CtpTdProvider.Instance;
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

        private Engine() {}

        //启动引擎
        public void Run()
        {
            //账号
            Account mac = Utils.Config.MyMdAccount;
            Account tac = Utils.Config.MyTdAccount;

            //启动行情接口
            mMp.Login(mac);
            mMp.OnTick += _RtnTick;
            foreach(string instrumentID in mStgManager.GetInstrumentIDs())
            {
                mMp.SubscribeMarketData(instrumentID);
            }

            //启动交易接口
            mTp.Login(tac);
            mStgManager.TdProvider = mTp;
        }

        private void _RtnTick(Tick tick)
        {
            if (!tickFilter.Check(tick))
                return;
            mStgManager.SendTick(tick);
        }

        private void _CancleOrder(Order order)
        {
            mTp.CancelOrder(order);
        }

    }
}
