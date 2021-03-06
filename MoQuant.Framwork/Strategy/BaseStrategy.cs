﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MoQuant.Framwork.Data;

namespace MoQuant.Framwork.Strategy {
    //委托
    internal delegate void SubscribeInstrumentDelegate(BaseStrategy stg, string instrumentId);

    internal delegate void SendOrderDelegate(Order order);

    internal delegate void CancelOrderDelegate(Order order);

    internal delegate void ChangePositionDelegate(Position position);

    public partial class BaseStrategy {
        internal event SubscribeInstrumentDelegate OnSubscribeInstrument;
        internal event SendOrderDelegate OnSendOrder;
        internal event CancelOrderDelegate OnCancelOrder;
        internal event ChangePositionDelegate OnChangePosition;
    }

    //用户实现
    public partial class BaseStrategy {
        public virtual void OnStart() { }
        public virtual void OnStop() { }
        public virtual void OnTick(Tick tick) { }
        public virtual void OnPositionChanged(string instrumentID, int position) { }
    }

    //行情
    public partial class BaseStrategy {

        //合约列表
        private string mMainInstID = string.Empty;
        private HashSet<string> mInstIDSet = new HashSet<string>();

        //最新tick
        private Dictionary<string, Tick> lastTickDic = new Dictionary<string, Tick>();

        //订阅行情
        public void SubscribInstrument(string instId) {
            OnSubscribeInstrument?.Invoke(this, instId);
            mMainInstID = mMainInstID ?? instId;
            mInstIDSet.Add(instId);
            AddPosition(instId, DataManager.Instance.GetPosition(GetType().Name, instId));
        }

        //发送开始信号
        internal void SendStart() {
            //启动
            OnStart();
        }

        //发送tick
        internal void SendTick(Tick tick) {
            lastTickDic[tick.InstrumentId] = tick;
            OnTick(tick);
        }
    }

    //交易
    public partial class BaseStrategy {
        //持仓
        private Dictionary<Instrument, int> mPositionMap = new Dictionary<Instrument, int>();
        public int GetPosition(Instrument instrument) {
            if (mPositionMap.ContainsKey(instrument)) {
                return mPositionMap[instrument];
            } else {
                return 0;
            }
        }
        internal void AddPosition(Instrument instrument, int vol) {
            mPositionMap[instrument] = mPositionMap.ContainsKey(instrument) ? mPositionMap[instrument] + vol : vol;
            Position p = new Position(this, instrument, mPositionMap[instrument], DateTime.Now);
            OnChangePosition?.Invoke(p);
        }

        //订单
        private List<Order> orderList = new List<Order>();
        private List<Order> activeOrderList = new List<Order>();
        private List<Order> doneOrderList = new List<Order>();

        //发送订单
        internal void SendOrder(Order order) {
            orderList.Add(order);
            activeOrderList.Add(order);
            if (mTdProvider != null) {
                Tick t;
                double uper = 0;
                double lower = 0;
                if (lastTickDic.TryGetValue(order.InstrumentID, out t)) {
                    uper = t.UpperLimitPrice;
                    lower = t.LowerLimitPrice;
                }
                //
                Instrument inst;
                double priceTick = 0;
                if (mTdProvider.TryGetInstrument(order.InstrumentID, out inst)) {
                    priceTick = inst.PriceTick;
                }
                //
                if (priceTick != 0) {
                    order.Price = ((int)(order.Price / priceTick)) * priceTick;
                }
                if (order.Price > uper && uper != 0) {
                    order.Price = uper;
                } else if (order.Price < lower) {
                    order.Price = lower;
                }
                //
                mTdProvider.SendOrder(order);
            }
        }

        //撤单
        internal void CancleOrder(Order order) {
            OnCancelOrder?.Invoke(order);
        }

        //更新订单
        internal void UpdateOrder(Order order) {
            if (order.Status == OrderStatus.Canceled
                || order.Status == OrderStatus.Error
                || order.Status == OrderStatus.Filled) {
                activeOrderList.Remove(order);
                doneOrderList.Add(order);
            }
        }

        //获取合约
        public bool TryGetInstrument(string instrumentID, out Instrument inst) {
            return mTdProvider.TryGetInstrument(instrumentID, out inst);
        }

        //订单生成函数
        public Order BuyOrder(int vol) {
            if (lastTickDic.ContainsKey(mMainInstID)) {
                double price = 0;
                price = lastTickDic[mMainInstID].UpperLimitPrice;
                return BuyOrder(vol, price);
            } else {
                return BuyOrder(0, 0);
            }

        }
        public Order BuyOrder(int vol, double price) {
            return BuyOrder(vol, price, mMainInstID);
        }
        public Order BuyOrder(int vol, double price, string instrumentID) {
            if (vol < 0) {
                return BuyOrder(0, price, instrumentID);
            }
            Order order = new Order(this, instrumentID, DirectionType.Buy, price, vol);
            return order;
        }
        public Order SellOrder(int vol) {
            if (lastTickDic.ContainsKey(mMainInstID)) {
                double price = 0;
                price = lastTickDic[mMainInstID].LowerLimitPrice;
                return SellOrder(vol, price);
            } else {
                return SellOrder(0, 0);
            }

        }
        public Order SellOrder(int vol, double price) {
            return SellOrder(vol, price, mMainInstID);
        }
        public Order SellOrder(int vol, double price, string instrumentID) {
            if (vol < 0) {
                return SellOrder(0, price, instrumentID);
            }
            Order order = new Order(this, instrumentID, DirectionType.Sell, price, vol);
            return order;
        }
        public Order ToPositionOrder(int position) {
            int myPos = GetPosition(mMainInstID);
            if (position > myPos && lastTickDic.ContainsKey(mMainInstID)) {
                return BuyOrder(position - myPos, lastTickDic[mMainInstID].UpperLimitPrice, mMainInstID);
            } else if (position < myPos && lastTickDic.ContainsKey(mMainInstID)) {
                return SellOrder(myPos - position, lastTickDic[mMainInstID].LowerLimitPrice, mMainInstID);
            } else {
                return BuyOrder(0);
            }
        }
        public Order ToPositionOrder(int position, double price) {
            return ToPositionOrder(position, price, mMainInstID);
        }
        public Order ToPositionOrder(int position, double price, string instrumentID) {
            int myPos = GetPosition(mMainInstID);
            if (position > myPos) {
                return BuyOrder(position - myPos, price, instrumentID);
            } else if (position < myPos) {
                return SellOrder(myPos - position, price, instrumentID);
            } else {
                return BuyOrder(0);
            }
        }
    }

    //其他
    public partial class BaseStrategy {
        //写日志
        public void Log(string content) {
            LogUtils.UserLog(content);
        }
    }


}
