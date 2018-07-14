using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QuantEngine
{
    internal class StrategyManager
    {
        private static StrategyManager instance;

        internal static StrategyManager Instance
        {
            get
            {
                return instance;
            }
        }
        private StrategyManager(){}

        #region 行情
        //策略添加
        private Dictionary<string, Strategy> mStrategyMap = new Dictionary<string, Strategy>();
        internal void AddStrategy(string name, Strategy strategy)
        {
            //策略去重
            if (mStrategyMap.ContainsKey(name))
                return;

            //行情映射
            mStrategyMap.Add(name, strategy);
            string[] instIDs = strategy.GetInstrumentIDs();
            foreach(string instID in instIDs)
            {
                HashSet<Strategy> strategySet;
                mInstIDStrategyMap.TryGetValue(instID, out strategySet);
                strategySet.Add(strategy);
                mInstIDStrategyMap[instID] = strategySet;
            }

        }

        //获取行情合约
        internal string[] GetInstrumentIDs()
        {
            return mInstIDStrategyMap.Keys.ToArray<string>();
        }
        

        //行情分发
        private Dictionary<string, HashSet<Strategy>> mInstIDStrategyMap = new Dictionary<string, HashSet<Strategy>>();
        internal void SendTick(Tick tick)
        {
            HashSet<Strategy> strategySet;
            mInstIDStrategyMap.TryGetValue(tick.InstrumentID,out strategySet);
            if (strategySet == null)
                return;
            foreach(Strategy stg in strategySet)
            {
                stg.SendTick(tick);
            }
        }
        #endregion

    }
}
