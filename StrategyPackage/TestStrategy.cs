using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QuantEngine;

namespace StrategyPackage
{
    public class TestStrategy:Strategy
    {
        public override string[] OnLoadInstrument()
        {
            return new string[] { "ru1901", "TA901", "y1901" };
        }
        public override void OnTick(Tick tick)
        {
            Console.WriteLine($"{tick.InstrumentID}:{tick.LastPrice}");
        }
    }
}
