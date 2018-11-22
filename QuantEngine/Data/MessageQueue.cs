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
        private int mMaxLen = 0;
        public MessageQueue(int maxLen)
        {
            mMaxLen = maxLen;
            run();
        }

        public bool add(MessageType type, object value)
        {
            if (queue.Count < mMaxLen)
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
                        execute();
                    }
                    catch (Exception ex)
                    {
                        Utils.EnginLog(ex.StackTrace);
                    }
                });
                mThread.Start();
            }

        }

        public event OnMessage OnMessage;
        private void execute()
        {
            while (true)
            {
                Message m = queue.Take();
                OnMessage?.Invoke(m);
            }
        }
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
        position = 1,
        order = 2
    }

    internal delegate void OnMessage(Message message);
}
