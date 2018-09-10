using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace QuantEngine
{
    internal class StrategyManager
    {
        private static StrategyManager instance = new StrategyManager();

        internal static StrategyManager Instance
        {
            get
            {
                return instance;
            }
        }
        private StrategyManager()
        {
            loadStrategy();
        }

        //订单分发
        internal event OnOrderSend OnOrderSend;
        //撤单
        internal event OnOrderCancle OnOrderCancle;

        //策略添加
        private Dictionary<string, BaseStrategy> mStrategyMap = new Dictionary<string, BaseStrategy>();
        internal void addStrategy(string name, BaseStrategy strategy)
        {
            //策略去重
            if (mStrategyMap.ContainsKey(name))
                return;

            //订单分发
            strategy.OnOrderSend += (Order order) =>
            {
                OnOrderSend(order);
            };
            //撤单
            strategy.OnOrderCancle += (Order order) =>
            {
                OnOrderCancle(order);
            };

            //行情映射
            mStrategyMap.Add(name, strategy);
            string[] instIDs = strategy.OnLoadInstrument();
            foreach(string instID in instIDs)
            {
                HashSet<BaseStrategy> strategySet;
                if(!mInstIDStrategyMap.TryGetValue(instID, out strategySet))
                {
                    strategySet = new HashSet<BaseStrategy>();
                }
                strategySet.Add(strategy);
                mInstIDStrategyMap[instID] = strategySet;
            }

        }

        private void loadStrategy()
        {
            Assembly assembly = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "\\StrategyPackage.dll");
            Type[] types = assembly.GetTypes();
            foreach(Type t in types)
            {
                if (!t.IsSubclassOf(typeof(BaseStrategy)))
                    continue;

                BaseStrategy stg = Activator.CreateInstance(t) as BaseStrategy;
                if (stg == null)
                    continue;

                addStrategy(t.Name, stg);
            }
        }

        #region 行情
        //获取行情合约
        internal string[] GetInstrumentIDs()
        {
            return mInstIDStrategyMap.Keys.ToArray<string>();
        }
        

        //行情分发
        private Dictionary<string, HashSet<BaseStrategy>> mInstIDStrategyMap = new Dictionary<string, HashSet<BaseStrategy>>();
        internal void SendTick(Tick tick)
        {
            HashSet<BaseStrategy> strategySet;
            if (mInstIDStrategyMap.TryGetValue(tick.InstrumentID, out strategySet))
            {
                foreach (BaseStrategy stg in strategySet)
                {
                    stg.SendTick(tick);
                }
            }
        }
        #endregion

    }
}
