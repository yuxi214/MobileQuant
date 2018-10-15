using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QuantEngine;

namespace StrategyPackage
{
    public class TestStrategy:BaseStrategy
    {
        public override string[] OnLoadInstrument()
        {
            return new string[] { "ru1901", "TA901", "y1901" };
        }
        long count = 0;
        public override void OnTick(Tick tick)
        {
            Console.WriteLine($"{DateTime.Now}-->{tick.InstrumentID}:{tick.LastPrice}");
            if(count++%100 == 0)
            {
                Order order = BuyOrder(1);
                order.Send();
            }
        }
    }
}
