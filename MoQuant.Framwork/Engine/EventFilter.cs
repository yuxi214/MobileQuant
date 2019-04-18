using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoQuant.Framwork.Strategy;

namespace MoQuant.Framwork.Engine {
    internal class EventFilter {
        //优先类型
        private HashSet<Type> mTypes = new HashSet<Type>();
        public EventFilter() {
            mTypes.Add(typeof(Order));
            mTypes.Add(typeof(Tick));
            mTypes.Add(typeof(Bar));
            mTypes.Add(typeof(Position));
        }

        //判断是否为优先类型
        internal bool filterPrior(Type type) {
            return mTypes.Contains(type);
        }
    }
}
