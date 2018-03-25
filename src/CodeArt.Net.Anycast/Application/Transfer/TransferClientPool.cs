using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Net.Anycast;
using CodeArt.Concurrent;

namespace CodeArt.Net.Anycast
{
    public class TransferClientPool : IDisposable
    {
        private Pool<TransferClient> _pool;

        public TransferClientPool(string communicatorName, string serverIP, int serverPort)
        {
            _pool = new Pool<TransferClient>(() =>
            {
                var client = new TransferClient(communicatorName, serverIP, serverPort);
                if(!client.IsConnected) throw new NetworkException();
                return client;
            },
            (obj, phase) =>
            {
                if (!obj.IsConnected) return false; //如果已经断开连接，那么抛弃该对象
                return true;
            }, new PoolConfig()
            {
                LoanCapacity = 4, //最多有4个同时上传或者下载
                MaxRemainTime = 300 //闲置时间300秒
            });
        }

        public IPoolItem<TransferClient> Borrow()
        {
            return _pool.Borrow();
        }

        public void Dispose()
        {
            _pool.Dispose();
        }

    }
}
