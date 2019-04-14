using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoQuant.Framwork.Engine {
    internal class MessageBus<T> {
        private static MessageQueue mMessageQueue = MessageQueue.Instance;
        public event OnMessageDelegate OnMessage;

        private MessageBus() {
            mMessageQueue.OnMessage += onReceiveMessage;
        }

        public static MessageBus<T> getBus() {
            return new MessageBus<T>();
        }

        private void onReceiveMessage(Message msg) {
            if (msg.Type == typeof(T)) {
                OnMessage?.Invoke((T)msg.Value);
            }
        }

        internal void post(T value) {
            Message msg = new Message(typeof(T), value);
            mMessageQueue.post(msg);
        }

        public delegate void OnMessageDelegate(T value);
    }
}
