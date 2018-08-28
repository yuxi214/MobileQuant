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
        public virtual void OnPositionChanged(string instrumentID,int position) { }
    }

    //行情
    public partial class BaseStrategy
    {
        //合约列表
        private string mMainInstID = string.Empty;
        private HashSet<string> mInstIDSet;

        //最新tick
        private Dictionary<string, Tick> lastTickMap = new Dictionary<string, Tick>();

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
            lastTickMap[tick.InstrumentID] = tick;
            OnTick(tick);
        }
    }

    //交易
    internal delegate void OnOrderSend(Order order);
    public partial class BaseStrategy
    {
        //持仓
        private int mPosition = 0;
        public int Position
        {
            get
            {
                return mPosition;
            }
        }

        //订单
        private List<Order> orderList = new List<Order>();
        private List<Order> activeOrderList = new List<Order>();
        private List<Order> doneOrderList = new List<Order>();
        //订单发送事件
        internal event OnOrderSend OnOrderSend;
        //发送订单
        internal void SendOrder(Order order)
        {
            orderList.Add(order);
            activeOrderList.Add(order);
            OnOrderSend(order);
        }
        //更新订单
        internal void UpdateOrder(Order order)
        {
            if(order.Status == OrderStatus.Canceled
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
            Order order = new Order(this, instrumentID, DirectionType.Sell, price, DateTime.Now, vol, vol, OrderStatus.Normal);
            return order;
        }
        public Order ToPositionOrder(int position)
        {
            if (position > mPosition && lastTickMap.ContainsKey(mMainInstID))
            {
                return BuyOrder(position - mPosition, lastTickMap[mMainInstID].UpperLimitPrice, mMainInstID);
            }
            else if (position < mPosition && lastTickMap.ContainsKey(mMainInstID))
            {
                return SellOrder(mPosition - position, lastTickMap[mMainInstID].LowerLimitPrice, mMainInstID);
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
            if(position > mPosition)
            {
                return BuyOrder(position - mPosition, price, instrumentID);
            }
            else if(position < mPosition)
            {
                return SellOrder(mPosition - position, price, instrumentID);
            }
            else
            {
                return BuyOrder(0);
            }
        }
    }
}
