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

        //策略添加
        private Dictionary<string, Strategy> mStrategyMap = new Dictionary<string, Strategy>();
        internal void addStrategy(string name, Strategy strategy)
        {
            //策略去重
            if (mStrategyMap.ContainsKey(name))
                return;

            //行情映射
            mStrategyMap.Add(name, strategy);
            string[] instIDs = strategy.OnLoadInstrument();
            foreach(string instID in instIDs)
            {
                HashSet<Strategy> strategySet;
                mInstIDStrategyMap.TryGetValue(instID, out strategySet);
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
                if (!t.IsInstanceOfType(typeof(Strategy)))
                    continue;

                Strategy stg = Activator.CreateInstance(t) as Strategy;
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
