﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using HaiFeng;

namespace QuantEngine
{
    internal class CtpTdProvider : ITdProvider
    {
        private Account mAccount;
        private CTPTrade mTrader = new CTPTrade();

        private int customID = 1; //自增编码，用来定位订单回报
        Dictionary<int, SubOrder> orderMap = new Dictionary<int, SubOrder>(); //订单
        List<SubOrder> activeOrders = new List<SubOrder>(); //活跃订单

        private static CtpTdProvider instance = new CtpTdProvider();

        public static CtpTdProvider Instance
        {
            get
            {
                return instance;
            }
        }
        private CtpTdProvider() { }

        //登陆
        public void Login(Account account)
        {
            if (account != null && mTrader.IsLogin)
                return;

            mAccount = account;

            //连接
            mTrader.OnFrontConnected += (object sender, EventArgs e) =>
            {
                mTrader.ReqUserLogin(account.Investor, account.Password, account.Broker);
                Utils.EnginLog("ctptd:OnFrontConnected");
            };
            //登入
            mTrader.OnRspUserLogin += (object sender, IntEventArgs e) =>
            {
                if(e.Value != 0)
                {
                    Logout();
                    Thread.Sleep(1000);
                    mTrader = new CTPTrade();
                    Login(mAccount);
                }
                Utils.EnginLog("ctptd:OnRspUserLogin:" + e.Value);
            };
            //登出
            mTrader.OnRspUserLogout += (object sender, IntEventArgs e) =>
            {
                Utils.EnginLog("ctptd:OnRspUserLogout");
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

            //开始连接
            mTrader.ReqConnect(mAccount.Server);
        }
        //登出
        public void Logout()
        {
            mTrader.ReqUserLogout();
        }
        //是否登陆
        public bool IsLogin()
        {
            return mTrader.IsLogin;
        }
        //发送订单
        public void SendOrder(Order order)
        {
            //未登录，则返回
            if (!IsLogin())
            {
                Utils.EnginLog("ctptd:交易未登录，下单失败");
                return;
            }

            //
            createSubOrder(order);
            foreach (SubOrder subOrder in order.SubOrders)
            {
                SendOrder(subOrder);
            }
        }
        //生成子订单
        private void createSubOrder(Order order)
        {
            List<SubOrder> subOrderList = new List<SubOrder>();

            PositionField position;
            if (mTrader.DicPositionField.TryGetValue(order.InstrumentID + "_" + (order.Direction == DirectionType.Buy ? "Sell" : "Buy"), out position))
            {
                //计算冻结数量
                int frozenTd = 0;
                int frozenYd = 0;

                foreach (SubOrder subOrder in activeOrders)
                {
                    if (order.Direction == subOrder.Direction)
                    {
                        frozenTd += subOrder.Offset == OffsetType.CloseToday ? subOrder.VolumeLeft : 0;
                        frozenYd += subOrder.Offset == OffsetType.Close ? subOrder.VolumeLeft : 0;
                    }
                }

                //剩余数量
                int volLeft = order.Volume;

                //先平今
                if (volLeft <= 0)
                {
                    order.SubOrders = subOrderList;
                    return;
                }
                int posLeft = position.TdPosition > frozenTd ? position.TdPosition - frozenTd : 0;
                int vol = posLeft > volLeft ? volLeft : posLeft;
                volLeft -= vol;
                if (vol > 0)
                {
                    SubOrder subOrder = new SubOrder(pOrder: order,
                    instrumentID: order.InstrumentID,
                    direction: order.Direction,
                    offset: OffsetType.CloseToday,
                    limitPrice: order.Price,
                    insertTime: DateTime.Now,
                    volume: vol,
                    volumeLeft: vol,
                    status: OrderStatus.Normal);

                    subOrderList.Add(subOrder);
                }

                //后平仓
                if (volLeft <= 0)
                {
                    order.SubOrders = subOrderList;
                    return;
                }
                posLeft = position.YdPosition > frozenYd ? position.YdPosition - frozenYd : 0;
                vol = posLeft > volLeft ? volLeft : posLeft;
                volLeft -= vol;
                if (vol > 0)
                {
                    SubOrder subOrder = new SubOrder(pOrder: order,
                    instrumentID: order.InstrumentID,
                    direction: order.Direction,
                    offset: OffsetType.Close,
                    limitPrice: order.Price,
                    insertTime: DateTime.Now,
                    volume: vol,
                    volumeLeft: vol,
                    status: OrderStatus.Normal);

                    subOrderList.Add(subOrder);
                }

                //再开仓
                if (volLeft <= 0)
                {
                    order.SubOrders = subOrderList;
                    return;
                }
                vol = volLeft;
                if (vol > 0)
                {
                    SubOrder subOrder = new SubOrder(pOrder: order,
                    instrumentID: order.InstrumentID,
                    direction: order.Direction,
                    offset: OffsetType.Open,
                    limitPrice: order.Price,
                    insertTime: DateTime.Now,
                    volume: vol,
                    volumeLeft: vol,
                    status: OrderStatus.Normal);

                    subOrderList.Add(subOrder);
                }
            }
            else
            {
                SubOrder subOrder = new SubOrder(pOrder: order,
                instrumentID: order.InstrumentID,
                direction: order.Direction,
                offset: OffsetType.Open,
                limitPrice: order.Price,
                insertTime: DateTime.Now,
                volume: order.Volume,
                volumeLeft: order.Volume,
                status: OrderStatus.Normal);

                subOrderList.Add(subOrder);

            }

            order.SubOrders = subOrderList;
        }
        //撤销订单
        public void CancelOrder(Order order)
        {
            foreach (SubOrder subOrder in order.SubOrders)
            {
                if (subOrder.OrderID.Equals(string.Empty))
                {
                    Utils.EnginLog($"撤单错误|未找到订单编号：{subOrder.InstrumentID}|{subOrder.Direction}|{subOrder.Offset}|{subOrder.LimitPrice}|{subOrder.Volume}|{subOrder.CustomID}");
                    continue;
                }
                int rtn;
                rtn = mTrader.ReqOrderAction(subOrder.OrderID);
                Utils.EnginLog($"撤单：{rtn}|{subOrder.InstrumentID}|{subOrder.Direction}|{subOrder.Offset}|{subOrder.LimitPrice}|{subOrder.Volume}|{subOrder.CustomID}|{subOrder.OrderID}");
            }
        }
        //发送订单
        private void SendOrder(SubOrder subOrder)
        {
            //自增编码
            subOrder.CustomID = customID++;
            orderMap.Add(subOrder.CustomID, subOrder);
            activeOrders.Add(subOrder);

            //转换
            HaiFeng.DirectionType direction = subOrder.Direction == DirectionType.Buy ? HaiFeng.DirectionType.Buy : HaiFeng.DirectionType.Sell;
            HaiFeng.OffsetType offset = HaiFeng.OffsetType.Open;
            switch (subOrder.Offset)
            {
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

            Utils.EnginLog($"发单：{rtn}\t{subOrder.CustomID}\t{subOrder.InstrumentID}\t{subOrder.Direction}\t{subOrder.Offset}\t{subOrder.LimitPrice}\t{subOrder.Volume}");
        }
        //撤销订单
        private void CancelOrder(SubOrder subOrder)
        {
            if (subOrder.OrderID == string.Empty)
            {
                Utils.EnginLog($"撤单|错误|还未更新orderID：{subOrder.CustomID}|{subOrder.InstrumentID}|{subOrder.Direction}|{subOrder.Offset}|{subOrder.LimitPrice}");
                return;
            }
            mTrader.ReqOrderAction(subOrder.OrderID);
            Utils.EnginLog($"撤单：{subOrder.OrderID}|{subOrder.InstrumentID}|{subOrder.Direction}|{subOrder.Offset}|{subOrder.LimitPrice}|{subOrder.Volume}|{subOrder.VolumeLeft}");
        }
        //撤单回报
        private void _OnRtnCancel(object sender, OrderArgs e)
        {
            OrderField order = e.Value;
            SubOrder subOrder;
            if (!orderMap.TryGetValue(order.Custom, out subOrder))
            {
                Utils.EnginLog($"撤单回报|未找到对应本地单：{order.Custom}\t{order.InstrumentID}\t{order.Direction}\t{order.Offset}\t{order.LimitPrice}\t{order.Volume}");
                return;
            }
            subOrder.EmitCancel();
            Utils.EnginLog($"撤单回报：{order.Custom}\t{order.InstrumentID}\t{order.Direction}\t{order.Offset}\t{order.LimitPrice}\t{order.Volume}");
        }
        //撤单错误
        private void _OnRtnErrCancel(object sender, ErrOrderArgs e)
        {
            OrderField order = e.Value;
            SubOrder subOrder;
            if (!orderMap.TryGetValue(order.Custom, out subOrder))
            {
                Utils.EnginLog($"撤单错误回报|未找到对应本地单：{order.Custom}\t{order.InstrumentID}\t{order.Direction}\t{order.Offset}\t{order.LimitPrice}\t{order.Volume}\t{e.ErrorID}\t{e.ErrorMsg}");
                return;
            }
            subOrder.EmitCancelFailed();
            Utils.EnginLog($"撤单错误回报：{order.Custom}\t{order.InstrumentID}\t{order.Direction}\t{order.Offset}\t{order.LimitPrice}\t{order.Volume}\t{e.ErrorID}\t{e.ErrorMsg}");

        }
        //交易回报
        private void _OnRtnTrade(object sender, TradeArgs e)
        {
            TradeField trade = e.Value;
            Utils.EnginLog($"交易回报：{trade.OrderID}\t{trade.InstrumentID}\t{trade.Direction}\t{trade.Offset}\t{trade.Price}\t{trade.Volume}");
        }
        //订单回报
        private void _OnRtnOrder(object sender, OrderArgs e)
        {
            OrderField order = e.Value;
            SubOrder subOrder;
            if (!orderMap.TryGetValue(order.Custom, out subOrder))
            {
                Utils.EnginLog($"订单回报|未找到对应本地单：{order.Custom}\t{order.InstrumentID}\t{order.Direction}\t{order.Offset}\t{order.LimitPrice}\t{order.Volume}");
                return;
            }

            //更新属性
            subOrder.OrderID = order.OrderID;
            int beforeTraded = subOrder.VolumeTraded;
            subOrder.VolumeLeft = order.VolumeLeft;
            switch (order.Status)
            {
                case HaiFeng.OrderStatus.Canceled:
                    subOrder.Status = OrderStatus.Canceled;
                    activeOrders.Remove(subOrder);
                    break;
                case HaiFeng.OrderStatus.Filled:
                    subOrder.Status = OrderStatus.Filled;
                    subOrder.VolumeTraded = subOrder.Volume;
                    activeOrders.Remove(subOrder);
                    break;
                case HaiFeng.OrderStatus.Error:
                    subOrder.Status = OrderStatus.Error;
                    activeOrders.Remove(subOrder);
                    break;
                case HaiFeng.OrderStatus.Normal:
                    subOrder.Status = OrderStatus.Normal;
                    break;
                case HaiFeng.OrderStatus.Partial:
                    subOrder.VolumeTraded = subOrder.Volume - subOrder.VolumeLeft;
                    subOrder.Status = OrderStatus.Partial;
                    break;

            }

            //发送事件
            int tradeVol = subOrder.VolumeTraded - beforeTraded;
            if (tradeVol > 0)
            {
                subOrder.EmitTrade(tradeVol);
            }

            Utils.EnginLog($"订单回报：{order.Custom}\t{order.InstrumentID}\t{order.Direction}\t{order.Offset}\t{order.LimitPrice}\t{order.Volume}");
        }
        //报单错误回报
        private void _OnRtnErrOrder(object sender, ErrOrderArgs e)
        {
            OrderField order = e.Value;
            SubOrder subOrder;
            if (!orderMap.TryGetValue(order.Custom, out subOrder))
            {
                Utils.EnginLog($"报单错误回报|未找到对应本地单：{order.Custom}\t{order.InstrumentID}\t{order.Direction}\t{order.Offset}\t{order.LimitPrice}\t{order.Volume}\t{e.ErrorID}\t{e.ErrorMsg}");
                return;
            }
            activeOrders.Remove(subOrder);
            subOrder.EmitError();
            Utils.EnginLog($"报单错误回报：{order.Custom}\t{order.InstrumentID}\t{order.Direction}\t{order.Offset}\t{order.LimitPrice}\t{order.Volume}\t{e.ErrorID}\t{e.ErrorMsg}");

        }
    }
}
