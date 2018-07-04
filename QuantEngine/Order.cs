﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantEngine
{
    class Order
    {
        //报单标识
        private string orderID = string.Empty;
        //合约
        private string instrumentID = string.Empty;
        //买卖
        private DirectionType direction = DirectionType.Buy;
        //报价
        private double price = 0d;
        //报单时间(本地时间)
        private DateTime insertTime = DateTime.Now;
        //报单数量
        private int volume = 0;
        //未成交
        private int volumeLeft = 0;
        //状态
        private OrderStatus status = OrderStatus.Normal;

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

        public string InstrumentID
        {
            get
            {
                return instrumentID;
            }
        }

        public DirectionType Direction
        {
            get
            {
                return direction;
            }
        }

        public double Price
        {
            get
            {
                return price;
            }
        }

        public int Volume
        {
            get
            {
                return volume;
            }
        }

        public int VolumeLeft
        {
            get
            {
                return volumeLeft;
            }

            set
            {
                volumeLeft = value;
            }
        }

        public OrderStatus Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
            }
        }

        public DateTime InsertTime
        {
            get
            {
                return insertTime;
            }

            set
            {
                insertTime = value;
            }
        }

        public override string ToString()
        {
            return $"{orderID},{instrumentID},{direction},{price},{volume},{volumeLeft},{Status}";
        }
    }

    public enum DirectionType
    {
        /// <summary>
        /// 买
        /// </summary>
        Buy,

        /// <summary>
        /// 卖
        /// </summary>
        Sell
    }

    public enum OrderStatus
    {
        /// <summary>
        /// 委托
        /// </summary>
        Normal,
        /// <summary>
        /// 部成
        /// </summary>
        Partial,
        /// <summary>
        /// 全成
        /// </summary>
        Filled,
        /// <summary>
        /// 撤单[某些"被拒绝"的委托也会触发此状态]
        /// </summary>
        Canceled,
        /// <summary>
        /// 错误
        /// </summary>
        Error,
    }
}
