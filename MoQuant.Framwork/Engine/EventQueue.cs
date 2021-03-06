﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using MoQuant.Framwork.Data;

namespace MoQuant.Framwork.Engine {
    internal class EventQueue {
        private BlockingCollection<Event> mL1Queue = new BlockingCollection<Event>();
        private BlockingCollection<Event> mL2Queue = new BlockingCollection<Event>();
        private Thread mL1Thread;
        private Thread mL2Thread;
        private EventFilter mFilter = new EventFilter();
        //
        private static EventQueue mInstance = new EventQueue();
        public static EventQueue Instance {
            get {
                return mInstance;
            }
        }

        private EventQueue() {
            start();
        }

        public bool post(Event msg) {
            if (mFilter.filterPrior(msg.ValueType)) {
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
                            Event e = mL1Queue.Take();
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
                            Event e = mL2Queue.Take();
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

        public event OnMessageDelegate OnMessage;
    }

    internal delegate void OnMessageDelegate(Event msg);
}
