using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using MoQuant.Framwork.Data;

namespace MoQuant.Framwork.Engine {
    internal class MessageQueue {
        private BlockingCollection<Message> mL1Queue = new BlockingCollection<Message>();
        private BlockingCollection<Message> mL2Queue = new BlockingCollection<Message>();
        private Thread mL1Thread;
        private Thread mL2Thread;
        //
        private static MessageQueue mInstance = new MessageQueue();
        public static MessageQueue Instance {
            get {
                return mInstance;
            }
        }

        private MessageQueue() {
            start();
        }

        public bool post(Message msg) {
            if (MessageFilter.filterPrior(msg.Type)) {
                return mL1Queue.TryAdd(msg, 1000);
            } else {
                return mL2Queue.TryAdd(msg, 1000);
            }

        }

        private void start() {
            //优先线程
            if (mL1Thread == null || !mL1Thread.IsAlive) {
                mL1Thread = new Thread(() => {
                    try {
                        while (true) {
                            Message e = mL1Queue.Take();
                            OnMessage?.Invoke(e);
                        }
                    } catch (Exception ex) {
                        LogUtils.EnginLog(ex.StackTrace);
                    }
                });
                mL1Thread.Priority = ThreadPriority.Highest;
                mL1Thread.Start();
            }
            //非优先线程
            if (mL2Thread == null || !mL1Thread.IsAlive) {
                mL2Thread = new Thread(() => {
                    try {
                        while (true) {
                            Message e = mL2Queue.Take();
                            OnMessage?.Invoke(e);
                        }
                    } catch (Exception ex) {
                        LogUtils.EnginLog(ex.StackTrace);
                    }
                });
                mL2Thread.Priority = ThreadPriority.Lowest;
                mL2Thread.Start();
            }

        }

        public event OnMessage OnMessage;
    }

    internal delegate void OnMessage(Message msg);
}
