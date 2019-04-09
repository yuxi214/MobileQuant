using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

using XAPI;
using XAPI.Callback;

namespace DataReceiver {
    internal class Receiver {
        private XApi mMdApi;
        private XApi mTdApi;

        public static Receiver Intance = new Receiver();

        internal void start() {

        }

        private void init() {
            Config config = ConfigUtils.Config;
            //
            string mdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CTP_Quote_x86.dll");
            mMdApi = new XApi(mdPath);
            mMdApi.Server.Address = config.MyMdAccount.Server;
            mMdApi.Server.BrokerID = config.MyMdAccount.Broker;
            mMdApi.User.UserID = config.MyMdAccount.Investor;
            mMdApi.User.Password = config.MyMdAccount.Password;
            mMdApi.OnRtnDepthMarketData = _onRtnDepthMarketData;
            //
            string tdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CTP_Trade_x86.dll");
            mTdApi = new XApi(tdPath);
            mTdApi.Server.Address = config.MyTdAccount.Server;
            mTdApi.Server.BrokerID = config.MyTdAccount.Broker;
            mTdApi.User.UserID = config.MyTdAccount.Investor;
            mTdApi.User.Password = config.MyTdAccount.Password;
        }

        private void _onRtnDepthMarketData(object sender, ref DepthMarketDataNClass marketData) {
            Tick tick = new Tick() {
                InstrumentID = marketData.InstrumentID
                        ,
                AskPrice = marketData.Asks.Length > 0 ? marketData.Asks[0].Price : -1
                        ,
                AskVolume = marketData.Asks.Length > 0 ? marketData.Asks[0].Size : -1
                        ,
                BidPrice = marketData.Bids.Length > 0 ? marketData.Bids[0].Price : -1
                        ,
                BidVolume = marketData.Bids.Length > 0 ? marketData.Bids[0].Size : -1
                        ,
                LastPrice = marketData.LastPrice
                        ,
                OpenInterest = marketData.OpenInterest
                        ,
                UpdateTime = new DateTime(
                            marketData.ActionDay / 10000
                            , marketData.ActionDay / 100 % 100
                            , marketData.ActionDay % 100
                            , marketData.UpdateTime / 10000
                            , marketData.UpdateTime / 100 % 100
                            , marketData.UpdateTime % 100
                            , marketData.UpdateMillisec)
                        ,
                Volume = Convert.ToInt32(marketData.Volume)
            };
        }

        private void _onRspQryInstrument(object sender, ref InstrumentField instrument, int size1, bool bIsLast) {
            //只订阅期货，并且不订阅套利等其他合约
            Regex re = new Regex(@"^[a-zA-Z]+\d+$", RegexOptions.None);
            if (instrument.Type == InstrumentType.Future
                && re.IsMatch(instrument.InstrumentID)) {
                mMdApi.Subscribe(instrument.InstrumentID, instrument.ExchangeID);
            }
        }

    }
}
