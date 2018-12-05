using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantEngine;

namespace StrategyPackage
{
    public class FishingOrder : BaseStrategy
    {
        private float gap = 0.02f;
        private float cancleMove = 0.005f;

        public override string[] OnLoadInstrument()
        {
            return new string[] { "ru1905","rb1905", "TA905","m1905","i1905", "y1905" };
        }

        Dictionary<string, Order> uperOrderDic = new Dictionary<string, Order>();
        Dictionary<string, Order> lowerOrderDic = new Dictionary<string, Order>();
        public override void OnTick(Tick tick)
        {
            //上方挂单
            if (uperOrderDic.ContainsKey(tick.InstrumentID))
            {
                Order order = uperOrderDic[tick.InstrumentID];
                if(order.Price < tick.LastPrice * (1 + gap - cancleMove))
                {
                    order.Cancle();
                }
            }
            else
            {
                Order order = SellOrder(1, tick.LastPrice * (1 + gap),tick.InstrumentID);
                if (order.Price < tick.UpperLimitPrice)
                {
                    order.Send();
                    uperOrderDic.Add(order.InstrumentID, order);
                    order.OnChanged += (Order o) =>
                    {
                        if (o.Status == OrderStatus.Canceled
                        && o.Status == OrderStatus.Error
                        && o.Status == OrderStatus.Filled)
                        {
                            uperOrderDic.Remove(o.InstrumentID);
                        }
                    };
                }
            }

            //下方挂单
            if (lowerOrderDic.ContainsKey(tick.InstrumentID))
            {
                Order order = lowerOrderDic[tick.InstrumentID];
                if(order.Price > tick.LastPrice * (1 - gap + cancleMove))
                {
                    order.Cancle();
                }
            }
            else
            {
                Order order = BuyOrder(1, tick.LastPrice * (1 - gap),tick.InstrumentID);
                if (order.Price > tick.LowerLimitPrice)
                {
                    order.Send();
                    lowerOrderDic.Add(order.InstrumentID, order);
                    order.OnChanged += (Order o) =>
                    {
                        if (o.Status == OrderStatus.Canceled
                        && o.Status == OrderStatus.Error
                        && o.Status == OrderStatus.Filled)
                        {
                            lowerOrderDic.Remove(o.InstrumentID);
                        }
                    };
                }
            }
        }
    }
}
