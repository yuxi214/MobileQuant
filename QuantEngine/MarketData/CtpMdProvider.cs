using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using HaiFeng;

namespace QuantEngine
{
    class CtpMdProvider :  IMdProvider
    {
        private Account mAccount;
        private Quote mQuoter = new CTPQuote();

        private static CtpMdProvider instance = new CtpMdProvider();

        public event RtnTick OnTick;

        public static CtpMdProvider Instance
        {
            get
            {
                return instance;
            }
        }

        private CtpMdProvider() { }

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

                //前置连接回调
                mQuoter.OnFrontConnected += (object sender, EventArgs e) =>
                {
                    mQuoter.ReqUserLogin(account.Investor, account.Password, account.Broker);
                    Utils.Log("OnFrontConnected");
                };
                //登陆回调
                mQuoter.OnRspUserLogin += (object sender, IntEventArgs e) =>
                {
                    _s.Release();
                    Utils.Log("OnRspUserLogin:"+e.Value);
                };
                //登出回调
                mQuoter.OnRspUserLogout += (object sender, IntEventArgs e) =>
                {
                    Utils.Log("OnRspUserLogout");
                };
                //行情回调
                mQuoter.OnRtnTick += (object sender, TickEventArgs e) =>
                {
                    MarketData _md = e.Tick;

                    //处理时间
                    DateTime _time = DateTime.Now;
                    try
                    {
                        _time = DateTime.ParseExact(_md.UpdateTime, "yyyyMMdd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                    }
                    catch(Exception ex)
                    {
                        Utils.Log(ex.StackTrace);
                    }
                    _time.AddMilliseconds(_md.UpdateMillisec);


                    Tick _tick = new Tick(_md.InstrumentID, _md.LastPrice, _md.BidPrice, _md.BidVolume, _md.AskPrice, _md.AskVolume
                        , _md.AveragePrice, _md.Volume, _md.OpenInterest, _time, _md.UpperLimitPrice, _md.LowerLimitPrice);

                    //发射
                    OnTick(_tick);
                };
                mQuoter.OnRtnError += (object sender, ErrorEventArgs e) =>
                {
                    Utils.Log("OnRtnError：" + e.ErrorMsg);
                };

                //开始连接
                mQuoter.ReqConnect(account.Server);


            })).Start();

            //等待登陆结果
            Thread.Sleep(500);
            _s.WaitOne();
            _s.Release();
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
