using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MoQuant.Framwork.Engine {
    internal class EventBus {

        //
        private static EventQueue mQueue = EventQueue.Instance;
        private static ConcurrentDictionary<Type, ConcurrentBag<IEventHandle>> mDelegateMap = new ConcurrentDictionary<Type, ConcurrentBag<IEventHandle>>();
        //

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="t"></param>
        public static void Pulish<T>(T t) {
            mQueue.post(new Event(EventType.publish, typeof(T), t));
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="del"></param>
        public static void Subscrib<T>(OnEventDelegate<T> del) {
            DelWrapper<T> wrapper = new DelWrapper<T>(del);
            ConcurrentBag<IEventHandle> bag;
            if (mDelegateMap.TryGetValue(typeof(T), out bag)) {
                foreach (DelWrapper<T> w in bag) {
                    if (w.Delegate == del) {
                        return;
                    }
                }
            } else {
                bag = new ConcurrentBag<IEventHandle>();
                mDelegateMap.TryAdd(typeof(T), bag);
            }
            bag.Add(wrapper);
        }

        public static void UnSubscrib<T>(OnEventDelegate<T> del) {
            ConcurrentBag<IEventHandle> bag;
            if (mDelegateMap.TryGetValue(typeof(T), out bag)) {
                ConcurrentBag<IEventHandle> removeBag = new ConcurrentBag<IEventHandle>();
                foreach (DelWrapper<T> w in bag) {
                    if (w.Delegate == del) {
                        removeBag.Add(w);
                    }
                }

                foreach (IEventHandle r in removeBag) {
                    IEventHandle one = r;
                    bag.TryTake(out one);
                }

            }
        }

        public static void Request<T>(T req) {

        }

        /// <summary>
        /// 为bus连接队列
        /// </summary>
        /// <param name="queue"></param>
        public static void ConectQueue(EventQueue queue) {
            mQueue = queue;
            mQueue.OnMessage += _onEvent;
        }

        /// <summary>
        /// 处理各种事件
        /// </summary>
        /// <param name="evt"></param>
        public static void _onEvent(Event evt) {
            if (evt.EventType != EventType.publish)
                return;
            //
            Type t = evt.ValueType;
            ConcurrentBag<IEventHandle> bag;
            if (mDelegateMap.TryGetValue(t, out bag)) {
                foreach (IEventHandle wrapper in bag) {
                    wrapper.Handle(evt);
                }
            }
        }

    }

    internal delegate void OnEventDelegate<T>(T t);
    internal class DelWrapper<T> : IEventHandle {
        private OnEventDelegate<T> mDelegate;

        public OnEventDelegate<T> Delegate {
            get { return mDelegate; }
        }

        public DelWrapper(OnEventDelegate<T> del) {
            mDelegate = del;
        }

        public void Handle(Event evt) {
            mDelegate?.Invoke((T)(evt.Value));
        }
    }

    internal interface IEventHandle {
        void Handle(Event evt);
    }

}
