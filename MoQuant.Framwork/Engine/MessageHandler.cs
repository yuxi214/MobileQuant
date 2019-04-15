using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoQuant.Framwork.Engine {
    internal class MessageHandler<T> {
        private static MessageBus mMessageBus;
        public event OnMessageDelegate OnMessage;

        internal MessageHandler(MessageBus bus)
        {
            mMessageBus = bus;
            mMessageBus.OnMessage += onReceiveMessage;
        }

        private void onReceiveMessage(Message msg) {
            if (msg.Type == typeof(T)) {
                OnMessage?.Invoke((T)msg.Value);
            }
        }

        internal void post(T value) {
            Message msg = new Message(typeof(T), value);
            mMessageBus.post(msg);
        }

        public delegate void OnMessageDelegate(T value);
    }
}
