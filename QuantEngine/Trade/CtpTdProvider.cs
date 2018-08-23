using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HaiFeng;

namespace QuantEngine
{
    internal class CtpTdProvider : ITdProvider
    {
        private Account mAccount;
        private CTPTrade mTrader = new CTPTrade();

        private int customID = 1; //自增编码，用来定位订单回报
        Dictionary<int, SubOrder> orderMap = new Dictionary<int, SubOrder>();
        Dictionary<string, SubOrder> orderMap2 = new Dictionary<string, SubOrder>();

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
            mTrader.OnFrontConnected += (object sender, EventArgs e)=>
            {
                mTrader.ReqUserLogin(account.Investor, account.Password, account.Broker);
                Utils.Log("ctptd:OnFrontConnected");
            };
            //登入
            mTrader.OnRspUserLogin += (object sender, IntEventArgs e) =>
            {
                Utils.Log("ctptd:OnRspUserLogin:" + e.Value);
            };
            //登出
            mTrader.OnRspUserLogout += (object sender, IntEventArgs e) =>
            {
                Utils.Log("ctptd:OnRspUserLogout");
            };
            //订单回报
            mTrader.OnRtnOrder += _OnRtnOrder;
            //交易回报
            mTrader.OnRtnTrade += _OnRtnTrade;
            //撤单回报
            mTrader.OnRtnCancel += _OnRtnCancel;
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
        public void SendOrder(SubOrder subOrder)
        {
            //自增编码
            subOrder.CustomID = customID++;
            orderMap.Add(subOrder.CustomID,subOrder);

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
            int rtn = mTrader.ReqOrderInsert(pInstrument:subOrder.InstrumentID, 
                pDirection:direction, 
                pOffset:offset, 
                pPrice:subOrder.LimitPrice, 
                pVolume:subOrder.Volume, 
                pCustom:subOrder.CustomID, 
                pType:OrderType.Limit, 
                pHedge:HedgeType.Speculation);

            Utils.Log($"发单：{rtn}|{subOrder.InstrumentID}|{subOrder.Direction}|{subOrder.Offset}|{subOrder.LimitPrice}|{subOrder.Volume}|{subOrder.CustomID}");
        }
        //撤销订单
        public void CancelOrder(SubOrder subOrder)
        {
            if(subOrder.OrderID == string.Empty)
            {
                Utils.Log($"撤单|错误|还未更新orderID：{subOrder.CustomID}|{subOrder.InstrumentID}|{subOrder.Direction}|{subOrder.Offset}|{subOrder.LimitPrice}");
                return;
            }
            mTrader.ReqOrderAction(subOrder.OrderID);
            Utils.Log($"撤单：{subOrder.OrderID}|{subOrder.InstrumentID}|{subOrder.Direction}|{subOrder.Offset}|{subOrder.LimitPrice}|{subOrder.Volume}|{subOrder.VolumeLeft}");
        }

        //撤单回报
        private void _OnRtnCancel(object sender, OrderArgs e)
        {
            OrderField order = e.Value;
            Utils.Log($"撤单回报：{order.StatusMsg}\t{order.InstrumentID}\t{order.Direction}\t{order.Offset}\t{order.LimitPrice}\t{order.Volume}");
        }
        //交易回报
        private void _OnRtnTrade(object sender, TradeArgs e)
        {
            TradeField trade = e.Value;
            Utils.Log($"交易回报：{trade.InstrumentID}\t{trade.Direction}\t{trade.Offset}\t{trade.Price}\t{trade.Volume}");
        }
        //订单回报
        private void _OnRtnOrder(object sender, OrderArgs e)
        {
            OrderField order = e.Value;
            SubOrder subOrder;
            if(!orderMap.TryGetValue(order.Custom,out subOrder))
            {
                Utils.Log($"订单回报|未找到对应本地单：{order.InstrumentID}\t{order.Direction}\t{order.Offset}\t{order.LimitPrice}\t{order.Volume}");
                return;
            }

            //更新属性
            subOrder.OrderID = order.OrderID;
            bool cancle = false;
            bool error = false;
            bool filled = false;
            long tradeVol = subOrder.VolumeLeft - order.VolumeLeft;
            subOrder.VolumeLeft = order.VolumeLeft;
            switch (order.Status)
            {
                case HaiFeng.OrderStatus.Canceled:
                    cancle = subOrder.Status == OrderStatus.Canceled ? false : true;
                    subOrder.Status = OrderStatus.Canceled;
                    break;
                case HaiFeng.OrderStatus.Error:
                    error = subOrder.Status == OrderStatus.Error ? false : true;
                    subOrder.Status = OrderStatus.Error;
                    break;
                case HaiFeng.OrderStatus.Filled:
                    filled = subOrder.Status == OrderStatus.Filled ? false : true;
                    subOrder.Status = OrderStatus.Filled;
                    break;
                case HaiFeng.OrderStatus.Normal:
                    subOrder.Status = OrderStatus.Normal;
                    break;
                case HaiFeng.OrderStatus.Partial:
                    subOrder.Status = OrderStatus.Partial;
                    break;
            }

            //发送事件
            if(tradeVol > 0)
            {
                subOrder.EmitTrade(tradeVol);
            }
            if (cancle)
            {
                subOrder.EmitCancel();
            }
            if (error)
            {
                subOrder.EmitError();
            }


            Utils.Log($"订单回报：{order.InstrumentID}\t{order.Direction}\t{order.Offset}\t{order.LimitPrice}\t{order.Volume}");
        }
    }
}
