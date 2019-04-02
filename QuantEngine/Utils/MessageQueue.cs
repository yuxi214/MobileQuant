using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace QuantEngine
{
    internal class MessageQueue
    {
        private BlockingCollection<Message> queue = new BlockingCollection<Message>();
        private static MessageQueue mInstance;
        public static MessageQueue Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new MessageQueue();
                }

                return mInstance;
            }
        }
        private MessageQueue()
        {
            run();
        }

        public bool add(MessageType type, object value)
        {
            if (queue.Count < 10000)
            {
                Message m = new Message(type, value);
                return queue.TryAdd(m, 1000);
            }
            else
            {
                return false;
            }

        }

        private Thread mThread;
        private void run()
        {
            if (mThread == null || !mThread.IsAlive)
            {
                mThread = new Thread(() =>
                {
                    try
                    {
                        while (true)
                        {
                            Message m = queue.Take();
                            OnMessage?.Invoke(m);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtils.EnginLog(ex.StackTrace);
                    }
                });
                mThread.Start();
            }

        }

        public event OnMessage OnMessage;
    }

    internal class Message
    {
        private MessageType mType;
        private object mValue;

        public Message(MessageType typeName, object value)
        {
            this.mType = typeName;
            this.mValue = value;
        }

        public object Value
        {
            get
            {
                return mValue;
            }
        }

        internal MessageType Type
        {
            get
            {
                return mType;
            }
        }
    }
    internal enum MessageType
    {
        position,
        order,
        log
    }

    internal delegate void OnMessage(Message message);
}
