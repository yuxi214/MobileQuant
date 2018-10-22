using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantEngine
{
    //用户实现
    public partial class BaseStrategy
    {
        public virtual void OnStart() { }
        public virtual void OnStop() { }
        public virtual string[] OnLoadInstrument() { return new string[] { }; }
        public virtual void OnTick(Tick tick) { }
        public virtual void OnPositionChanged(string instrumentID, int position) { }
    }

    //行情
    public partial class BaseStrategy
    {
        //合约列表
        private string mMainInstID = string.Empty;
        private HashSet<string> mInstIDSet = new HashSet<string>();

        //最新tick
        private Dictionary<string, Tick> lastTickMap = new Dictionary<string, Tick>();

        //发送开始信号
        internal void SendStart()
        {
            string[] instIDs = OnLoadInstrument();
            //添加合约
            for (int i = 0; i < instIDs.Length; i++)
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
            lastTickMap[tick.InstrumentID] = tick;
            OnTick(tick);
        }
    }

    //交易
    public partial class BaseStrategy
    {
        //交易接口
        private ITdProvider mTdProvider;
        internal ITdProvider TdProvider
        {
            set
            {
                mTdProvider = value;
            }
        }

        //持仓
        private Dictionary<string, int> mPositionMap = new Dictionary<string, int>();
        public int GetPosition(string instrumentID)
        {
            if (mPositionMap.ContainsKey(instrumentID))
            {
                return mPositionMap[instrumentID];
            }
            else
            {
                return 0;
            }
        }
        internal void AddPosition(string instrumentID, int vol)
        {
            mPositionMap[instrumentID] = mPositionMap.ContainsKey(instrumentID) ? mPositionMap[instrumentID] + vol : vol;
        }

        //订单
        private List<Order> orderList = new List<Order>();
        private List<Order> activeOrderList = new List<Order>();
        private List<Order> doneOrderList = new List<Order>();

        //发送订单
        internal void SendOrder(Order order)
        {
            orderList.Add(order);
            activeOrderList.Add(order);
            if (mTdProvider != null)
            {
                mTdProvider.SendOrder(order);
            }
        }

        //撤单
        internal void CancleOrder(Order order)
        {
            if (mTdProvider != null)
            {
                mTdProvider.CancelOrder(order);
            }
        }

        //更新订单
        internal void UpdateOrder(Order order)
        {
            if (order.Status == OrderStatus.Canceled
                || order.Status == OrderStatus.Error
                || order.Status == OrderStatus.Filled)
            {
                activeOrderList.Remove(order);
                doneOrderList.Add(order);
            }
        }

        //订单生成函数
        public Order BuyOrder(int vol)
        {
            if (lastTickMap.ContainsKey(mMainInstID))
            {
                double price = 0;
                price = lastTickMap[mMainInstID].UpperLimitPrice;
                return BuyOrder(vol, price);
            }
            else
            {
                return BuyOrder(0, 0);
            }

        }
        public Order BuyOrder(int vol, double price)
        {
            return BuyOrder(vol, price, mMainInstID);
        }
        public Order BuyOrder(int vol, double price, string instrumentID)
        {
            if (vol < 0)
            {
                return BuyOrder(0, price, instrumentID);
            }
            Order order = new Order(this, instrumentID, DirectionType.Buy, price, DateTime.Now, vol, vol, OrderStatus.Normal);
            return order;
        }
        public Order SellOrder(int vol)
        {
            if (lastTickMap.ContainsKey(mMainInstID))
            {
                double price = 0;
                price = lastTickMap[mMainInstID].LowerLimitPrice;
                return SellOrder(vol, price);
            }
            else
            {
                return SellOrder(0, 0);
            }

        }
        public Order SellOrder(int vol, double price)
        {
            return SellOrder(vol, price, mMainInstID);
        }
        public Order SellOrder(int vol, double price, string instrumentID)
        {
            if (vol < 0)
            {
                return SellOrder(0, price, instrumentID);
            }
            Order order = new Order(this, instrumentID, DirectionType.Sell, price, DateTime.Now, vol, vol, OrderStatus.Normal);
            return order;
        }
        public Order ToPositionOrder(int position)
        {
            int myPos = GetPosition(mMainInstID);
            if (position > myPos && lastTickMap.ContainsKey(mMainInstID))
            {
                return BuyOrder(position - myPos, lastTickMap[mMainInstID].UpperLimitPrice, mMainInstID);
            }
            else if (position < myPos && lastTickMap.ContainsKey(mMainInstID))
            {
                return SellOrder(myPos - position, lastTickMap[mMainInstID].LowerLimitPrice, mMainInstID);
            }
            else
            {
                return BuyOrder(0);
            }
        }
        public Order ToPositionOrder(int position, double price)
        {
            return ToPositionOrder(position, price, mMainInstID);
        }
        public Order ToPositionOrder(int position, double price, string instrumentID)
        {
            int myPos = GetPosition(mMainInstID);
            if (position > myPos)
            {
                return BuyOrder(position - myPos, price, instrumentID);
            }
            else if (position < myPos)
            {
                return SellOrder(myPos - position, price, instrumentID);
            }
            else
            {
                return BuyOrder(0);
            }
        }
    }

    //其他
    public partial class BaseStrategy
    {
        public void Log(string content)
        {
            Utils.UserLog(content);
        }
    }
}
