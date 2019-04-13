using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReceiver {
    /// <summary>
    /// 账号
    /// </summary>
    internal class Account {
        private string server;
        private string broker;
        private string investor;
        private string password;

        public Account(string server, string broker, string investor, string password) {
            this.server = server;
            this.broker = broker;
            this.investor = investor;
            this.password = password;
        }

        public string Server {
            get {
                return server;
            }
        }

        public string Broker {
            get {
                return broker;
            }
        }

        public string Investor {
            get {
                return investor;
            }
        }

        public string Password {
            get {
                return password;
            }
        }
    }
    /// <summary>
    /// tick
    /// </summary>
    internal class Tick {
        /// <summary>
        /// 合约代码
        /// </summary>
        public string InstrumentID;
        /// <summary>
        /// 最新价
        /// </summary>
        public double LastPrice;
        /// <summary>
        ///申买价一
        /// </summary>
        public double BidPrice;
        /// <summary>
        ///申买量一
        /// </summary>
        public int BidVolume;
        /// <summary>
        ///申卖价一
        /// </summary>
        public double AskPrice;
        /// <summary>
        ///申卖量一
        /// </summary>
        public int AskVolume;
        /// <summary>
        ///数量
        /// </summary>
        public int Volume;
        /// <summary>
        ///持仓量
        /// </summary>
        public double OpenInterest;
        /// <summary>
        ///时间
        /// </summary>
        public DateTime UpdateTime;

    }
    /// <summary>
    /// bar
    /// </summary>
    internal class Min1Bar {
        /// <summary>
        /// 合约代码
        /// </summary>
        public string InstrumentID;
        /// <summary>
        /// 开始价
        /// </summary>
        public double OpenPrice;
        /// <summary>
        /// 最高价
        /// </summary>
        public double HighPrice;
        /// <summary>
        /// 最低价
        /// </summary>
        public double LowPrice;
        /// <summary>
        /// 结束价
        /// </summary>
        public double ClosePrice;
        /// <summary>
        ///数量
        /// </summary>
        public int Volume;
        /// <summary>
        ///持仓量
        /// </summary>
        public double OpenInterest;
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime OpenTime;
    }
}
