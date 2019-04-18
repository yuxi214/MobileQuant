using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoQuant.Framwork.Engine {
    internal class Event {
        private EventType mEventType;
        private Type mValueType;
        private object mValue;

        public Event(EventType eventType, Type valueType, object value) {
            mEventType = eventType;
            mValueType = valueType;
            mValue = value;
        }

        internal object Value {
            get {
                return mValue;
            }
        }

        internal Type ValueType {
            get {
                return mValueType;
            }
        }

        internal EventType EventType {
            get { return mEventType; }
        }
    }

    internal enum EventType {
        publish, request
    }
}
