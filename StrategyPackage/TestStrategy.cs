using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QuantEngine;

namespace StrategyPackage
{
    public class TestStrategy : BaseStrategy
    {
        public override string[] OnLoadInstrument()
        {
            return new string[] { "ru1901", "TA901", "y1901" };
        }
        long count = 0;
        public override void OnTick(Tick tick)
        {
            Console.WriteLine($"{DateTime.Now}-->{tick.InstrumentID}:{tick.LastPrice}");
            Random r = new Random();
            int number = r.Next(1,1000);
            if (count++ % number == 0)
            {
                int pos = GetPosition(tick.InstrumentID);
                int vol = r.Next(1,10);

                Order order;
                if (number % 2 == 0)
                {
                    order = BuyOrder(vol - pos % 3, tick.AskPrice, tick.InstrumentID);
                }
                else
                {
                    order = SellOrder(vol + pos % 3, tick.AskPrice, tick.InstrumentID);
                }
                order.OnChanged += (Order o) => {
                    if(o.Status == OrderStatus.Canceled
                    || o.Status == OrderStatus.Error
                    || o.Status == OrderStatus.Filled)
                    {
                        Log($"订单：{o.InstrumentID}\t{o.Direction}\t{o.Volume-o.VolumeLeft}");
                        Log($"持仓:{o.InstrumentID}\t{GetPosition(o.InstrumentID)}");
                    }
                };
                order.Send();
            }
        }
    }
}
