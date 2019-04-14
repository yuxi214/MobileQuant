using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoQuant.Framwork.Strategy;

namespace MoQuant.Framwork.Engine {
    internal class MessageFilter {
        //优先类型
        private static Type[] types = new Type[]{
            typeof(Order),
            typeof(Tick),
            typeof(Bar),
            typeof(Position)
        };
        //判断是否为
        internal static bool filterPrior(Type type) {
            foreach (Type t in types) {
                if (t.Equals(type)) {
                    return true;
                }
            }
            return false;
        }
    }
}
