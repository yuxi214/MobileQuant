using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoQuant.Framwork.Engine {
    internal class Message {
        private Type mType;
        private object mValue;

        public Message(Type type, object value) {
            mType = type;
            mValue = value;
        }

        public object Value {
            get {
                return mValue;
            }
        }

        internal Type Type {
            get {
                return mType;
            }
        }
    }
}
