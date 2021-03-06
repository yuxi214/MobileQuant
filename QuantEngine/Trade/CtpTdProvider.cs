﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using HaiFeng;

namespace QuantEngine {
    internal class CtpTdProvider : ITdProvider {
        private Timer mTimer;
        private Account mAccount;
        private CTPTrade mTrader;

        private int customID = 1; //自增编码，用来定位订单回报
        Dictionary<int, SubOrder> orderMap = new Dictionary<int, SubOrder>(); //订单
        List<SubOrder> activeOrders = new List<SubOrder>(); //活跃订单

        private static CtpTdProvider instance = new CtpTdProvider();

        public static CtpTdProvider Instance {
            get {
                return instance;
            }
        }
        private CtpTdProvider() {
            mTimer = new Timer(check, null, 1000 * 60, 1000 * 60);
        }

        //定时检查
        private void check(object o) {
            long now = DateTime.Now.Hour * 100 + DateTime.Now.Minute;

            //
            if (now > 231 && now < 845)
                return;
            if (now > 1516 && now < 2045)
                return;
            if (mTrader != null && mTrader.IsLogin)
                return;
            if (mAccount == null)
                return;

            //自动登陆
            Login(mAccount);
        }

        //登陆
        public void Login(Account account) {
            if (account == null)
                return;
            mAccount = account;

            if (mTrader != null && mTrader.IsLogin)
                return;

            //初始化
            mTrader = new CTPTrade();

            //连接
            mTrader.OnFrontConnected += (object sender, EventArgs e) => {
                mTrader.ReqUserLogin(mAccount.Investor, mAccount.Password, mAccount.Broker);
                LogUtils.EnginLog("ctptd:OnFrontConnected");
            };
            //登入
            mTrader.OnRspUserLogin += (object sender, IntEventArgs e) => {
                if (e.Value != 0) {
                    Logout();
                    mTrader = null;
                }
                LogUtils.EnginLog("ctptd:OnRspUserLogin:" + e.Value);
            };
            //登出
            mTrader.OnRspUserLogout += (object sender, IntEventArgs e) => {
                LogUtils.EnginLog("ctptd:OnRspUserLogout");
            };
            //订单回报
            mTrader.OnRtnOrder += _OnRtnOrder;
            //报单错误回报
            mTrader.OnRtnErrOrder += _OnRtnErrOrder;
            //交易回报
            mTrader.OnRtnTrade += _OnRtnTrade;
            //撤单回报
            mTrader.OnRtnCancel += _OnRtnCancel;
            //撤单错误
            mTrader.OnRtnErrCancel += _OnRtnErrCancel;
            mTrader.OnRtnExchangeStatus += _OnRtnExchangeStatus;
            //开始连接
            mTrader.ReqConnect(mAccount.Server);
        }
        //登出
        public void Logout() {
            mTrader.ReqUserLogout();
        }
        //是否登陆
        public bool IsLogin() {
            return mTrader.IsLogin;
        }
        //发送订单
        public void SendOrder(Order order) {
            //未登录，则返回
            if (!IsLogin()) {
                LogUtils.EnginLog("ctptd:交易未登录，下单失败");
                return;
            }

            //
            createSubOrder(order);
            foreach (SubOrder subOrder in order.SubOrders) {
                SendOrder(subOrder);
            }
        }
        //生成子订单
        private void createSubOrder(Order order) {
            PositionField position;
            if (mTrader.DicPositionField.TryGetValue(order.InstrumentID + "_" + (order.Direction == DirectionType.Buy ? "Sell" : "Buy"), out position)) {
                //计算冻结数量
                int frozenTd = 0;
                int frozenYd = 0;

                foreach (SubOrder subOrder in activeOrders) {
                    if (order.Direction == subOrder.Direction) {
                        frozenTd += subOrder.Offset == OffsetType.CloseToday ? subOrder.VolumeLeft : 0;
                        frozenYd += subOrder.Offset == OffsetType.Close ? subOrder.VolumeLeft : 0;
                    }
                }

                //剩余数量
                int volLeft = order.Volume;

                //先平今
                if (volLeft <= 0) {
                    return;
                }
                int posLeft = position.TdPosition > frozenTd ? position.TdPosition - frozenTd : 0;
                int vol = posLeft > volLeft ? volLeft : posLeft;
                volLeft -= vol;
                if (vol > 0) {
                    SubOrder subOrder = new SubOrder(pOrder: order,
                    instrumentID: order.InstrumentID,
                    direction: order.Direction,
                    offset: OffsetType.CloseToday,
                    limitPrice: order.Price,
                    insertTime: DateTime.Now,
                    volume: vol,
                    volumeLeft: vol,
                    status: OrderStatus.Normal);

                    order.AddSubOrder(subOrder);
                }

                //后平仓
                if (volLeft <= 0) {
                    return;
                }
                posLeft = position.YdPosition > frozenYd ? position.YdPosition - frozenYd : 0;
                vol = posLeft > volLeft ? volLeft : posLeft;
                volLeft -= vol;
                if (vol > 0) {
                    SubOrder subOrder = new SubOrder(pOrder: order,
                    instrumentID: order.InstrumentID,
                    direction: order.Direction,
                    offset: OffsetType.Close,
                    limitPrice: order.Price,
                    insertTime: DateTime.Now,
                    volume: vol,
                    volumeLeft: vol,
                    status: OrderStatus.Normal);

                    order.AddSubOrder(subOrder);
                }

                //再开仓
                if (volLeft <= 0) {
                    return;
                }
                vol = volLeft;
                if (vol > 0) {
                    SubOrder subOrder = new SubOrder(pOrder: order,
                    instrumentID: order.InstrumentID,
                    direction: order.Direction,
                    offset: OffsetType.Open,
                    limitPrice: order.Price,
                    insertTime: DateTime.Now,
                    volume: vol,
                    volumeLeft: vol,
                    status: OrderStatus.Normal);

                    order.AddSubOrder(subOrder);
                }
            } else {
                SubOrder subOrder = new SubOrder(pOrder: order,
                instrumentID: order.InstrumentID,
                direction: order.Direction,
                offset: OffsetType.Open,
                limitPrice: order.Price,
                insertTime: DateTime.Now,
                volume: order.Volume,
                volumeLeft: order.Volume,
                status: OrderStatus.Normal);

                order.AddSubOrder(subOrder);

            }
        }
        //撤销订单
        public void CancelOrder(Order order) {
            if (order.SubOrders == null)
                return;
            foreach (SubOrder subOrder in order.SubOrders) {
                cancelOrder(subOrder);
            }
        }
        //发送订单
        private void SendOrder(SubOrder subOrder) {
            //自增编码
            subOrder.CustomID = customID++;
            orderMap.Add(subOrder.CustomID, subOrder);
            activeOrders.Add(subOrder);

            //转换
            HaiFeng.DirectionType direction = subOrder.Direction == DirectionType.Buy ? HaiFeng.DirectionType.Buy : HaiFeng.DirectionType.Sell;
            HaiFeng.OffsetType offset = HaiFeng.OffsetType.Open;
            switch (subOrder.Offset) {
                case OffsetType.Open:
                    offset = HaiFeng.OffsetType.Open;
                    break;
                case OffsetType.Close:
                    offset = HaiFeng.OffsetType.Close;
                    break;
                case OffsetType.CloseToday:
                    offset = HaiFeng.OffsetType.CloseToday;
                    break;
            }

            //报单
            int rtn = mTrader.ReqOrderInsert(pInstrument: subOrder.InstrumentID,
                pDirection: direction,
                pOffset: offset,
                pPrice: subOrder.LimitPrice,
                pVolume: subOrder.Volume,
                pCustom: subOrder.CustomID,
                pType: OrderType.Limit,
                pHedge: HedgeType.Speculation);

            LogUtils.EnginLog($"发单：{rtn}\t{subOrder.CustomID}\t{subOrder.InstrumentID}\t{subOrder.Direction}\t{subOrder.Offset}\t{subOrder.LimitPrice}\t{subOrder.Volume}");
        }
        //撤销订单
        private void cancelOrder(SubOrder subOrder) {
            //
            if (subOrder.OrderID.Equals(string.Empty)) {
                LogUtils.EnginLog($"撤单错误|未找到订单编号：{subOrder.InstrumentID}|{subOrder.Direction}|{subOrder.Offset}|{subOrder.LimitPrice}|{subOrder.Volume}|{subOrder.CustomID}");
                return;
            }
            //
            if (subOrder.Status == OrderStatus.Canceled
                || subOrder.Status == OrderStatus.Error
                || subOrder.Status == OrderStatus.Filled) {
                return;
            }
            //
            int rtn;
            rtn = mTrader.ReqOrderAction(subOrder.OrderID);

            //这种情况多是盘后未撤订单，按撤单处理
            if (rtn == -1) {
                subOrder.Status = OrderStatus.Canceled;
                subOrder.VolumeLeft = 0;
                activeOrders.Remove(subOrder);
                subOrder.Refresh();
            }

            LogUtils.EnginLog($"撤单：{rtn}|{subOrder.InstrumentID}|{subOrder.Direction}|{subOrder.Offset}|{subOrder.LimitPrice}|{subOrder.Volume}|{subOrder.CustomID}|{subOrder.OrderID}");
        }
        //获取合约信息
        public bool TryGetInstrument(string instrumentID, out Instrument inst) {
            InstrumentField field;
            if (mTrader.DicInstrumentField.TryGetValue(instrumentID, out field)) {
                Exchange exchange;
                switch (field.ExchangeID) {
                    case HaiFeng.Exchange.CFFEX:
                        exchange = Exchange.CFFEX;
                        break;
                    case HaiFeng.Exchange.CZCE:
                        exchange = Exchange.CZCE;
                        break;
                    case HaiFeng.Exchange.DCE:
                        exchange = Exchange.DCE;
                        break;
                    case HaiFeng.Exchange.INE:
                        exchange = Exchange.INE;
                        break;
                    case HaiFeng.Exchange.SHFE:
                        exchange = Exchange.SHFE;
                        break;
                    default:
                        exchange = Exchange.CFFEX;
                        break;
                }
                inst = new Instrument(instrumentID: field.InstrumentID
                    , productID: field.ProductID
                    , exchange: exchange
                    , volumeMultiple: field.VolumeMultiple
                    , priceTick: field.PriceTick
                    , maxOrderVolume: field.MaxOrderVolume);
                return true;
            } else {
                inst = null;
                return false;
            }
        }
        //撤单回报
        private void _OnRtnCancel(object sender, OrderArgs e) {
            while (0 != Interlocked.Exchange(ref BaseStrategy.Locker, 1)) { }
            OrderField order = e.Value;
            SubOrder subOrder;
            if (!orderMap.TryGetValue(order.Custom, out subOrder)) {
                LogUtils.EnginLog($"撤单回报|未找到对应本地单：{order.Custom}\t{order.InstrumentID}\t{order.Direction}\t{order.Offset}\t{order.LimitPrice}\t{order.Volume}");
                Interlocked.Exchange(ref BaseStrategy.Locker, 0);
                return;
            }
            syncOrder(order, subOrder);
            LogUtils.EnginLog($"撤单回报：{order.Custom}\t{order.InstrumentID}\t{order.Direction}\t{order.Offset}\t{order.LimitPrice}\t{order.Volume}");
            Interlocked.Exchange(ref BaseStrategy.Locker, 0);
        }
        //撤单错误
        private void _OnRtnErrCancel(object sender, ErrOrderArgs e) {
            while (0 != Interlocked.Exchange(ref BaseStrategy.Locker, 1)) { }
            OrderField order = e.Value;
            SubOrder subOrder;
            if (!orderMap.TryGetValue(order.Custom, out subOrder)) {
                LogUtils.EnginLog($"撤单错误回报|未找到对应本地单：{order.Custom}\t{order.InstrumentID}\t{order.Direction}\t{order.Offset}\t{order.LimitPrice}\t{order.Volume}\t{e.ErrorID}\t{e.ErrorMsg}");
                Interlocked.Exchange(ref BaseStrategy.Locker, 0);
                return;
            }
            syncOrder(order, subOrder);

            LogUtils.EnginLog($"撤单错误回报：{order.Custom}\t{order.InstrumentID}\t{order.Direction}\t{order.Offset}\t{order.LimitPrice}\t{order.Volume}\t{e.ErrorID}\t{e.ErrorMsg}");
            Interlocked.Exchange(ref BaseStrategy.Locker, 0);
        }
        //交易回报
        private void _OnRtnTrade(object sender, TradeArgs e) {
            while (0 != Interlocked.Exchange(ref BaseStrategy.Locker, 1)) { }
            TradeField trade = e.Value;
            LogUtils.EnginLog($"交易回报：{trade.OrderID}\t{trade.InstrumentID}\t{trade.Direction}\t{trade.Offset}\t{trade.Price}\t{trade.Volume}");
            Interlocked.Exchange(ref BaseStrategy.Locker, 0);
        }
        //订单回报
        private void _OnRtnOrder(object sender, OrderArgs e) {
            while (0 != Interlocked.Exchange(ref BaseStrategy.Locker, 1)) { }
            OrderField order = e.Value;
            SubOrder subOrder;
            if (!orderMap.TryGetValue(order.Custom, out subOrder)) {
                LogUtils.EnginLog($"订单回报|未找到对应本地单：{order.Custom}\t{order.InstrumentID}\t{order.Direction}\t{order.Offset}\t{order.LimitPrice}\t{order.Volume}");
                Interlocked.Exchange(ref BaseStrategy.Locker, 0);
                return;
            }
            syncOrder(order, subOrder);
            LogUtils.EnginLog($"订单回报：{order.Custom}\t{order.InstrumentID}\t{order.Direction}\t{order.Offset}\t{order.LimitPrice}\t{order.Volume}");
            Interlocked.Exchange(ref BaseStrategy.Locker, 0);
        }
        //报单错误回报
        private void _OnRtnErrOrder(object sender, ErrOrderArgs e) {
            while (0 != Interlocked.Exchange(ref BaseStrategy.Locker, 1)) { }
            OrderField order = e.Value;
            SubOrder subOrder;
            if (!orderMap.TryGetValue(order.Custom, out subOrder)) {
                LogUtils.EnginLog($"报单错误回报|未找到对应本地单：{order.Custom}\t{order.InstrumentID}\t{order.Direction}\t{order.Offset}\t{order.LimitPrice}\t{order.Volume}\t{e.ErrorID}\t{e.ErrorMsg}");
                Interlocked.Exchange(ref BaseStrategy.Locker, 0);
                return;
            }
            activeOrders.Remove(subOrder);
            syncOrder(order, subOrder);
            LogUtils.EnginLog($"报单错误回报：{order.Custom}\t{order.InstrumentID}\t{order.Direction}\t{order.Offset}\t{order.LimitPrice}\t{order.Volume}\t{e.ErrorID}\t{e.ErrorMsg}");
            Interlocked.Exchange(ref BaseStrategy.Locker, 0);
        }
        //收盘
        private void _OnRtnExchangeStatus(object sender, StatusEventArgs e) {
            //收盘后统一视为撤单
            ExchangeStatusType status = e.Status;
            if (status == ExchangeStatusType.Closed) {
                foreach (SubOrder s in activeOrders) {
                    s.VolumeLeft = 0;
                    s.Status = OrderStatus.Canceled;
                    s.Refresh();
                }
            }
        }
        //同步订单
        private void syncOrder(OrderField o, SubOrder s) {
            //更新编号
            s.OrderID = o.OrderID;
            //只有正常和部成，才响应订单事件
            if (s.Status == OrderStatus.Normal || s.Status == OrderStatus.Partial) {
                switch (o.Status) {
                    case HaiFeng.OrderStatus.Canceled:
                        s.VolumeLeft = 0;
                        s.Status = OrderStatus.Canceled;
                        activeOrders.Remove(s);
                        break;
                    case HaiFeng.OrderStatus.Filled:
                        s.VolumeLeft = 0;
                        s.Status = OrderStatus.Filled;
                        s.VolumeTraded = s.Volume;
                        activeOrders.Remove(s);
                        break;
                    case HaiFeng.OrderStatus.Error:
                        s.VolumeLeft = 0;
                        s.Status = OrderStatus.Error;
                        activeOrders.Remove(s);
                        break;
                    case HaiFeng.OrderStatus.Partial:
                        s.VolumeLeft = o.VolumeLeft;
                        s.VolumeTraded = s.Volume - s.VolumeLeft;
                        s.Status = OrderStatus.Partial;
                        break;
                }
            }
            s.Refresh();

        }
    }
}
