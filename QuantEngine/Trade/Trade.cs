using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantEngine
{
    public class Trade
    {
        // 成交编号
        private string tradeID = string.Empty;

        // 合约代码
        private string instrumentID = string.Empty;

        // 买卖方向
        private DirectionType direction;

        // 开平标志
        private OffsetType offset;

        // 成交价格
        private double price;

        // 成交数量
        private int volume;

        // 成交时间
        private DateTime tradeTime;

        // 对应的委托标识
        private string orderID = string.Empty;

        public string TradeID
        {
            get
            {
                return tradeID;
            }

            set
            {
                tradeID = value;
            }
        }

        public string InstrumentID
        {
            get
            {
                return instrumentID;
            }

            set
            {
                instrumentID = value;
            }
        }

        public DirectionType Direction
        {
            get
            {
                return direction;
            }

            set
            {
                direction = value;
            }
        }

        public OffsetType Offset
        {
            get
            {
                return offset;
            }

            set
            {
                offset = value;
            }
        }

        public double Price
        {
            get
            {
                return price;
            }

            set
            {
                price = value;
            }
        }

        public int Volume
        {
            get
            {
                return volume;
            }

            set
            {
                volume = value;
            }
        }

        public DateTime TradeTime
        {
            get
            {
                return tradeTime;
            }

            set
            {
                tradeTime = value;
            }
        }

        public string OrderID
        {
            get
            {
                return orderID;
            }

            set
            {
                orderID = value;
            }
        }
    }
}
