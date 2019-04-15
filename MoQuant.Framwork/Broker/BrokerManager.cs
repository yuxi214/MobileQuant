using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoQuant.Framwork.Broker {
    internal class BrokerManager {
        private static BrokerManager mIntance = new BrokerManager();
        private BrokerManager() {

        }

        public static BrokerManager Intance {
            get { return mIntance; }
            set { mIntance = value; }
        }
    }
}
