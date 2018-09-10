using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantEngine
{
    //订单委托
    public delegate void OrderCopleted(Order order);
    public delegate void OrderError(Order order);
    public delegate void OrderTrade(int vol, Order order);
    public delegate void OrderCancelFailed(Order order);
    public delegate void OrderCanceled(Order order);
    public delegate void OrderChanged(Order order);
    public class Order
    {
        //策略
        private BaseStrategy strategy;
        //合约
        private string instrumentID = string.Empty;
        //买卖
        private DirectionType direction = DirectionType.Buy;
        //报价
        private double price = 0d;
        //报单时间(本地时间)
        private DateTime orderTime = DateTime.Now;
        //报单数量
        private int volume = 0;
        //成交数量
        private int volumeTraded = 0;
        //未成交
        private int volumeLeft = 0;
        //状态
        private OrderStatus status = OrderStatus.Normal;
        //子订单
        private List<SubOrder> subOrders;

        //事件
        public event OrderCopleted OnCompleted;
        public event OrderError OnError;
        public event OrderTrade OnTraded;
        public event OrderCanceled OnCanceled;
        public event OrderChanged OnChanged;

        public Order(BaseStrategy strategy, string instrumentID, DirectionType direction, double price, DateTime orderTime, int volume, int volumeLeft, OrderStatus status)
        {
            this.strategy = strategy;
            this.instrumentID = instrumentID;
            this.direction = direction;
            this.price = price;
            this.orderTime = orderTime;
            this.volume = volume;
            this.volumeLeft = volumeLeft;
            this.status = status;
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
        }

        public OrderStatus Status
        {
            get
            {
                return status;
            }
        }

        public DateTime OrderTime
        {
            get
            {
                return orderTime;
            }
        }

        public BaseStrategy Strategy
        {
            get
            {
                return strategy;
            }
        }

        internal List<SubOrder> SubOrders
        {
            set
            {
                if (subOrders != null)
                    return;

                subOrders = value;
                if (subOrders == null)
                    return;
                foreach (SubOrder sOrder in subOrders)
                {
                    sOrder.OnError += (SubOrder order) =>
                    {
                        refresh();
                        OnChanged(this);
                    };
                    sOrder.OnTraded += (int vol, SubOrder order) =>
                    {
                        volumeTraded += vol;
                        volumeLeft -= vol;
                        OnTraded(vol, this);
                        refresh();
                    };
                    sOrder.OnCanceled += (SubOrder order) =>
                    {
                        refresh();
                        OnChanged(this);
                    };
                    sOrder.OnCancelFailed += (SubOrder order) =>
                    {
                        怎么处理撤单失败呢
                    };
                }
            }
            get
            {
                return subOrders;
            }
        }

        //刷新订单状态
        private void refresh()
        {
            if (subOrders != null)
                return;

            int left = 0;
            int traded = 0;

            bool error = true;
            bool cancled = true;

            //统计
            foreach (SubOrder sOrder in subOrders)
            {
                left += sOrder.VolumeLeft;
                traded += sOrder.VolumeTraded;

                error = sOrder.Status != OrderStatus.Error ? false : true;
                cancled = sOrder.Status != OrderStatus.Canceled ? false : true;
            }

            //错误
            if (error)
            {
                OnError(this);
            }

            //撤单
            if (cancled)
            {
                OnCanceled(this);
            }

            //订单完成
            if(volumeLeft == 0)
            {
                OnCompleted(this);
            }
        }

        public override string ToString()
        {
            return $"{instrumentID},{direction},{price},{volume},{volumeLeft},{Status}";
        }

        //发送订单
        public void Send()
        {
            strategy.SendOrder(this);
        }

        public void Cancle()
        {
            strategy.CancleOrder(this);
        }
    }

    //子订单委托
    internal delegate void SubOrderError(SubOrder sOrder);
    internal delegate void SubOrderTrade(int vol, SubOrder sOrder);
    internal delegate void SubOrderCancelFailed(SubOrder sOrder);
    internal delegate void SubOrderCanceled(SubOrder sOrder);
    internal class SubOrder
    {
        //合并订单
        private Order pOrder;

        //用户标识
        private int customID = 0;

        // 报单标识
        private string orderID = string.Empty;

        // 合约
        private string instrumentID = string.Empty;

        // 买卖
        private DirectionType direction;

        // 开平
        private OffsetType offset;

        // 报价
        private double limitPrice;

        // 报单时间(交易所)
        private DateTime insertTime;

        // 报单数量
        private int volume;

        //成交数量
        private int volumeTraded;

        // 未成交,trade更新
        private int volumeLeft;

        // 状态
        private OrderStatus status;

        public SubOrder(Order pOrder, string instrumentID, DirectionType direction, OffsetType offset, double limitPrice, DateTime insertTime, int volume, int volumeLeft, OrderStatus status)
        {
            this.pOrder = pOrder;
            this.instrumentID = instrumentID;
            this.direction = direction;
            this.offset = offset;
            this.limitPrice = limitPrice;
            this.insertTime = insertTime;
            this.volume = volume;
            this.volumeLeft = volumeLeft;
            this.status = status;
        }

        //事件
        internal event SubOrderError OnError;
        internal event SubOrderTrade OnTraded;
        internal event SubOrderCanceled OnCanceled;
        internal event SubOrderCancelFailed OnCancelFailed;

        internal void EmitTrade(int vol)
        {
            OnTraded(vol, this);
        }
        internal void EmitError()
        {
            OnError(this);
        }
        internal void EmitCancel()
        {
            OnCanceled(this);
        }
        internal void EmitCancelFailed()
        {
            OnCancelFailed(this);
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

        public double LimitPrice
        {
            get
            {
                return limitPrice;
            }

            set
            {
                limitPrice = value;
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
                VolumeLeft = value;
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

        public Order POrder
        {
            get
            {
                return pOrder;
            }
        }

        public int CustomID
        {
            get
            {
                return customID;
            }

            set
            {
                customID = value;
            }
        }

        public int VolumeTraded
        {
            get
            {
                return volumeTraded;
            }

            set
            {
                volumeTraded = value;
            }
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
        Error
    }

    public enum OffsetType
    {
        /// <summary>
        /// 开仓
        /// </summary>
        Open,
        /// <summary>
        /// 平仓
        /// </summary>
        Close,
        /// <summary>
        /// 平今
        /// </summary>
        CloseToday
    }
}
