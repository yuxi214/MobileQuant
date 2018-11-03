using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantEngine
{
    //订单委托
    public delegate void OrderChanged(Order order);
    public class Order
    {
        //策略
        private BaseStrategy mStrategy;
        //合约
        private string mInstrumentID = string.Empty;
        //买卖
        private DirectionType mDirection = DirectionType.Buy;
        //报价
        private double mPrice = 0d;
        //报单时间(本地时间)
        private DateTime mOrderTime = DateTime.Now;
        //报单数量
        private int mVolume = 0;
        //成交数量
        private int mVolumeTraded = 0;
        //未成交
        private int mVolumeLeft = 0;
        //状态
        private OrderStatus mStatus = OrderStatus.Normal;
        //子订单
        private List<SubOrder> mSubOrders = new List<SubOrder>();

        //事件
        public event OrderChanged OnChanged;

        public Order(BaseStrategy strategy, string instrumentID, DirectionType direction, double price, int volume)
        {
            this.mStrategy = strategy;
            this.mInstrumentID = instrumentID;
            this.mDirection = direction;
            this.mPrice = price;
            this.mOrderTime = DateTime.Now;
            this.mVolume = volume;
            this.mVolumeLeft = volume;
            this.mStatus = OrderStatus.Normal;
        }

        //
        public string InstrumentID
        {
            get
            {
                return mInstrumentID;
            }
        }

        public DirectionType Direction
        {
            get
            {
                return mDirection;
            }
        }

        public double Price
        {
            get
            {
                return mPrice;
            }
        }

        public int Volume
        {
            get
            {
                return mVolume;
            }
        }

        public int VolumeLeft
        {
            get
            {
                return mVolumeLeft;
            }
        }

        public OrderStatus Status
        {
            get
            {
                return mStatus;
            }
        }

        public DateTime OrderTime
        {
            get
            {
                return mOrderTime;
            }
        }

        public BaseStrategy Strategy
        {
            get
            {
                return mStrategy;
            }
        }

        internal List<SubOrder> SubOrders
        {
            get
            {
                return mSubOrders;
            }
        }

        //
        internal void AddSubOrder(SubOrder o)
        {
            mSubOrders.Add(o);
        }
        internal void RefreshSubOrders()
        {
            if (mSubOrders == null)
                return;

            int left = 0;
            int traded = 0;

            bool normal = true;
            bool partial = true;
            bool filled = true;
            bool error = true;
            bool cancled = true;

            //统计
            foreach (SubOrder sOrder in mSubOrders)
            {
                left += sOrder.VolumeLeft;
                traded += sOrder.VolumeTraded;

                normal = normal && sOrder.Status == OrderStatus.Normal;
                filled = filled && sOrder.Status == OrderStatus.Filled;
                error = error && sOrder.Status == OrderStatus.Error;
                cancled = cancled && sOrder.Status == OrderStatus.Canceled;
            }

            mVolumeLeft = left;
            mVolumeTraded = traded;

            //状态
            if (normal)
            {
                mStatus = OrderStatus.Normal;
            }
            else if (filled)
            {
                mStatus = OrderStatus.Filled;
            }
            else if (error)
            {
                mStatus = OrderStatus.Error;
            }
            else if (cancled)
            {
                mStatus = OrderStatus.Canceled;
            }
            else
            {
                mStatus = OrderStatus.Partial;
            }

            //订单状态变化
            mStrategy.UpdateOrder(this);
            OnChanged?.Invoke(this);
        }

        //发送订单
        public void Send()
        {
            mStrategy.SendOrder(this);
        }

        public void Cancle()
        {
            mStrategy.CancleOrder(this);
        }
    }

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

        internal void Refresh()
        {
            pOrder.RefreshSubOrders();
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
