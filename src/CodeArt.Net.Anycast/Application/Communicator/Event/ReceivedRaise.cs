using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Net.Anycast;

namespace CodeArt.Net.Anycast
{
    public class ReceivedRaiseEventArgs : EventArgs
    {
        /// <summary>
        /// 事件的触发来源，也就是谁触发了这个事件
        /// </summary>
        public string Origin
        {
            get;
            private set;
        }

        /// <summary>
        /// 事件名称
        /// </summary>
        public string EventName
        {
            get;
            private set;
        }

        /// <summary>
        /// 与事件相关的数据
        /// </summary>
        public RtpData Data
        {
            get;
            private set;
        }

        private Dictionary<string, object> _items;

        /// <summary>
        /// 获得数据项，该方法可以在多个事件中共享数据，提高性能
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public T GetItem<T>(string name, Func<T> create)
        {
            if (_items == null) _items = new Dictionary<string, object>();
            if (!_items.TryGetValue(name, out var item))
            {
                item = create();
                _items.Add(name, item);
            }
            return (T)item;
        }

        public ReceivedRaiseEventArgs(string origin, string eventName, RtpData data)
        {
            this.Origin = origin;
            this.EventName = eventName;
            this.Data = data;
        }
    }

    public delegate void ReceivedRaiseEventHandler(object sender, ReceivedRaiseEventArgs ea);

}