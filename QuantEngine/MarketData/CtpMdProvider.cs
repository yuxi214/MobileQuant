using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using HaiFeng;

namespace QuantEngine
{
    class CtpMdProvider : BaseMdProvider, IMdProvider
    {
        private Account mAccount;
        private Quote mQuoter = new CTPQuote();

        private static CtpMdProvider instance = new CtpMdProvider();

        public static CtpMdProvider Instance
        {
            get
            {
                return instance;
            }
        }

        //登陆
        public bool Login(Account account)
        {
            if (account != null && mQuoter.IsLogin)
                return true;

            mAccount = account;

            //登陆线程
            Semaphore _s = new Semaphore(1, 1);
            new Thread(new ThreadStart(() =>
            {
                _s.WaitOne();

                //
                mQuoter.OnFrontConnected += (object sender, EventArgs e) =>
                {
                    mQuoter.ReqUserLogin(account.Investor, account.Password, account.Broker);
                    Utils.Log("OnFrontConnected");
                };
                mQuoter.OnRspUserLogin += (object sender, IntEventArgs e) =>
                {
                    _s.Release();
                    Utils.Log("OnRspUserLogin:"+e.Value);
                };
                mQuoter.OnRspUserLogout += (object sender, IntEventArgs e) =>
                {
                    Utils.Log("OnRspUserLogout");
                };
                mQuoter.OnRtnTick += (object sender, TickEventArgs e) =>
                {
                };
                mQuoter.OnRtnError += (object sender, ErrorEventArgs e) =>
                {
                    Utils.Log("OnFrontConnected："+e.ErrorMsg);
                };

                //
                mQuoter.ReqConnect(account.Server);


            })).Start();

            //等待登陆结果
            Thread.Sleep(500);
            _s.WaitOne();
            return mQuoter.IsLogin;

        }

        //登出
        public void Logout()
        {
            mQuoter.ReqUserLogout();
        }
        //是否登陆
        public bool IsLogin()
        {
            return mQuoter.IsLogin;
        }
        //订阅行情
        public void SubscribeMarketData(String instrumentID)
        {
            mQuoter.ReqSubscribeMarketData(instrumentID);
        }
        //退订行情
        public void UnSubscribeMarketData(String instrumentID)
        {
            mQuoter.ReqUnSubscribeMarketData(instrumentID);
        }
    }
}
