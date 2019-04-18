using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoQuant.Framwork.Engine {
    internal class MessageHandler<T> {
        private static EventQueue mMessageBus;
        public event OnMessageDelegate OnMessage;

        internal MessageHandler(EventQueue bus)
        {
            mMessageBus = bus;
            mMessageBus.OnMessage += onReceiveMessage;
        }

        private void onReceiveMessage(Event msg) {
            if (msg.ValueType == typeof(T)) {
                OnMessage?.Invoke((T)msg.Value);
            }
        }

        internal void post(T value) {
            Event msg = new Event(typeof(T), value);
            mMessageBus.post(msg);
        }

        public delegate void OnMessageDelegate(T value);
    }
}
