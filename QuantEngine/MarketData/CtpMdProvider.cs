using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using HaiFeng;

namespace QuantEngine
{
    internal class CtpMdProvider : IMdProvider
    {
        private Account mAccount;
        private CTPQuote mQuoter = new CTPQuote();

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
        public void Login(Account account)
        {
            if (account != null && mQuoter.IsLogin)
                return;

            mAccount = account;

            //前置连接回调
            mQuoter.OnFrontConnected += (object sender, EventArgs e) =>
            {
                mQuoter.ReqUserLogin(account.Investor, account.Password, account.Broker);
                Utils.Log("ctpmd:OnFrontConnected");
            };
            //登陆回调
            mQuoter.OnRspUserLogin += (object sender, IntEventArgs e) =>
            {
                mQuoter.ReqSubscribeMarketData(mSubscribeMap.Keys.ToArray<string>());
                Utils.Log("ctpmd:OnRspUserLogin:" + e.Value);
            };
            //登出回调
            mQuoter.OnRspUserLogout += (object sender, IntEventArgs e) =>
            {
                Utils.Log("ctpmd:OnRspUserLogout");
            };
            //行情回调
            mQuoter.OnRtnTick += (object sender, TickEventArgs e) =>
            {
                MarketData _md = e.Tick;

                //处理时间
                DateTime _time = DateTime.Now;
                try
                {
                    _time = DateTime.ParseExact(_md.UpdateTime, "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                    if((_time-DateTime.Now).TotalHours > 23)
                    {
                        _time.AddDays(-1);
                    }else if((_time - DateTime.Now).TotalHours < -23)
                    {
                        _time.AddDays(1);
                    }
                }
                catch (Exception ex)
                {
                    Utils.Log(ex.StackTrace);
                }
                _time.AddMilliseconds(_md.UpdateMillisec);


                Tick _tick = new Tick(_md.InstrumentID, _md.LastPrice, _md.BidPrice, _md.BidVolume, _md.AskPrice, _md.AskVolume
                    , _md.AveragePrice, _md.Volume, _md.OpenInterest, _time, _md.UpperLimitPrice, _md.LowerLimitPrice);

                //发送
                OnTick(_tick);
            };
            mQuoter.OnRtnError += (object sender, ErrorEventArgs e) =>
            {
                Utils.Log("OnRtnError：" + e.ErrorMsg);
            };

            //开始连接
            mQuoter.ReqConnect(account.Server);
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
        private Dictionary<string, bool> mSubscribeMap = new Dictionary<string, bool>();
        public void SubscribeMarketData(string instrumentID)
        {
            //已经订阅了，就不订了
            if (mSubscribeMap.ContainsKey(instrumentID)
                && mSubscribeMap[instrumentID] == true)
            {
                return;
            }


            //判断登陆状态
            if (IsLogin())
            {
                mQuoter.ReqSubscribeMarketData(instrumentID);
                mSubscribeMap[instrumentID] = true;
            }
            else
            {
                mSubscribeMap[instrumentID] = false;
            }
        }
        //退订行情
        public void UnSubscribeMarketData(string instrumentID)
        {
            mQuoter.ReqUnSubscribeMarketData(instrumentID);
        }
    }
}
