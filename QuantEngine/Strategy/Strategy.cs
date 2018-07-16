using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantEngine
{
    public partial class Strategy
    {
        public virtual void OnStart() { }
        public virtual void OnStop() { }
        public virtual string[] OnLoadInstrument() { return new string[] { }; }
        public virtual void OnTick(Tick tick) { }
        public virtual void OnPositionChanged(string instrumentID,int position) { }
    }

    public partial class Strategy
    {
        //合约列表
        private string mMainInstID = string.Empty;
        private HashSet<string> mInstIDSet;

        //发送开始信号
        internal void SendStart()
        {
            string[] instIDs = OnLoadInstrument();
            //添加合约
            for(int i=0;i<instIDs.Length;i++)
            {
                if (i == 0)
                {
                    mMainInstID = instIDs[0];
                }
                mInstIDSet.Add(instIDs[i]);
            }

            //启动
            OnStart();
        }

        //发送tick
        internal void SendTick(Tick tick)
        {
            OnTick(tick);
        }
    }
}
