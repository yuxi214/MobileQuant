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

        }
        //是否登陆
        public bool IsLogin()
        {
            return mTrader.IsLogin;
        }
        //发送订单
        public void SendOrder(SubOrder subOrder)
        {

        }
        //撤销订单
        public void CancelOrder(SubOrder subOrder)
        {

        }

        //撤单回报
        private void _OnRtnCancel(object sender, OrderArgs e)
        {
            OrderField order = e.Value;
            Utils.Log($"{order.StatusMsg}\t{order.InstrumentID}\t{order.Direction}\t{order.Offset}\t{order.LimitPrice}\t{order.Volume}");
        }
        //交易回报
        private void _OnRtnTrade(object sender, TradeArgs e)
        {
            TradeField trade = e.Value;
            Utils.Log($"{trade.InstrumentID}\t{trade.Direction}\t{trade.Offset}\t{trade.Price}\t{trade.Volume}");
        }
        //订单回报
        private void _OnRtnOrder(object sender, OrderArgs e)
        {
            OrderField order = e.Value;
            Utils.Log($"{order.InstrumentID}\t{order.Direction}\t{order.Offset}\t{order.LimitPrice}\t{order.Volume}");
        }
    }
}
