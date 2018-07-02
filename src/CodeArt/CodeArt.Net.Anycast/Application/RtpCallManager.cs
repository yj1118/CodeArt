using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;

namespace CodeArt.Net.Anycast
{
    /// <summary>
    /// rtp调用管理器
    /// </summary>
    internal static class RtpCallManager
    {
        private static PoolWrapper<CallIdentity> _pool;

        static RtpCallManager()
        {
            _pool = new PoolWrapper<CallIdentity>(() =>
            {
                return new CallIdentity();
            },
            (obj, phase) =>
            {
                if(phase == PoolItemPhase.Returning)
                {
                    obj.Reset();
                }
                return true;
            }, new PoolConfig()
            {
                MaxRemainTime = 300 //闲置时间300秒
            });

            ClientEvents.Disconnected += OnDisconnected;
        }


        private static ConcurrentDictionary<Guid, CallIdentity> _borrowedIdentities = new ConcurrentDictionary<Guid, CallIdentity>();

        public static CallIdentity BorrowIdentity()
        {
            var identity = _pool.Borrow();
            _borrowedIdentities.TryAdd(identity.RequestId, identity);
            return identity;
        }

        public static void ReturnIdentity(CallIdentity identity)
        {
            _borrowedIdentities.TryRemove(identity.RequestId, out identity);
            _pool.Return(identity);
        }

        public static CallIdentity GetIdentity(Guid requestId)
        {
            if (_borrowedIdentities.TryGetValue(requestId, out var identity)) return identity;
            return null;
        }

        public static void Clear()
        {
            foreach(var p in _borrowedIdentities)
            {
                var identity = p.Value;
                _pool.Return(identity);
            }
            _borrowedIdentities.Clear();
        }

        private static void OnDisconnected(object sender, ClientEvents.DisconnectedEventArgs ea)
        {
            Clear();//如果断线，需要重置数据
        }
    }
}
