using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantEngine
{
    public class Engine
    {
        private CtpMdProvider mMp = CtpMdProvider.Instance;
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

        private Engine() {
            init();
        }

        private void init()
        {
            //连接订单分发事件
            mStgManager.OnOrderSend += (Order order) =>
            {
                _SendOrder(order);
            };
        }

        //启动引擎
        public void Run()
        {
            Account ac = Utils.Config.MyAccount;
            mMp.Login(ac);
            mMp.OnTick += new RtnTick(_RtnTick);
            foreach(string instrumentID in mStgManager.GetInstrumentIDs())
            {
                mMp.SubscribeMarketData(instrumentID);
            }
        }

        private void _RtnTick(Tick tick)
        {
            if (!tickFilter.Check(tick))
                return;
            mStgManager.SendTick(tick);
        }

        private void _SendOrder(Order order)
        {

        }

    }
}
