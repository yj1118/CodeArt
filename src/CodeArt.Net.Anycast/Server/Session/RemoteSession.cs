using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DotNetty.Transport.Channels;

namespace CodeArt.Net.Anycast
{
    public class RemoteSession : IServerSession
    {
        public bool IsActive
        {
            get
            {
                return this.Channel.Active;
            }
        }

        public IChannel Channel
        {
            get;
            private set;
        }

        public string UnicastAddress
        {
            get;
            private set;
        }


        public RemoteSession(IChannel channel)
        {
            this.Channel = channel;
            this.UnicastAddress = GetUnicastAddress(this.Channel);
        }

        internal static string GetUnicastAddress(IChannel channel)
        {
            return channel.RemoteAddress.ToString(); //在服务器端，我们记录session的地址是客户端的本地地址，因此，对于服务器端是通道的远程地址
        }

        /// <summary>
        /// 为当前会话对应的客户端发送一条消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async void Send(Message msg)
        {
            try
            {
                await this.Channel.WriteAndFlushAsync(msg);
            }
            catch(Exception)
            {
                //写日志，该问题一般是由于发送数据过程中，发送方断开连接导致
            }
        }

        public void Process(Message msg)
        {
            Send(msg);
        }


        /// <summary>
        /// 结束工作
        /// </summary>
        public void Close()
        {
            //this.ClearItems();
            this.Channel.CloseAsync();
        }

        //#region 发送数据


        //internal void SendRtpData(RtpDataPackage package)
        //{
        //    InternalSend(package);
        //}

        //private void InternalSend(RtpDataPackage package)
        //{
        //    byte[] rtpData = null;
        //    using (var item0 = ByteBuffer.Borrow(package.Data.Length))
        //    {
        //        var bytes0 = item0.Item;
        //        package.Read(bytes0);

        //        int completeLength = bytes0.Length;

        //        //需要前置数据长度
        //        using (var item1 = bytes0.Insert(completeLength))
        //        {
        //            rtpData = item1.Item.ToArray();
        //        }
        //    }

        //    this.Send(rtpData, 0, rtpData.Length);
        //}

        //#endregion


        //#region 自定义数据项


        //private object _itemsSyncObject = new object();

        //public T GetOrCreateItem<T>(string name, Func<RtpServerSession, T> creator)
        //    where T : class
        //{
        //    lock (_itemsSyncObject)
        //    {
        //        object value = null;
        //        if (!this.Items.TryGetValue(name, out value))
        //        {
        //            value = creator(this);
        //            this.Items.Add(name, value);
        //        }
        //        return (T)value;
        //    }
        //}

        //public T GetItem<T>(string name)
        //    where T : class
        //{
        //    lock (_itemsSyncObject)
        //    {
        //        object value = null;
        //        if (this.Items.TryGetValue(name, out value)) return (T)value;
        //        return null;
        //    }
        //}

        //public bool RemoveItem(string name)
        //{
        //    lock (_itemsSyncObject)
        //    {
        //        return this.Items.Remove(name);
        //    }
        //}

        //public void ClearItems()
        //{
        //    lock (_itemsSyncObject)
        //    {
        //        //foreach (var p in this.Items)
        //        //{   额外项由插件自身去清理，我们不主动清理，以免引起并发性错误
        //        //    var disposable = p.Value as IDisposable;
        //        //    if (disposable != null) disposable.Dispose();
        //        //}
        //        this.Items.Clear();
        //    }
        //}


        //#endregion

    }
}
